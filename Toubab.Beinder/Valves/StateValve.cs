
namespace Toubab.Beinder.Valves
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Tools;
    using Bindables;

    /// <summary>
    /// Valve for state-bearing bindables
    /// </summary>
    /// <remarks>
    /// State-bearing bindables are properties.
    /// 
    /// Properties have a state, and can produce events when their value changes.
    /// 
    /// Properties can also consume those events from other properties,
    /// for example by setting this property to the changed value announced
    /// by the other propety.
    /// 
    /// The difference between the <see cref="StateValve"/> and the <see cref="BroadcastValve"/>
    /// is that (a) consumed events result in a state change, and (b) not every consumed event
    /// has effect: *only* if it may cause a state change, an event from one of the bindables
    /// in the valve is propagated to the other bindables in the valve.
    /// </remarks>
    public class StateValve : Valve
    {
        // the secret object array.
        // it is guaranteed that no object array outside this class
        // can hold the same array as this one; and that is what we want.
        static readonly object _secret = new object();
       
        object _value = _secret;

        /// <summary>
        /// Activate one of the objects for which there is a bindable in the valve.
        /// </summary>
        /// <remarks>
        /// "Activate" in this context is "push the value of my property to the bound property
        /// on other objects in the valve". 
        /// 
        /// Example if property `X` of objects `A` and `B` are in a `valve`, and `valve.Activate(A)` is
        /// called, this means the value in `A.X` whill be transferred to `B.X`.
        /// </remarks>
        /// <param name="toActivate">The object in the valve to activate. 
        /// If the object is not participating in the valve, nothing happens.</param>
        public async Task<bool> Activate(object toActivate)
        {
            if (toActivate == null)
                return false;

            // this is an ugly and temporary solution.
            // in the near future, we want to activate by tag, not by object
            // reference.

            foreach (var attachment in this)
            {
                using (attachment)
                {
                    var prop = attachment.Conduit.Bindable as IProperty;
                    if (prop != null && ReferenceEquals(toActivate, prop.Object))
                    {
                        var value = prop.Value;
                        await Push(prop, new[] { value });
                        OnValueChanged(prop.Object,  value);
                        return true;
                    }
                }
            }

            return false;
        }

        public object[] GetChildValveObjects()
        {
            return this
                .Select(attachment =>
                {
                    using (attachment)
                    {   
                        return attachment == null ? null : ((IProperty)attachment.Conduit.Bindable).Value;
                    }
                })
                .Where(values => values != null)
                .ToArray();
        }

        public object GetValueForObject(object ob)
        {
            return this
                .Select(a =>
                {
                    using (a)
                    {
                        var prop = a.Conduit.Bindable as IProperty;
                        return prop != null && ReferenceEquals(ob, prop.Object) ? prop.Value : null;
                    }
                })
                .FirstOrDefault(p => p != null);
        }

        public event EventHandler<BroadcastEventArgs> ValueChanged;

        void OnValueChanged(object sourceObject, object newValue)
        {
            var evt = ValueChanged;
            if (evt != null)
            {
                evt(this, new BroadcastEventArgs(sourceObject, new[] { newValue }));
            }
        }

        protected override async Task HandleBroadcastAsync(IEvent sender, object[] e)
        {
            await base.HandleBroadcastAsync(sender, e);
            foreach (var attachment in this)
            {
                using (attachment)
                {
                    var bnd = attachment.Conduit.Bindable as IEvent;
                    if (ReferenceEquals(sender, bnd))
                    {
                        OnValueChanged(sender.Object, e);
                        return;
                    }
                }
            }
        }

        protected override async Task<bool> Push(IEvent source, object[] payload)
        {
            // important! any event that emanates from an IProperty source is
            // interpreted as a property change. Therefore, the payload is 
            // replaced with the new value of the property; the original event 
            // arguments are discarded.
            var prop = source as IProperty;
            foreach (var attachment in this)
            {
                using (attachment)
                {
                    if (ReferenceEquals(prop, attachment.Conduit.Bindable))
                    {
                        payload = new[] { prop.Value };
                        break;
                    }
                }
            }

            lock (_secret)
            {
                if (payload.Length > 0 && !Equals(_value, payload[0]))
                {
                    _value = payload[0];
                }
                else
                {
                    return false;
                }
            }

            return await base.Push(source, payload);
        }

    }

}

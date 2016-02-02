
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
    public class StateValve : BroadcastValve
    {
        static readonly object[] _secret = new[] { new object() };
       
        object[] _values = _secret;

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
            var prop = this.FirstOrDefault(p => ReferenceEquals(toActivate, p.Object)) as IProperty;
            if (prop != null)
            {
                var values = prop.Values;
                await Push(prop, values);
                OnValuesChanged(prop, values);
                return true;
            }
            return false;
        }

        public object[][] GetChildValveObjects() {
            return this
                .Where(p => p is IProperty)
                .Select(p => ((IProperty)p).Values)
                .TransposeWithPadding()
                .Select(enu => enu.ToArray())
                .ToArray();
        }
            
        public object[] GetValuesForObject(object ob) 
        {
            return this
                .Where(p => p is IProperty && ReferenceEquals(ob, p.Object))
                .Select(p => ((IProperty)p).Values)
                .FirstOr(new object[0]);
        }

        public event EventHandler<BroadcastEventArgs> ValueChanged;

        void OnValuesChanged(object sourceObject, object[] newValue)
        {
            var evt = ValueChanged;
            if (evt != null)
            {
                evt(this, new BroadcastEventArgs(sourceObject, newValue));
            }
        }

        protected override async Task HandleBroadcastAsync(object sender, BroadcastEventArgs e)
        {
            await base.HandleBroadcastAsync(sender, e);
            OnValuesChanged(e.SourceObject, e.Payload);
        }

        protected override async Task<bool> Push(object source, object[] payload)
        {
            lock (_secret)
            {
                if (!Equals(_values, payload) && !Enumerable.SequenceEqual(_values, payload))
                {
                    _values = payload;
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

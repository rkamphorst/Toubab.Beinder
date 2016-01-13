using System;
using System.Linq;
using Toubab.Beinder.Valve;
using Toubab.Beinder.Util;

namespace Toubab.Beinder.Valve
{
    public class StateValve : BroadcastValve
    {
        static readonly object[] _secret = new[] { new object() };
       
        object[] _value = _secret;

        public bool Activate(object toActivate)
        {
            if (toActivate == null)
                return false;
            var prop = this.FirstOrDefault(p => ReferenceEquals(toActivate, p.Object)) as IBindableState;
            if (prop != null)
            {
                var value = prop.Value;
                Push(prop, value);
                OnValueChanged(prop, value);
                return true;
            }
            return false;
        }

        public object[][] GetChildValveObjects() {
            return this
                .Where(p => p is IBindableState)
                .Select(p => ((IBindableState)p).Value)
                .TransposeWithPadding()
                .Select(enu => enu.ToArray())
                .ToArray();
        }
            
        public int GetIndexForObject(object ob) 
        {
            return this
                .Where(p => p is IBindableState)
                .Select((p, i) => ReferenceEquals(ob, p.Object) ? i : -1)
                .Where(i => i >= 0).FirstOr(-1);
        }

        public object[] GetValueForObject(object ob) 
        {
            return this
                .Where(p => p is IBindableState && ReferenceEquals(ob, p.Object))
                .Select(p => ((IBindableState)p).Value)
                .FirstOr(new object[0]);
        }

        public event EventHandler<BindableBroadcastEventArgs> ValueChanged;

        void OnValueChanged(IBindable property, object[] newValue)
        {
            var evt = ValueChanged;
            if (evt != null)
            {
                evt(this, new BindableBroadcastEventArgs(property, newValue));
            }
        }

        protected override void HandleBroadcast(object sender, BindableBroadcastEventArgs e)
        {
            base.HandleBroadcast(sender, e);
            OnValueChanged(e.Source, e.Argument);
        }

        protected override bool Push(object source, object[] payload)
        {
            lock (_secret)
            {
                if (!Equals(_value, payload) && !Enumerable.SequenceEqual(_value, payload))
                {
                    _value = payload;
                }
                else
                {
                    return false;
                }
            }
            return base.Push(source, payload);
        }

        public override string ToString()
        {
            var firstprop = this.FirstOrDefault(p => p is IBindableState) as IBindableState;
            if (firstprop != null)
            {
                return string.Format("[Valve: Path={0}]", firstprop.Path);  
            }
            else
            {
                return "[Valve: Path=(none), Value=(none)]";
            }
        }
    }

}

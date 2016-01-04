using System;
using System.Linq;
using Toubab.Beinder.Valve;

namespace Toubab.Beinder.Valve
{
    public class StateValve : BroadcastValve
    {
        static readonly object _secret = new object();
       
        object _value = _secret;

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

        public object[] GetValues()
        { 
            return this.Where(p => p is IBindableState).Select(p => ((IBindableState)p).Value).Where(v => v != null).ToArray();
        }

        public object GetValueForObject(object ob)
        {
            var prop = this.FirstOrDefault(p => ReferenceEquals(ob, p.Object)) as IBindableState;
            return prop != null ? prop.Value : null;
        }

        public event EventHandler<BindableBroadcastEventArgs> ValueChanged;

        void OnValueChanged(IBindable property, object newValue)
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

        protected override bool Push(object source, object payload)
        {
            lock (_secret)
            {
                if (!Equals(_value, payload))
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
                return string.Format("[Valve: Path={0}, Values={1}]", firstprop.Path, string.Join(",", GetValues().Distinct()));  
            }
            else
            {
                return "[Valve: Path=(none), Value=(none)]";
            }
        }
    }

}

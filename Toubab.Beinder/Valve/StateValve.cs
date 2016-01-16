using System;
using System.Linq;
using Toubab.Beinder.Valve;
using Toubab.Beinder.Util;
using Toubab.Beinder.Bindable;

namespace Toubab.Beinder.Valve
{
    public class StateValve : BroadcastValve
    {
        static readonly object[] _secret = new[] { new object() };
       
        object[] _values = _secret;

        public bool Activate(object toActivate)
        {
            if (toActivate == null)
                return false;
            var prop = this.FirstOrDefault(p => ReferenceEquals(toActivate, p.Object)) as IBindableState;
            if (prop != null)
            {
                var values = prop.Values;
                Push(prop, values);
                OnValuesChanged(prop, values);
                return true;
            }
            return false;
        }

        public object[][] GetChildValveObjects() {
            return this
                .Where(p => p is IBindableState)
                .Select(p => ((IBindableState)p).Values)
                .TransposeWithPadding()
                .Select(enu => enu.ToArray())
                .ToArray();
        }
            
        public object[] GetValuesForObject(object ob) 
        {
            return this
                .Where(p => p is IBindableState && ReferenceEquals(ob, p.Object))
                .Select(p => ((IBindableState)p).Values)
                .FirstOr(new object[0]);
        }

        public event EventHandler<BroadcastEventArgs> ValueChanged;

        void OnValuesChanged(IBindable property, object[] newValue)
        {
            var evt = ValueChanged;
            if (evt != null)
            {
                evt(this, new BroadcastEventArgs(property, newValue));
            }
        }

        protected override void HandleBroadcast(object sender, BroadcastEventArgs e)
        {
            base.HandleBroadcast(sender, e);
            OnValuesChanged(e.Source, e.Payload);
        }

        protected override bool Push(object source, object[] payload)
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
                return "[Valve: Path=(none)";
            }
        }
    }

}

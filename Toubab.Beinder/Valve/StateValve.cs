namespace Toubab.Beinder.Valve
{
    using System;
    using System.Linq;
    using Tools;
    using Bindable;

    public class StateValve : BroadcastValve
    {
        static readonly object[] _secret = new[] { new object() };
       
        object[] _values = _secret;

        public bool Activate(object toActivate)
        {
            if (toActivate == null)
                return false;
            var prop = this.FirstOrDefault(p => ReferenceEquals(toActivate, p.Object)) as IProperty;
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

        protected override void HandleBroadcast(object sender, BroadcastEventArgs e)
        {
            base.HandleBroadcast(sender, e);
            OnValuesChanged(e.SourceObject, e.Payload);
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

    }

}

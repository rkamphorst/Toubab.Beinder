using System;
using System.Linq;
using Toubab.Beinder.Valve;

namespace Toubab.Beinder
{
    public class AggregateBindableState : IBindableState
    {
        readonly IBindableState[] _states;

        public AggregateBindableState(IBindableState[] states)
        {
            _states = states;
            _value = states[0].Value;
            foreach (var prop in states)
            {
                prop.Broadcast += HandleContainedBroadcast;
            }
        }

        void HandleContainedBroadcast(object sender, BindableBroadcastEventArgs e)
        {
            if (!Equals(_value, e.Argument))
            {
                _value = e.Argument;
                OnBroadcast(e.Argument);
            }
        }

        void OnBroadcast(object argument)
        {
            if (Broadcast != null)
                Broadcast(this, new BindableBroadcastEventArgs(this, argument));
        }

        public event EventHandler<BindableBroadcastEventArgs> Broadcast;


        public Type ValueType { get { return _states[0].ValueType; } }

        object _value;

        public object Value
        {
            get
            {
                _value = _states[0].Value;
                return _value;
            }
        }

        public bool TryHandleBroadcast(object argument)
        {
            // first, make sure _value is up to date
            _value = _states[0].Value;

            // if new value equals old value, do nothing.
            if (Equals(_value, argument))
                return false;

            // prevent lots of events from propagating
            // by setting _value first.
            // That way, HandleContainedPropertyValueChanged won't call 
            // OnValueChanged
            _value = argument;

            // write the property, try each one until one accepts
            foreach (var prop in _states)
            {
                if (prop.TryHandleBroadcast(argument))
                {
                    // Now, call OnValueChanged to make sure the event is fired exactly once.
                    OnBroadcast(argument);
                    return true;
                }
            }
            _value = _states[0].Value;
            return false;
        }

        public object Object
        {
            get
            {
                return _states[0].Object;
            }
        }

        public void SetObject(object value)
        {
            foreach (var prop in _states)
                prop.SetObject(value);
        }

        public Path Path
        {
            get
            {
                return _states[0].Path;
            }
        }

        public IBindable CloneWithoutObject()
        {
            return new AggregateBindableState(_states.Select(p => (IBindableState)p.CloneWithoutObject()).ToArray());
        }

        public override string ToString()
        {
            return string.Format("[AggregateProperty: Path={0}, Value={1}]", Path, Value);
        }
    }

}

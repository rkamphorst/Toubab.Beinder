using System;
using System.Linq;
using Toubab.Beinder.Valve;

namespace Toubab.Beinder.Bindable
{
    public class CombinedState : IBindableState
    {
        readonly IBindableState[] _states;

        public CombinedState(IBindableState[] states)
        {
            _states = states;
            _values = states[0].Values;
            foreach (var prop in states)
            {
                prop.Broadcast += HandleContainedBroadcast;
            }
        }

        void HandleContainedBroadcast(object sender, BroadcastEventArgs e)
        {
            if (!Equals(_values, e.Payload))
            {
                _values = e.Payload;
                OnBroadcast(e.Payload);
            }
        }

        void OnBroadcast(object[] argument)
        {
            if (Broadcast != null)
                Broadcast(this, new BroadcastEventArgs(this, argument));
        }

        public event EventHandler<BroadcastEventArgs> Broadcast;


        public Type[] ValueTypes { get { return _states[0].ValueTypes; } }

        object[] _values;

        public object[] Values
        {
            get
            {
                _values = _states[0].Values;
                return _values;
            }
        }

        public bool TryHandleBroadcast(object[] argument)
        {
            // first, make sure _value is up to date
            _values = _states[0].Values;

            // if new value equals old value, do nothing.
            if (_values.SequenceEqual(argument))
                return false;

            // prevent lots of events from propagating
            // by setting _value first.
            // That way, HandleContainedPropertyValueChanged won't call 
            // OnValueChanged
            _values = argument;

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
            _values = _states[0].Values;
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
            return new CombinedState(_states.Select(p => (IBindableState)p.CloneWithoutObject()).ToArray());
        }

    }

}

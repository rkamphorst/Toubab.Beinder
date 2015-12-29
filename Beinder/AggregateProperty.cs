using System;
using System.Linq;

namespace Beinder
{
    public class AggregateProperty : IProperty
    {
        readonly IProperty[] _properties;

        public AggregateProperty(IProperty[] properties)
        {
            _properties = properties;
            _value = properties[0].Value;
            foreach (var prop in properties) 
            {
                prop.ValueChanged += HandleContainedPropertyValueChanged;
            }
        }

        void HandleContainedPropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            if (!Equals(_value, e.NewValue))
            {
                _value = e.NewValue;
                OnValueChanged(e.NewValue);
            }
        }

        void OnValueChanged(object newValue)
        {
            if (ValueChanged != null)
                ValueChanged(this, new PropertyValueChangedEventArgs(this, newValue));
        }

        public event EventHandler<PropertyValueChangedEventArgs> ValueChanged;

        object _value;

        public object Value
        {
            get
            {
                _value = _properties[0].Value;
                return _value;
            }
        }

        public bool TrySetValue(object value)
        {
            // first, make sure _value is up to date
            _value = _properties[0].Value;

            // if new value equals old value, do nothing.
            if (Equals(value, _value))
                return false;

            // prevent lots of events from propagating
            // by setting _value first.
            // That way, HandleContainedPropertyValueChanged won't call 
            // OnValueChanged
            _value = value;

            // write the property, try each one until one accepts
            foreach (var prop in _properties)
            {
                if (prop.TrySetValue(value))
                {
                    // Now, call OnValueChanged to make sure the event is fired exactly once.
                    OnValueChanged(_value);
                    return true;
                }
            }
            _value = _properties[0].Value;
            return false;
        }

        public object Object
        {
            get
            {
                return _properties[0].Object;
            }
        }

        public void SetObject(object value)
        {
            foreach (var prop in _properties)
                prop.SetObject(value);
        }

        public PropertyPath Path
        {
            get
            {
                return _properties[0].Path;
            }
        }

        public IProperty CloneWithoutObject()
        {
            return new AggregateProperty(_properties.Select(p => p.CloneWithoutObject()).ToArray());
        }

        public override string ToString()
        {
            return string.Format("[AggregateProperty: Path={0}, Value={1}]", Path, Value);
        }
    }

}

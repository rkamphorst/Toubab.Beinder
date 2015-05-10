using System;
using System.Collections.Generic;
using System.Linq;

namespace Beinder
{
    public class AggregateProperty : IProperty
    {
        readonly IProperty[] _properties;
        readonly IProperty _readProperty;
        readonly IProperty _writeProperty;

        public AggregateProperty(IProperty[] properties)
        {
            _properties = properties;
            IProperty readProp = null;
            IProperty writeProp = null;
            foreach (var prop in _properties)
            {
                prop.ValueChanged += HandleContainedPropertyValueChanged;
                if (readProp == null)
                {
                    if (prop.IsReadable)
                        readProp = prop;
                }
                if (writeProp == null)
                {
                    if (prop.IsWritable)
                        writeProp = prop;
                }
            }
            _readProperty = readProp;
            _writeProperty = writeProp;
            _value = _readProperty != null ? _readProperty.Value : null;
        }

        void HandleContainedPropertyValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (!Equals(_value, e.NewValue))
            {
                OnValueChanged(e.NewValue);
            }
        }

        void OnValueChanged(object newValue)
        {
            if (ValueChanged != null)
                ValueChanged(this, new ValueChangedEventArgs(newValue));
        }

        public event EventHandler<ValueChangedEventArgs> ValueChanged;

        public Type ValueType
        {
            get
            {
                return _properties.FirstOrDefault(p => p.ValueType != null)?.ValueType;
            }
        }

        public Type ObjectType
        {
            get
            {
                return _properties.FirstOrDefault(p => p.ObjectType != null)?.ObjectType;
            }
        }

        public bool IsReadable { get { return _readProperty != null; } }

        public bool IsWritable { get { return _writeProperty != null; } }

        object _value;

        public object Value
        {
            get
            {
                _value = _readProperty != null ? _readProperty.Value : null;
                return _value;
            }
        }

        public bool TrySetValue(object value)
        {
            // first, make sure _value is up to date
            _value = _readProperty != null ? _readProperty.Value : null;

            // if new value equals old value, do nothing.
            if (Equals(value, _value))
                return false;

            // prevent lots of events from propagating
            // by setting _value first.
            // That way, HandleValueChanged won't call 
            // OnValueChanged
            _value = value;

            // write the property
            if (_writeProperty != null && _writeProperty.TrySetValue(value))
            {
                // Now, call OnValueChanged to make sure the event is fired exactly once.
                OnValueChanged(_value);
                return true;
            }
            return false;
        }

        public object Object
        {
            get
            {
                return _readProperty.Object;
            }
        }

        public bool TrySetObject(object value)
        {
            bool result = false;
            foreach (var prop in _properties)
                result |= prop.TrySetObject(value);
            return result;
        }

        public PropertyPath Path
        {
            get
            {
                return _readProperty.Path;
            }
        }

        public IProperty Clone()
        {
            return new AggregateProperty(_properties.Select(p => p.Clone()).ToArray());
        }

        public override string ToString()
        {
            return string.Format("[AggregateProperty: Path={0}, Value={1}]", Path, Value);
        }
    }

}

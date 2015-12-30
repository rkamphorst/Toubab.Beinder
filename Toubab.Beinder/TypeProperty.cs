using System;
using System.Reflection;

namespace Toubab.Beinder
{
    public abstract class TypeProperty : IProperty
    {
        protected readonly PropertyInfo _propertyInfo;
        protected readonly IPropertyPathParser _pathParser;

        protected TypeProperty(IPropertyPathParser pathParser, PropertyInfo property)
        {
            _propertyInfo = property;
            _pathParser = pathParser;
        }

        protected abstract void DetachObjectPropertyChangeEvent(object obj);

        protected abstract void AttachObjectPropertyChangeEvent(object obj);

        protected void OnValueChanged(object source, EventArgs args)
        {
            var evt = ValueChanged;
            if (evt != null)
                evt(this, new PropertyValueChangedEventArgs(this, Value));
        }

        public event EventHandler<PropertyValueChangedEventArgs> ValueChanged;

        object _object;

        public object Object
        { 
            get { return _object; }
        }

        public void SetObject(object value)
        {
            var t = _object;
            if (t != null)
                DetachObjectPropertyChangeEvent(t);

            t = _object = value;

            if (t != null)
                AttachObjectPropertyChangeEvent(t);
        }

        PropertyPath _path;

        public PropertyPath Path
        {
            get
            { 
                if (_path == null)
                    _path = _pathParser.Parse(_propertyInfo.Name); 
                return _path;
            }
        }

        public object Value
        {
            get
            { 
                if (!_propertyInfo.CanRead)
                    return null;

                var t = Object;
                return t == null ? null : _propertyInfo.GetValue(t); 
            }
        }

        public bool TrySetValue(object value)
        {
            if (!_propertyInfo.CanWrite)
                return false;

            var t = Object;
            if (t != null && (
                    value == null ||
                    _propertyInfo.PropertyType.GetTypeInfo().IsAssignableFrom(
                        value.GetType().GetTypeInfo()
                    )
                ))
            {
                _propertyInfo.SetValue(t, value);
                return true;
            }
            return false;
        }


        public abstract IProperty CloneWithoutObject();

        public override string ToString()
        {
            return string.Format("[{0}: Path={1}, Value={2}]", GetType().Name, Path, Value);
        }
    }

}

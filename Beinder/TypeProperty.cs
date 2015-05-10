using System;
using System.Reflection;

namespace Beinder
{
    public abstract class TypeProperty : IProperty
    {
        protected readonly PropertyInfo _propertyInfo;
        protected readonly IPropertyPathParser _pathParser;
        readonly bool _canRead;
        readonly bool _canWrite;

        protected TypeProperty(Type type, IPropertyPathParser pathParser, PropertyInfo property)
        {
            _propertyInfo = property;
            ObjectType = type;
            ValueType = property.PropertyType;
            _pathParser = pathParser;
            _canRead = property.GetMethod != null && property.GetMethod.IsPublic;
            _canWrite = property.SetMethod != null && property.SetMethod.IsPublic;
        }

        protected abstract void DetachObjectPropertyChangeEvent(object obj);

        protected abstract void AttachObjectPropertyChangeEvent(object obj);

        protected void OnValueChanged(object source, EventArgs args)
        {
            var evt = ValueChanged;
            if (evt != null)
                evt(this, new ValueChangedEventArgs(Value));
        }

        public Type ObjectType { get; private set; }

        public Type ValueType { get; private set; }

        public event EventHandler<ValueChangedEventArgs> ValueChanged;

        object _object;

        public object Object
        { 
            get { return _object; }
        }

        public bool TrySetObject(object value)
        {
            var t = _object;
            if (value == null || _propertyInfo.DeclaringType.GetTypeInfo()
                .IsAssignableFrom(value.GetType().GetTypeInfo()))
            {
                if (t != null)
                    DetachObjectPropertyChangeEvent(t);

                t = _object = value;

                if (t != null)
                    AttachObjectPropertyChangeEvent(t);
                return true;
            }
            return false;
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

        public bool IsReadable { get { return _canRead; } }

        public bool IsWritable { get { return _canWrite; } }

        public object Value
        {
            get
            { 
                if (!_canRead)
                    return null;

                var t = Object;
                return t == null ? null : _propertyInfo.GetValue(t); 
            }
        }

        public bool TrySetValue(object value)
        {
            if (!_canWrite)
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


        public abstract IProperty Clone();

        public override string ToString()
        {
            return string.Format("[NotProp: Path={0}, Value={1}]", Path, Value);
        }
    }

}

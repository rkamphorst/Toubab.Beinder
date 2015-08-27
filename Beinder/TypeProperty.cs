using System;
using System.Reflection;

namespace Beinder
{
    public abstract class TypeProperty : IProperty
    {
        protected readonly PropertyInfo _propertyInfo;
        protected readonly IPropertyPathParser _pathParser;
        readonly PropertyMetaInfo _metaInfo;

        protected TypeProperty(Type type, IPropertyPathParser pathParser, PropertyInfo property)
        {
            _propertyInfo = property;
            _pathParser = pathParser;
            _metaInfo = new PropertyMetaInfo(
                type,
                property.PropertyType,
                property.GetMethod != null && property.GetMethod.IsPublic,
                property.SetMethod != null && property.SetMethod.IsPublic
            );
        }

        protected abstract void DetachObjectPropertyChangeEvent(object obj);

        protected abstract void AttachObjectPropertyChangeEvent(object obj);

        protected void OnValueChanged(object source, EventArgs args)
        {
            var evt = ValueChanged;
            if (evt != null)
                evt(this, new ValueChangedEventArgs(Value));
        }

        public PropertyMetaInfo MetaInfo 
        {
            get { return _metaInfo; }
        }

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

        public object Value
        {
            get
            { 
                if (!MetaInfo.IsReadable)
                    return null;

                var t = Object;
                return t == null ? null : _propertyInfo.GetValue(t); 
            }
        }

        public bool TrySetValue(object value)
        {
            if (!MetaInfo.IsWritable)
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

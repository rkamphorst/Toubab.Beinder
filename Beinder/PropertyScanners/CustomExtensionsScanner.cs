using System;
using System.Collections.Generic;

namespace Beinder.PropertyScanners
{
    public class CustomExtensionsScanner : IObjectPropertyScanner
    {
        readonly IObjectPropertyScanner _extensionsScanner;
        readonly TypeAdapterFactory<IExtensions> _adapterFactory;

        public CustomExtensionsScanner(IObjectPropertyScanner extensionsScanner)
        {
            _adapterFactory = new TypeAdapterFactory<IExtensions>();
            _extensionsScanner = extensionsScanner;
        }

        public TypeAdapterRegistry<IExtensions> AdapterRegistry { get { return _adapterFactory.Registry; } }

        public IEnumerable<IProperty> Scan(object ob)
        {
            Type objectType = ob.GetType();

            foreach (var ext in _adapterFactory.GetAdaptersFor(objectType))
            {
                foreach (IProperty prop in _extensionsScanner.Scan(ext))
                {
                    if (prop.TrySetObject(ext))
                    {
                        yield return new ExtensionProperty(objectType, prop);
                    }
                }
            }
        }

        class ExtensionProperty : IProperty
        {
            readonly PropertyMetaInfo _metaInfo;
            readonly Type _objectType;
            readonly IProperty _property;

            public ExtensionProperty(Type objectType, IProperty property)
            {
                _property = property;
                _objectType = objectType;
                _property.ValueChanged += (sender, e) =>
                {
                    var evt = ValueChanged;
                    if (evt != null)
                        evt(sender, e);
                };
                _metaInfo = new PropertyMetaInfo(
                    objectType,
                    property.MetaInfo.ValueType,
                    property.MetaInfo.IsReadable,
                    property.MetaInfo.IsWritable
                );
            }

            public event EventHandler<ValueChangedEventArgs> ValueChanged;

            public bool TrySetObject(object newObject)
            {
                var ext = _property.Object as IExtensions;
                if (ext != null && ext.TrySetObject(newObject))
                {
                    _object = newObject;
                    return true;
                }
                return false;
            }

            public bool TrySetValue(object newValue)
            {
                return _property.TrySetValue(newValue);
            }

            public IProperty Clone()
            {
                var prop = _property.Clone();
                prop.TrySetObject(((IExtensions) _property.Object).Clone());
                var result = new ExtensionProperty(_objectType, prop);
                result.TrySetObject(Object);
                return result;
            }

            public PropertyMetaInfo MetaInfo
            {
                get
                {
                    return _metaInfo;
                }
            }

            object _object;

            public object Object
            { 
                get { return _object; }
            }

            public PropertyPath Path
            {
                get
                {
                    return _property.Path;
                }
            }

            public object Value
            {
                get
                {
                    return _property.Value;
                }
            }

            public override string ToString()
            {
                return string.Format("[ExtProp: Path={0}, Value={1}]", Path, Value);
            }
        }
    }
}


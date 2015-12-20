using System;
using System.Collections.Generic;

namespace Beinder.PropertyScanners
{
    public class CustomExtensionsScanner : IObjectPropertyScanner
    {
        readonly TypeAdapterRegistry<IExtensions> _adapterRegistry;
        readonly IObjectPropertyScanner _extensionsScanner;

        protected CustomExtensionsScanner(IObjectPropertyScanner extensionsScanner)
        {
            _adapterRegistry = new TypeAdapterRegistry<IExtensions>();
            _extensionsScanner = extensionsScanner;
        }

        public TypeAdapterRegistry<IExtensions> AdapterRegistry { get { return _adapterRegistry; } }

        public IEnumerable<IProperty> Scan(object ob)
        {
            Type objectType = ob.GetType();

            foreach (IExtensions ext in AdapterRegistry.FindAdaptersTypesFor(objectType))
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
                return new ExtensionProperty(_objectType, _property);
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
        }
    }
}


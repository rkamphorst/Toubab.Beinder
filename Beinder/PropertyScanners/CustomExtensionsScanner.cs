﻿using System;
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
                    prop.SetObject(ext);
                    yield return new ExtensionProperty(objectType, prop);
                }
            }
        }

        class ExtensionProperty : IProperty
        {
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
            }

            public event EventHandler<PropertyValueChangedEventArgs> ValueChanged;

            public void SetObject(object newObject)
            {
                var ext = _property.Object as IExtensions;
                ext.SetObject(newObject);
                _object = newObject;
            }

            public bool TrySetValue(object newValue)
            {
                return _property.TrySetValue(newValue);
            }

            public IProperty CloneWithoutObject()
            {
                var prop = _property.CloneWithoutObject();
                prop.SetObject(((IExtensions) _property.Object).CloneWithoutObject());
                return new ExtensionProperty(_objectType, prop);
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


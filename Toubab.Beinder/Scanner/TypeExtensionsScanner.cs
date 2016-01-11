using System;
using System.Collections.Generic;
using Toubab.Beinder.Valve;

namespace Toubab.Beinder.Scanner
{
    public class TypeExtensionsScanner : IBindableScanner
    {
        readonly IBindableScanner _extensionsScanner;
        readonly TypeAdapterFactory<ITypeExtension> _adapterFactory;

        public TypeExtensionsScanner(IBindableScanner extensionsScanner)
        {
            _adapterFactory = new TypeAdapterFactory<ITypeExtension>();
            _extensionsScanner = extensionsScanner;
        }

        public TypeAdapterRegistry<ITypeExtension> AdapterRegistry { get { return _adapterFactory.Registry; } }

        public IEnumerable<IBindable> Scan(object ob)
        {
            Type objectType = ob.GetType();

            foreach (var ext in _adapterFactory.GetAdaptersFor(objectType))
            {
                foreach (IBindableState prop in _extensionsScanner.Scan(ext))
                {
                    prop.SetObject(ext);
                    yield return new ExtensionProperty(objectType, prop);
                }
            }
        }

        class ExtensionProperty : IBindableState
        {
            readonly Type _objectType;
            readonly IBindableState _property;

            public ExtensionProperty(Type objectType, IBindableState property)
            {
                _property = property;
                _objectType = objectType;
                _property.Broadcast += (sender, e) =>
                {
                    var evt = Broadcast;
                    if (evt != null)
                        evt(sender, e);
                };
            }

            public event EventHandler<BindableBroadcastEventArgs> Broadcast;

            public void SetObject(object newObject)
            {
                var ext = _property.Object as ITypeExtension;
                ext.SetObject(newObject);
                _object = newObject;
            }

            public bool TryHandleBroadcast(object newValue)
            {
                return _property.TryHandleBroadcast(newValue);
            }

            public IBindable CloneWithoutObject()
            {
                var prop = (IBindableState)_property.CloneWithoutObject();
                prop.SetObject(((ITypeExtension) _property.Object).CloneWithoutObject());
                return new ExtensionProperty(_objectType, prop);
            }

            object _object;

            public object Object
            { 
                get { return _object; }
            }

            public Path Path
            {
                get
                {
                    return _property.Path;
                }
            }

            public Type ValueType { get { return _property.ValueType; } }

            public object Value
            {
                get
                {
                    var t = Object;
                    return t == null ? null : _property.Value;
                }
            }

            public override string ToString()
            {
                return string.Format("[ExtProp: Path={0}, Value={1}]", Path, Value);
            }
        }
    }
}


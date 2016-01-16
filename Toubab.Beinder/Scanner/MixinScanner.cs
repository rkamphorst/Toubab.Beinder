using System;
using System.Collections.Generic;
using Toubab.Beinder.Bindable;

namespace Toubab.Beinder.Scanner
{
    public class MixinScanner : IScanner
    {
        readonly IScanner _scanner;
        readonly TypeAdapterFactory<IMixin> _adapterFactory;

        public MixinScanner(IScanner scanner)
        {
            _adapterFactory = new TypeAdapterFactory<IMixin>();
            _scanner = scanner;
        }

        public TypeAdapterRegistry<IMixin> AdapterRegistry { get { return _adapterFactory.Registry; } }

        public IEnumerable<IBindable> Scan(object ob)
        {
            Type objectType = ob.GetType();

            foreach (var ext in _adapterFactory.GetAdaptersFor(objectType))
            {
                foreach (IBindable prop in _scanner.Scan(ext))
                {
                    var sprop = prop as IBindableState;
                    if (sprop != null)
                    {
                        sprop.SetObject(ext);
                        yield return new MixinProperty(objectType, sprop);
                    }
                }
            }
        }

        class MixinProperty : IBindableState
        {
            readonly Type _objectType;
            readonly IBindableState _property;

            public MixinProperty(Type objectType, IBindableState property)
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

            public event EventHandler<BroadcastEventArgs> Broadcast;

            public void SetObject(object newObject)
            {
                var ext = _property.Object as IMixin;
                ext.SetObject(newObject);
                _object = newObject;
            }

            public bool TryHandleBroadcast(object[] newValue)
            {
                return _property.TryHandleBroadcast(newValue);
            }

            public IBindable CloneWithoutObject()
            {
                var prop = (IBindableState)_property.CloneWithoutObject();
                prop.SetObject(((IMixin)_property.Object).CloneWithoutObject());
                return new MixinProperty(_objectType, prop);
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

            public Type[] ValueType { get { return _property.ValueType; } }

            public object[] Values
            {
                get
                {
                    var t = Object;
                    return t == null ? new object[] { null } : _property.Values;
                }
            }

            public override string ToString()
            {
                return string.Format("[ExtProp: Path={0}]", Path);
            }
        }
    }
}


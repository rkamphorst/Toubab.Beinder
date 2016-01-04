using System;
using System.Collections.Generic;
using Toubab.Beinder.Valve;

namespace Toubab.Beinder.PropertyScanners
{
    public class CustomBindableScanner : IBindableScanner
    {
        readonly TypeAdapterFactory<ICustomBindable> _adapterFactory;

        public CustomBindableScanner()
        {
            _adapterFactory = new TypeAdapterFactory<ICustomBindable>();
        }

        public TypeAdapterRegistry<ICustomBindable> AdapterRegistry { get { return _adapterFactory.Registry; } }

        public IEnumerable<IBindable> Scan(object ob)
        {
            Type objectType = ob.GetType();

            foreach (var prop in _adapterFactory.GetAdaptersFor(objectType))
            {
                yield return prop;
            }
        }

    }
}


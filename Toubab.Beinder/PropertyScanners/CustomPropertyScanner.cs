using System;
using System.Collections.Generic;

namespace Toubab.Beinder.PropertyScanners
{
    public class CustomPropertyScanner : IPropertyScanner
    {
        readonly TypeAdapterFactory<IProperty> _adapterFactory;

        public CustomPropertyScanner()
        {
            _adapterFactory = new TypeAdapterFactory<IProperty>();
        }

        public TypeAdapterRegistry<IProperty> AdapterRegistry { get { return _adapterFactory.Registry; } }

        public IEnumerable<IProperty> Scan(object ob)
        {
            Type objectType = ob.GetType();

            foreach (var prop in _adapterFactory.GetAdaptersFor(objectType))
            {
                yield return prop;
            }
        }

    }
}


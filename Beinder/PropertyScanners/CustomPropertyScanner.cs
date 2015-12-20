using System;
using System.Collections.Generic;

namespace Beinder.PropertyScanners
{
    public class CustomPropertyScanner<TParent, TNode> : IObjectPropertyScanner
    {
        readonly TypeAdapterFactory<IProperty> _adapterFactory;

        protected CustomPropertyScanner()
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


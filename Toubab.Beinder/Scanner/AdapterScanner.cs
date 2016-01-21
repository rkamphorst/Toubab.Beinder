using System;
using Toubab.Beinder.Tools;

namespace Toubab.Beinder.Scanner
{
    public abstract class AdapterScanner<IAdapter> : TypeScanner
        where IAdapter : class
    {
        readonly TypeAdapterFactory<IAdapter> _adapterFactory;

        protected AdapterScanner()
        {
            _adapterFactory = new TypeAdapterFactory<IAdapter>();
        }

        protected TypeAdapterFactory<IAdapter> AdapterFactory { get { return _adapterFactory; } }

        public TypeAdapterRegistry<IAdapter> AdapterRegistry { get { return _adapterFactory.Registry; } }
    }
}


using System;
using System.Collections.Generic;

namespace Toubab.Beinder.Tools
{
    public class TypeAdapterFactory<IAdapter>
        where IAdapter : class
    {

        readonly static Func<Type, IAdapter> DefaultCreator = 
            new Func<Type, IAdapter>(type => (IAdapter)Activator.CreateInstance(type));

        readonly Dictionary<Type, IAdapter> _singletonCache;
        readonly TypeAdapterRegistry<IAdapter> _typeAdapterRegistry;
        readonly Func<Type, IAdapter> _adapterCreator;

        public TypeAdapterFactory(
            bool singletonAdapters = false,
            TypeAdapterRegistry<IAdapter> registry = null,
            Func<Type,IAdapter> creator = null
        ) 
        {
            _typeAdapterRegistry = registry ?? new TypeAdapterRegistry<IAdapter>();
            _adapterCreator = creator ?? DefaultCreator;
            _singletonCache = singletonAdapters ? new Dictionary<Type, IAdapter>() : null;
        }
            
        public TypeAdapterRegistry<IAdapter> Registry { get { return _typeAdapterRegistry; } }

        public IEnumerable<IAdapter> GetAdaptersFor(Type adapteeType) 
        {
            foreach (var adapterType in _typeAdapterRegistry.FindAdapterTypesFor(adapteeType)) 
            {
                IAdapter result = null;
                if (_singletonCache == null || !_singletonCache.TryGetValue(adapterType, out result)) 
                {
                    result = _adapterCreator(adapterType);
                    if (_singletonCache != null)
                        _singletonCache[adapterType] = result;
                }
                yield return result;
            }
        }
    }
}


using System.Collections.Generic;
using System;
using System.Linq;
using Toubab.Beinder.Valve;

namespace Toubab.Beinder
{
    /// <summary>
    /// Abstract class for strongly typed scanners
    /// </summary>
    public abstract class TypePropertyScanner : IBindableScanner
    {
        readonly Dictionary<Type, List<IBindableState>> _typeCache = new Dictionary<Type, List<IBindableState>>();

        public IEnumerable<IBindable> Scan(object obj)
        {
            var type = obj.GetType();
            List<IBindableState> result;
            if (!_typeCache.TryGetValue(type, out result))
            {
                result = Scan(type).ToList();
                _typeCache[type] = result;
            }
            return result;
        }


        /// <summary>
        /// Scan the specified type and enumerate the properties
        /// </summary>
        /// <param name="type">Type to scan.</param>
        public abstract IEnumerable<IBindableState> Scan(Type type);
    }
        
}

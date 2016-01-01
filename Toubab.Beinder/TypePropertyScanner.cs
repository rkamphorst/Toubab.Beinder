using System.Collections.Generic;
using System;
using System.Linq;

namespace Toubab.Beinder
{
    /// <summary>
    /// Abstract class for strongly typed scanners
    /// </summary>
    public abstract class TypePropertyScanner : IPropertyScanner
    {
        readonly Dictionary<Type, List<IProperty>> _typeCache = new Dictionary<Type, List<IProperty>>();

        public IEnumerable<IProperty> Scan(object obj)
        {
            var type = obj.GetType();
            List<IProperty> result;
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
        public abstract IEnumerable<IProperty> Scan(Type type);
    }
        
}

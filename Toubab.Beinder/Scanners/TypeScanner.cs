namespace Toubab.Beinder.Scanners
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Bindables;

    /// <summary>
    /// Abstract class for strongly typed scanners
    /// </summary>
    /// <remarks>
    /// A strongly typed scanner scans a *type*, not an object; it is assumed 
    /// that an object of the same type always has the same properties (which 
    /// does not have to be the case).
    /// </remarks>
    public abstract class TypeScanner : IScanner
    {
        readonly Dictionary<Type, List<IBindable>> _typeCache = new Dictionary<Type, List<IBindable>>();

        /// <inheritdoc />
        public IEnumerable<IBindable> Scan(object obj)
        {
            var type = obj.GetType();
            List<IBindable> result;
            if (!_typeCache.TryGetValue(type, out result))
            {
                result = Scan(type).ToList();
                _typeCache[type] = result;
            }
            return result;
        }


        /// <summary>
        /// Scan the specified type and enumerate the bindable members.
        /// </summary>
        /// <param name="type">Type to scan.</param>
        public abstract IEnumerable<IBindable> Scan(Type type);
    }
        
}
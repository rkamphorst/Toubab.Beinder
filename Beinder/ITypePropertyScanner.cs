using System.Collections.Generic;
using System;

namespace Beinder
{
    /// <summary>
    /// Interface for a property scanner of types.
    /// </summary>
    public interface ITypePropertyScanner
    {
        /// <summary>
        /// Scan the specified type and enumerate the properties
        /// </summary>
        /// <param name="type">Type to scan.</param>
        IEnumerable<IProperty> Scan(Type type);
    }

}

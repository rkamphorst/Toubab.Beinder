using System.Collections.Generic;
using System;
using Toubab.Beinder.Valve;

namespace Toubab.Beinder
{

    /// <summary>
    /// Interface for a property scanner of object instances.
    /// </summary>
    public interface IBindableScanner
    {
        /// <summary>
        /// Scan the specified object and enumerate the properties
        /// </summary>
        /// <remarks>
        /// Every returned property should *not* have the object associated
        /// to it! This is done by the binder at the latest stage possible.
        /// </remarks>
        /// <param name="obj">Object to scan.</param>
        IEnumerable<IBindable> Scan(object obj);
    }

}
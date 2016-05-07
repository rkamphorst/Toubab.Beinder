
namespace Toubab.Beinder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Bindables;
    using Paths;
    using Valves;

    /// <summary>
    /// Constructed bindings (returned by <see cref="Binder.Bind(object[])"/>).
    /// </summary>
    /// <remarks>
    /// The bindings implement IDisposable, for easy disposal of all bindings 
    /// that are constructed by <see cref="Binder.Bind(object[])"/>.
    /// 
    /// Furthermore, <see cref="IBindings"/> can be iterated to produce
    /// the different (top-level) groups of bound <see cref="IBindable"/> 
    /// instances. Child bindables (i.e., "properties of properties") that are
    /// bound are not accessible.
    /// </remarks>
    public interface IBindings : IDisposable, IEnumerable<IGrouping<Path,Outlet.Attachment>>
    {
    }
 

}

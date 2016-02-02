namespace Toubab.Beinder.Bindables
{
    using System;
    using System.Linq;
    using Mixins;
    using Paths;

    /// <summary>
    /// Base class for classes that implement <see cref="IBindable"/>
    /// </summary>
    /// <seealso cref="CombinedBindable{T}"/>
    /// <seealso cref="DelegatedBindable{T}"/>
    /// <seealso cref="ReflectedBindable{T}"/>
    public abstract class Bindable : Mixin, IBindable
    {
        readonly Path _path;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">Set the <see cref="Path"/> of the bindable to this value</param>
        protected Bindable(Path path)
        {
            _path = path;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="toCopy">To copy.</param>
        protected Bindable(Bindable toCopy)
        {
            _path = toCopy._path;
        }

        /// <inheritdoc/>
        public object Object 
        { 
            get 
            { 
                return GetObject(); 
            } 
        }

        /// <inheritdoc/>
        public Path Path
        {
            get
            { 
                return _path;
            }
        }

        /// <inheritdoc/>
        public abstract Type[] ValueTypes { get; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0}: {1}->{2} ({3})", 
                GetType().Name,
                Object == null ? "[?]" : Object.GetType().Name, 
                Path, 
                string.Join(",", ValueTypes.Select(vt => vt.Name))
            );
        }

    }
}

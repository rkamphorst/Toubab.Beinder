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
        readonly Fragment _nameSyllables;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="nameSyllables">Set the <see cref="Name"/> of the bindable to this value</param>
        protected Bindable(Fragment nameSyllables)
        {
            _nameSyllables = nameSyllables;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="toCopy">To copy.</param>
        protected Bindable(Bindable toCopy)
        {
            _nameSyllables = toCopy._nameSyllables;
        }

        /// <inheritdoc/>
        public abstract BindingOperations Capabilities { get; }

        /// <inheritdoc/>
        public object Object 
        { 
            get 
            { 
                return GetObject(); 
            } 
        }

        /// <inheritdoc/>
        public Fragment Name
        {
            get
            { 
                return _nameSyllables;
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
                Name, 
                string.Join(",", ValueTypes.Select(vt => vt.Name))
            );
        }

    }
}

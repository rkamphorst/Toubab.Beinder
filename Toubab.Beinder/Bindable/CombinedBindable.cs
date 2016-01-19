using System;
using System.Linq;

namespace Toubab.Beinder.Bindable
{
    /// <summary>
    /// Base class for combined bindables.
    /// </summary>
    /// <seealso cref="CombinedProperty"/>
    /// <seealso cref="CombinedEventHandler"/>
    public abstract class CombinedBindable<TBindable> : Bindable
        where TBindable : IBindable
    {
        /// <summary>
        /// The bindables this bindable is a combination of (read-only).
        /// </summary>
        readonly protected TBindable[] Bindables;

        /// <summary>
        /// Protected constructor
        /// </summary>
        /// <param name="bindables">The bindables this bindable is a combination of.</param>
        protected CombinedBindable(TBindable[] bindables)
            : base(bindables[0].Path)
        {
            Bindables = bindables;
        }

        /// <summary>
        /// Copy constructor (to be used by <see cref="CloneWithoutObject"/>).
        /// The object this bindable belongs to is not copied.
        /// </summary>
        /// <param name="toCopy">The object to copy into a new instance.</param>
        protected CombinedBindable(CombinedBindable<TBindable> toCopy)
            : this(toCopy.Bindables.Select(b => (TBindable)b.CloneWithoutObject()).ToArray())
        {
        }

        /// <inheritdoc/>
        public override Type[] ValueTypes
        {
            get
            {
                return Bindables[0].ValueTypes;
            }
        }

        /// <inheritdoc/>
        public override void SetObject(object value)
        {
            foreach (var b in Bindables)
                b.SetObject(value);
            base.SetObject(value);
        }
    }
}


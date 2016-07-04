namespace Toubab.Beinder.Bindables
{
    using System;

    /// <summary>
    /// Base class for combined bindables.
    /// </summary>
    /// <remarks>
    /// Multiple <see cref="IBindable"/> instances are combined as follows.
    /// 
    /// **Set Object**: <see cref="SetObject"/> results in the object being set for all
    /// contained properties.
    /// 
    /// <see cref="ValueTypes"/> and <see cref="Path"/> are all taken from the first event handler
    /// in the list. 
    /// 
    /// For more information on how other properties / methods are combined, see the
    /// more specific implementations.
    /// </remarks>
    /// <seealso cref="CombinedEvent"/>
    /// <seealso cref="CombinedEventHandler"/>
    /// <seealso cref="CombinedProperty"/>
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
            : base(bindables[0].NameSyllables)
        {
            Bindables = bindables;
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


using System;
using Toubab.Beinder.Annex;

namespace Toubab.Beinder.Bindable
{
    /// <summary>
    /// Base class for a bindable that delegates to another bindable.
    /// </summary>
    /// <remarks>
    /// Concrete subclasses of this class are used by <see cref="Scanner.MixinScanner"/>
    /// to delegate to propeties, events and event handlers (methods) on mix-ins.
    /// </remarks>
    /// <seealso cref="DelegatedEvent"/>
    /// <seealso cref="DelegatedEventHandler"/>
    /// <seealso cref="DelegatedProperty"/>
    /// <seealso cref="Mixin.IMixin{T}"/>
    public abstract class DelegatedBindable<TBindable> : Bindable
        where TBindable : IBindable
    {
        protected readonly TBindable Delegate;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="delegateBindable">Bindable to delegate to</param>
        protected DelegatedBindable(TBindable delegateBindable)
            : base(delegateBindable.Path)
        {
            if (!(delegateBindable.Object is IAnnex))
                throw new ArgumentException(
                    "Delegate bindable has to have a non-null Object that implements IAnnex", 
                    "delegateBindable"
                );
            Delegate = delegateBindable;
        }

        /// <summary>
        /// Copy constructor (used by <see cref="CloneWithoutObject"/>).
        /// The object this bindable belongs to is not copied.
        /// </summary>
        /// <param name="toCopy">The object to copy into a new instance.</param>
        protected DelegatedBindable(DelegatedBindable<TBindable> toCopy) 
            : base(toCopy)
        {
            Delegate = (TBindable) toCopy.Delegate.CloneWithoutObject();
            Delegate.SetObject(((IAnnex)toCopy.Delegate.Object).CloneWithoutObject());
        }

        /// <inheritdoc/>
        public override void SetObject(object value)
        {
            ((IAnnex)Delegate.Object).SetObject(value);
            base.SetObject(value);
        }

        /// <inheritdoc/>
        public override Type[] ValueTypes
        {
            get
            {
                return Delegate.ValueTypes;
            }
        }

    }

}


namespace Toubab.Beinder.Bindable
{
    using System;
    using Annex;

    /// <summary>
    /// Property bindable that delegates to another property bindable.
    /// </summary>
    public class DelegatedProperty : DelegatedBindable<IProperty>, IProperty
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="delegateProperty">Property to delegate to</param>
        public DelegatedProperty(IProperty delegateProperty)
            : base(delegateProperty)
        {
            delegateProperty.Broadcast += DelegateBroadcast;
        }

        /// <summary>
        /// Copy constructor (used by <see cref="CloneWithoutObject"/>).
        /// The object this bindable belongs to is not copied.
        /// </summary>
        /// <param name="toCopy">The object to copy into a new instance.</param>
        protected DelegatedProperty(DelegatedProperty toCopy)
            : base((DelegatedBindable<IProperty>) toCopy)
        {
            Delegate.Broadcast += DelegateBroadcast;
        }

        void DelegateBroadcast(object sender, BroadcastEventArgs args)
        {
            var evt = Broadcast;
            if (evt != null)
                evt(this, new BroadcastEventArgs(Object, args.Payload));
        }

        /// <inheritdoc/>
        public event EventHandler<BroadcastEventArgs> Broadcast;

        /// <inheritdoc/>
        public bool TryHandleBroadcast(object[] argument)
        {
            return Delegate.TryHandleBroadcast(argument);
        }

        /// <inheritdoc/>
        public object[] Values
        {
            get
            {
                return Delegate.Values;
            }
        }

        /// <inheritdoc/>
        public override IAnnex CloneWithoutObject()
        {
            return new DelegatedProperty(this);
        }

    }
}

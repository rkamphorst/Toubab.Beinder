namespace Toubab.Beinder.Bindable
{
    using System;
    using Mixin;

    /// <summary>
    /// Event bindable that delegates to another event bindable.
    /// </summary>
    public class DelegatedEvent : DelegatedBindable<IEvent>, IEvent
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="delegateEvent">Event to delegate to</param>
        public DelegatedEvent(IEvent delegateEvent) 
            : base(delegateEvent)
        {
            delegateEvent.Broadcast += DelegateBroadcast;
        }

        /// <summary>
        /// Copy constructor (used by <see cref="CloneWithoutObject"/>).
        /// The object this bindable belongs to is not copied.
        /// </summary>
        /// <param name="toCopy">The object to copy into a new instance.</param>
        protected DelegatedEvent(DelegatedEvent toCopy) 
            : base((DelegatedBindable<IEvent>) toCopy)
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
        public override IMixin CloneWithoutObject()
        {
            return new DelegatedEvent(this);
        }
    }

}

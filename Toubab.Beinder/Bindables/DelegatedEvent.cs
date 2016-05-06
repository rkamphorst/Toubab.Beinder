namespace Toubab.Beinder.Bindables
{
    using System;
    using Mixins;

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
        }

        /// <summary>
        /// Copy constructor (used by <see cref="CloneWithoutObject"/>).
        /// The object this bindable belongs to is not copied.
        /// </summary>
        /// <param name="toCopy">The object to copy into a new instance.</param>
        protected DelegatedEvent(DelegatedEvent toCopy) 
            : base((DelegatedBindable<IEvent>) toCopy)
        {
        }

        /// <inheritdoc/>
        public void SetBroadcastListener(Action<object[]> listener) 
        {
            Delegate.SetBroadcastListener(listener);
        }

        /// <inheritdoc/>
        public override IMixin CloneWithoutObject()
        {
            return new DelegatedEvent(this);
        }
    }

}

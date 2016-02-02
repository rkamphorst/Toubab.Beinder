namespace Toubab.Beinder.Bindables
{
    using System.Threading.Tasks;
    using Mixins;

    /// <summary>
    /// Event handler (method) bindable that delegates to another event handler (method) bindable.
    /// </summary>
    public class DelegatedEventHandler : DelegatedBindable<IEventHandler>, IEventHandler
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="delegateEventHandler">Event handler to delegate to</param>
        public DelegatedEventHandler(IEventHandler delegateEventHandler)
            : base(delegateEventHandler)
        {
        }

        /// <summary>
        /// Copy constructor (used by <see cref="CloneWithoutObject"/>).
        /// The object this bindable belongs to is not copied.
        /// </summary>
        /// <param name="toCopy">The object to copy into a new instance.</param>
        protected DelegatedEventHandler(DelegatedEventHandler toCopy)
            : base((DelegatedBindable<IEventHandler>) toCopy)
        {
        }

        /// <inheritdoc/>
        public async Task<bool> TryHandleBroadcast(object[] argument)
        {
            return await Delegate.TryHandleBroadcast(argument);
        }

        /// <inheritdoc/>
        public override IMixin CloneWithoutObject()
        {
            return new DelegatedEventHandler(this);
        }

    }

}

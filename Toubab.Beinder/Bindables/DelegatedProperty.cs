namespace Toubab.Beinder.Bindables
{
    using System;
    using System.Threading.Tasks;
    using Mixins;

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
        }

        /// <summary>
        /// Copy constructor (used by <see cref="CloneWithoutObject"/>).
        /// The object this bindable belongs to is not copied.
        /// </summary>
        /// <param name="toCopy">The object to copy into a new instance.</param>
        protected DelegatedProperty(DelegatedProperty toCopy)
            : base((DelegatedBindable<IProperty>)toCopy)
        {
        }

        /// <inheritdoc/>
        public void SetBroadcastListener(Action<object[]> listener)
        {
            Delegate.SetBroadcastListener(listener);
        }

        /// <inheritdoc/>
        public async Task<bool> TryHandleBroadcast(object[] argument)
        {
            return await Delegate.TryHandleBroadcast(argument);
        }

        /// <inheritdoc/>
        public object Value
        {
            get
            {
                return Delegate.Value;
            }
        }

        /// <inheritdoc/>
        public override IMixin CloneWithoutObject()
        {
            return new DelegatedProperty(this);
        }

    }
}

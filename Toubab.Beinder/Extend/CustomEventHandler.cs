namespace Toubab.Beinder.Extend
{
    using System.Threading.Tasks;
    using Bindables;
    using Paths;

    public abstract class CustomEventHandler<T> : Bindable, ICustomEventHandler<T>
    {
        protected CustomEventHandler(Fragment nameSyllables)
            : base(nameSyllables)
        {
        }

        protected CustomEventHandler(CustomEvent<T> toCopy)
            : base(toCopy)
        {
        }

        /// <inheritdoc />
        public override BindingOperations Capabilities
        {
            get
            {
                return BindingOperations.HandleBroadcast;
            }
        }

        public abstract Task<bool> TryHandleBroadcastAsync(object[] payload);
    }
}

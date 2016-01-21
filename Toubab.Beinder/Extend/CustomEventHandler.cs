namespace Toubab.Beinder.Extend
{
    using Bindable;

    public abstract class CustomEventHandler<T> : Bindable, ICustomEventHandler<T>
    {
        protected CustomEventHandler(Path.Path path)
            : base(path)
        {
        }

        protected CustomEventHandler(CustomEvent<T> toCopy)
            : base(toCopy)
        {
        }

        public abstract bool TryHandleBroadcast(object[] payload);
    }
}

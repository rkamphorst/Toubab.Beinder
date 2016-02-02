namespace Toubab.Beinder.Extend
{
    using System.Threading.Tasks;
    using Bindables;
    using Paths;

    public abstract class CustomEventHandler<T> : Bindable, ICustomEventHandler<T>
    {
        protected CustomEventHandler(Path path)
            : base(path)
        {
        }

        protected CustomEventHandler(CustomEvent<T> toCopy)
            : base(toCopy)
        {
        }

        public abstract Task<bool> TryHandleBroadcast(object[] payload);
    }
}

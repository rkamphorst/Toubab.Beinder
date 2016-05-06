namespace Toubab.Beinder.Extend
{
    using System;
    using System.Threading.Tasks;
    using Bindables;
    using Paths;

    public abstract class CustomProperty<T> : Bindable, ICustomProperty<T>
    {
        protected CustomProperty(Path path) 
            : base(path)
        {
        }

        protected CustomProperty(CustomEvent<T> toCopy)
            : base(toCopy)
        {
        }

        public abstract void SetBroadcastListener(Action<object[]> listener);
    
        public abstract Task<bool> TryHandleBroadcast(object[] payload);

        public abstract object[] Values { get; }
    }
}

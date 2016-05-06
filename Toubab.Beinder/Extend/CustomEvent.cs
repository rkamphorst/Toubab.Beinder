namespace Toubab.Beinder.Extend
{
    using System;
    using Bindables;
    using Paths;

    public abstract class CustomEvent<T> : Bindable, ICustomEvent<T>
    {
        protected CustomEvent(Path path)
            : base(path)
        {
        }

        protected CustomEvent(CustomEvent<T> toCopy)
            : base(toCopy)
        {
        }

        public abstract void SetBroadcastListener(Action<object[]> listener);
    }

}


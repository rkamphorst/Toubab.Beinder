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

        public event EventHandler<BroadcastEventArgs> Broadcast;

        protected virtual void OnBroadcast(object[] payload)
        {
            var evt = Broadcast;
            if (evt != null)
            {
                var args = new BroadcastEventArgs(Object, payload);
                evt(this, args);
            }
        }
    }

}


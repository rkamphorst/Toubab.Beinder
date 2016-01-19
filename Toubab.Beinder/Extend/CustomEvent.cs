using System;
using Toubab.Beinder.Bindable;
using Toubab.Beinder.Path;

namespace Toubab.Beinder.Extend
{
    public abstract class CustomEvent<T> : Bindable.Bindable, ICustomEvent<T>
    {
        protected CustomEvent(Path.Path path)
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


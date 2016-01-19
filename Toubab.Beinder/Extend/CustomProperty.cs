using System;
using Toubab.Beinder.Bindable;
using Toubab.Beinder.Path;

namespace Toubab.Beinder.Extend
{

    public abstract class CustomProperty<T> : Bindable.Bindable, ICustomProperty<T>
    {
        protected CustomProperty(Path.Path path) 
            : base(path)
        {
        }

        protected CustomProperty(CustomEvent<T> toCopy)
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

        public abstract bool TryHandleBroadcast(object[] payload);

        public abstract object[] Values { get; }
    }
}

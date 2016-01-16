using System;
using Toubab.Beinder.Bindable;
using Toubab.Beinder.PathParser;

namespace Toubab.Beinder.Extend
{

    public abstract class CustomProperty<T> : Bindable.Bindable, ICustomProperty<T>
    {
        protected CustomProperty() 
            : base(new CamelCasePathParser())
        {
        }

        protected CustomProperty(IPathParser pathParser)
            : base(pathParser)
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
                var args = new BroadcastEventArgs(this, payload);
                evt(this, args);
            }
        }

        public abstract bool TryHandleBroadcast(object[] payload);

        public abstract object[] Values { get; }
    }
}

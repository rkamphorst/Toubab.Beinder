using System;
using Toubab.Beinder.Bindable;
using Toubab.Beinder.PathParser;

namespace Toubab.Beinder
{
    public abstract class CustomEvent<T> : Bindable.Bindable, ICustomEvent<T>
    {
        protected CustomEvent() 
            : base(new CamelCasePathParser())
        {
        }

        protected CustomEvent(IPathParser pathParser)
            : base(pathParser)
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
                var args = new BroadcastEventArgs(this, payload);
                evt(this, args);
            }
        }
    }

}


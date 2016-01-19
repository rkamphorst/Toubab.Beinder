using System;
using Toubab.Beinder.Bindable;
using Toubab.Beinder.Path;

namespace Toubab.Beinder.Extend
{
    public abstract class CustomEventHandler<T> : Bindable.Bindable, ICustomEventHandler<T>
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

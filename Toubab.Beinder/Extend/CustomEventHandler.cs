using System;
using Toubab.Beinder.Bindable;
using Toubab.Beinder.PathParser;

namespace Toubab.Beinder.Extend
{
    public abstract class CustomEventHandler<T> : Bindable.Bindable, ICustomEventHandler<T>
    {
        protected CustomEventHandler()
            : base(new CamelCasePathParser())
        {
        }

        protected CustomEventHandler(IPathParser pathParser)
            : base(pathParser)
        {
        }

        protected CustomEventHandler(CustomEvent<T> toCopy)
            : base(toCopy)
        {
        }

        public abstract bool TryHandleBroadcast(object[] payload);
    }
}

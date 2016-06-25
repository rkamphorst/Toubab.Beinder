﻿namespace Toubab.Beinder.Extend
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

        public override BindingOperations Capabilities
        {
            get
            {
                return BindingOperations.Broadcast;
            }
        }

        public abstract void SetBroadcastListener(Action<object[]> listener);
    }

}


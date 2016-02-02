namespace Toubab.Beinder.Extend
{
    using System;
    using Mixins;

    public abstract class CustomMixin<T> : Mixin, ICustomMixin<T>
    {
        protected new T GetObject()
        {
            return (T)base.GetObject();
        }
    }
}


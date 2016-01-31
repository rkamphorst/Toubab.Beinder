using System;
using Toubab.Beinder.Mixin;

namespace Toubab.Beinder.Extend
{
    public abstract class CustomMixin<T> : Mixin.Mixin, ICustomMixin<T>
    {
        protected new T GetObject()
        {
            return (T)base.GetObject();
        }
    }
}


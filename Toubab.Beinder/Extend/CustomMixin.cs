using System;

namespace Toubab.Beinder.Extend
{
    public abstract class CustomMixin<T> : Mixin.Mixin<T>, ICustomMixin<T>
    {
    }
}


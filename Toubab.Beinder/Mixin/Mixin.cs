using System;
using Toubab.Beinder.Annex;

namespace Toubab.Beinder.Mixin
{
    /// <summary>
    /// Partial implementation of <see cref="IMixin"/>
    /// </summary>
    public abstract class Mixin<T> : Annex.Annex, IMixin<T>
    {
        protected new T GetObject()
        {
            return (T)base.GetObject();
        }
    }
}

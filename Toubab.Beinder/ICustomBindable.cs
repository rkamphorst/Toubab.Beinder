﻿
using Toubab.Beinder.Valve;

namespace Toubab.Beinder
{
    public interface ICustomBindable : IBindable
    {
    }

    public interface ICustomBindable<T> : ICustomBindable
    {
    }

}

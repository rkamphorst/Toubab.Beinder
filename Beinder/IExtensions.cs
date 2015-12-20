using System;

namespace Beinder
{
    public interface IExtensions
    {
        bool TrySetObject(object newObject);
    }

    public interface IExtensions<T> : IExtensions 
    {
    }
}


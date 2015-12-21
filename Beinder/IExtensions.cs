using System;

namespace Beinder
{
    public interface IExtensions
    {
        bool TrySetObject(object newObject);

        IExtensions Clone();
    }

    public interface IExtensions<T> : IExtensions 
    {
    }
}


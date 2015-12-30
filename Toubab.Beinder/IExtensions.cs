using System;

namespace Toubab.Beinder
{
    public interface IExtensions
    {
        void SetObject(object newObject);

        IExtensions CloneWithoutObject();
    }

    public interface IExtensions<T> : IExtensions 
    {
    }
}


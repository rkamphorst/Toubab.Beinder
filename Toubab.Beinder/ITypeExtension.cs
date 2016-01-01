using System;

namespace Toubab.Beinder
{
    public interface ITypeExtension
    {
        void SetObject(object newObject);

        ITypeExtension CloneWithoutObject();
    }

    public interface ITypeExtension<T> : ITypeExtension 
    {
    }

}


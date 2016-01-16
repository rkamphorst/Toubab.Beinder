using System;

namespace Toubab.Beinder
{
    public interface IMixin
    {
        void SetObject(object newObject);

        IMixin CloneWithoutObject();
    }

    public interface IMixin<T> : IMixin 
    {
    }

}


using System;

namespace Toubab.Beinder.Mixin
{

    public abstract class Mixin<T> : IMixin<T> {

        T _object;

        protected T GetObject()
        { 
            return _object;
        }

        public void SetObject(object newObject)
        {
            if (!ReferenceEquals(_object, null))
                BeforeSetObject(_object);
            _object = (T)newObject;
            if (!ReferenceEquals(_object, null)) 
                AfterSetObject(_object);
        }

        protected virtual void BeforeSetObject(object oldObject) 
        {
        }

        protected virtual void AfterSetObject(object newObject) 
        {
        }

        public abstract IMixin CloneWithoutObject();
    }
}

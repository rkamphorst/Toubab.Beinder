using System;

namespace Toubab.Beinder
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
                DetachObject(_object);
            _object = (T)newObject;
            if (!ReferenceEquals(_object, null)) 
                AttachObject(_object);
        }

        protected abstract void DetachObject(object oldObject);

        protected abstract void AttachObject(object newObject);

        public abstract IMixin CloneWithoutObject();
    }
}

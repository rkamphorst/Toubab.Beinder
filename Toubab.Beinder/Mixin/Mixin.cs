using System;

namespace Toubab.Beinder.Mixin
{
    /// <summary>
    /// Partial implementation of <see cref="IMixin"/>
    /// </summary>
    public abstract class Mixin<T> : IMixin<T> {

        T _object;

        /// <summary>
        /// Get the object set by <see cref="SetObject"/>
        /// </summary>
        protected T GetObject()
        { 
            return _object;
        }

        /// <inheritdoc/>
        public void SetObject(object newObject)
        {
            var oldObject = _object;
            BeforeSetObject(oldObject, newObject);
            _object = (T)newObject;
            AfterSetObject(oldObject, newObject);
        }

        /// <summary>
        /// Override this method to perform some work before the object is actually set by 
        /// <see cref="SetObject"/>.
        /// </summary>
        /// <remarks>
        /// If overridden, there is no need to call the base method.
        /// </remarks>
        protected virtual void BeforeSetObject(object oldObject, object newObject) 
        {
        }

        /// <summary>
        /// Override this method to perform some work right after the object is actually set 
        /// by <see cref="SetObject"/>.
        /// </summary>
        /// <remarks>
        /// If overridden, there is no need to call the base method.
        /// </remarks>
        protected virtual void AfterSetObject(object oldObject, object newObject) 
        {
        }

        /// <inheritdoc/>
        public abstract IMixin CloneWithoutObject();
    }
}

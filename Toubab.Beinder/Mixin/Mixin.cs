namespace Toubab.Beinder.Mixin
{
    /// <summary>
    /// Annex implementation
    /// </summary>
    /// <remarks>
    /// Serves as a base implementation for <see cref="Bindable.Bindable"/> and
    /// for <see cref="Beinder.Mixin.Mixin{T}"/>.
    /// </remarks>
    /// <seealso cref="IMixin"/>
    public abstract class Mixin : IMixin
    {
        object _object;

        /// <summary>
        /// Protected constructor to prevent default constructor
        /// </summary>
        protected Mixin()
        {
        }

        /// <summary>
        /// Get the object set by <see cref="SetObject"/>.
        /// </summary>
        protected object GetObject()
        {
            return _object;
        }

        /// <inheritdoc/>
        public virtual void SetObject(object value)
        {
            _object = value;
        }

        /// <inheritdoc/>
        public abstract IMixin CloneWithoutObject();
    }

    /// <summary>
    /// Partial implementation of <see cref="IMixin"/>
    /// </summary>
    public abstract class Mixin<T> : Mixin, IMixin<T>
    {
        /// <summary>
        /// Gets the object this is a mix-in for
        /// </summary>
        protected new T GetObject()
        {
            return (T)base.GetObject();
        }
    }
}

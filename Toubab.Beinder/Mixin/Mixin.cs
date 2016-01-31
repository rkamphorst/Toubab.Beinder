namespace Toubab.Beinder.Mixin
{
    /// <summary>
    /// Base Mixin implementation
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

}

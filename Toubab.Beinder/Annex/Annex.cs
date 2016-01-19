namespace Toubab.Beinder.Annex
{

    /// <summary>
    /// Annex implementation
    /// </summary>
    /// <remarks>
    /// Serves as a base implementation for <see cref="Bindable.Bindable"/> and
    /// for <see cref="Mixin.Mixin{T}"/>.
    /// </remarks>
    /// <seealso cref="IAnnex"/>
    public abstract class Annex : IAnnex
    {
        object _object;

        /// <summary>
        /// Protected constructor to prevent default constructor
        /// </summary>
        protected Annex()
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
        public abstract IAnnex CloneWithoutObject();
    }
}

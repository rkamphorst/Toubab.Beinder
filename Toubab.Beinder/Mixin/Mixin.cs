namespace Toubab.Beinder.Mixin
{
    /// <summary>
    /// Partial implementation of <see cref="IMixin"/>
    /// </summary>
    public abstract class Mixin<T> : Annex.Annex, IMixin<T>
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

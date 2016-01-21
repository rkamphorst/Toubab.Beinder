namespace Toubab.Beinder.Annex
{
    /// <summary>
    /// Annex: Carries extra (metadata about) properties, events and methods on an object
    /// </summary>
    /// <remarks>
    /// This is the base interface for <see cref="Bindable.IBindable" /> and <see cref="Mixin.IMixin"/>.
    /// 
    /// An Annex is any object that contains "extra" information about another object, be it a single
    /// event/method/property (<see cref="Bindable.IBindable" />) or "extra" mebers (such as defined
    /// in a <see cref="Mixin.IMixin"/>).
    /// </remarks>
    public interface IAnnex
    {
        /// <summary>
        /// Set the object that this <see cref="IAnnex"/> is attached to.
        /// </summary>
        /// <remarks>
        /// An Annex is "detachted" if the associated object is null, otherwise
        /// it is  "attached".
        /// 
        /// Whenever a non-<c>null</c> object is set with this method,
        /// event handlers might be attached to that object. For that reason,
        /// make sure a <c>null</c> object is set whenever possible to avoid
        /// obsolete event subscriptions (that e.g. prevent an object from being
        /// collected by the garbage collector).
        /// <remarks>
        /// <param name="newObject">The new object to set.</param>
        void SetObject(object newObject);

        /// <summary>
        /// Makes a detached copy of this object.
        /// </summary>
        /// <remarks>
        /// The returned object is a copy,
        /// except that it is not "attached". It has to be 
        /// separately attached by calling <see cref="SetObject"/>.
        /// </remarks>
        IAnnex CloneWithoutObject();
    }

}


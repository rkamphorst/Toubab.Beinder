namespace Toubab.Beinder.Mixins
{
    /// <summary>
    /// Interface for a mix-in
    /// </summary>
    /// <remarks>
    /// A mix-in is a class that defines methods, events and properties
    /// that are "mixed in" with another class that it is a mix-in for.
    /// 
    /// In other words, a mix-in is a way of separately specifying extensions for
    /// another class.
    /// 
    /// In the context of Beinder, the <see cref="Scanner.CustomMixinScanner"/> class
    /// can scan for mix-ins (actually, it scans vor <see cref="Mixin{T}"/> implementations).
    /// The mix-ins are themselves scanned as if they were normal objects; the 
    /// <see cref="Scanner.CustomMixinScanner"/> then transforms the resulting <see cref="Bindable.IBindable"/>
    /// objects into bindables that "pretend" to live on the type the mix-ins are
    /// for.
    /// </remarks>
    public interface IMixin 
    {
        /// <summary>
        /// Set the object that this <see cref="IMixin"/> is attached to.
        /// </summary>
        /// <remarks>
        /// An mixin is "detachted" if the associated object is null, otherwise
        /// it is  "attached".
        /// 
        /// Whenever a non-<c>null</c> object is set with this method,
        /// event handlers might be attached to that object. For that reason,
        /// make sure a <c>null</c> object is set whenever possible to avoid
        /// obsolete event subscriptions (that e.g. prevent an object from being
        /// collected by the garbage collector).
        /// </remarks>
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
        IMixin CloneWithoutObject();
    }

}


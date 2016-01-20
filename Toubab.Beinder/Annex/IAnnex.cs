﻿namespace Toubab.Beinder.Annex
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
        /// Set the object this is an annex to
        /// </summary>
        /// <remarks>
        /// An Annex is "detachted" if the associated object is null, otherwise
        /// it is in an "attached" state.
        /// 
        /// Consider that whenever a non-null object is set with this method,
        /// event handlers might be attached to that object. For that reason,
        /// make sure a <c>null</c> object is set whenever possible to avoid
        /// spurios event subscriptions (that e.g. prevent an object from being
        /// collected by the garbage collector).
        /// <remarks>
        /// <param name="newObject">The new object to set.</param>
        void SetObject(object newObject);

        /// <summary>
        /// Makes a detached copy of this <see cref="IAnnex"/>.
        /// </summary>
        /// <remarks>
        /// The returned <see cref="IAnnex"/> is a copy of this property,
        /// except that it has no object attached. An object has to be 
        /// separately attached with <see cref="SetObject"/>.
        /// </remarks>
        IAnnex CloneWithoutObject();
    }

}


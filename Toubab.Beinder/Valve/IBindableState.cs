using System;

namespace Toubab.Beinder.Valve
{
    /// <summary>
    /// Property adapter interface.
    /// </summary>
    /// <remarks>
    /// This interface represents a property of an object.
    /// 
    /// A property can be a C# property, but it can also be anything else that
    /// has a name and is associated to an object. For example, it can be a
    /// dictionary entry or a named child view. 
    /// 
    /// The object that holds the property can be retrieved through 
    /// <see cref="Object"/>, and can be set with <see cref="SetObject"/>. 
    /// Note that <see cref="SetObject"/> should be used sparingly, as it is
    /// the place where events are attached and detached to the object in order
    /// to make <see cref="ValueChanged"/> emit the correct events.
    /// 
    /// The property has a name, which is represented by the <see cref="Path"/>. 
    /// 
    /// The property has a <see cref="Value"/> that is 
    /// propagated when the property is bound to other properties.
    /// </remarks>
    public interface IBindableState : IBindableBroadcastProducer, IBindableBroadcastConsumer
    {
        /// <summary>
        /// Value of the property.
        /// </summary>
        /// <remarks>
        /// This is the value that is propagated to other <see cref="IProperty"/>
        /// instances through binding (see <see cref="Binder.Bind"/> and
        /// <see cref="Valve"/>).
        /// </remarks>
        object Value { get; }
    }

}

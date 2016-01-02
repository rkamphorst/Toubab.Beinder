using System;

namespace Toubab.Beinder
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
    public interface IProperty 
    {
        /// <summary>
        /// Object: holder of the property
        /// </summary>
        /// <remarks>
        /// The <see cref="IProperty"/> implementation contains logic that,
        /// given the <see cref="Object"/> and <see cref="Path"/>,
        /// enables the retrieval of the value value through <see cref="Value"/>
        /// and setting of the value through <see cref="TrySetValue"/>.
        /// </remarks>
        object Object { get; }

        /// <summary>
        /// Property path
        /// </summary>
        /// <remarks>
        /// Identifies the property, given an object (<see cref="Object"/>).
        /// 
        /// It also is a path, i.e., starting at <see cref="Object"/>, follow
        /// this path to get <see cref="Value"/>.
        /// </remarks>
        /// <seealso cref="PropertyPath"/>
        PropertyPath Path { get; }

        /// <summary>
        /// Type of values that can be set
        /// </summary>
        Type ValueType { get; } 

        /// <summary>
        /// Value of the property.
        /// </summary>
        /// <remarks>
        /// This is the value that is propagated to other <see cref="IProperty"/>
        /// instances through binding (see <see cref="Binder.Bind"/> and
        /// <see cref="Valve"/>).
        /// </remarks>
        object Value { get; }

        /// <summary>
        /// Set the object that holds the property
        /// </summary>
        /// <remarks>
        /// In order to be able to raise the <see cref="ValueChanged"/> event,
        /// a property adapter (implementation of <see cref="IProperty"/>) needs
        /// to attach event handlers to the object that holds the property.
        /// 
        /// This method first determines whether the <paramref name="newObject"/>
        /// is compatible with this <see cref="IProperty"/> implementation. Then,
        /// if an "old object" exists, the event handlers are detached from that
        /// object. Finally, the <paramref name="newObject"/> replaces the old
        /// object and the event handlers are attached to the new object.
        /// 
        /// If <paramref name="newObject"/> is <c>null</c>, this effectively 
        /// detaches the events from the existing ("old") object, if any. 
        /// Therefore, if you are done using this <see cref="IProperty"/>, it
        /// is good practice to call <see cref="SetObject"/> with a 
        /// <c>null</c> parameter.
        /// </remarks>
        /// <param name="newObject">The new object to set.</param>
        void SetObject(object newObject);

        /// <summary>
        /// Try to set the value of the property
        /// </summary>
        /// <remarks>
        /// Whether this method succeeds is entirely up to the property 
        /// implementation. It may fail because <paramref name="newValue"/>
        /// has the wrong type, because the <see cref="Object"/> is in the 
        /// wrong state, or maybe even because sealevels are rising. However,
        /// if it fails, it should return <c>false</c>.
        /// 
        /// This method is called a lot, so make sure it returns as quickly as 
        /// possible!
        /// 
        /// </remarks>
        /// <returns><c>true</c>, if value was set succesfully, 
        /// <c>false</c> otherwise.</returns>
        /// <param name="newValue">The new value to set.</param>
        bool TrySetValue(object newValue);

        /// <summary>
        /// Occurs when the property's value on <see cref="Object"/> changes.
        /// </summary>
        /// <remarks>
        /// This event may or may not be raised when the value changes
        /// due to a call to <see cref="TrySetValue"/>. It is used to detect
        /// value changes that happen on the actual <see cref="Object"/>'s 
        /// property.
        /// </remarks>
        event EventHandler<PropertyValueChangedEventArgs> ValueChanged;

        /// <summary>
        /// Makes an independent copy of this property, without attaching an 
        /// <see cref="Object"/>.
        /// </summary>
        /// <remarks>
        /// The returned <see cref="IProperty"/> is a copy of this property,
        /// except that it has no <see cref="Object"/> attached. An 
        /// <see cref="Object"/> has to be separately attached with 
        /// <see cref="SetObject"/>.
        /// </remarks>
        IProperty CloneWithoutObject();

    }


    public interface IProperty<T> : IProperty
    {
    }
}

using System;

namespace Beinder
{
    /// <summary>
    /// Property adapter interface
    /// </summary>
    /// <remarks>
    /// <para>
    /// This represents a property of an object, which is stored in 
    /// <see cref="Object"/>. The property has a name, which is represented by 
    /// the <see cref="Path"/>. And, last but not least: the property has a 
    /// <see cref="Value"/> that is propagated when the property is bound to
    /// other properties.
    /// </para>
    /// </remarks>
    public interface IProperty 
    {
        /// <summary>
        /// Property meta information
        /// </summary>
        /// <remarks>
        /// Note: This meta information is static: it should not change after the 
        /// IProperty is instantiated!
        /// </remarks>
        PropertyMetaInfo MetaInfo { get; }

        /// <summary>
        /// Object: holder of the property
        /// </summary>
        /// <remarks>
        /// With the <see cref="Object"/> and the <see cref="Path"/>, a
        /// <see cref="Value"/> can be retrieved.
        /// </remarks>
        object Object { get; }

        /// <summary>
        /// Property path
        /// </summary>
        /// <remarks>
        /// <para>
        /// Identifies the property, given an object (<see cref="Object"/>)
        /// <para>
        /// <para>
        /// It also is a path, i.e., starting at <see cref="Object"/>, follow
        /// this path to get <see cref="Value"/>
        /// </para>
        /// </remarks>
        /// <seealso cref="PropertyPath"/>
        PropertyPath Path { get; }

        /// <summary>
        /// Value of the property, propagated through binding.
        /// </summary>
        object Value { get; }

        /// <summary>
        /// Tries to set the object that holds the property
        /// </summary>
        /// <remarks>
        /// <para>
        /// In order to be able to raise the <see cref="ValueChanged"/> event,
        /// a property adapter (implementation of <see cref="IProperty"/>) needs
        /// to attach event handlers to the object that holds the property.
        /// </para>
        /// 
        /// <para>
        /// This method first determines whether the <paramref name="newObject"/>
        /// is compatible with this <see cref="IProperty"/> implementation. Then,
        /// if an "old object" exists, the event handlers are detached from that
        /// object. Finally, the <paramref name="newObject"/> replaces the old
        /// object and the event handlers are attached to the new object.
        /// </para>
        /// 
        /// <para>
        /// If <paramref name="newObject"/> is <c>null</c>, this effectively 
        /// detaches the events from the existing ("old") object, if any. 
        /// Therefore, if you are done using this <see cref="IProperty"/>, it
        /// is good practice to call <see cref="TrySetObject"/> with a 
        /// <c>null</c> parameter.
        /// </para>
        /// </remarks>
        /// <returns><c>true</c>, if object was set succesfully, <c>false</c> 
        /// otherwise.</returns>
        /// <param name="newObject">The new object to set.</param>
        bool TrySetObject(object newObject);

        /// <summary>
        /// Tries to set the value of the property
        /// </summary>
        /// <remarks>
        /// <para>
        /// Whether this method succeeds is entirely up to the property 
        /// implementation. It may fail because <paramref name="newValue"/>
        /// has the wrong type, because the <see cref="Object"/> is in the 
        /// wrong state, or maybe even because sealevels are rising. However,
        /// if it fails, it should return <c>false</c>
        /// </para>
        /// 
        /// <para>
        /// This method is called a lot, so make sure it returns as quickly as 
        /// possible!
        /// </para>
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
        /// due to a call to <see cref="TrySetValue"/>; it is meant to detect
        /// value changes that happen on the actual <see cref="Object"/>'s 
        /// property.
        /// </remarks>
        event EventHandler<PropertyValueChangedEventArgs> ValueChanged;

        /// <summary>
        /// Makes an independent copy of this property
        /// </summary>
        /// <remarks>
        /// The property adapter acts on the same object, i.e., the adaptee
        /// (<see cref="Object"/>) is not a copy but a reference to the same
        /// object as for the original property.
        /// </remarks>
        IProperty Clone();

    }


    public interface IProperty<T> : IProperty
    {
    }
}

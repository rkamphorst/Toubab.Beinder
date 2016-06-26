namespace Toubab.Beinder.Bindables
{
    using System;
    using Mixins;
    using Paths;

    /// <summary>
    /// Represents a bindable property, event or method
    /// </summary>
    /// A bindable can be a C# property, event or method; but it can also be
    /// anything else that has a name and is associated to an object, and can
    /// *act* as a property, event or method. For example, it can be a
    /// dictionary entry or a named child view. 
    /// 
    /// The object that holds the property can be retrieved through 
    /// <see cref="Object"/>, and can be set with <see cref="IMixin.SetObject"/>. 
    /// 
    /// The bindable has a name, which is represented by the <see cref="Path"/>. 
    /// 
    /// The bindable has zero or more <see cref="ValueTypes"/> that it can 
    /// produce (if it is an event), consume (event handler / method) or
    /// store (property). These three kinds of bindables have corresponding
    /// interface that inherit from this one: <see cref="IEvent"/>, 
    /// <see cref="IEventHandler"/> and <see cref="IProperty"/>.
    public interface IBindable : IMixin
    {
        /// <summary>
        /// The capabilities of this bindable.
        /// </summary>
        /// <remarks>
        /// A combination of:
        /// 
        ///     - read the binding value (i.e., bindable has state)
        ///     - broadcast a value ("event")
        ///     - handle broadcasted value ("event handler")
        /// </remarks>
        BindingOperations Capabilities { get; }

        /// <summary>
        /// Types of values this bindable can bind to
        /// </summary>
        Type[] ValueTypes { get; } 

        /// <summary>
        /// Property path
        /// </summary>
        /// <remarks>
        /// Gives the "path" of this bindable, i.e., starting at <see cref="Object"/>, follow
        /// this path to get access to the bindable property, event or event handler.
        /// </remarks>
        /// <seealso cref="Path"/>
        Path Path { get; }

        /// <summary>
        /// Object this bindable is associated with.
        /// </summary>
        object Object { get; }

    }

}


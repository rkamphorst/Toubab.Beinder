using System;

namespace Toubab.Beinder.Bindable
{
    public interface IBindable 
    {
        /// <summary>
        /// Object: holder of the bindable
        /// </summary>
        object Object { get; }

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
        /// Type of values that can be set
        /// </summary>
        Type[] ValueType { get; } 

        /// <summary>
        /// Property path
        /// </summary>
        /// <remarks>
        /// Identifies the bindable, given an object (<see cref="Object"/>).
        /// 
        /// It also is a path, i.e., starting at <see cref="Object"/>, follow
        /// this path to get access to the bindable property, event or event handler.
        /// </remarks>
        /// <seealso cref="PropertyPath"/>
        Path Path { get; }

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
        IBindable CloneWithoutObject();
    }

}


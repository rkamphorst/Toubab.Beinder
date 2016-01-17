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
        /// <param name="newObject">The new object to set.</param>
        void SetObject(object newObject);

        /// <summary>
        /// Type of values this bindable can bind to
        /// </summary>
        Type[] ValueTypes { get; } 

        /// <summary>
        /// Property path
        /// </summary>
        /// <remarks>
        /// Identifies the bindable, given an object (<see cref="Object"/>).
        /// 
        /// It also is a path, i.e., starting at <see cref="Object"/>, follow
        /// this path to get access to the bindable property, event or event handler.
        /// </remarks>
        /// <seealso cref="Path"/>
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


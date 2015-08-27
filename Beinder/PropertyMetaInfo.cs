using System;

namespace Beinder
{

    public struct PropertyMetaInfo
    {

        public PropertyMetaInfo(
            Type objectType, Type valueType, bool isReadable, bool isWritable
        )
        {
            ObjectType = objectType;
            ValueType = valueType;
            IsReadable = isReadable;
            IsWritable = isWritable;
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of the object that holds the property
        /// </summary>
        /// <remarks>
        /// If the object type is dynamic (i.e., can change), this property should
        /// return <c>null</c>.
        /// </remarks>
        public Type ObjectType { get; private set; }

        /// <summary>
        /// Gets the <see cref="Type"/> of the value of the property
        /// </summary>
        /// <remarks>
        /// If the value type is dynamic (i.e., can change), this property should
        /// return <c>null</c>.
        /// </remarks>
        public Type ValueType { get; private set; }

        /// <summary>
        /// Whether the property's value is readable.
        /// </summary>
        /// <value><c>true</c> if this instance is readable; otherwise, <c>false</c>.</value>
        public bool IsReadable { get; private set; }

        /// <summary>
        /// Whether the property's value is writable.
        /// </summary>
        /// <value><c>true</c> if this instance is writable; otherwise, <c>false</c>.</value>
        public bool IsWritable { get; private set; }
    }
}

using System;

namespace Beinder
{
    public interface IProperty 
    {
        Type ObjectType { get; }

        Type ValueType { get; }

        bool IsReadable { get; }

        bool IsWritable { get; }

        object Object { get; }

        object Value { get; }

        bool TrySetObject(object newObject);

        bool TrySetValue(object newValue);

        PropertyPath Path { get; }

        event EventHandler<ValueChangedEventArgs> ValueChanged;

        IProperty Clone();

    }
    
}

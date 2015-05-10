using System;

namespace Beinder
{
    public interface IProperty 
    {
        Type ObjectType { get; }

        Type ValueType { get; }

        object Object { get; }

        bool TrySetObject(object newObject);

        PropertyPath Path { get; }

        object Value { get; }

        bool TrySetValue(object newValue);

        bool IsReadable { get; }

        bool IsWritable { get; }

        event EventHandler<ValueChangedEventArgs> ValueChanged;

        IProperty Clone();

    }
    
}

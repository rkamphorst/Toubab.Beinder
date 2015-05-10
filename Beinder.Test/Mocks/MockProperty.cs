using System;

namespace Beinder.Mocks
{
    class MockProperty : IProperty
    {
        public int Changed { get; set; }

        public string Name { get; set; }

        public event EventHandler<ValueChangedEventArgs> ValueChanged;

        public Type ValueType { get { return null; } }

        public Type ObjectType { get { return null; } }

        public bool IsReadable { get { return true; } }

        public bool IsWritable { get { return true; } }

        object _value;

        public object Value
        {
            get { return _value; }
        }

        public bool TrySetValue(object value)
        {
            _value = value;
            Changed++;
            if (ValueChanged != null)
                ValueChanged(this, new ValueChangedEventArgs(value));
            return true;
        }

        public object Object
        {
            get;
            set; 
        }

        public bool TrySetObject(object value)
        {
            Object = value;
            return true;
        }

        public PropertyPath Path
        {
            get { return "abc"; }
        }

        public IProperty Clone()
        {
            return new MockProperty
            {
                Name = Name,
                _value = _value,
                Object = Object
            };
        }

    }
}


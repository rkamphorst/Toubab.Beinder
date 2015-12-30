using System;

namespace Toubab.Beinder.Mocks
{
    class MockProperty : IProperty
    {
        public int Changed { get; set; }

        public string Name { get; set; }

        public event EventHandler<PropertyValueChangedEventArgs> ValueChanged;

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
                ValueChanged(this, new PropertyValueChangedEventArgs(this, value));
            return true;
        }

        public object Object
        {
            get;
            set; 
        }

        public void SetObject(object value)
        {
            Object = value;
        }

        public PropertyPath Path
        {
            get { return "abc"; }
        }

        public IProperty CloneWithoutObject()
        {
            return new MockProperty
            {
                Name = Name,
                _value = _value
            };
        }

    }
}


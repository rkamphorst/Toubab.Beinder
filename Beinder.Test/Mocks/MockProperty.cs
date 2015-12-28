using System;

namespace Beinder.Mocks
{
    class MockProperty : IProperty
    {
        PropertyMetaInfo _metaInfo = new PropertyMetaInfo(null, null, true, true);

        public int Changed { get; set; }

        public string Name { get; set; }

        public event EventHandler<PropertyValueChangedEventArgs> ValueChanged;

        public PropertyMetaInfo MetaInfo
        {
            get { return _metaInfo; }
        }

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


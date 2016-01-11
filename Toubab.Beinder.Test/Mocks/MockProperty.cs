using System;
using Toubab.Beinder.Valve;

namespace Toubab.Beinder.Mocks
{
    class MockProperty : IBindableState
    {
        public int Changed { get; set; }

        public string Name { get; set; }

        public event EventHandler<BindableBroadcastEventArgs> Broadcast;

        public Type ValueType
        { 
            get { return typeof(object); }
        }

        object _value;

        public object Value
        {
            get { return _value; }
        }

        public bool TryHandleBroadcast(object value)
        {
            _value = value;
            Changed++;
            if (Broadcast != null)
                Broadcast(this, new BindableBroadcastEventArgs(this, value));
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

        public Path Path
        {
            get { return "abc"; }
        }

        public IBindable CloneWithoutObject()
        {
            return new MockProperty
            {
                Name = Name,
                _value = _value
            };
        }

    }
}


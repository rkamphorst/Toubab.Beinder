using System;
using Toubab.Beinder.Bindable;

namespace Toubab.Beinder.Mock
{
    class MockProperty : IBindableState
    {
        public int Changed { get; set; }

        public string Name { get; set; }

        public event EventHandler<BroadcastEventArgs> Broadcast;

        public Type[] ValueType
        { 
            get { return new[] { typeof(object) }; }
        }

        object[] _values;

        public object[] Values
        {
            get { return _values; }
        }

        public bool TryHandleBroadcast(object[] values)
        {
            _values = values;
            Changed++;
            if (Broadcast != null)
                Broadcast(this, new BroadcastEventArgs(this, values));
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
                _values = _values
            };
        }

    }
}


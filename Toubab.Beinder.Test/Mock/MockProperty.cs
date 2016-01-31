namespace Toubab.Beinder.Mock
{
    using System;
    using System.Threading.Tasks;
    using Mixin;
    using Bindable;

    class MockProperty : IProperty
    {
        public int Changed { get; set; }

        public string Name { get; set; }

        public event EventHandler<BroadcastEventArgs> Broadcast;

        public Type[] ValueTypes
        { 
            get { return new[] { typeof(object) }; }
        }

        object[] _values;

        public object[] Values
        {
            get { return _values; }
        }

        public Task<bool> TryHandleBroadcast(object[] values)
        {
            _values = values;
            Changed++;
            if (Broadcast != null)
                Broadcast(this, new BroadcastEventArgs(this, values));
            return Task.FromResult(true);
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

        public Path.Path Path
        {
            get { return "abc"; }
        }

        public IMixin CloneWithoutObject()
        {
            return new MockProperty
            {
                Name = Name,
                _values = _values
            };
        }

    }
}


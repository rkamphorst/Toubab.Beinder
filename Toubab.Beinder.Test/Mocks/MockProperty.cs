﻿namespace Toubab.Beinder.Mocks
{
    using System;
    using System.Threading.Tasks;
    using Mixins;
    using Paths;
    using Bindables;

    class MockProperty : IProperty
    {
        public int Changed { get; set; }

        public string Name { get; set; }

        Action<object[]> _broadcastListener;

        public void SetBroadcastListener(Action<object[]> listener)
        {
            _broadcastListener = listener;
        }

        public Type[] ValueTypes
        { 
            get { return new[] { typeof(object) }; }
        }

        object _value;

        public object Value
        {
            get { return _value; }
        }

        public Task<bool> TryHandleBroadcast(object[] values)
        {
            _value = values[0];
            Changed++;
            if (_broadcastListener != null)
                _broadcastListener(values);
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

        public Path Path
        {
            get { return "abc"; }
        }

        public IMixin CloneWithoutObject()
        {
            return new MockProperty
            {
                Name = Name,
                _value = _value
            };
        }

    }
}


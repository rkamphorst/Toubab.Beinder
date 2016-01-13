using System;
using System.Reflection;
using Toubab.Beinder.Valve;
using System.Linq;

namespace Toubab.Beinder.Scanner
{

    public abstract class TypeProperty : IBindableState
    {
        protected readonly PropertyInfo _propertyInfo;
        protected readonly IPathParser _pathParser;

        protected TypeProperty(IPathParser pathParser, PropertyInfo property)
        {
            _propertyInfo = property;
            _pathParser = pathParser;
        }

        protected abstract void DetachObjectPropertyChangeEvent(object obj);

        protected abstract void AttachObjectPropertyChangeEvent(object obj);

        protected void OnBroadcast(object source, EventArgs args)
        {
            var evt = Broadcast;
            if (evt != null)
                evt(this, new BindableBroadcastEventArgs(this, Value));
        }

        public event EventHandler<BindableBroadcastEventArgs> Broadcast;

        object _object;

        public object Object
        { 
            get { return _object; }
        }

        public void SetObject(object value)
        {
            var t = _object;
            if (t != null)
                DetachObjectPropertyChangeEvent(t);

            t = _object = value;

            if (t != null)
                AttachObjectPropertyChangeEvent(t);
        }

        Path _path;

        public Path Path
        {
            get
            { 
                if (_path == null)
                    _path = _pathParser.Parse(_propertyInfo.Name); 
                return _path;
            }
        }

        public Type[] ValueType { get { return new[] { _propertyInfo.PropertyType }; } }

        public object[] Value
        {
            get
            { 
                if (!_propertyInfo.CanRead)
                    return null;

                var t = Object;
                return t == null ? new object[]{ null } : new[] { _propertyInfo.GetValue(t) };
            }
        }

        public bool TryHandleBroadcast(object[] value)
        {
            if (!_propertyInfo.CanWrite)
                return false;

            var t = Object;
            if (t != null)
            {
                _propertyInfo.SetValue(t, value[0]);
                return true;
            }
            return false;
        }


        public abstract IBindable CloneWithoutObject();

        public override string ToString()
        {
            return string.Format("[{0}: Path={1}, Value={2}]", GetType().Name, Path, Value);
        }
    }

}

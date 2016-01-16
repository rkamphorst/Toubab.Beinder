using System;
using System.Reflection;
using Toubab.Beinder.Valve;
using System.Linq;

namespace Toubab.Beinder.Bindable
{

    public abstract class Bindable : IBindable
    {
        IPathParser _pathParser;
        Path _path;

        protected Bindable(IPathParser pathParser)
        {
            _pathParser = pathParser;
        }

        protected Bindable(Bindable toCopy)
        {
            _pathParser = toCopy._pathParser;
            _path = toCopy._path;
        }

        object _object;

        public object Object
        { 
            get { return _object; }
        }

        public void SetObject(object value)
        {
            var oldValue = _object;
            BeforeSetObject(oldValue, value);
            _object = value;
            AfterSetObject(oldValue, value);
        }

        public Path Path
        {
            get
            { 
                if (_path == null)
                {
                    _path = _pathParser.Parse(GetName());
                    _pathParser = null;
                }
                return _path;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}->{2} ({3})", 
                GetType().Name,
                Object == null ? "[?]" : Object.GetType().Name, 
                Path, 
                string.Join(",", ValueType.Select(vt => vt.Name))
            );
        }

        protected virtual void BeforeSetObject(object oldValue, object newValue)
        {
        }

        protected virtual void AfterSetObject(object oldValue, object newValue)
        {
        }

        protected abstract string GetName();

        public abstract Type[] ValueType { get; }

        public abstract IBindable CloneWithoutObject();
    }
}

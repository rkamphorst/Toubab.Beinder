using System;
using System.Reflection;
using Toubab.Beinder.Valve;
using System.Linq;

namespace Toubab.Beinder.Scanner
{

    public abstract class ReflectedBindable<T> : IBindable
        where T : MemberInfo
    {
        protected readonly T _memberInfo;
        protected readonly Path _path;

        protected ReflectedBindable(IPathParser pathParser, T memberInfo)
        {
            _memberInfo = memberInfo;
            _path = pathParser.Parse(memberInfo.Name);
        }

        protected ReflectedBindable(ReflectedBindable<T> toCopy) 
        {
            _memberInfo = toCopy._memberInfo;
            _path = toCopy._path;
        }

        protected T Member { get { return _memberInfo; } }

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
                return _path;
            }
        }

        protected virtual void BeforeSetObject(object oldValue, object newValue) 
        {
        }

        protected virtual void AfterSetObject(object oldValue, object newValue) {
        }

        public abstract Type[] ValueType { get; }

        public abstract IBindable CloneWithoutObject();

    }

}

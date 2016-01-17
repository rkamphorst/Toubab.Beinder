using System;
using System.Reflection;
using Toubab.Beinder.Valve;
using System.Linq;
using Toubab.Beinder.Extend;
using Toubab.Beinder.PathParser;

namespace Toubab.Beinder.Bindable
{
        
    public class ReflectedEventHandler : ReflectedBindable<MethodInfo>, IBindableConsumer
    {
        readonly Type[] _parameterTypes;

        public ReflectedEventHandler(IPathParser pathParser, MethodInfo methodInfo)
            : base(pathParser, methodInfo)
        {
            _parameterTypes = Member.GetParameters().Select(p => p.ParameterType).ToArray();
        }

        ReflectedEventHandler(ReflectedEventHandler toCopy)
            : base(toCopy)
        {
            _parameterTypes = toCopy._parameterTypes;
        }

        public override Type[] ValueTypes
        {
            get
            {
                return _parameterTypes;
            }
        }

        public bool TryHandleBroadcast(object[] argument)
        {
            Member.Invoke(Object, argument);
            return true;
        }

        public override IBindable CloneWithoutObject()
        {
            return new ReflectedEventHandler(this);
        }
    }

}

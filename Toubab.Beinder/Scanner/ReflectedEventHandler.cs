using System;
using System.Reflection;
using Toubab.Beinder.Valve;
using System.Linq;

namespace Toubab.Beinder.Scanner
{
        
    public class ReflectedEventHandler : ReflectedBindable<MethodInfo>, IBindableBroadcastConsumer
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

        public override Type ValueType
        {
            get
            {
                return _parameterTypes.Length > 0 ? _parameterTypes[0] : null;
            }
        }

        public bool TryHandleBroadcast(object argument)
        {
            var args = argument as object[];
            if (args.Select(t => t.GetType()).SequenceEqual(_parameterTypes)) {
                Member.Invoke(Object, args);
                return true;
            }
            return false;
        }

        public override IBindable CloneWithoutObject()
        {
            return new ReflectedEventHandler(this);
        }
    }

}

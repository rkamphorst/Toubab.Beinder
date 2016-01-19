using System;
using System.Reflection;
using Toubab.Beinder.Valve;
using System.Linq;
using Toubab.Beinder.Extend;
using Toubab.Beinder.Path;
using Toubab.Beinder.Annex;

namespace Toubab.Beinder.Bindable
{
        
    public class ReflectedEventHandler : ReflectedBindable<MethodInfo>, IEventHandler
    {
        readonly Type[] _parameterTypes;

        public ReflectedEventHandler(Path.Path path, MethodInfo methodInfo)
            : base(path, methodInfo)
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
            if (argument.Length > ValueTypes.Length) 
            {
                var args = new object[ValueTypes.Length];
                Array.Copy(argument, args, args.Length);
                Member.Invoke(Object, args);
            } 
            else 
            {
                Member.Invoke(Object, argument);
            }

            return true;
        }

        public override IAnnex CloneWithoutObject()
        {
            return new ReflectedEventHandler(this);
        }
    }

}

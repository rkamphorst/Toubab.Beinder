using System;
using System.Reflection;
using Toubab.Beinder.Valve;
using System.Linq;

namespace Toubab.Beinder.Scanner
{

    public class ReflectedProperty : ReflectedBindable<PropertyInfo>, IBindableState
    {
        readonly ReflectedEvent _rflEvent;

        public ReflectedProperty(IPathParser pathParser, PropertyInfo propertyInfo, EventInfo eventInfo)
            : base(pathParser, propertyInfo)
        {
            _rflEvent = new ReflectedEvent(pathParser, eventInfo);
            _rflEvent.Broadcast += PropagateBroadcast;
        }

        ReflectedProperty(ReflectedProperty toCopy)
            : base(toCopy)
        {
            _rflEvent = (ReflectedEvent)toCopy._rflEvent.CloneWithoutObject();
            _rflEvent.Broadcast += PropagateBroadcast;
        }

        public override IBindable CloneWithoutObject()
        {
            return new ReflectedProperty(this);
        }

        public override Type[] ValueType
        {
            get
            {
                return new[] { Member.PropertyType };
            }
        }

        protected override void BeforeSetObject(object oldValue, object newValue)
        {
            _rflEvent.SetObject(newValue);
        }

        void PropagateBroadcast(object source, BindableBroadcastEventArgs args)
        {
            var evt = Broadcast;
            if (evt != null)
            {
                var myArgs = new BindableBroadcastEventArgs(this, Values);
                evt(this, myArgs);
            }
        }

        public event EventHandler<BindableBroadcastEventArgs> Broadcast;

        public bool TryHandleBroadcast(object[] argument)
        {
            if (argument.Length != 1)
                return false;
            
            if (!Equals(argument[0], Values[0]))
            {
                try
                {
                    // temporarily set object on reflected event to null,
                    // so event does not fire.
                    _rflEvent.SetObject(null);

                    // set new value
                    if (argument.Length > 0)
                        Member.SetValue(Object, argument[0]);
                }
                finally
                {
                    // restore the object on reflected event
                    _rflEvent.SetObject(Object);
                }
                return true;
            }
            return false;
        }
            
        public object[] Values
        {
            get
            {
                return new[] { Member.GetValue(Object) };
            }
        }
    }
}

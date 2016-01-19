using System;
using System.Reflection;
using Toubab.Beinder.Valve;
using System.Linq;
using Toubab.Beinder.Extend;
using Toubab.Beinder.Path;
using Toubab.Beinder.Bindable;
using Toubab.Beinder.Annex;

namespace Toubab.Beinder.Bindable
{

    public class ReflectedProperty : ReflectedBindable<PropertyInfo>, IProperty
    {
        readonly ReflectedEvent _rflEvent;
        readonly Func<BroadcastEventArgs, bool> _broadcastFilter;

        public ReflectedProperty(
            Path.Path path, 
            PropertyInfo propertyInfo, 
            EventInfo eventInfo = null,
            Func<BroadcastEventArgs, bool> broadcastFilter = null
        )
            : base(path, propertyInfo)
        {
            if (eventInfo != null)
            {
                _rflEvent = new ReflectedEvent(path, eventInfo);
                _rflEvent.Broadcast += PropagateBroadcast;
                _broadcastFilter = broadcastFilter;
            }
        }

        ReflectedProperty(ReflectedProperty toCopy)
            : base(toCopy)
        {
            if (toCopy._rflEvent != null)
            {
                _rflEvent = (ReflectedEvent)toCopy._rflEvent.CloneWithoutObject();
                _rflEvent.Broadcast += PropagateBroadcast;
                _broadcastFilter = toCopy._broadcastFilter;
            }
        }

        public override IAnnex CloneWithoutObject()
        {
            return new ReflectedProperty(this);
        }

        public override Type[] ValueTypes
        {
            get
            {
                return new[] { Member.PropertyType };
            }
        }

        public override void SetObject(object value)
        {
            if (_rflEvent != null)
                _rflEvent.SetObject(value);
            base.SetObject(value);
        }

        void PropagateBroadcast(object source, BroadcastEventArgs args)
        {
            var evt = Broadcast;
            if (evt == null || (_broadcastFilter != null && !_broadcastFilter(args)))
                return;
            var myArgs = new BroadcastEventArgs(Object, Values);
            evt(this, myArgs);
        }

        public event EventHandler<BroadcastEventArgs> Broadcast;

        public bool TryHandleBroadcast(object[] argument)
        {
            var t = Object;
            if (argument.Length != 1 || t == null || !Member.CanWrite)
                return false;
            
            if (!Equals(argument[0], Values[0]))
            {
                try
                {
                    // temporarily set object on reflected event to null,
                    // so event does not fire.
                    if (_rflEvent != null)
                        _rflEvent.SetObject(null);

                    // set new value
                    Member.SetValue(t, argument[0]);
                }
                finally
                {
                    // restore the object on reflected event
                    if (_rflEvent != null)
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
                if (!Member.CanRead)
                    return null;

                var t = Object;
                return t == null ? new object[]{ null } : new[] { Member.GetValue(t) };
            }
        }
    }
}
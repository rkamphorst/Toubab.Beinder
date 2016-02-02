
namespace Toubab.Beinder.Bindables
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Mixins;
    using Paths;

    /// <summary>
    /// Reflected property bindable
    /// </summary>
    /// <remarks>
    /// Adapts a reflected proeprty (<see cref="PropertyInfo"/>) to the <see cref="IProperty"/>
    /// interface.
    /// 
    /// The property's value can be accessed through <see cref="Values"/> at index 0.
    /// In the case of <see cref="ReflectedProperty"/>, <see cref="Values"/> will always 
    /// have a length of 1.
    /// 
    /// If the property's value changes, an event might be raised to indicate this. This event
    /// can be specified in the constructor. 
    /// 
    /// <see cref="ReflectedProperty"/> can also react to <see cref="IEvent.Broadcast"/> events through
    /// <see cref="TryHandleBroadcast"/>: it sets the value of the underlying property
    /// accordingly. While setting the property, the "change event" handler is temporarily
    /// disconnected to prevent <see cref="Broadcast"/> events emanating from this instance,
    /// to prevent <see cref="Broadcast"/> events caused by other <see cref="Broadcast"/> events
    /// (which would result in an endless loop).
    /// </remarks>   
    public class ReflectedProperty : ReflectedBindable<PropertyInfo>, IProperty
    {
        readonly ReflectedEvent _rflEvent;
        readonly Func<BroadcastEventArgs, bool> _broadcastFilter;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>
        /// The <paramref name="eventInfo"/> should represent an event that is raised
        /// when the property changes. If <paramref name="eventInfo"/> is an event that is
        /// also raised when other things happen, specify <paramref name="broadcastFilter"/>
        /// to filter out the events that don't signal a change in this property.
        /// </remarks>
        /// <param name="path">Set the <see cref="Path"/> of the bindable to this value</param>
        /// <param name="propertyInfo">Property to bind to</param>
        /// <param name="eventInfo">Event that indicates a property change</param>
        /// <param name="broadcastFilter">Filter applied on an event by <paramref name="eventInfo"/>,
        /// used to decide whether an event should result in setting the property.
        /// Given a <see cref="BroadcastEventArgs"/> parameter, it should return <c>true</c>
        /// if this is an event that should result in setting the property, <c>false</c> otherwise.</param>
        public ReflectedProperty(
            Path path, 
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

        /// <summary>
        /// Copy constructor (used by <see cref="CloneWithoutObject"/>).
        /// The object this bindable belongs to is not copied.
        /// </summary>
        /// <param name="toCopy">The object to copy into a new instance.</param>
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

        void PropagateBroadcast(object source, BroadcastEventArgs args)
        {
            var evt = Broadcast;
            if (evt == null || (_broadcastFilter != null && !_broadcastFilter(args)))
                return;
            var myArgs = new BroadcastEventArgs(Object, Values);
            evt(this, myArgs);
        }

        /// <inheritdoc/>
        public override IMixin CloneWithoutObject()
        {
            return new ReflectedProperty(this);
        }

        /// <inheritdoc/>
        public override Type[] ValueTypes
        {
            get
            {
                return new[] { Member.PropertyType };
            }
        }

        /// <inheritdoc/>
        public override void SetObject(object value)
        {
            if (_rflEvent != null)
                _rflEvent.SetObject(value);
            base.SetObject(value);
        }

        /// <inheritdoc/>
        public event EventHandler<BroadcastEventArgs> Broadcast;

        /// <inheritdoc/>
        public Task<bool> TryHandleBroadcast(object[] argument)
        {
            var t = Object;
            if (argument.Length != 1 || t == null || !Member.CanWrite)
                return Task.FromResult(false);
            
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
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        /// <inheritdoc/>
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

namespace Toubab.Beinder.Bindables
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Mixins;

    /// <summary>
    /// Combines multiple <see cref="IProperty"/> instances into one
    /// </summary>
    /// <remarks>
    /// This class is used by <see cref="Scanner.CombinedScanner"/> in case two (or more)
    /// scanners return an <see cref="IProperty"/> instance with the same name for the same
    /// object scan. 
    /// 
    /// Multiple <see cref="IProperty"/> instances are combined as follows.
    /// 
    /// **Broadcasts**: All contained properties' broadcast events can result in a broadcast
    /// event from this combined property. The combined property only fires
    /// a broadcast when a contained broadcast carries a changed set of <see cref="Values"/>.
    /// 
    /// **Handle Broadcasts**: The combined property handles broadcasts that are received by calling 
    /// <see cref="TryHandleBroadcast"/> on all contained properties (in order), until 
    /// one signals that the broadcast has been handled (by returning <c>true</c>).
    /// 
    /// See <see cref="CombinedBindable{T}"/> documentation for more information on how 
    /// <see cref="IBindable"/> properties and methods on instances are are combined. 
    /// </remarks>
    public class CombinedProperty : CombinedBindable<IProperty>, IProperty
    {
        readonly CombinedEvent _cmbEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="properties">Combined properties.</param>
        public CombinedProperty(IProperty[] properties)
            : base(properties)
        {
            _values = properties[0].Values;
            _cmbEvent = new CombinedEvent(properties);
        }

        /// <summary>
        /// Copy constructor (used by <see cref="CloneWithoutObject"/>).
        /// The object this bindable belongs to is not copied.
        /// </summary>
        /// <param name="toCopy">The object to copy into a new instance.</param>
        protected CombinedProperty(CombinedProperty toCopy)
            : this(toCopy.Bindables.Select(b => (IProperty)b.CloneWithoutObject()).ToArray()) 
        {
        }

        /// <inheritdoc/>
        public void SetBroadcastListener(Action<object[]> listener) 
        {
            _cmbEvent.SetBroadcastListener(listener);
        }

        object[] _values;

        /// <inheritdoc/>
        public object[] Values
        {
            get
            {
                _values = Bindables[0].Values;
                return _values;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> TryHandleBroadcast(object[] argument)
        {
            // first, make sure _value is up to date
            _values = Bindables[0].Values;

            // if new value equals old value, do nothing.
            if (_values.SequenceEqual(argument))
                return false;

            // prevent lots of events from propagating
            // by setting _value first.
            // That way, HandleContainedPropertyValueChanged won't call 
            // OnValueChanged (if TryHandleBroadcast further down raises
            // an event -- which it shouldn't btw)
            _values = argument;

            // write the property, try each one until one accepts
            foreach (var prop in Bindables)
            {
                if (await prop.TryHandleBroadcast(argument))
                {
                    return true;
                }
            }
            _values = Bindables[0].Values;
            return false;
        }

        /// <inheritdoc/>
        public override IMixin CloneWithoutObject()
        {
            return new CombinedProperty(this);
        }

    }



}

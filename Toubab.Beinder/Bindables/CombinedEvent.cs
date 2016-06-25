namespace Toubab.Beinder.Bindables
{
    using System;
    using System.Linq;
    using Mixins;

    /// <summary>
    /// Combines multiple <see cref="IEvent"/> instances into one
    /// </summary>
    /// <remarks>
    /// This class is used by <see cref="Scanner.CombinedScanner"/> in case two (or more)
    /// scanners return an <see cref="IEvent"/> instance with the same name for the same
    /// object scan. 
    /// 
    /// Multiple <see cref="IEvent"/> instances are combined as follows.
    /// 
    /// **Broadcast**: The combined property raises its own <see cref="Broadcast"/> in 
    /// reaction to broadcasts from only one of the <see cref="IEvent"/> instances it combines;
    /// this is the "chosen event". This is needed to prevent multiple broadcasts originating 
    /// from the same actual event. The "chosen event" is chosen to be the first one that 
    /// raises a <see cref="Broadcast"/> after a call to <see cref="SetObject"/>. After this 
    /// first broadcast, the "chosen event" is the only <see cref="IEvent"/> instance to 
    /// cause broadcasts from the <see cref="CombinedEvent"/> , until <see cref="IMixin.SetObject"/> 
    /// is called again.
    /// 
    /// See <see cref="CombinedBindable{T}"/> documentation for more information on how 
    /// <see cref="IBindable"/> properties and methods on instances are are combined. 
    /// </remarks>
    public class CombinedEvent : CombinedBindable<IEvent>, IEvent
    {

        int _chosenEvent = -1;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="events">Events to combine</param>
        public CombinedEvent(IEvent[] events)
            : base(events)
        {
        }

        /// <summary>
        /// Copy constructor (used by <see cref="CloneWithoutObject"/>).
        /// The object this bindable belongs to is not copied.
        /// </summary>
        /// <param name="toCopy">The object to copy into a new instance.</param>
        protected CombinedEvent(CombinedEvent toCopy)
            : this(toCopy.Bindables.Select(b => (IEvent)b.CloneWithoutObject()).ToArray())
        {
            
        }

        /// <inheritdoc />
        public override BindingOperations Capabilities
        {
            get
            {
                return BindingOperations.Broadcast;
            }
        }

        void SetContainedBroadcastListener(int index, Action<object[]> listener)
        {
            if (listener == null)
            {
                Bindables[index].SetBroadcastListener(null);
            }
            else
            {
                Bindables[index].SetBroadcastListener(payload =>
                    {
                        if (_chosenEvent < 0)
                            _chosenEvent = index;
                        if (Equals(_chosenEvent, index))
                            listener(payload);
                    });
            }
        }

        /// <inheritdoc/>
        public override void SetObject(object value)
        {
            // reset chosen event.
            _chosenEvent = -1;
            base.SetObject(value);
        }

        public void SetBroadcastListener(Action<object[]> listener)
        {
            for (int i = 0; i < Bindables.Length; i++)
            {
                SetContainedBroadcastListener(i, listener);
            }
        }

        /// <inheritdoc/>
        public override IMixin CloneWithoutObject()
        {
            return new CombinedEvent(this);
        }
    }

}

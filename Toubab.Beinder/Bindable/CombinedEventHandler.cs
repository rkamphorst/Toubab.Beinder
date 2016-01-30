namespace Toubab.Beinder.Bindable
{
    using System.Linq;
    using Mixin;

    /// <summary>
    /// Combines multiple <see cref="IEventHandler"/> instances into one
    /// </summary>
    /// <remarks>
    /// This class is used by <see cref="Scanner.CombinedScanner"/> in case two (or more)
    /// scanners return an <see cref="IEventHandler"/> instance with the same name for the same
    /// object scan. 
    /// 
    /// Multiple <see cref="IEventHandler"/> instances are combined as follows.
    /// 
    /// **Handle Broadcasts**: The combined property handles broadcasts that are received by calling 
    /// <see cref="TryHandleBroadcast"/> on all contained properties (in order), until 
    /// one signals that the broadcast has been handled (by returning <c>true</c>).
    /// If <see cref="IBindable.ValueTypes"/> of a contained event handler are not compatible
    /// with the arguments supplied to <see cref="TryHandleBroadcast"/>, that method is not 
    /// called on that instance.
    /// 
    /// See <see cref="CombinedBindable{T}"/> documentation for more information on how 
    /// <see cref="IBindable"/> properties and methods on instances are are combined. 
    /// </remarks>
    public class CombinedEventHandler : CombinedBindable<IEventHandler>, IEventHandler
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="eventHandlers">Combined properties.</param>
        public CombinedEventHandler(IEventHandler[] eventHandlers)
            : base(eventHandlers)
        {
        }

        /// <summary>
        /// Copy constructor (used by <see cref="CloneWithoutObject"/>).
        /// The object this bindable belongs to is not copied.
        /// </summary>
        /// <param name="toCopy">The object to copy into a new instance.</param>
        protected CombinedEventHandler(CombinedEventHandler toCopy)
            : this(toCopy.Bindables.Select(b => (IEventHandler)b.CloneWithoutObject()).ToArray())
        {
        }

        /// <inheritdoc/>
        public bool TryHandleBroadcast(object[] argument)
        {
            foreach (var eventHandler in Bindables)
            {
                if (eventHandler.TryHandleBroadcast(argument))
                {
                    return true;
                }
            }
            return false;
        }

        /// <inheritdoc/>
        public override IMixin CloneWithoutObject()
        {
            return new CombinedEventHandler(this);
        }

    }


}

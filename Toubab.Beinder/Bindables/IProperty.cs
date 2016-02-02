namespace Toubab.Beinder.Bindables
{
    /// <summary>
    /// Bindable property
    /// </summary>
    /// <remarks>
    /// A property stores a state, represented by <see cref="Values"/>.
    /// 
    /// When this state changes, it can raise the <see cref="IEvent.Broadcast"/> event;
    /// 
    /// When another property (or event) raises the <see cref="IEvent.Broadcast"/> event,
    /// this <see cref="IProperty"/> handle that broadcast with <see cref="IEventHandler.TryHandleBroadcast"/>
    /// and update <see cref="Values"/>.
    /// 
    /// Hence, the <see cref="IProperty"/> aggregates the <see cref="IEvent"/> and 
    /// <see cref="IEventHandler"/> interfaces, and adds a property <see cref="Values"/>
    /// to store the state in.
    /// 
    /// Note that an <see cref="IProperty"/> implementation should *only* raise
    /// the <see cref="IEvent.Broadcast"/> event.
    /// </remarks>
    public interface IProperty : IEvent, IEventHandler
    {
        /// <summary>
        /// Value(s) of the property.
        /// </summary>
        /// <remarks>
        /// A property wil generally store only one value. The reason
        /// that <see cref="Values"/> can contain zero or more values (it 
        /// being an array) is to be consistent with the other
        /// bindables (<see cref="IEvent"/>, <see cref="IEventHandler"/>)
        /// which can broadcast or handle zero or more values.
        /// </remarks>
        object[] Values { get; }
    }

}

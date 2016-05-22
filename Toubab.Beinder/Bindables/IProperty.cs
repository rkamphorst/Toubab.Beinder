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
        /// Value of the property.
        /// </summary>
        object Value { get; }
    }

}

namespace Toubab.Beinder.Bindable
{
    using System.Threading.Tasks;

    /// <summary>
    /// Bindable event handler (method)
    /// </summary>
    public interface IEventHandler : IBindable
    {
        /// <summary>
        /// Handle a broadcast by <see cref="IEvent.Broadcast"/> (well, try to).
        /// </summary>
        /// <remarks>
        /// The <paramref name="payload"/> parameter:
        /// 
        ///   * Comes from the <see cref="BroadcastEventArgs.Payload"/>, as broadcast by 
        ///     <see cref="IEvent.Broadcast"/>. 
        ///   * Will be "shared" with other calls, i.e., this array instance is also passed in calls
        ///     to other <see cref="IEventHandler"/> instances that handle the same event.
        ///   * Will have at least the length of the array in <see cref="IBindable.ValueTypes"/>, and each of the 
        ///     types of the payload will be assignable ty the corresponding types in <see cref="IBindable.ValueTypes"/>.
        /// 
        /// If <see cref="BroadcastEventArgs.Payload"/> does not meet these requirements,
        /// <see cref="TryHandleBroadcast"/> will not be called to handle that particular event.
        /// 
        /// If this method *is* called, it returns whetner whether the broadcast could be handled
        /// by returning <c>true</c> or <c>false</c>.
        /// </remarks>
        /// <returns><c>true</c>, if the broadcast could be handled by this <see cref="IEventHandler"/>, 
        /// <c>false</c> otherwise.</returns>
        /// <param name="payload">Payload from the event.</param>
        Task<bool> TryHandleBroadcast(object[] payload);
    }

}

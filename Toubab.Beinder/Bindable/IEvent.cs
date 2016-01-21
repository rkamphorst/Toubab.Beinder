namespace Toubab.Beinder.Bindable
{
    using System;

    /// <summary>
    /// Bindable event
    /// </summary>
    public interface IEvent : IBindable
    {
        /// <summary>
        /// When the event that this <see cref="IEvent" /> represents occurs,
        /// <see cref="Broadcast"/> is raised.
        /// </summary>
        /// <seealso cref="BroadcastEventArgs"/>
        event EventHandler<BroadcastEventArgs> Broadcast;
    }

}

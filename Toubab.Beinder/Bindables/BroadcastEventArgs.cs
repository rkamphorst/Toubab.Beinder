namespace Toubab.Beinder.Bindables
{
    using System;

    /// <summary>
    /// Event arguments for broadcasts
    /// </summary>
    [Obsolete]
    public class BroadcastEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sourceObject">Source object (actual object) for this event.</param>
        /// <param name="payload">The value that is propagated by the event.</param>
        public BroadcastEventArgs(object sourceObject, object[] payload)
        {
            SourceObject = sourceObject;
            Payload = payload;
        }

        /// <summary>
        /// Source object (actual object) for this event.
        /// </summary>
        public object SourceObject { get; private set; }

        /// <summary>
        /// The value that is propagated by the event.
        /// </summary>
        public object[] Payload { get; private set; }
    }
}

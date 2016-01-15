using System;

namespace Toubab.Beinder.Valve
{

    public class BindableBroadcastEventArgs : EventArgs
    {
        public BindableBroadcastEventArgs(IBindable property, object[] payload)
        {
            Source = property;
            Payload = payload;
        }

        public IBindable Source { get; private set; }

        public object[] Payload { get; private set; }
    }

}

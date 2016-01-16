using System;

namespace Toubab.Beinder.Bindable
{

    public class BroadcastEventArgs : EventArgs
    {
        public BroadcastEventArgs(IBindable property, object[] payload)
        {
            Source = property;
            Payload = payload;
        }

        public IBindable Source { get; private set; }

        public object[] Payload { get; private set; }
    }

}

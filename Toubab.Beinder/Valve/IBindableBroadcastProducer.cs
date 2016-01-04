using System;

namespace Toubab.Beinder.Valve
{

    public interface IBindableBroadcastProducer : IBindable
    {
        event EventHandler<BindableBroadcastEventArgs> Broadcast;
    }

}

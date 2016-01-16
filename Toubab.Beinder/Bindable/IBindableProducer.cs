using System;

namespace Toubab.Beinder.Bindable
{

    public interface IBindableProducer : IBindable
    {
        event EventHandler<BroadcastEventArgs> Broadcast;
    }

}

using System;
using Toubab.Beinder.Annex;

namespace Toubab.Beinder.Bindable
{

    public class DelegatedEvent : DelegatedBindable<IEvent>, IEvent
    {

        public DelegatedEvent(IEvent delegateProducer) 
            : base(delegateProducer)
        {
            delegateProducer.Broadcast += DelegateBroadcast;
        }

        protected DelegatedEvent(DelegatedEvent toCopy) 
            : base((DelegatedBindable<IEvent>) toCopy)
        {
            Delegate.Broadcast += DelegateBroadcast;
        }

        void DelegateBroadcast(object sender, BroadcastEventArgs args) 
        {
            var evt = Broadcast;
            if (evt != null)
                evt(this, new BroadcastEventArgs(Object, args.Payload));
        }

        public event EventHandler<BroadcastEventArgs> Broadcast;
        
        public override IAnnex CloneWithoutObject()
        {
            return new DelegatedEvent(this);
        }
    }

}

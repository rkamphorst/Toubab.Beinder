using System;
using Toubab.Beinder.Annex;

namespace Toubab.Beinder.Bindable
{

    public class DelegatedProperty : DelegatedBindable<IProperty>, IProperty
    {
        
        public DelegatedProperty(IProperty delegateState)
            : base(delegateState)
        {
            delegateState.Broadcast += DelegateBroadcast;
        }

        protected DelegatedProperty(DelegatedProperty toCopy)
            : base((DelegatedBindable<IProperty>) toCopy)
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

        public bool TryHandleBroadcast(object[] argument)
        {
            return Delegate.TryHandleBroadcast(argument);
        }

        public object[] Values
        {
            get
            {
                return Delegate.Values;
            }
        }

        public override IAnnex CloneWithoutObject()
        {
            return new DelegatedProperty(this);
        }

    }
}

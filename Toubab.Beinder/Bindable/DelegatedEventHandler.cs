using System;
using Toubab.Beinder.Annex;

namespace Toubab.Beinder.Bindable
{

    public class DelegatedEventHandler : DelegatedBindable<IEventHandler>, IEventHandler
    {
        public DelegatedEventHandler(IEventHandler delegateProducer)
            : base(delegateProducer)
        {
        }

        protected DelegatedEventHandler(DelegatedEventHandler toCopy)
            : base((DelegatedBindable<IEventHandler>) toCopy)
        {
        }

        public bool TryHandleBroadcast(object[] argument)
        {
            return Delegate.TryHandleBroadcast(argument);
        }

        public override IAnnex CloneWithoutObject()
        {
            return new DelegatedEventHandler(this);
        }

    }

}

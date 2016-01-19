using System;
using Toubab.Beinder.Annex;

namespace Toubab.Beinder.Bindable
{
    public abstract class DelegatedBindable<TBindable> : Bindable
        where TBindable : IBindable
    {
        protected readonly TBindable Delegate;

        protected DelegatedBindable(TBindable delegateBindable)
            : base(delegateBindable.Path)
        {
            if (!(delegateBindable.Object is IAnnex))
                throw new ArgumentException(
                    "Delegate bindable has to have a non-null Object that implements IAnnex", 
                    "delegateBindable"
                );
            Delegate = delegateBindable;
        }

        protected DelegatedBindable(DelegatedBindable<TBindable> toCopy) 
            : base(toCopy)
        {
            Delegate = (TBindable) toCopy.Delegate.CloneWithoutObject();
            Delegate.SetObject(((IAnnex)toCopy.Delegate.Object).CloneWithoutObject());
        }

        public override void SetObject(object value)
        {
            ((IAnnex)Delegate.Object).SetObject(value);
            base.SetObject(value);
        }

        public override Type[] ValueTypes
        {
            get
            {
                return Delegate.ValueTypes;
            }
        }

    }

}


namespace Toubab.Beinder.Bindables
{
    using System;

    /// <summary>
    /// Bindable event
    /// </summary>
    public interface IEvent : IBindable
    {
        void SetBroadcastListener(Action<object[]> listener);
    }

}

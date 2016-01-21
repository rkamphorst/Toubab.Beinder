namespace Toubab.Beinder.Extend
{
    using Bindable;

    public interface ICustomEvent<T> : ICustomBindable<T>, IEvent
    {
    }

}

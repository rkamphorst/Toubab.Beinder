namespace Toubab.Beinder.Extend
{
    using Bindables;

    public interface ICustomEvent<T> : ICustomBindable<T>, IEvent
    {
    }

}

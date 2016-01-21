namespace Toubab.Beinder.Extend
{
    using Bindable;

    public interface ICustomEventHandler<T> : ICustomBindable<T>, IEventHandler
    {
    }
}

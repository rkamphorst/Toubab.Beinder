namespace Toubab.Beinder.Extend
{
    using Bindables;

    public interface ICustomEventHandler<T> : ICustomBindable<T>, IEventHandler
    {
    }
}

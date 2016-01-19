using Toubab.Beinder.Bindable;

namespace Toubab.Beinder.Extend
{
    public interface ICustomEventHandler<T> : ICustomBindable<T>, IEventHandler
    {
    }
}

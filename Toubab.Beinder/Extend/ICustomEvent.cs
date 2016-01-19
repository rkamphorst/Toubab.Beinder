using Toubab.Beinder.Bindable;

namespace Toubab.Beinder.Extend
{

    public interface ICustomEvent<T> : ICustomBindable<T>, IEvent
    {
    }

}

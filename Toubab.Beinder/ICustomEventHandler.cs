using Toubab.Beinder.Bindable;

namespace Toubab.Beinder
{

    public interface ICustomEventHandler<T> : ICustomBindable<T>, IBindableConsumer
    {
    }
}

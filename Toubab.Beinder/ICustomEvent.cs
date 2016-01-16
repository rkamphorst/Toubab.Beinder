using Toubab.Beinder.Bindable;

namespace Toubab.Beinder
{

    public interface ICustomEvent<T> : ICustomBindable<T>, IBindableProducer
    {
    }

}

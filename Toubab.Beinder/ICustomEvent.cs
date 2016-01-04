
using Toubab.Beinder.Valve;

namespace Toubab.Beinder
{

    public interface ICustomEvent<T> : ICustomBindable<T>, IBindableBroadcastProducer
    {
    }

}

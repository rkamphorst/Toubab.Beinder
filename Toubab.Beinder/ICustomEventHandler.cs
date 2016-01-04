
using Toubab.Beinder.Valve;

namespace Toubab.Beinder
{

    public interface ICustomEventHandler<T> : ICustomBindable<T>, IBindableBroadcastConsumer
    {
    }
}

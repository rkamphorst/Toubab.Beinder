namespace Toubab.Beinder.Valve
{

    public interface IBindableBroadcastConsumer : IBindable
    {
        bool TryHandleBroadcast(object[] argument);
    }

}

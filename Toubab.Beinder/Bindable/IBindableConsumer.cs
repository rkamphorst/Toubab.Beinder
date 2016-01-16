namespace Toubab.Beinder.Bindable
{

    public interface IBindableConsumer : IBindable
    {
        bool TryHandleBroadcast(object[] argument);
    }

}

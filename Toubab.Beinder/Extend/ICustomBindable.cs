using Toubab.Beinder.Bindable;

namespace Toubab.Beinder.Bindable
{
    public interface ICustomBindable : IBindable
    {
    }

    public interface ICustomBindable<T> : ICustomBindable
    {
    }

}


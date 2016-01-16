using Toubab.Beinder.Bindable;

namespace Toubab.Beinder
{
    public interface ICustomBindable : IBindable
    {
    }

    public interface ICustomBindable<T> : ICustomBindable
    {
    }

}


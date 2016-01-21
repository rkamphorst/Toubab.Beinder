namespace Toubab.Beinder.Extend
{
    using Bindable;

    public interface ICustomBindable : IBindable
    {
    }

    public interface ICustomBindable<T> : ICustomBindable
    {
    }

}


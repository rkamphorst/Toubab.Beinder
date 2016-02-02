namespace Toubab.Beinder.Extend
{
    using Bindables;

    public interface ICustomBindable : IBindable
    {
    }

    public interface ICustomBindable<T> : ICustomBindable
    {
    }

}


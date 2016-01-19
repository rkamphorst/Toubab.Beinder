using System.Reflection;
using Toubab.Beinder.Extend;
using Toubab.Beinder.Path;

namespace Toubab.Beinder.Bindable
{
    /// <summary>
    /// Base class for bindables that were harvested through reflection
    /// </summary>
    /// <remarks>
    /// Used by <see cref="Scanner.NotifyPropertyChangedScanner"/> and 
    /// <see cref="Scanner.ReflectionScanner"/>.
    /// </remarks>
    public abstract class ReflectedBindable<T> : Bindable
        where T : MemberInfo
    {
        protected readonly T _memberInfo;

        protected ReflectedBindable(Path.Path path, T memberInfo)
            : base(path)
        {
            _memberInfo = memberInfo;
        }

        protected ReflectedBindable(ReflectedBindable<T> toCopy)
            : base(toCopy)
        {
            _memberInfo = toCopy._memberInfo;
        }

        protected T Member { get { return _memberInfo; } }

    }


}

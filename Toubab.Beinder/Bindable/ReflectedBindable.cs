using System.Reflection;

namespace Toubab.Beinder.Bindable
{
    /// <summary>
    /// Base class for bindables that were harvested through reflection
    /// </summary>
    /// <remarks>
    /// Used by <see cref="NotifyPropertyChangedScanner"/> and 
    /// <see cref="ReflectionScanner"/>.
    /// </remarks>
    public abstract class ReflectedBindable<T> : Bindable, IBindable
        where T : MemberInfo
    {
        protected readonly T _memberInfo;

        protected ReflectedBindable(IPathParser pathParser, T memberInfo)
            : base(pathParser)
        {
            _memberInfo = memberInfo;
        }

        protected ReflectedBindable(ReflectedBindable<T> toCopy)
            : base(toCopy)
        {
            _memberInfo = toCopy._memberInfo;
        }

        protected T Member { get { return _memberInfo; } }

        protected override string GetName()
        {
            return Member.Name;
        }
    }


}

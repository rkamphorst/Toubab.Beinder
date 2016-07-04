namespace Toubab.Beinder.Bindables
{
    using System.Reflection;
    using Paths;

    /// <summary>
    /// Base class for bindables that were harvested through reflection
    /// </summary>
    /// <remarks>
    /// Subclasses are used by <see cref="Scanners.NotifyPropertyChangedScanner"/> and 
    /// <see cref="Scanners.ReflectionScanner"/>.
    /// </remarks>
    /// <seealso cref="ReflectedEvent"/>
    /// <seealso cref="ReflectedEventHandler"/>
    /// <seealso cref="ReflectedProperty"/>
    public abstract class ReflectedBindable<T> : Bindable
        where T : MemberInfo
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nameSyllables">Set the <see cref="NameSyllables"/> of the bindable to this value</param>
        /// <param name="memberInfo">The reflected member.</param>
        protected ReflectedBindable(Syllables nameSyllables, T memberInfo)
            : base(nameSyllables)
        {
            Member = memberInfo;
        }

        /// <summary>
        /// Copy constructor (used by <see cref="Mixin.IMixin.CloneWithoutObject"/>).
        /// The object this bindable belongs to is not copied.
        /// </summary>
        /// <param name="toCopy">The object to copy into a new instance.</param>
        protected ReflectedBindable(ReflectedBindable<T> toCopy)
            : base(toCopy)
        {
            Member = toCopy.Member;
        }

        /// <summary>
        /// The reflected member (event, property or method)
        /// </summary>
        protected readonly T Member;

    }


}

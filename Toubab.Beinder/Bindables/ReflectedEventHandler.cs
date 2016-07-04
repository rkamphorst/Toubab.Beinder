namespace Toubab.Beinder.Bindables
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Mixins;
    using Paths;

    /// <summary>
    /// Reflected event handler bindable
    /// </summary>
    /// <remarks>
    /// Adapts a reflected event handler (<see cref="MethodInfo"/>) to the <see cref="IEventHandler"/>
    /// interface.
    /// 
    /// <see cref="TryHandleBroadcastAsync(object[])"/> will be called in response to a broadcast from an
    /// <see cref="IEvent"/>: the parameters passed to the listener installed with 
    /// <see cref="IEvent.SetBroadcastListener(Action{object[]})"/> are passed to 
    /// <see cref="TryHandleBroadcastAsync(object[])"/> by <see cref="Valves.Valve2"/>.
    /// <see cref="TryHandleBroadcastAsync(object[])"/> in turn passes them as arguments to the method 
    /// represented by <see cref="ReflectedBindable{T}.Member"/>.
    /// </remarks>    
    public class ReflectedEventHandler : ReflectedBindable<MethodInfo>, IEventHandler
    {
        readonly Type[] _parameterTypes;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nameSyllables">Set the <see cref="NameSyllables"/> of the bindable to this value</param>
        /// <param name="methodInfo">The reflected event handler (method).</param>
        public ReflectedEventHandler(Syllables nameSyllables, MethodInfo methodInfo)
            : base(nameSyllables, methodInfo)
        {
            _parameterTypes = Member.GetParameters().Select(p => p.ParameterType).ToArray();
        }

        /// <summary>
        /// Copy constructor (used by <see cref="CloneWithoutObject"/>).
        /// The object this bindable belongs to is not copied.
        /// </summary>
        /// <param name="toCopy">The object to copy into a new instance.</param>
        ReflectedEventHandler(ReflectedEventHandler toCopy)
            : base(toCopy)
        {
            _parameterTypes = toCopy._parameterTypes;
        }

        /// <inheritdoc />
        public override BindingOperations Capabilities
        {
            get
            {
                return BindingOperations.HandleBroadcast;
            }
        }

        /// <inheritdoc/>
        public override Type[] ValueTypes
        {
            get
            {
                return _parameterTypes;
            }
        }

        /// <inheritdoc/>
        public Task<bool> TryHandleBroadcastAsync(object[] argument)
        {
            if (argument.Length > ValueTypes.Length) 
            {
                var args = new object[ValueTypes.Length];
                Array.Copy(argument, args, args.Length);
                Member.Invoke(Object, args);
            } 
            else 
            {
                Member.Invoke(Object, argument);
            }

            return Task.FromResult(true);
        }

        /// <inheritdoc/>
        public override IMixin CloneWithoutObject()
        {
            return new ReflectedEventHandler(this);
        }
    }

}

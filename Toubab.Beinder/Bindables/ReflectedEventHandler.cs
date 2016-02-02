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
    /// <see cref="TryHandleBroadcast"/> will be called in response to a <see cref="IEvent.Broadcast"/>
    /// event. When this happens, the parameters in <see cref="BroadcastEventArgs.Payload"/> are passed
    /// to the method represented by <see cref="ReflectedBindable{T}.Member"/>.
    /// </remarks>    
    public class ReflectedEventHandler : ReflectedBindable<MethodInfo>, IEventHandler
    {
        readonly Type[] _parameterTypes;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Set the <see cref="Path"/> of the bindable to this value</param>
        /// <param name="methodInfo">The reflected event handler (method).</param>
        public ReflectedEventHandler(Path path, MethodInfo methodInfo)
            : base(path, methodInfo)
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

        /// <inheritdoc/>
        public override Type[] ValueTypes
        {
            get
            {
                return _parameterTypes;
            }
        }

        /// <inheritdoc/>
        public Task<bool> TryHandleBroadcast(object[] argument)
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

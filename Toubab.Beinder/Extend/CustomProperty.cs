namespace Toubab.Beinder.Extend
{
    using System;
    using System.Threading.Tasks;
    using Bindables;
    using Paths;

    public abstract class CustomProperty<T> : Bindable, ICustomProperty<T>
    {
        protected CustomProperty(Fragment nameSyllables) 
            : base(nameSyllables)
        {
        }

        protected CustomProperty(CustomEvent<T> toCopy)
            : base(toCopy)
        {
        }

        /// <summary>
        /// Capabilities of this property
        /// </summary>
        /// <remarks>
        /// Capabilities are pertly based on the values returned by <see cref="CanWrite"/>
        /// and <see cref="CanBroadcast"/>.
        /// </remarks>
        public override BindingOperations Capabilities
        {
            get
            {
                var result = BindingOperations.Read;
                if (CanWrite)
                    result |= BindingOperations.HandleBroadcast;
                if (CanBroadcast)
                    result |= BindingOperations.Broadcast;
                return result;
            }
        }

        /// <summary>
        /// Whether it is possible to write a value to this property
        /// </summary>
        /// <remarks>
        /// This property should always return the same for a given property!
        /// I.e., it should not be subject to circumstantial changes.
        /// </remarks>
        public abstract bool CanWrite { get; }

        /// <summary>
        /// Whether this property is capable of broadcasting its value, i.e.,
        /// whether a broadcast listener will be notified when the value changes.
        /// </summary>
        /// <remarks>
        /// This property should always return the same for a given property!
        /// I.e., it should not be subject to circumstantial changes.
        /// </remarks>
        public abstract bool CanBroadcast { get; }

        public abstract void SetBroadcastListener(Action<object[]> listener);
    
        public abstract Task<bool> TryHandleBroadcastAsync(object[] payload);

        public abstract object Value { get; }
    }
}

namespace Toubab.Beinder.Bindables
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Mixins;
    using Paths;

    /// <summary>
    /// Reflected event bindable
    /// </summary>
    /// <remarks>
    /// Adapts a reflected event (<see cref="EventInfo"/>) to the <see cref="IEvent"/>
    /// interface.
    /// 
    /// This class raises <see cref="Broadcast"/> every time the actual event is raised.
    /// The <see cref="BroadcastEventArgs.Payload"/> will contain the parameters that were 
    /// passed to the event delegate.
    /// 
    /// An event handler that raises the <see cref="Broadcast"/> event is attached to the
    /// original event when <see cref="SetObject"/> is called (with a non-null parameter). 
    /// If a non-null object was already attached (<see cref="Object"/> was not null), 
    /// the event handlers are first detached from that object.
    /// 
    /// When <see cref="SetObject"/> is called with a <c>null</c> parameter, event handlers 
    /// are detached from the <see cref="Object"/> if it was non-<c>null</c>.
    /// 
    /// Note that only events with a <c>void</c> return type, and with up to 32 parameters
    /// are supported. If you attempt to construct this type with events that don't meet
    /// these requirements, an <see cref="ArgumentException"/> is thrown.
    /// </remarks>
    public class ReflectedEvent : ReflectedBindable<EventInfo>, IEvent
    {
        const string HandleEventMethod = "HandleEvent";
        const string DelegateInvokeMethod = "Invoke";

        readonly Delegate _handleEventDelegate;
        readonly Type[] _parameterTypes;

        readonly Func<object[], bool> _broadcastFilter;
        Action<object[]> _broadcastListener;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="eventInfo"/> has more than 32 parameters or a 
        ///     non-void return type.
        /// </exception>
        /// <param name="nameSyllables">Set the <see cref="Name"/> of the bindable to this value</param>
        /// <param name="eventInfo">The reflected event.</param>
        /// <param name="broadcastFilter">If <paramref name="eventInfo"/> refers to an event that is
        /// overloaded (e.g. this is the case with <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/>),
        /// the broadcastFilter decides which events are represented by this bindable.
        /// The broadcastFilter is a callback that takes the broadcast payload and returns true
        /// if this is an event that should be broadcast. If not supplied, default is to let all
        /// events through.</param>
        public ReflectedEvent(Fragment nameSyllables, EventInfo eventInfo, Func<object[], bool> broadcastFilter)
            : base(nameSyllables, eventInfo)
        {
            var invokeMethod = Member.EventHandlerType
                .GetRuntimeMethods().First(m => Equals(m.Name, DelegateInvokeMethod));
            _parameterTypes = invokeMethod.GetParameters().Select(p => p.ParameterType).ToArray();

            if (_parameterTypes.Length > 32)
                throw new ArgumentException("Not supported: Event delegate has more than 32 parameters", "eventInfo");
            if (invokeMethod.ReturnType != typeof(void))
                throw new ArgumentException("Not supported: Event delegate has a non-void return type", "eventInfo");

            var handlerMethodInfo =
                typeof(ReflectedEvent).GetRuntimeMethods().Single(
                    m => Equals(m.Name, HandleEventMethod) && m.GetParameters().Length == _parameterTypes.Length
                );
            if (_parameterTypes.Length > 0)
                handlerMethodInfo = handlerMethodInfo.MakeGenericMethod(_parameterTypes);
            _handleEventDelegate = handlerMethodInfo.CreateDelegate(eventInfo.EventHandlerType, this);
            _broadcastFilter = broadcastFilter;
        }

        /// <summary>
        /// Copy constructor (used by <see cref="CloneWithoutObject"/>).
        /// The object this bindable belongs to is not copied.
        /// </summary>
        /// <param name="toCopy">The object to copy into a new instance.</param>
        ReflectedEvent(ReflectedEvent toCopy)
            : base(toCopy)
        {
            _parameterTypes = toCopy._parameterTypes;
            _handleEventDelegate = toCopy._handleEventDelegate.GetMethodInfo().CreateDelegate(Member.EventHandlerType, this);
            _broadcastFilter = toCopy._broadcastFilter;
        }

        /// <inheritdoc />
        public override BindingOperations Capabilities
        {
            get
            {
                return BindingOperations.Broadcast;
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

        public void SetBroadcastListener(Action<object[]> listener)
        {
            var oldListener = _broadcastListener;
            _broadcastListener = listener;

            if (oldListener != null && listener == null)
                Member.RemoveEventHandler(Object, _handleEventDelegate);
            else if (oldListener == null && listener != null)
                Member.AddEventHandler(Object, _handleEventDelegate);
        }

        /// <inheritdoc/>
        public override IMixin CloneWithoutObject()
        {
            return new ReflectedEvent(this);
        }

        #region event handlers

        /*
         * HandleEvent() has 33 overloads (to support 0 to 32 parameters).
         * 
         * One of them is used to create a delegate in the constructor. Which one depends on how
         * many parameters the actual event has.
         * 
         * Every overload ov HandleEvent() calls OnBroadcast() with all the parameters
         * that were passed to HandleEvent();
         */

        void HandleEvent()
        {
            OnBroadcast();
        }

        void HandleEvent<T1>(T1 p1)
        { 
            OnBroadcast(p1);
        }

        void HandleEvent<T1,T2>(T1 p1, T2 p2)
        {
            OnBroadcast(p1, p2);
        }

        void HandleEvent<T1, T2, T3>(T1 p1, T2 p2, T3 p3)
        {
            OnBroadcast(p1, p2, p3);
        }

        void HandleEvent<T1, T2, T3, T4>(T1 p1, T2 p2, T3 p3, T4 p4)
        {
            OnBroadcast(p1, p2, p3, p4);
        }

        void HandleEvent<T1, T2, T3, T4, T5>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
        {
            OnBroadcast(p1, p2, p3, p4, p5);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16, T17 p17)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16, T17 p17, T18 p18)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16, T17 p17, T18 p18, T19 p19)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16, T17 p17, T18 p18, T19 p19, T20 p20)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16, T17 p17, T18 p18, T19 p19, T20 p20, T21 p21)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16, T17 p17, T18 p18, T19 p19, T20 p20, T21 p21, T22 p22)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16, T17 p17, T18 p18, T19 p19, T20 p20, T21 p21, T22 p22, T23 p23)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16, T17 p17, T18 p18, T19 p19, T20 p20, T21 p21, T22 p22, T23 p23, T24 p24)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16, T17 p17, T18 p18, T19 p19, T20 p20, T21 p21, T22 p22, T23 p23, T24 p24, T25 p25)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24, p25);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16, T17 p17, T18 p18, T19 p19, T20 p20, T21 p21, T22 p22, T23 p23, T24 p24, T25 p25, T26 p26)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24, p25, p26);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16, T17 p17, T18 p18, T19 p19, T20 p20, T21 p21, T22 p22, T23 p23, T24 p24, T25 p25, T26 p26, T27 p27)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24, p25, p26, p27);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16, T17 p17, T18 p18, T19 p19, T20 p20, T21 p21, T22 p22, T23 p23, T24 p24, T25 p25, T26 p26, T27 p27, T28 p28)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24, p25, p26, p27, p28);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16, T17 p17, T18 p18, T19 p19, T20 p20, T21 p21, T22 p22, T23 p23, T24 p24, T25 p25, T26 p26, T27 p27, T28 p28, T29 p29)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24, p25, p26, p27, p28, p29);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16, T17 p17, T18 p18, T19 p19, T20 p20, T21 p21, T22 p22, T23 p23, T24 p24, T25 p25, T26 p26, T27 p27, T28 p28, T29 p29, T30 p30)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24, p25, p26, p27, p28, p29, p30);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16, T17 p17, T18 p18, T19 p19, T20 p20, T21 p21, T22 p22, T23 p23, T24 p24, T25 p25, T26 p26, T27 p27, T28 p28, T29 p29, T30 p30, T31 p31)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24, p25, p26, p27, p28, p29, p30, p31);
        }

        void HandleEvent<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, T21, T22, T23, T24, T25, T26, T27, T28, T29, T30, T31, T32>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9, T10 p10, T11 p11, T12 p12, T13 p13, T14 p14, T15 p15, T16 p16, T17 p17, T18 p18, T19 p19, T20 p20, T21 p21, T22 p22, T23 p23, T24 p24, T25 p25, T26 p26, T27 p27, T28 p28, T29 p29, T30 p30, T31 p31, T32 p32)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24, p25, p26, p27, p28, p29, p30, p31, p32);
        }

        #endregion

        void OnBroadcast(params object[] value)
        {
            var lst = _broadcastListener;
            var bcf = _broadcastFilter;
            if (lst != null && (bcf == null || bcf(value)))
                lst(value);
        }
    }

}

using System;
using System.Reflection;
using Toubab.Beinder.Valve;
using System.Linq;

namespace Toubab.Beinder.Scanner
{

    public class ReflectedEvent : ReflectedBindable<EventInfo>, IBindableBroadcastProducer
    {
        readonly Delegate _handleEventDelegate;
        readonly Type[] _parameterTypes;

        public ReflectedEvent(IPathParser pathParser, EventInfo eventInfo)
            : base(pathParser, eventInfo)
        {
            var invokeMethod = Member.EventHandlerType
                .GetRuntimeMethods().First(m => Equals(m.Name, "Invoke"));
            _parameterTypes = invokeMethod.GetParameters().Select(p => p.ParameterType).ToArray();

            var handlerMethodInfo =
                typeof(ReflectedEvent).GetRuntimeMethods().Single(
                    m => Equals(m.Name, "HandleEvent") && m.GetParameters().Length == _parameterTypes.Length
                );
            _handleEventDelegate = handlerMethodInfo.CreateDelegate(eventInfo.EventHandlerType, this);
        }

        ReflectedEvent(ReflectedEvent toCopy) 
            : base(toCopy)
        {
            _handleEventDelegate = toCopy._handleEventDelegate;
        }

        public override Type[] ValueType
        {
            get
            {
                return _parameterTypes;
            }
        }

        public event EventHandler<BindableBroadcastEventArgs> Broadcast;

        protected override void BeforeSetObject(object oldValue, object newValue)
        {
            if (oldValue != null)
                Member.RemoveEventHandler(oldValue, _handleEventDelegate);
        }

        protected override void AfterSetObject(object oldValue, object newValue)
        {
            if (newValue != null)
                Member.AddEventHandler(newValue, _handleEventDelegate);
        }

        public override IBindable CloneWithoutObject()
        {
            return new ReflectedEvent(this);
        }

        #region event handlers

        void HandleEvent(object p1)
        { 
            OnBroadcast(p1);
        }

        void HandleEvent(object p1, object p2)
        {
            OnBroadcast(p1, p2);
        }

        void HandleEvent(object p1, object p2, object p3)
        {
            OnBroadcast(p1, p2, p3);
        }

        void HandleEvent(object p1, object p2, object p3, object p4)
        {
            OnBroadcast(p1, p2, p3, p4);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5)
        {
            OnBroadcast(p1, p2, p3, p4, p5);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12, object p13)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12, object p13, object p14)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12, object p13, object p14, object p15)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12, object p13, object p14, object p15, object p16)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12, object p13, object p14, object p15, object p16, object p17)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12, object p13, object p14, object p15, object p16, object p17, object p18)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12, object p13, object p14, object p15, object p16, object p17, object p18, object p19)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12, object p13, object p14, object p15, object p16, object p17, object p18, object p19, object p20)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12, object p13, object p14, object p15, object p16, object p17, object p18, object p19, object p20, object p21)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12, object p13, object p14, object p15, object p16, object p17, object p18, object p19, object p20, object p21, object p22)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12, object p13, object p14, object p15, object p16, object p17, object p18, object p19, object p20, object p21, object p22, object p23)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12, object p13, object p14, object p15, object p16, object p17, object p18, object p19, object p20, object p21, object p22, object p23, object p24)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12, object p13, object p14, object p15, object p16, object p17, object p18, object p19, object p20, object p21, object p22, object p23, object p24, object p25)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24, p25);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12, object p13, object p14, object p15, object p16, object p17, object p18, object p19, object p20, object p21, object p22, object p23, object p24, object p25, object p26)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24, p25, p26);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12, object p13, object p14, object p15, object p16, object p17, object p18, object p19, object p20, object p21, object p22, object p23, object p24, object p25, object p26, object p27)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24, p25, p26, p27);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12, object p13, object p14, object p15, object p16, object p17, object p18, object p19, object p20, object p21, object p22, object p23, object p24, object p25, object p26, object p27, object p28)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24, p25, p26, p27, p28);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12, object p13, object p14, object p15, object p16, object p17, object p18, object p19, object p20, object p21, object p22, object p23, object p24, object p25, object p26, object p27, object p28, object p29)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24, p25, p26, p27, p28, p29);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12, object p13, object p14, object p15, object p16, object p17, object p18, object p19, object p20, object p21, object p22, object p23, object p24, object p25, object p26, object p27, object p28, object p29, object p30)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24, p25, p26, p27, p28, p29, p30);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12, object p13, object p14, object p15, object p16, object p17, object p18, object p19, object p20, object p21, object p22, object p23, object p24, object p25, object p26, object p27, object p28, object p29, object p30, object p31)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24, p25, p26, p27, p28, p29, p30, p31);
        }

        void HandleEvent(object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9, object p10, object p11, object p12, object p13, object p14, object p15, object p16, object p17, object p18, object p19, object p20, object p21, object p22, object p23, object p24, object p25, object p26, object p27, object p28, object p29, object p30, object p31, object p32)
        {
            OnBroadcast(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24, p25, p26, p27, p28, p29, p30, p31, p32);
        }

        #endregion

        void OnBroadcast(params object[] value)
        {
            var evt = Broadcast;
            if (evt != null)
                evt(this, new BindableBroadcastEventArgs(this, value));
        }
    }

}

using System;

namespace Toubab.Beinder.Mock
{
    public class ClassWithPropertyAndEvents
    {
        public struct ValueTypeForComplexEvent
        {
            public string A { get; set; }

            public int B { get; set; }

        }

        string _property;

        public string Property
        {
            get { return _property; }
            set
            { 
                _property = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler PropertyChanged;

        string _secondProperty;

        public string SecondProperty
        {
            get { return _secondProperty; }
            set
            { 
                _secondProperty = value;
                if (SecondPropertyChanged != null)
                    SecondPropertyChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler SecondPropertyChanged;


        public void OnSimpleEvent()
        {
            var evt = SimpleEvent;
            if (evt != null)
            {
                evt(this, EventArgs.Empty);
            }
        }

        public event EventHandler SimpleEvent;

        public void OnComplexEvent(string str, int i, ClassWithPropertyAndEvents cwpae, ValueTypeForComplexEvent vtfce, object obj)
        {
            var evt = ComplexEvent;
            if (evt != null)
            {
                evt(str, i, cwpae, vtfce, obj);
            }
        }

        public event Action<string, int, ClassWithPropertyAndEvents, ValueTypeForComplexEvent, object> ComplexEvent;

    }
}

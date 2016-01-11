using System;

namespace Toubab.Beinder.Mock
{

    public class ClassWithPropertyAndEvents 
    {
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

    }
}

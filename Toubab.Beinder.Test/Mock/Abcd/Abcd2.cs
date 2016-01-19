using System;
using Toubab.Beinder.Tools;

namespace Toubab.Beinder.Mock.Abcd
{

    public class Abcd2 : NotifyPropertyChanged
    {
        public Abcd2()
        {
            AaaBee = new AB();
        }

        public class AB : NotifyPropertyChanged
        {
            public AB()
            {
                CeeDee = new CD();
            }

            CD _ceeDee;

            public CD CeeDee
            {
                get { return _ceeDee; }
                set { SetProperty(ref _ceeDee, value); }
            }
        }

        public class CD : NotifyPropertyChanged
        {
            string _eee;

            public string Eee
            {
                get { return _eee; }
                set { SetProperty(ref _eee, value); }
            }
        }

        AB _aaaBee;

        public AB AaaBee
        {
            get { return _aaaBee; }
            set { SetProperty(ref _aaaBee, value); }
        }
    }

}

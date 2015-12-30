using System;

namespace Toubab.Beinder.Mocks
{


    public class Abcd3 : NotifyPropertyChanged
    {
        public Abcd3()
        {
            Aaa = new A();
        }

        public class A : NotifyPropertyChanged
        {
            public A()
            {
                BeeCee = new BC();
            }

            BC _beeCee;

            public BC BeeCee
            {
                get { return _beeCee; }
                set { SetProperty(ref _beeCee, value); }
            }
        }

        public class  BC : NotifyPropertyChanged
        {
            public BC()
            {
                Dee = new D();
            }

            D _dee;

            public D Dee
            {
                get { return _dee; }
                set { SetProperty(ref _dee, value); }
            }
        }

        public class D : NotifyPropertyChanged
        {
            string _eee;

            public string Eee
            {
                get { return _eee; }
                set { SetProperty(ref _eee, value); }
            }
        }

        A _aaa;

        public A Aaa
        {
            get { return _aaa; }
            set { SetProperty(ref _aaa, value); }
        }
    }

}


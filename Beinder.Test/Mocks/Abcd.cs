using System;

namespace Beinder.Mocks
{
    public class Abcd1 : NotifyPropertyChanged
    {
        public Abcd1()
        {
            Aaa = new A();
        }

        public class A : NotifyPropertyChanged
        {
            public A()
            {
                Bee = new B();
            }

            B _bee;

            public B Bee
            {
                get { return _bee; }
                set { SetProperty(ref _bee, value); }
            }
        }

        public class B : NotifyPropertyChanged
        {
            public B()
            {
                Cee = new C();
            }

            C _cee;

            public C Cee
            {
                get { return _cee; }
                set { SetProperty(ref _cee, value); }
            }
        }

        public class C : NotifyPropertyChanged
        {
            public C()
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


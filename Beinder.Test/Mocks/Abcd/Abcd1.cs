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

}

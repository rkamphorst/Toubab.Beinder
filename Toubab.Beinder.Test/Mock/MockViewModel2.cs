using System;
using Toubab.Beinder.Tools;

namespace Toubab.Beinder.Mock
{

    public class MockViewModel2 : NotifyPropertyChanged
    {

        MockControlViewModel _control;

        public MockControlViewModel Control
        {
            get { return _control; }
            set { SetProperty(ref _control, value); }
        }
    }
}

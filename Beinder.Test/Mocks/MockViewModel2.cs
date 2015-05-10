using System;

namespace Beinder.Mocks
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

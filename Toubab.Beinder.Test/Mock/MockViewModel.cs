using System;

namespace Toubab.Beinder.Mock
{

    public class MockViewModel : NotifyPropertyChanged
    {

        string _controlText;

        public string ControlText
        {
            get { return _controlText; }
            set { SetProperty(ref _controlText, value); }
        }

        int _controlSize;

        public int ControlSize
        {
            get { return _controlSize; }
            set { SetProperty(ref _controlSize, value); }
        }

        int _specialProperty;

        public int SpecialProperty
        {
            get { return _specialProperty; }
            set { SetProperty(ref _specialProperty, value); }
        }

        string _specialProperty2;

        public string SpecialProperty2
        {
            get { return _specialProperty2; }
            set { SetProperty(ref _specialProperty2, value); }
        }

        public int ClickCount = 0;

        public void Click()
        {
            ClickCount++;
        }
    }
}

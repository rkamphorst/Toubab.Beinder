namespace Toubab.Beinder.Mocks
{
    using System.Windows.Input;
    using Tools;

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

        public ICommand _bogusCommand;

        public ICommand BogusCommand
        { 
            get
            {
                if (_bogusCommand == null)
                {
                    _bogusCommand = new Command(
                        execute: p => 
                        {
                            if (!(p is int)) 
                            {
                                BogusCount++;
                            } 
                            else 
                            {
                                BogusCount += (int)p;
                            }
                            
                        }, 
                        canExecute: p => Equals(p, (object)42) || string.IsNullOrEmpty(SpecialProperty2)
                    );
                }
                return _bogusCommand;
            }
        }

        public int BogusCount = 0;
    }
}

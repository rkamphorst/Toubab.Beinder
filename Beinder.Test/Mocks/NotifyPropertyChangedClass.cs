namespace Beinder.Mocks
{
    public class NotifyPropertyChangedClass : NotifyPropertyChanged 
    {
        string _property;

        public string Property
        {
            get { return _property; }
            set { SetProperty(ref _property, value); }
        }

        object _secondProperty;

        public object SecondProperty
        {
            get { return _secondProperty; }
            set { SetProperty(ref _secondProperty, value); }
        }
    }
}

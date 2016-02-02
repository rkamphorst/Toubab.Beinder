namespace Toubab.Beinder.Mocks
{
    using Mixins;
    using Tools;
    using Extend;

    public class MockViewMixin : NotifyPropertyChanged, ICustomMixin<MockView>
    {

        MockView _mockView;

        public void SetObject(object newObject)
        {
            _mockView = ((MockView) newObject);
            _specialProperty = _mockView.GetSpecialProperty();
       }

        int _specialProperty;

        public int SpecialProperty
        {
            get { return _specialProperty; }
            set
            { 
                if (SetProperty(ref _specialProperty, value))
                {
                    _mockView.SetSpecialProperty(value);
                }
            }
        }

        public IMixin CloneWithoutObject() 
        {
            return new MockViewMixin();
        }

    }
}

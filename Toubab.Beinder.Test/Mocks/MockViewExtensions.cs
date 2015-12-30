using System;
using System.ComponentModel;

namespace Toubab.Beinder.Mocks
{

    public class MockViewExtensions : NotifyPropertyChanged, IExtensions<MockView>
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

        public IExtensions CloneWithoutObject() 
        {
            return new MockViewExtensions();
        }

    }
}
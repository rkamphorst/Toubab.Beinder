using System;
using System.ComponentModel;

namespace Beinder.Mocks
{

    public class MockViewExtensions : NotifyPropertyChanged, IExtensions<MockView>
    {

        MockView _mockView;

        public bool TrySetObject(object newObject)
        {
            _mockView = newObject as MockView;
            if (_mockView != null)
                _specialProperty = _mockView.GetSpecialProperty();
            return true;
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

        public IExtensions Clone() 
        {
            return new MockViewExtensions();
        }

    }
}

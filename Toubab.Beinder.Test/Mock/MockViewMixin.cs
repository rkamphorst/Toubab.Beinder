using System;
using System.ComponentModel;
using Toubab.Beinder.Mixin;

namespace Toubab.Beinder.Mock
{

    public class MockViewMixin : NotifyPropertyChanged, IMixin<MockView>
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

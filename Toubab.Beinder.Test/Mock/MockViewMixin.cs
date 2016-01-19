using System;
using System.ComponentModel;
using Toubab.Beinder.Mixin;
using Toubab.Beinder.Annex;
using Toubab.Beinder.Tools;

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

        public IAnnex CloneWithoutObject() 
        {
            return new MockViewMixin();
        }

    }
}

using System;
using System.ComponentModel;

namespace Toubab.Beinder.Mocks
{

    public class MockViewExtensions2 : ITypeExtension<MockView>
    {

        MockView _mockView;

        public void SetObject(object newObject)
        {
            _mockView = (MockView) newObject;
        }

        public ITypeExtension CloneWithoutObject() 
        {
            return new MockViewExtensions2();
        }


        public string SpecialProperty2
        {
            get { return _mockView.GetSpecialProperty2(); }
            set 
            {
                _mockView.SetSpecialProperty2(value);
                var evt = SpecialProperty2Changed;
                if (evt != null) 
                {
                    evt(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler SpecialProperty2Changed;

    }
}

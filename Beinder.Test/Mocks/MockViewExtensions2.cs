using System;
using System.ComponentModel;

namespace Beinder.Mocks
{

    public class MockViewExtensions2 : IExtensions<MockView>
    {

        MockView _mockView;

        public bool TrySetObject(object newObject)
        {
            _mockView = newObject as MockView;
            return Equals(_mockView,newObject);
        }

        public IExtensions Clone() 
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

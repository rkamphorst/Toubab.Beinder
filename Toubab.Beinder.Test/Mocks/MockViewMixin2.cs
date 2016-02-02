namespace Toubab.Beinder.Mocks
{
    using System;
    using Mixins;
    using Extend;

    public class MockViewMixin2 : ICustomMixin<MockView>
    {

        MockView _mockView;

        public void SetObject(object newObject)
        {
            _mockView = (MockView) newObject;
        }

        public IMixin CloneWithoutObject() 
        {
            return new MockViewMixin2();
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

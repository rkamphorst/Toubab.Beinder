using System;

namespace Beinder
{

    public class ChildProperty : CandidateChildProperty
    {
        object _value;

        public ChildProperty(CandidateChildProperty candidate)
            : base(candidate)
        {
            _parent.ValueChanged += HandleValueChanged;
            _child.ValueChanged += HandleValueChanged;
        }

        ChildProperty(IProperty parent, IProperty child)
            : base(parent, child)
        {
            _parent.ValueChanged += HandleValueChanged;
            _child.ValueChanged += HandleValueChanged;
        }

        public void HandleValueChanged(object sender, EventArgs e)
        {
            if (!Equals(_child.Object, _parent.Value))
                _child.TrySetObject(_parent.Value);
            
            var newvalue = _child.Value;
            if (!Equals(_value, newvalue))
            {
                _value = newvalue;
                OnValueChanged(newvalue);
            }
        }

        public override IProperty Clone()
        {
            return new ChildProperty(_parent.Clone(), _child.Clone());
        }

        public override string ToString()
        {
            return string.Format("[ChildProperty: {0}, Value={1}]", Path, Value);
        }

    }

}

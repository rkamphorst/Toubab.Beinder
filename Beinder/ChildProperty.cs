using System;

namespace Beinder
{
    public class ChildProperty : IProperty
    {
        readonly PropertyPath _propertyPath;
        readonly IProperty _parent;
        readonly IProperty _child;
        readonly PropertyMetaInfo _metaInfo;
        object _value;

        public ChildProperty(IProperty parent, IProperty child)
        {
            _parent = parent;
            _child = child;
            _propertyPath = _parent.Path + _child.Path;
            _parent.ValueChanged += HandleValueChanged;
            _child.ValueChanged += HandleValueChanged;
            _metaInfo = new PropertyMetaInfo(
                _parent.MetaInfo.ObjectType,
                _child.MetaInfo.ValueType,
                _child.MetaInfo.IsReadable,
                _child.MetaInfo.IsWritable
            );
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

        public PropertyPath Path
        { 
            get { return _propertyPath; } 
        }

        protected virtual void OnValueChanged(object newvalue)
        {
            var evt = ValueChanged;
            if (evt != null)
                evt(this, new ValueChangedEventArgs(newvalue));
        }

        public event EventHandler<ValueChangedEventArgs> ValueChanged;

        public PropertyMetaInfo MetaInfo
        {
            get { return _metaInfo; }
        }

        public object Value
        {
            get
            { 
                if (!Equals(_child.Object, _parent.Value))
                    _child.TrySetObject(_parent.Value);
                return _child.Value; 
            }
        }

        public bool TrySetValue(object value)
        {
            if (!Equals(_child.Object, _parent.Value))
                _child.TrySetObject(_parent.Value);
            return _child.TrySetValue(value);
        }

        public object Object
        {
            get { return _parent.Object; }
        }

        public bool TrySetObject(object value)
        {
            var result = _parent.TrySetObject(value);
            _child.TrySetObject(_parent.Value);
            return result;
        }

        public virtual IProperty Clone()
        {
            return new ChildProperty(_parent.Clone(), _child.Clone());
        }

        public override string ToString()
        {
            return string.Format("[CandidateChildProperty: {0}, Value={1}]", Path, Value);
        }

    }

}

using System;

namespace Beinder
{
    public class CandidateChildProperty : IProperty
    {
        protected IProperty _parent;
        protected IProperty _child;
        readonly PropertyPath _propertyPath;

        public CandidateChildProperty(CandidateChildProperty candidate)
            : this(candidate._parent, candidate._child)
        {
            
        }

        public CandidateChildProperty(IProperty parent, IProperty child)
        {
            _parent = parent;
            _child = child;
            _propertyPath = _parent.Path + _child.Path;
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

        public Type ValueType
        {
            get { return _child.ValueType; }
        }

        public Type ObjectType
        {
            get { return _parent.ObjectType; }
        }

        public bool IsReadable { get { return _child.IsReadable; } }

        public bool IsWritable { get { return _child.IsWritable; } }

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
            return new CandidateChildProperty(_parent.Clone(), _child.Clone());
        }

        public override string ToString()
        {
            return string.Format("[CandidateChildProperty: {0}, Value={1}]", Path, Value);
        }

    }

}

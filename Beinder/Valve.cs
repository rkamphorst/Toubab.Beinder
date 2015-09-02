using System;
using System.Collections.Generic;
using System.Linq;

namespace Beinder
{
    /// <summary>
    /// A Valve is a hub to which properties (<see cref="IProperty"/>) can be attached. 
    /// Whenever one <see cref="IProperty"/>'s value changes, that value is 
    /// propagated to other properties (without propagating it back to the original
    /// property; hence, "Valve").
    /// </summary>
    /// <remarks>
    /// References to the properties are kept weak, i.e., the properties are allowed
    /// to be garbage collected even if they are connected to a <see cref="Valve"/>.
    /// </remarks>
    public class Valve : IProperty, IDisposable
    {
        readonly LinkedList<WeakReference<IProperty>> _properties = 
            new LinkedList<WeakReference<IProperty>>();
        readonly PropertyMetaInfo _metaInfo = 
            new PropertyMetaInfo(null, null, true, true);

        object _value;

        public void AddProperty(IProperty prop)
        {
            AssertNotDisposed();
            lock (_properties)
            {                
                _properties.AddLast(new WeakReference<IProperty>(prop));
                prop.ValueChanged += HandleValueChanged;
            }
        }

        /// <summary>
        /// Give a list of live properties, i.e., properties that have not been garbage
        /// collected
        /// </summary>
        /// <remarks>
        /// Every time this property is accesed, a new list is returned.
        /// Modifying the list will not modify the collection of properties connected
        /// to this valve. To do that, use <see cref="AddProperty" />.
        /// </remarks>
        public List<IProperty> LiveProperties
        {
            get
            {
                AssertNotDisposed();
                return new List<IProperty>(EnumerateLiveRefsAndRemoveDefuncts(_properties));
            }
        }

        #region IProperty implementation


        public PropertyMetaInfo MetaInfo 
        {
            get { return _metaInfo; }
        }

        public object Object
        { 
            get { return null; }
        }

        public bool TrySetObject(object value)
        {
            return false;
        }


        public PropertyPath Path
        {
            get
            { 
                return EnumerateLiveRefsAndRemoveDefuncts(_properties)
                    .Select(p => p.Path).FirstOrDefault();
            }
        }


        public object Value
        {
            get { return _value; }
        }

        public bool TrySetValue(object value)
        {
            AssertNotDisposed();
            if (Push(null, value))
            {
                OnValueChanged(value);
                return true;
            }
            return false;
        }

        public event EventHandler<ValueChangedEventArgs> ValueChanged;

        public IProperty Clone()
        {
            var result = new Valve();
            foreach (var prop in EnumerateLiveRefsAndRemoveDefuncts(_properties))
            {
                result.AddProperty(prop.Clone());
            }
            return result;
        }

        #endregion

        public void AcceptValue(object payload)
        {
            AssertNotDisposed();
            Push(null, payload);
                
        }

        void HandleValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (Push(sender, e.NewValue))
                OnValueChanged(e.NewValue);
        }

        void OnValueChanged(object newValue)
        {
            var evt = ValueChanged;
            if (evt != null)
            {
                evt(this, new ValueChangedEventArgs(newValue));
            }
        }


        #region IDisposable implementation

        bool _disposed;

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Valve()
        {
            if (_disposed)
                return;
            _disposed = true;
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var t in EnumerateLiveRefsAndRemoveDefuncts(_properties))
                    t.ValueChanged -= HandleValueChanged;
            } 
        }

        void AssertNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        #endregion

        bool Push(object source, object payload)
        {
            lock (_properties)
            {
                if (!Equals(payload, _value))
                {
                    _value = payload;
                    foreach (var prop in EnumerateLiveRefsAndRemoveDefuncts(_properties))
                    {
                        if (!ReferenceEquals(source, prop))
                            prop.TrySetValue(payload);
                    }
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("[Valve: Path={0}, Value={1}]", Path, Value);
        }

        static IEnumerable<T> EnumerateLiveRefsAndRemoveDefuncts<T>(LinkedList<WeakReference<T>> list) 
            where T : class
        {
            if (list == null)
                yield break;
            var node = list.First;
            while (node != null)
            {
                T target;
                while (!node.Value.TryGetTarget(out target))
                {
                    node = node.Next;
                    list.Remove(node.Previous);
                    if (node == null)
                        yield break;
                }
                yield return target;
                node = node.Next;
            }
        }
    }


}

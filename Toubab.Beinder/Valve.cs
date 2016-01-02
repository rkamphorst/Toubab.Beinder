using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Toubab.Beinder
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
    public class Valve : IDisposable
    {
        static readonly object _initialValue = new object();

        readonly LinkedList<WeakReference<IProperty>> _properties = 
            new LinkedList<WeakReference<IProperty>>();
        

        object _value = _initialValue;

        public void AddProperty(IProperty prop)
        {
            AssertNotDisposed();
            lock (_properties)
            {                
                _properties.AddLast(new WeakReference<IProperty>(prop));
                prop.ValueChanged += HandleValueChanged;
            }
        }

        public bool Activate(object toActivate)
        {
            if (toActivate == null)
                return false;
            var prop = LiveProperties.FirstOrDefault(p => ReferenceEquals(toActivate, p.Object));
            if (prop != null)
            {
                var value = prop.Value;
                Push(prop, value);
                OnValueChanged(prop, value);
                return true;
            }
            return false;
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

        public object[] GetValues()
        { 
            return LiveProperties.Select(p => p.Value).Where(v => v != null).ToArray();
        }

        public object GetValueForObject(object ob)
        {
            var prop = LiveProperties.FirstOrDefault(p => ReferenceEquals(ob, p.Object));
            return prop != null ? prop.Value : null;
        }

        public event EventHandler<PropertyValueChangedEventArgs> ValueChanged;

        void HandleValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            Push(sender, e.NewValue);
            OnValueChanged(e.Property, e.NewValue);
        }

        void OnValueChanged(IProperty property, object newValue)
        {
            var evt = ValueChanged;
            if (evt != null)
            {
                evt(this, new PropertyValueChangedEventArgs(property, newValue));
            }
        }


        #region IDisposable implementation

        public event EventHandler Disposing;

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
                var evt = Disposing;
                if (evt != null)
                    evt(this, EventArgs.Empty);

                ValueChanged = null;
                Disposing = null;
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
            var payloadType = payload != null ? payload.GetType().GetTypeInfo() : null;
            if (payloadType == null)
            {
                var srcProp = source as IProperty;
                payloadType = srcProp == null ? null : srcProp.ValueType.GetTypeInfo();
            }

            lock (_properties)
            {
                if (!Equals(payload, _value))
                {
                    _value = payload;
                    bool valueWasSet = false;
                    foreach (var prop in EnumerateLiveRefsAndRemoveDefuncts(_properties))
                    {
                        if (!ReferenceEquals(source, prop))
                        {
                            var propValueType = prop.ValueType.GetTypeInfo();
                            if (propValueType.IsAssignableFrom(payloadType))
                            {
                                valueWasSet |= prop.TrySetValue(payload);
                            }
                        }
                    }
                    return valueWasSet;
                }
            }
            return false;
        }

        public override string ToString()
        {
            var firstprop = LiveProperties.FirstOrDefault();
            if (firstprop != null)
            {
                return string.Format("[Valve: Path={0}, Values={1}]", firstprop.Path, string.Join(",", LiveProperties.Select(p => p.Value).Distinct()));  
            }
            else
            {
                return "[Valve: Path=(none), Value=(none)]";
            }
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

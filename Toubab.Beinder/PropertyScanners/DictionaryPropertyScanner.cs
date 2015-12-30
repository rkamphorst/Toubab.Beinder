using System;
using System.Collections.Generic;
using System.Linq;
using Toubab.Beinder.PropertyPathParsers;
using System.Collections.Specialized;

namespace Toubab.Beinder.PropertyScanners
{
    public class DictionaryPropertyScanner : IObjectPropertyScanner
    {
        IPropertyPathParser _pathParser = new CamelCasePropertyPathParser();

        public IPropertyPathParser PathParser
        { 
            get { return _pathParser; }
            set { _pathParser = value; }
        }

        public IEnumerable<IProperty> Scan(object obj)
        {
            var dict = obj as Dictionary<string,object>;

            if (dict == null)
                return Enumerable.Empty<IProperty>();
            
            return dict.Select(
                kvp => new DictionaryEntryProperty(kvp.Key, _pathParser)
            );
        }

        class DictionaryEntryProperty : IProperty
        {
            readonly string _key;
            readonly PropertyPath _propertyPath;
            Dictionary<string,object> _object;

            public DictionaryEntryProperty(string key, IPropertyPathParser pathParser)
            {
                _propertyPath = pathParser.Parse(key);
                _key = key;
            }

            DictionaryEntryProperty(string key, PropertyPath path)
            {
                _propertyPath = path;
                _key = key;
            }

            void HandleDictionaryChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (
                    e.OldItems
                    .Cast<KeyValuePair<string,object>>()
                    .Concat(e.OldItems.Cast<KeyValuePair<string,object>>())
                    .Select(kvp => kvp.Key)
                    .Contains(_key))
                {
                    OnValueChanged(Value);
                }
            }

            void OnValueChanged(object newValue)
            {
                var e = ValueChanged;
                if (e != null)
                    e(this, new PropertyValueChangedEventArgs(this, newValue));
            }

            public event EventHandler<PropertyValueChangedEventArgs> ValueChanged;

            public object Value
            {
                get
                {
                    object result;
                    return _object.TryGetValue(_key, out result) ? result : null;
                }
            }

            public bool TrySetValue(object value)
            { 
                if (value == null)
                    _object.Remove(_key);
                _object[_key] = value;
                return true;
            }

            public object Object
            { 
                get
                {
                    return _object;
                } 
            }

            public void SetObject(object value)
            {
                var newdict = (Dictionary<string,object>)value;

                {
                    var incc = _object as INotifyCollectionChanged;
                    if (incc != null)
                    {
                        incc.CollectionChanged -= HandleDictionaryChanged;
                    }
                }

                _object = newdict;

                {
                    var incc = _object as INotifyCollectionChanged;
                    if (incc != null)
                    {
                        incc.CollectionChanged += HandleDictionaryChanged;
                    }
                }

            }

            public PropertyPath Path
            {
                get
                {
                    return _propertyPath;
                }
            }

            public IProperty CloneWithoutObject()
            {
                return new DictionaryEntryProperty(_key, _propertyPath);
            }

        }
    }


    
}

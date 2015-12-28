using System;
using System.Collections.Generic;
using System.Linq;
using Beinder.PropertyPathParsers;
using System.Collections.Specialized;

namespace Beinder.PropertyScanners
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
                kvp => new DictionaryEntryProperty(dict, kvp.Key, _pathParser)
            );
        }

        class DictionaryEntryProperty : IProperty
        {
            readonly string _key;
            readonly PropertyPath _propertyPath;
            readonly PropertyMetaInfo _metaInfo = new PropertyMetaInfo(null, null, true, true);
            Dictionary<string,object> _object;

            public DictionaryEntryProperty(Dictionary<string,object> obj, string key, IPropertyPathParser pathParser)
            {
                _propertyPath = pathParser.Parse(key);
                _key = key;
                TrySetObject(obj);
            }

            private DictionaryEntryProperty(Dictionary<string,object> obj, string key, PropertyPath path)
            {
                _propertyPath = path;
                _key = key;
                TrySetObject(obj);
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

            public PropertyMetaInfo MetaInfo
            {
                get { return _metaInfo; }
            }

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

            public bool TrySetObject(object value)
            {
                var newdict = value as Dictionary<string,object>;
                if (newdict == null)
                    return false;

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

                return true;
            }

            public PropertyPath Path
            {
                get
                {
                    return _propertyPath;
                }
            }

            public IProperty Clone()
            {
                var result = new DictionaryEntryProperty(_object, _key, _propertyPath);
                result.TrySetObject(Object);
                return result;
            }

        }
    }


    
}

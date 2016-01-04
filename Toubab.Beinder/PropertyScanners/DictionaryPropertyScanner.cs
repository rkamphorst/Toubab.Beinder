using System;
using System.Collections.Generic;
using System.Linq;
using Toubab.Beinder.PropertyPathParsers;
using System.Collections.Specialized;
using Toubab.Beinder.Valve;

namespace Toubab.Beinder.PropertyScanners
{
    public class DictionaryPropertyScanner : IBindableScanner
    {
        IPropertyPathParser _pathParser = new CamelCasePropertyPathParser();

        public IPropertyPathParser PathParser
        { 
            get { return _pathParser; }
            set { _pathParser = value; }
        }

        public IEnumerable<IBindable> Scan(object obj)
        {
            var dict = obj as Dictionary<string,object>;

            if (dict == null)
                return Enumerable.Empty<IBindableState>();
            
            return dict.Select(
                kvp => new DictionaryEntryProperty(kvp.Key, _pathParser)
            );
        }

        class DictionaryEntryProperty : IBindableState
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
                    OnBroadcast(Value);
                }
            }

            void OnBroadcast(object argument)
            {
                var e = Broadcast;
                if (e != null)
                    e(this, new BindableBroadcastEventArgs(this, argument));
            }

            public event EventHandler<BindableBroadcastEventArgs> Broadcast;

            public Type ValueType { get { return typeof(object); } }

            public object Value
            {
                get
                {
                    object result;
                    return _object.TryGetValue(_key, out result) ? result : null;
                }
            }

            public bool TryHandleBroadcast(object argument)
            { 
                if (argument == null)
                    _object.Remove(_key);
                _object[_key] = argument;
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

            public IBindable CloneWithoutObject()
            {
                return new DictionaryEntryProperty(_key, _propertyPath);
            }

        }
    }


    
}

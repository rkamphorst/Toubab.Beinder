using System;
using System.Collections.Generic;
using System.Linq;
using Toubab.Beinder.PathParser;
using System.Collections.Specialized;
using Toubab.Beinder.Valve;

namespace Toubab.Beinder.Scanner
{
    /// <summary>
    /// Scans a dictionary for properties
    /// </summary>
    /// <remarks>
    /// The keys are the property names, and the values are the property values.
    /// </remarks>
    public class DictionaryScanner : IScanner
    {
        IPathParser _pathParser = new CamelCasePathParser();

        public IPathParser PathParser
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
            readonly Path _propertyPath;
            Dictionary<string,object> _object;

            public DictionaryEntryProperty(string key, IPathParser pathParser)
            {
                _propertyPath = pathParser.Parse(key);
                _key = key;
            }

            DictionaryEntryProperty(string key, Path path)
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
                    OnBroadcast(Values);
                }
            }

            void OnBroadcast(object[] argument)
            {
                var e = Broadcast;
                if (e != null)
                    e(this, new BindableBroadcastEventArgs(this, argument));
            }

            public event EventHandler<BindableBroadcastEventArgs> Broadcast;

            public Type[] ValueType { get { return new[] { typeof(object) }; } }

            public object[] Values
            {
                get
                {
                    object result;
                    return _object.TryGetValue(_key, out result) ? new object[] { result } : new object[] { null };
                }
            }

            public bool TryHandleBroadcast(object[] argument)
            { 
                var value = argument[0];
                if (value == null)
                    _object.Remove(_key);
                else
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

            public Path Path
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

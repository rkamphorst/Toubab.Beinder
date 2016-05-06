namespace Toubab.Beinder.Scanners
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using Bindables;
    using Paths;
    using Mixins;

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
                return Enumerable.Empty<IProperty>();
            
            return dict.Select(
                kvp => new DictionaryEntryProperty(kvp.Key, _pathParser)
            );
        }

        class DictionaryEntryProperty : Bindable
        {
            readonly string _key;

            public DictionaryEntryProperty(string key, IPathParser pathParser)
                : base(pathParser.Parse(key))
            {
                _key = key;
            }

            DictionaryEntryProperty(DictionaryEntryProperty toCopy)
                : base(toCopy)
            {
                _key = toCopy._key;
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
                    // TODO: do something with broadcasts
                }
            }

            public override Type[] ValueTypes { get { return new[] { typeof(object) }; } }

            public object[] Values
            {
                get
                {
                    object result;
                    return ((Dictionary<string,object>)GetObject()).TryGetValue(_key, out result) 
                        ? new [] { result } 
                        : new object[] { null };
                }
            }

            public bool TryHandleBroadcast(object[] argument)
            { 
                var dict = (Dictionary<string,object>)GetObject();
                var value = argument[0];
                if (value == null)
                    dict.Remove(_key);
                else
                    dict[_key] = value;
                
                return true;
            }

            public override void SetObject(object value)
            {
                var newdict = (Dictionary<string,object>)value;

                {
                    var incc = GetObject() as INotifyCollectionChanged;
                    if (incc != null)
                    {
                        incc.CollectionChanged -= HandleDictionaryChanged;
                    }
                }

                base.SetObject(newdict);

                {
                    var incc = newdict as INotifyCollectionChanged;
                    if (incc != null)
                    {
                        incc.CollectionChanged += HandleDictionaryChanged;
                    }
                }

            }

            public override IMixin CloneWithoutObject()
            {
                return new DictionaryEntryProperty(this);
            }

        }
    }


    
}

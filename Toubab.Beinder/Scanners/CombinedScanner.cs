namespace Toubab.Beinder.Scanners
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Bindables;

    public class CombinedScanner : IScanner, IEnumerable<IScanner>
    {
        readonly LinkedList<IScanner> _scanners = new LinkedList<IScanner>();

        public void Add(IScanner scanner)
        {
            _scanners.AddFirst(scanner);
        }

        public IEnumerator<IScanner> GetEnumerator()
        {
            return _scanners.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<IBindable> Scan(object obj)
        {
            var propgroups =
                (from prop in _scanners.SelectMany(s => s.Scan(obj))
                             group prop by new 
                            { 
                                Path = prop.NameSyllables, 
                                IsProperty = (prop is IProperty),
                                IsEvent = (prop is IEvent),
                                IsEventHandler = (prop is IEventHandler)
                            });
            foreach (var propgroup in propgroups)
            {
                IBindable prop = null;
                if (propgroup.Key.IsProperty)
                {
                    var proparray = propgroup.Cast<IProperty>().ToArray();
                    prop = proparray.Length > 1 
                        ? new CombinedProperty(proparray) 
                        : proparray[0];
                }
                else if (propgroup.Key.IsEvent)
                {
                    var proparray = propgroup.Cast<IEvent>().ToArray();
                    prop = proparray.Length > 1 
                        ? new CombinedEvent(proparray) 
                        : proparray[0];
                }
                else if (propgroup.Key.IsEventHandler)
                {
                    var proparray = propgroup.Cast<IEventHandler>().ToArray();
                    prop = proparray.Length > 1 
                        ? new CombinedEventHandler(proparray) 
                        : proparray[0];
                }

                if (prop != null)
                    yield return prop;
            }
        }

    }
}


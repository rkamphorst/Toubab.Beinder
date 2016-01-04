using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Toubab.Beinder.Valve;

namespace Toubab.Beinder.PropertyScanners
{

    public class CombinedBindableScanner : IBindableScanner, IEnumerable<IBindableScanner>
    {
        readonly LinkedList<IBindableScanner> _scanners = new LinkedList<IBindableScanner>();

        public void Add(IBindableScanner scanner)
        {
            _scanners.AddFirst(scanner);
        }

        public IEnumerator<IBindableScanner> GetEnumerator()
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
                                Path = prop.Path, 
                                IsState = (prop is IBindableState),
                                IsProducer = (prop is IBindableBroadcastProducer),
                                IsConsumer = (prop is IBindableBroadcastConsumer)
                            });
            foreach (var propgroup in propgroups)
            {
                IBindable prop;
                if (propgroup.Key.IsState)
                {
                    var proparray = propgroup.Cast<IBindableState>().ToArray();
                    prop = proparray.Length > 1 
                        ? new AggregateBindableState(proparray) 
                        : proparray[0];
                    yield return prop;
                }
                else 
                {
                    prop = propgroup.First();
                }
                yield return prop;
            }
        }

    }
}


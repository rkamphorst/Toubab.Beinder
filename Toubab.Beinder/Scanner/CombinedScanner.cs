using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toubab.Beinder.Bindable;
using Toubab.Beinder.Extend;

namespace Toubab.Beinder.Scanner
{

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
                                Path = prop.Path, 
                                IsState = (prop is IBindableState),
                                IsProducer = (prop is IBindableProducer),
                                IsConsumer = (prop is IBindableConsumer)
                            });
            foreach (var propgroup in propgroups)
            {
                IBindable prop;
                if (propgroup.Key.IsState)
                {
                    var proparray = propgroup.Cast<IBindableState>().ToArray();
                    prop = proparray.Length > 1 
                        ? new CombinedState(proparray) 
                        : proparray[0];
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


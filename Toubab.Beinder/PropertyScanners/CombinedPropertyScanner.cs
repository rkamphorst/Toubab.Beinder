using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Toubab.Beinder.PropertyScanners
{

    public class CombinedPropertyScanner : IPropertyScanner, IEnumerable<IPropertyScanner>
    {
        readonly LinkedList<IPropertyScanner> _scanners = new LinkedList<IPropertyScanner>();

        public void Add(IPropertyScanner scanner)
        {
            _scanners.AddFirst(scanner);
        }

        public IEnumerator<IPropertyScanner> GetEnumerator()
        {
            return _scanners.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<IProperty> Scan(object obj)
        {
            var propgroups =
                (from prop in _scanners.SelectMany(s => s.Scan(obj))
                             group prop by prop.Path);
            foreach (var propgroup in propgroups)
            {
                var proparray = propgroup.ToArray();
                IProperty prop = proparray.Length > 1 
                    ? new AggregateProperty(proparray) 
                    : proparray[0];
                yield return prop;
            }
        }

    }
}


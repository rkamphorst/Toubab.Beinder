using System;
using System.Collections.Generic;
using System.Linq;

namespace Beinder.PropertyScanners
{
    public class AggregatePropertyScanner : IObjectPropertyScanner, ITypePropertyScanner
    {
        readonly List<IObjectPropertyScanner> _objectScanners = new List<IObjectPropertyScanner>();
        readonly List<ITypePropertyScanner> _typeScanners = new List<ITypePropertyScanner>();
        readonly Dictionary<Type, List<IProperty>> _typeCache = new Dictionary<Type, List<IProperty>>();

        public void AddScanner(IObjectPropertyScanner scanner)
        {
            _objectScanners.Add(scanner);
        }

        public void AddScanner(ITypePropertyScanner scanner)
        {
            if (_typeCache.Count > 0)
                _typeCache.Clear();
            
            _typeScanners.Add(scanner);
        }

        public IEnumerable<IProperty> Scan(object obj)
        {
            foreach (var propgroup in 
                    from prop in
                        Scan(obj.GetType())
                            .Concat(_objectScanners.SelectMany(s => s.Scan(obj)))
                    group prop by prop.Path)
            {
                var proparray = propgroup.ToArray();
                IProperty prop = proparray.Length > 1 
                    ? new AggregateProperty(proparray) 
                    : proparray[0];
                yield return prop;
            }
        }

        public IEnumerable<IProperty> Scan(Type type)
        {
            if (!_typeCache.ContainsKey(type))
            {
                _typeCache[type] =
                    _typeScanners.SelectMany(s => s.Scan(type)).ToList();
            }
            return _typeCache[type].Select(tp => tp.Clone());
        }

    }
}


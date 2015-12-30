using System.Collections.Generic;
using System.Linq;
using Toubab.Beinder.PropertyScanners;
using System;

namespace Toubab.Beinder
{

    public class Binder
    {
        readonly AggregatePropertyScanner _propertyScanner = new AggregatePropertyScanner();

        public AggregatePropertyScanner PropertyScanner { get { return _propertyScanner; } }

        public Valve[] Bind(IEnumerable<object> objects)
        {
            return Bind(objects.ToArray(), null, null);
        }

        Valve[] Bind(object[] objects, object activator, BinderState externalState)
        {
            var resultList = new List<Valve>();

            // entryList: list of Binder Entries, each holding an IProperty instance.
            // The list is sorted on Path, which is important because
            // the shortest property path will be on top, followed by the
            // property paths that start with that path, and so on.
            // Furthermore, all properties with the same paths will be next
            // to each other in the list.
            var state = BinderState.FromScan(PropertyScanner, objects);

            if (externalState != null)
                state.Merge(externalState);

            ValveParameters valveParams;
            while (state.PopValveParameters(out valveParams))
            {
                var newValve = new Valve();
                foreach (var entry in valveParams.Properties)
                {
                    var newProp = entry.Property.CloneWithoutObject();
                    newProp.SetObject(entry.Object);
                    newValve.AddProperty(newProp);
                }
                newValve.Activate(activator);

                BindChildValves(newValve, activator, valveParams.ExternalState);

                resultList.Add(newValve);
            }

            return resultList.ToArray();
        }

        void BindChildValves(Valve parentValve, object parentActivator, BinderState externalState)
        {
            var childValves = Bind(
                                  parentValve.GetValues(), 
                                  GetChildActivator(parentActivator, parentValve, externalState), 
                                  externalState
                              );
            parentValve.ValueChanged += (source, evt) =>
            {
                foreach (var cvalve in childValves)
                    cvalve.Dispose();
                childValves = Bind(parentValve.GetValues(), GetChildActivator(evt.Property.Object, parentValve, externalState), externalState);
            };
            parentValve.Disposing += delegate
            {
                foreach (var cvalve in childValves)
                    cvalve.Dispose();
            };
        }

        object GetChildActivator(object parentActivator, Valve parentValve, BinderState externalState)
        {
            return 
                externalState.ContainsPropertyForObject(parentActivator) 
                    ? parentActivator 
                    : parentValve.GetValueForObject(parentActivator);
        }

        struct CandidateProperty
        {
            public CandidateProperty(object obj, IProperty property)
                : this(obj, property, new PropertyPath(property.Path))
            {
            }

            CandidateProperty(object obj, IProperty property, PropertyPath path)
            {
                Object = obj;
                Property = property;
                RelativePath = path;
            }

            public IProperty Property { get; private set; }

            public PropertyPath RelativePath { get; private set; }

            public object Object { get; private set; }

            public CandidateProperty? RelativeTo(PropertyPath basePath)
            {
                var newPath = RelativePath.RelativeTo(basePath);
                return newPath != null ? (CandidateProperty?)new CandidateProperty(Object, Property, newPath) : null;
            }

            public override string ToString()
            {
                return string.Format("[BinderEntry: Path={0}, Object={1}]", RelativePath, Object);
            }
        }

        struct ValveParameters
        {

            public ValveParameters(IEnumerable<CandidateProperty> properties, BinderState relativeProperties)
            {
                Properties = properties;
                ExternalState = relativeProperties;
            }

            public IEnumerable<CandidateProperty> Properties { get; private set; }

            public BinderState ExternalState { get; private set; }
        }

        class BinderState
        {
            public static BinderState FromScan(IObjectPropertyScanner scanner, IEnumerable<object> objects)
            {
                return new BinderState(
                    objects
                    .SelectMany(o => scanner.Scan(o).Select(p => new CandidateProperty(o, p)))
                    .OrderBy(be => be.RelativePath)
                );
            }

            readonly LinkedList<CandidateProperty> _list;

            BinderState()
            {
                _list = new LinkedList<CandidateProperty>();
            }

            BinderState(IEnumerable<CandidateProperty> collection)
            {
                _list = new LinkedList<CandidateProperty>(collection);
            }

            public bool PopValveParameters(out ValveParameters result)
            {
                var firstPath = _list.Count == 0 ? null : _list.First.Value.RelativePath;

                var properties = new LinkedList<CandidateProperty>();
                var first = _list.First;
                while (first != null && first.Value.RelativePath.CompareTo(firstPath) == 0)
                {
                    _list.RemoveFirst();
                    properties.AddLast(first);
                    first = _list.First;
                }

                var relativeProperties = new BinderState();
                while (_list.Count > 0)
                {
                    var rebased = _list.First.Value.RelativeTo(firstPath);
                    if (!rebased.HasValue)
                        break;

                    relativeProperties._list.AddLast(rebased.Value);
                    _list.RemoveFirst();
                }

                if (_list.Count > 0 && properties.Count < 2 && relativeProperties._list.Count == 0)
                {
                    return PopValveParameters(out result);
                }
                result = new ValveParameters(properties, relativeProperties);
                return (properties.Count >= 2 || relativeProperties._list.Count > 0);
            }

            public void Merge(BinderState toMerge)
            {
                var cur = _list.First;
                foreach (var mergeEntry in toMerge._list)
                {
                    while (cur != null)
                    {
                        if (cur.Value.RelativePath.CompareTo(mergeEntry.RelativePath) > 0)
                        {
                            // cur comes *after* mergeEntry, so add newProp before cur
                            _list.AddBefore(cur, mergeEntry);
                            break;
                        }
                        cur = cur.Next;
                    }
                    if (cur == null)
                    {
                        // traversed the whole list without adding...
                        _list.AddLast(mergeEntry);
                    }
                }
            }

            public bool ContainsPropertyForObject(object o)
            {
                return _list.Any(p => ReferenceEquals(o, p.Object));
            }
        }

    }
}

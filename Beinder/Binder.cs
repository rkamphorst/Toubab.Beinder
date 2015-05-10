using System.Collections.Generic;
using System.Linq;
using Beinder.PropertyScanners;
using System;

namespace Beinder
{

    public class Binder
    {
        readonly AggregatePropertyScanner _propertyScanner = new AggregatePropertyScanner();

        public AggregatePropertyScanner PropertyScanner { get { return _propertyScanner; } }

        public IProperty[] Bind(IEnumerable<object> objects)
        {
            var resultList = new List<IProperty>();
            var propList
                = new LinkedList<Tuple<IProperty, Valve>>(
                      objects
                        .SelectMany(PropertyScanner.Scan)
                        .OrderBy(p => p.Path)
                        .Select(p => new Tuple<IProperty, Valve>(p, null))
                  );

            while (propList.Count > 0)
            {
                // determine what the first path is.
                var firstPath = propList.First.Value.Item1.Path;

                // all properties with the same path as first path will come in
                // firstGroup
                var firstGroup = new List<Tuple<IProperty, Valve>>();
                while (propList.Count > 0 && propList.First.Value.Item1.Path.CompareTo(firstPath) == 0)
                {
                    firstGroup.Add(propList.First.Value);
                    propList.RemoveFirst();
                }

                // at this point, firstGroup has a count of *at least* (>=) 1.

                // now, we try to create a valve.
                // a valve has to contain at least two properties,
                // and at least one of them has to be writable.
                Valve newValve = null;
                if (firstGroup.Count > 1 && firstGroup.Count(p => p.Item1.IsWritable) > 0)
                {
                    // if firstGroup has at least 2 members, we can bind them
                    // with a valve.
                    newValve = new Valve();
                    foreach (var prop in firstGroup)
                    {
                        var cand = prop.Item1 as CandidateChildProperty;
                        if (cand != null)
                        {
                            newValve.AddProperty(new ChildProperty((CandidateChildProperty)cand.Clone()));
                        }
                        else
                        {
                            newValve.AddProperty(prop.Item1.Clone());
                        }
                    }
                    resultList.Add(newValve);
                }


                if (
                    firstGroup.Count >= 2 ||
                    (propList.Count >= 1 && firstPath.MatchesStartOf(propList.First.Value.Item1.Path)))
                {
                    var newProps = new List<IProperty>();
                    foreach (var prop in firstGroup)
                    {
                        if (prop.Item1.Value != null)
                        {
                            newProps.AddRange(
                                PropertyScanner
                                    .Scan(prop.Item1.Value)
                                .Select(child => new CandidateChildProperty(prop.Item1.Clone(), child))
                            );
                        }
                        else if (prop.Item1.ValueType != null)
                        {
                            newProps.AddRange(
                                PropertyScanner
                                    .Scan(prop.Item1.ValueType)
                                .Select(child => new CandidateChildProperty(prop.Item1.Clone(), child))
                            );
                        }

                        // this should detach all events that the property attached to its object
                        prop.Item1.TrySetObject(null);
                    }
                    newProps.Sort((a, b) => a.Path.CompareTo(b.Path));
                    foreach (var newProp in newProps)
                    {
                        var cur = propList.First;
                        while (cur != null)
                        {
                            if (cur.Value.Item1.Path.CompareTo(newProp.Path) > 0)
                            {
                                // cur comes *after* newProp, so add newProp before cur
                                propList.AddBefore(cur, new Tuple<IProperty, Valve>(newProp, newValve));
                                break;
                            }
                            cur = cur.Next;
                        }
                        if (cur == null)
                        {
                            // traversed the whole list without adding...
                            propList.AddLast(new Tuple<IProperty, Valve>(newProp, newValve));
                        }
                    }
                }
            }

            return resultList.ToArray();
        }
    }
}

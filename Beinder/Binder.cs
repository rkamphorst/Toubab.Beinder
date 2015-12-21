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

            // propList: list of IProperty instances.
            // The list is sorted on Path, which is important because
            // the shortest property path will be on top, followed by the
            // property paths that start with that path, and so on.
            // Furthermore, all properties with the same paths will be next
            // to each other in the list.
            var propList
                = new LinkedList<IProperty>(
                      objects
                        .SelectMany(PropertyScanner.Scan)
                        .OrderBy(p => p.Path)
                  );

            while (propList.Count > 0)
            {
                // determine what the first path is.
                var firstPath = propList.First.Value.Path;

                // all properties with the same path as first path will come in
                // firstGroup
                var firstGroup = new List<IProperty>();
                while (propList.Count > 0 && propList.First.Value.Path.CompareTo(firstPath) == 0)
                {
                    firstGroup.Add(propList.First.Value);
                    propList.RemoveFirst();
                }

                // at this point, firstGroup has a count of *at least* (>=) 1.

                // now, we try to create a valve.
                // a valve has to contain at least two properties,
                // and at least one of them has to be writable.
                Valve newValve = null;
                if (firstGroup.Count >= 2 && firstGroup.Count(p => p.MetaInfo.IsWritable) > 0)
                {
                    // if firstGroup has at least 2 members, we can bind them
                    // with a valve.
                    newValve = new Valve();
                    foreach (var prop in firstGroup)
                    {
                        newValve.AddProperty(prop.Clone());
                    }
                    resultList.Add(newValve);
                }

                // Now, the properties from firstGroup are removed from propList, 
                // so they will not be processed again.
                // However, the properties from firstGroup might have child properties
                // (properties of properties) that we may want to bind to each other
                // or to other properties that are in propList.
                // That is the case if either 
                //
                //   (1) There are one or more properties in firstGroup. They 
                //       have the same property path, therefore if they have 
                //       properties (or child properties) that match, they may
                //       be bindable to each other.
                //   (2) There are other properties in propList of which the
                //       path starts with the path of the properties in firstGroup.
                //       If that is the case, these properties are at the start
                //       of propList.
                if (
                    firstGroup.Count >= 2 ||
                    (propList.Count >= 1 && firstPath.MatchesStartOf(propList.First.Value.Path)))
                {
                    var newProps = new List<IProperty>();
                    foreach (var prop in firstGroup)
                    {
                        if (prop.Value != null)
                        {
                            newProps.AddRange(
                                PropertyScanner
                                    .Scan(prop.Value)
                                .Select(child => new ChildProperty(prop.Clone(), child))
                            );
                        }
                        else if (prop.MetaInfo.ValueType != null)
                        {
                            newProps.AddRange(
                                PropertyScanner
                                    .Scan(prop.MetaInfo.ValueType)
                                .Select(child => new ChildProperty(prop.Clone(), child))
                            );
                        }

                        // this should detach all events that the property attached to its object
                        prop.TrySetObject(null);
                    }
                    // sort the new properties and merge them into propList, maintaining
                    // order. because propList is a linked list, this is a relatively
                    // fast operation.
                    newProps.Sort((a, b) => a.Path.CompareTo(b.Path));
                    foreach (var newProp in newProps)
                    {
                        var cur = propList.First;
                        while (cur != null)
                        {
                            if (cur.Value.Path.CompareTo(newProp.Path) > 0)
                            {
                                // cur comes *after* newProp, so add newProp before cur
                                propList.AddBefore(cur, newProp);
                                break;
                            }
                            cur = cur.Next;
                        }
                        if (cur == null)
                        {
                            // traversed the whole list without adding...
                            propList.AddLast(newProp);
                        }
                    }
                }
            }

            return resultList.ToArray();
        }
    }
}

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

        struct BinderEntry
        {
            public BinderEntry(object obj, IProperty property)
                : this(obj, property, new PropertyPath(property.Path))
            {
            }

            BinderEntry(object obj, IProperty property, PropertyPath path)
            {
                Object = obj;
                Property = property;
                Path = path;
            }

            public IProperty Property { get; private set; }

            public PropertyPath Path { get; private set; }

            public object Object { get; private set; }

            public BinderEntry? Rebase(PropertyPath onto)
            {
                var newPath = Path.Rebase(onto);
                return newPath != null ? (BinderEntry?)new BinderEntry(Object, Property, newPath) : null;
            }

            public override string ToString()
            {
                return string.Format("[BinderEntry: Path={0}, Object={1}]", Path, Object);
            }
        }



        public Valve[] Bind(IEnumerable<object> objects)
        {
            var obarray = objects.ToArray();
            return Bind(obarray, null, new BinderEntry[0]);
        }

        Valve[] Bind(object[] objects, object activator, BinderEntry[] parentEntries)
        {
            var resultList = new List<Valve>();

            // entryList: list of Binder Entries, each holding an IProperty instance.
            // The list is sorted on Path, which is important because
            // the shortest property path will be on top, followed by the
            // property paths that start with that path, and so on.
            // Furthermore, all properties with the same paths will be next
            // to each other in the list.
            var entryList
                = new LinkedList<BinderEntry>(
                      objects
                        .SelectMany(o => PropertyScanner.Scan(o).Select(p => new BinderEntry(o, p)))
                        .OrderBy(be => be.Path)
                  );

            MergeBinderEntries(entryList, parentEntries);

            while (entryList.Count > 0)
            {
                // determine what the first path is.
                var firstPath = entryList.First.Value.Path;

                // all properties with the same path as first path will come in
                // firstGroup
                var firstGroup = new List<BinderEntry>();
                while (entryList.Count > 0 && entryList.First.Value.Path.CompareTo(firstPath) == 0)
                {
                    firstGroup.Add(entryList.First.Value);
                    entryList.RemoveFirst();
                }

                // at this point, firstGroup has a count of *at least* (>=) 1.

                // now, we try to create a valve.
                // a valve has to contain at least two properties,
                // and at least one of them has to be writable.
                Valve newValve = null;
                if (firstGroup.Count >= 1)
                {
                    // if firstGroup has at least 2 members, we can bind them
                    // with a valve.
                    newValve = new Valve();
                    foreach (var entry in firstGroup)
                    {
                        var newProp = entry.Property.Clone();
                        newProp.TrySetObject(entry.Object);
                        newValve.AddProperty(newProp);
                    }
                    resultList.Add(newValve);

                    newValve.Activate(activator);

                    BindChildValves(newValve, activator, RebaseAndPopBinderEntries(entryList, firstPath));
                }
            }

            return resultList.ToArray();
        }

        void BindChildValves(Valve valve, object activator, BinderEntry[] rebasedEntries)
        {
            Func<object, object> getChildActivator = act => {
                BinderEntry? actEntry = null;
                foreach (var e in rebasedEntries) {
                    if (ReferenceEquals(e.Object, act)) {
                        actEntry = e;
                        break;
                    }
                }
                return actEntry.HasValue ? actEntry.Value.Object : valve.GetValueForObject(act);
            };

            var childValves = Bind(valve.GetValues(), getChildActivator(activator), rebasedEntries);
            valve.ValueChanged += (source, evt) =>
                {
                    foreach (var cvalve in childValves) cvalve.Dispose();
                    childValves = Bind(valve.GetValues(), getChildActivator(evt.Property.Object), rebasedEntries);
                };
            valve.Disposing += delegate
                {
                    foreach (var cvalve in childValves) cvalve.Dispose();
                };
        }

        static BinderEntry[] RebaseAndPopBinderEntries(LinkedList<BinderEntry> entries, PropertyPath rebaseOnPath)
        {
            var resultList = new List<BinderEntry>();
            while (entries.Count > 0)
            {
                var rebased = entries.First.Value.Rebase(rebaseOnPath);
                if (!rebased.HasValue)
                    break;

                resultList.Add(rebased.Value);
                entries.RemoveFirst();
            }
            var result = resultList.ToArray();
            return result;
        }

        static void MergeBinderEntries(LinkedList<BinderEntry> entries, IEnumerable<BinderEntry> toMerge) 
        {
            foreach (var mergeEntry in toMerge)
            {
                var cur = entries.First;
                while (cur != null)
                {
                    if (cur.Value.Path.CompareTo(mergeEntry.Path) > 0)
                    {
                        // cur comes *after* newProp, so add newProp before cur
                        entries.AddBefore(cur, mergeEntry);
                        break;
                    }
                    cur = cur.Next;
                }
                if (cur == null)
                {
                    // traversed the whole list without adding...
                    entries.AddLast(mergeEntry);
                }
            }
        }

    }
}

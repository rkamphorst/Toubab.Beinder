namespace Toubab.Beinder.Valves
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Linq;
    using System.Reflection;
    using Scanners;
    using Paths;
    using Bindables;

    public class Fixture
    {
        readonly IScanner _scanner;
        readonly ConditionalWeakTable<object,object> _scannedObjects;
        readonly LinkedList<Conduit> _conduits;
        readonly LinkedList<Conduit> _descendants;

        public static IEnumerable<Fixture> FromScan(IScanner scanner, object[] objects)
        {
            var f = new Fixture(scanner, new ConditionalWeakTable<object, object>(), null, null);
            return f.EnumerateFixtures(Merge(objects.SelectMany((o, i) => f.ScanObject(o, null, i)), new LinkedList<Conduit>()));
        }

        public LinkedList<Conduit> Conduits { get { return _conduits; } }

        public IEnumerable<Fixture> EnumerateChildren()
        {
            return EnumerateFixtures(
                Merge(Conduits.SelectMany(ScanConduit), new LinkedList<Conduit>(_descendants))
            );
        }

        Fixture(IScanner scanner, ConditionalWeakTable<object,object> scannedObjects, LinkedList<Conduit> conduits, LinkedList<Conduit> descendants)
        {
            _scannedObjects = scannedObjects;
            _scanner = scanner;
            _conduits = conduits;
            _descendants = descendants;
        }

        IEnumerable<Fixture> EnumerateFixtures(LinkedList<Conduit> conduits)
        {
            var result = new List<Fixture>();
            LinkedListNode<Conduit> first;
            while ((first = conduits.First) != null)
            {
                var members = new LinkedList<Conduit>();
                conduits.RemoveFirst();
                members.AddLast(first);

                /**
                 * "members" are all candidate bindables that have the exact
                 * same path as the first candidate
                 */
                while (conduits.First != null
                       && Equals(first.Value.AbsolutePath, conduits.First.Value.AbsolutePath))
                {
                    var item = conduits.First;
                    conduits.RemoveFirst();
                    members.AddLast(item);
                }

                bool hasDescendants =
                    conduits.First != null &&
                    first.Value.AbsolutePath.MatchesStartOf(conduits.First.Value.AbsolutePath);

                if (MembersQualifyForFixture(members))
                {
                    /**
                     * descendants" are all candidate bindables that have a path
                     * that starts with the same path as the first candidate
                     */
                    var descendants = new LinkedList<Conduit>();
                    while (
                        conduits.First != null &&
                        first.Value.AbsolutePath.MatchesStartOf(conduits.First.Value.AbsolutePath))
                    {
                        var item = conduits.First;
                        conduits.RemoveFirst();
                        descendants.AddLast(item);
                    }

                    // "members" contains bindables from more than one of
                    // the original objecs ("trees"): this warrants a valve.
                    yield return new Fixture(_scanner, _scannedObjects, _conduits, _descendants);
                }
                else if (hasDescendants)
                {
                    // otherwise, we recursively scan members for (child) bindables
                    // and put them into candidates.
                    Merge(members.SelectMany(ScanConduit), conduits);
                } 
                // else: members do not qualify for valve, no descendants: 
                // we can discard members! :-D
            }
        }

        List<Conduit> ScanConduit(Conduit toScan)
        {
            var prop = toScan.Bindable as IProperty;
            if (prop != null)
            {
                using (toScan.Attach())
                {
                    return prop.Values
                        .SelectMany(v => ScanObject(v, toScan.AbsolutePath, toScan.Tag))
                        .ToList();
                }
            }
            return new List<Conduit>();
        }

        List<Conduit> ScanObject(object toScan, Path basePath, int tag)
        {
            if (toScan != null)
            {
                bool wasNotYetScanned;
                CacheScannedObject(toScan, out wasNotYetScanned);
                if (wasNotYetScanned)
                {
                    return _scanner
                        .Scan(toScan)
                        .Select(b => Conduit.Create(b, toScan, basePath, tag))
                        .ToList();
                }
            }
            return new List<Conduit>();
        }

        void CacheScannedObject(object o, out bool wasNotYetScanned)
        {
            if (o == null)
            {
                // never scan null objects
                wasNotYetScanned = false;
            }
            if (o.GetType().GetTypeInfo().IsValueType || o is string)
            {
                // always scan value types and strings
                wasNotYetScanned = true;
            }
            else
            {
                // other reference types: only scan if not scanned already
                object isScanned;
                wasNotYetScanned = !_scannedObjects.TryGetValue(o, out isScanned);

                // about to scan, so add to the _scannedObjects table
                if (wasNotYetScanned)
                    _scannedObjects.Add(o, true);
            }
        }

        static LinkedList<Conduit> Merge(IEnumerable<Conduit> enumerableToMerge, LinkedList<Conduit> sortedListToMergeInto)
        {
            var sortedEnumerableToMerge = enumerableToMerge.OrderBy(c => c.AbsolutePath);

            if (sortedListToMergeInto.Count == 0)
            {
                foreach (var b in sortedEnumerableToMerge)
                    sortedListToMergeInto.AddLast(b);
            }
            else
            {
                var sortedListToMerge = new LinkedList<Conduit>(sortedEnumerableToMerge);
                LinkedListNode<Conduit> nodeToMerge, mergeBeforeNode;
                mergeBeforeNode = sortedListToMergeInto.First;
                while ((nodeToMerge = sortedListToMerge.First) != null)
                {
                    sortedListToMerge.RemoveFirst();

                    while (
                        mergeBeforeNode != null
                        && nodeToMerge.Value.AbsolutePath.CompareTo(mergeBeforeNode.Value.AbsolutePath) > 0)
                        mergeBeforeNode = mergeBeforeNode.Next;

                    if (mergeBeforeNode != null)
                        sortedListToMergeInto.AddBefore(mergeBeforeNode, nodeToMerge);
                    else
                        sortedListToMergeInto.AddLast(nodeToMerge);
                }
            }

            return sortedListToMergeInto;
        }

        static bool MembersQualifyForFixture(LinkedList<Conduit> members)
        {
            int distinctTrees =
                members.Select(p => p.Tag).Distinct().Count();
            return distinctTrees > 0;
        }
    }
}


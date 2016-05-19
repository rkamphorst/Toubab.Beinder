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
    using Tools;

    public class Fixture
    {
        readonly OnceScanner _scanner;
        readonly LinkedList<Conduit> _conduits;
        readonly LinkedList<Conduit> _descendants;

        Fixture(OnceScanner scanner, LinkedList<Conduit> conduits, LinkedList<Conduit> descendants)
        {
            _scanner = scanner;
            _conduits = conduits;
            _descendants = descendants;
        }

        public LinkedList<Conduit> Conduits { get { return _conduits; } }

        public List<Fixture> CreateChildFixtures()
        {
            var childScanner = _scanner.NewScope();
            var childConduits = 
                Conduits
                    .SelectMany(c => childScanner.ScanChildConduits(c))
                    .MergeIntoSortedLinkedList(new LinkedList<Conduit>(_descendants), c => c.AbsolutePath);
            return CreateFixtures(childScanner, childConduits);
        }

        public static List<Fixture> CreateAncestorFixtures(IScanner scanner, object[] objects)
        {
            var onceScanner = OnceScanner.Decorate(scanner);
            var scannedConduits = 
                objects
                    .SelectMany((o, i) => onceScanner.ScanObjectToConduits(o, null, i))
                    .MergeIntoSortedLinkedList(new LinkedList<Conduit>(), c => c.AbsolutePath);
            return CreateFixtures(onceScanner, scannedConduits);
        }

        public static List<Fixture> CreateFixtures(OnceScanner scopedScanner, LinkedList<Conduit> conduits)
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

                if (ConduitsQualifyForFixture(members))
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
                    result.Add(new Fixture(scopedScanner, members, descendants));
                }
                else if (hasDescendants)
                {
                    // otherwise, we recursively scan members for (child) bindables
                    // and put them into candidates.
                    members
                        .SelectMany(m => scopedScanner.ScanChildConduits(m))
                        .MergeIntoSortedLinkedList(conduits, c => c.AbsolutePath);
                } 
                // else: members do not qualify for valve, no descendants: 
                // we can discard members! :-D
            }
            return result;
        }

        public static bool ConduitsQualifyForFixture(IEnumerable<Conduit> members)
        {
            int distinctTrees =
                members.Select(p => p.Tag).Distinct().Count();
            return distinctTrees > 0;
        }
    }
}


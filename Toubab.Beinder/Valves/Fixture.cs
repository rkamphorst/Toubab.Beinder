namespace Toubab.Beinder.Valves
{
    using System.Collections.Generic;
    using System.Linq;
    using Scanners;
    using Tools;

    public class Fixture
    {
        readonly OnceScanner _scanner;
        readonly LinkedList<Conduit> _conduits;
        readonly LinkedList<Conduit> _descendantConduits;

        List<Fixture> _childFixtures;

        Fixture(OnceScanner scanner, LinkedList<Conduit> conduits, LinkedList<Conduit> descendantConduits)
        {
            _scanner = scanner;
            _conduits = conduits;
            _descendantConduits = descendantConduits;
            _childFixtures = null;
        }
            

        public LinkedList<Conduit> Conduits { get { return _conduits; } }

        public List<Fixture> ChildFixtures { get { return _childFixtures; } }

        public void UpdateChildFixtures()
        {
            var childScanner = _scanner.NewScope();
            var childConduits = 
                Conduits
                    .SelectMany(c => childScanner.ScanForChildrenOnConduit(c))
                    .MergeIntoSortedLinkedList(new LinkedList<Conduit>(_descendantConduits), c => c.AbsolutePath);

            _childFixtures = CreateFixtures(childScanner, childConduits);
        }

        public static List<Fixture> CreateFixtures(IScanner scanner, object[] objects)
        {
            const int generation = 0; // we create a first generation here
            Paths.Path path = null; // path of first generation is empty
            var onceScanner = OnceScanner.Decorate(scanner);
            var scannedConduits = 
                objects
                    .SelectMany((ancestor, family) => onceScanner.ScanObjectAndCreateConduits(ancestor, path, family))
                    .MergeIntoSortedLinkedList(new LinkedList<Conduit>(), c => c.AbsolutePath);
            return CreateFixtures(onceScanner, scannedConduits);
        }

        public static List<Fixture> CreateFixtures(OnceScanner scopedScanner, LinkedList<Conduit> conduits)
        {
            var result = new List<Fixture>();
            LinkedListNode<Conduit> first;
            while ((first = conduits.First) != null)
            {
                LinkedList<Conduit> members = 
                    conduits.Shift(c => Equals(first.Value.AbsolutePath, c.AbsolutePath));

                bool hasDescendants =
                    conduits.First != null &&
                    conduits.First.Value.AbsolutePath.StartsWith(first.Value.AbsolutePath);

                if (ConduitsQualifyForFixture(members))
                {
                    LinkedList<Conduit> descendants = 
                        conduits.Shift(c => c.AbsolutePath.StartsWith(first.Value.AbsolutePath));

                    var fixture = new Fixture(scopedScanner, members, descendants);
                    fixture.UpdateChildFixtures();
                    result.Add(fixture);
                }
                else if (hasDescendants)
                {
                    // otherwise, we recursively scan members for (child) bindables
                    // and put them into candidates.
                    members
                        .SelectMany(m => scopedScanner.ScanForChildrenOnConduit(m))
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
                members.Select(p => p.Family).Distinct().Count();
            return distinctTrees > 0;
        }
            
    }
}


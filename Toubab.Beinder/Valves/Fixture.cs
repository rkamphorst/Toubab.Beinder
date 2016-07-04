namespace Toubab.Beinder.Valves
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Scanners;
    using Tools;
    using Bindables;
    using Paths;


    /// <summary>
    /// Groups <see cref="Conduit"/>s with corresponding <see cref="Path"/>s.
    /// </summary>
    public class Fixture
    {
        readonly IScopedScanner _scanner;
        readonly LinkedList<Conduit> _conduits;
        readonly LinkedList<Conduit> _descendantConduits;

        List<Fixture> _childFixtures;

        Fixture(IScopedScanner scanner, LinkedList<Conduit> conduits, LinkedList<Conduit> descendantConduits)
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

        public static List<Fixture> CreateFixtures(IScopedScanner scanner, object[] objects)
        {
            Path path = null; // path of first generation is empty
            var scannedConduits = 
                objects
                    .SelectMany((ancestor, family) => scanner.ScanObjectAndCreateConduits(ancestor, path, family, 0))
                    .MergeIntoSortedLinkedList(new LinkedList<Conduit>(), c => c.AbsolutePath);
            return CreateFixtures(scanner, scannedConduits);
        }

        public static List<Fixture> CreateFixtures(IScopedScanner scopedScanner, LinkedList<Conduit> conduits)
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
            // if the collection of members is empty, they do not qualify
            var memArray = members as Conduit[] ?? members.ToArray();
            var firstMember = memArray.FirstOrDefault();
            if (firstMember == null)
                return false;

            // now, there should be at least two families involved
            bool distinctFamilies =
                memArray.Skip(1).Any(m => m.Family != firstMember.Family);
            if (!distinctFamilies)
                return false;

            // if one of the conduits represents a bindable that has state (i.e., is a property),
            // the members qualify ('cause we might need child fixtures)
            bool hasState =
                memArray.Any(m => m.Bindable.CanRead());
            if (hasState)
                return true;

            // a fixture should consist of at least one conduit that can broadcast / can be read,
            // and a different conduit (from a different family) that can receive the broadcast / read value.
            bool hasSenderAndReceiver =
                memArray
                    .Where(m => m.Bindable.CanBroadcastOrRead())
                    .SelectMany(
                        m1 => memArray.Where(
                            m2 => m1.Family != m2.Family && m2.Bindable.CanHandleBroadcast()
                            ))
                    .Any();
            return hasSenderAndReceiver;
        }

        public override string ToString()
        {
            
            return string.Format("{0}@{{{1}}}", _conduits.First().AbsolutePath, string.Join(",", _conduits.Select(c => c.Family)));
        }

        public LinkedList<LinkedList<Conduit>> GetLines(Path parentPath) 
        {
            var line = new LinkedList<Conduit>(Conduits
                .OrderBy(c => c.Family)
                .ThenBy(c => c.Generation)
                .ThenBy(c => c.Bindable.NameSyllables)
            );
            throw new NotImplementedException();       
        }

    }
}


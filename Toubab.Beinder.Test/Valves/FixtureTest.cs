namespace Toubab.Beinder.Valves
{
    using NUnit.Framework;
    using System.Collections.Generic;
    using Mocks;
    using Valves;
    using Paths;
    using Mocks.Abcd;
    using Scanners;
    using System;
    using Bindables;

    [TestFixture]
    public class FixtureTest
    {
        IScopedScanner _scanner = new MyScanner(new CombinedScanner { new NotifyPropertyChangedScanner(), new ReflectionScanner() });

        [Test, TestCaseSource("CreateFixtureTestCases")]
        public void CreateFixture(object ob1, object ob2, int expectNumberOfFixtures)
        {

            var fixtures = Fixture.CreateFixtures(_scanner, new object[] { ob1, ob2 });

            Assert.AreEqual(expectNumberOfFixtures, fixtures.Count);
            AssertFixturesAreSane(fixtures);
        }

        public IEnumerable<TestCaseData> CreateFixtureTestCases()
        {
            yield return new TestCaseData(new MockViewModel(), new MockViewModel2(), 0);
            yield return new TestCaseData(new MockViewModel(), new MockView(), 2);
        }

        void AssertFixturesAreSane(IEnumerable<Fixture> fixtures)
        {
            foreach (var f in fixtures)
            {
                Assert.Greater(f.Conduits.Count, 1);
            }
            foreach (var f in fixtures)
            {
                if (f.ChildFixtures != null)
                    AssertFixturesAreSane(f.ChildFixtures);
            }
        }

        class MyScanner : IScopedScanner
        {
            readonly IScanner _scanner;

            public MyScanner(IScanner scanner)
            {
                _scanner = scanner;
            }

            public IScopedScanner NewScope()
            {
                return this;
            }

            public IEnumerable<IBindable> Scan(object obj)
            {
                return _scanner.Scan(obj);
            }
        }
    }
}

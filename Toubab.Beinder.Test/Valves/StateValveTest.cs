namespace Toubab.Beinder.Valves
{
    using System.Linq;
    using NUnit.Framework;
    using Mocks;
    using Bindables;

    [TestFixture]
    public class StateValveTest
    {
        [Test]
        public void ValuesArePropagated()
        {
            // Arrange
            var propa = CreateConduit("a");
            var propb = CreateConduit("b");
            var propc = CreateConduit("c");
            var v = new StateValve();
            v.Add(propa);
            v.Add(propb);
            v.Add(propc);

            // Act
            ((IProperty)propa.Bindable).TryHandleBroadcast(new object [] { "banaan" });

            // Assert
            Assert.AreEqual(new object[] { "banaan" }, ((IProperty)propb.Bindable).Values);
            Assert.AreEqual(new object[] { "banaan" }, ((IProperty)propc.Bindable).Values);
        }

        [Test]
        public void UnchangedValueIsNotChanged()
        {
            // Arrange
            var propa = CreateConduit("a");
            var propb = CreateConduit("b");
            var propc = CreateConduit("c");
            var v = new StateValve();
            v.Add(propa);
            v.Add(propb);
            v.Add(propc);

            // Act
            ((IProperty)propa.Bindable).TryHandleBroadcast(new object[] { "banaan" });
            ((IProperty)propb.Bindable).TryHandleBroadcast(new object[] { "banaan" });

            // Assert
            Assert.AreEqual(1, ((MockProperty)propa.Bindable).Changed);
            Assert.AreEqual(2, ((MockProperty)propb.Bindable).Changed); // because we changed it manually
            Assert.AreEqual(1, ((MockProperty)propc.Bindable).Changed);
        }

        [Test, Ignore("This does not happen this way. TODO: write a better test.")]
        public void PropertiesAreGarbageCollectedFromValve()
        {
            // Arrange
            var propa = CreateConduit("a");
            var propb = CreateConduit("b");
            var propc = CreateConduit("c");
            var v = new StateValve();
            v.Add(propa);
            v.Add(propb);
            v.Add(propc);

            // Act
            propa = null;

            // allocate lots of arrays to trigger the GC
            object[] array;
            for (int i = 0; i < 100000; i++)
            {
                array = new object[1000];
                for (int j = 0; j < 10; j++)
                {
                    array[j] = new object();
                }
            }

            // Assert
            Assert.IsNull(propa);
            // propa should be garbage collected
            Assert.AreEqual(2, v.Count());
        }

        static Conduit CreateConduit(string name)
        {
            return Conduit.Create(new MockProperty { Changed = 0, Name = name }, new object());
        }


    }
}


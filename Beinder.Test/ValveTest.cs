using NUnit.Framework;
using System;
using Beinder.Mocks;

namespace Beinder
{
    [TestFixture()]
    public class ValveTest
    {
        [Test()]
        public void ValuesArePropagated()
        {
            // Arrange
            var propa = CreateProperty("a");
            var propb = CreateProperty("b");
            var propc = CreateProperty("c");
            Valve v = new Valve();
            v.AddProperty(propa);
            v.AddProperty(propb);
            v.AddProperty(propc);

            // Act
            propa.TrySetValue("banaan");

            // Assert
            Assert.AreEqual("banaan", propb.Value);
            Assert.AreEqual("banaan", propc.Value);
        }

        [Test()]
        public void UnchangedValueIsNotChanged()
        {
            // Arrange
            var propa = CreateProperty("a");
            var propb = CreateProperty("b");
            var propc = CreateProperty("c");
            Valve v = new Valve();
            v.AddProperty(propa);
            v.AddProperty(propb);
            v.AddProperty(propc);

            // Act
            propa.TrySetValue("banaan");
            propb.TrySetValue("banaan");

            // Assert
            Assert.AreEqual(1, propa.Changed);
            Assert.AreEqual(2, propb.Changed); // because we changed it manually
            Assert.AreEqual(1, propc.Changed);
        }

        [Test()]
        public void PropertiesAreGarbageCollectedFromValve()
        {
            // Arrange
            var propa = CreateProperty("a");
            var propb = CreateProperty("b");
            var propc = CreateProperty("c");

            var v = new Valve();
            v.AddProperty(propa);
            v.AddProperty(propb);
            v.AddProperty(propc);

            // Precondition check
            Assert.AreEqual(3, v.LiveProperties.Count);

            // Act
            propa = null;

            // allocate lots of arrays to trigger the GC
            object[] array;
            for (int i = 0; i < 10000; i++)
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
            Assert.AreEqual(2, v.LiveProperties.Count);
        }

        static MockProperty CreateProperty(string name)
        {
            return new MockProperty() { Changed = 0, Name = name };
        }


    }
}


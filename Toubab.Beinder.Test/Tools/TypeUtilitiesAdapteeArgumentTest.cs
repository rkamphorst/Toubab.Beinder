namespace Toubab.Beinder.Tools
{
    using System;
    using System.Linq;
    using NUnit.Framework;
    using Mocks.Fruits;

    [TestFixture]
    public class TypeUtilitiesAdapteeArgumentTest
    {

        [Test]
        public void ApplePearExample()
        {
            var adapteeTypes = typeof(DisguisePearAsApple).EnumerateGenericAdapteeArguments<IApple>();

            Assert.AreEqual(typeof(Pear), adapteeTypes.Single());
        }

        [Test]
        public void ApplePearExampleWithPearInterface()
        {
            var adapteeTypes = typeof(DisguisePearInterfaceAsApple).EnumerateGenericAdapteeArguments<IApple>();

            Assert.AreEqual(typeof(IPear), adapteeTypes.Single());
        }

        [Test]
        public void InterfaceWithDifferentNameDoesntCarryAdapteeParameter()
        {
            var adapteeTypes = typeof(DisguisePearAsBadApple).EnumerateGenericAdapteeArguments<IApple>();

            Assert.IsEmpty(adapteeTypes);
        }

        [Test]
        public void MultipleGenericArgumentPossibilities()
        {
            var adapteeTypes = typeof(DisguisePearAndOrangeAsApple).EnumerateGenericAdapteeArguments<IApple>().ToArray();
            Assert.AreEqual(2, adapteeTypes.Count());
            Assert.IsTrue(adapteeTypes.Contains(typeof(Pear)));
            Assert.IsTrue(adapteeTypes.Contains(typeof(Orange)));
        }

    }
}


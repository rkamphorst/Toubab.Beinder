using System;
using NUnit.Framework;
using Beinder.Mocks;
using System.Linq;
using Beinder.Mocks.Fruits;

namespace Beinder
{
    [TestFixture]
    public class TypeAdapterRegistryTest
    {
        [Test]
        public void SimpleRegisterAndFind()
        {
            // arrange
            var registry = new TypeAdapterRegistry<IApple>();

            // act
            registry.Register<DisguisePearAsApple>();
            var adapter = registry.FindAdapterTypesFor<Pear>().Single();

            // assert 
            Assert.AreEqual(typeof(DisguisePearAsApple), adapter);
        }

        [Test]
        public void RegisterAndFindSpecializedAdaptee1()
        {
            // arrange
            var registry = new TypeAdapterRegistry<IApple>();

            // act
            registry.Register<DisguisePearAsApple>();
            registry.Register<DisguiseSpecialPearAsApple>();
            var pearAdapter = registry.FindAdapterTypesFor<Pear>().Single();
            var specialPearAdapter = registry.FindAdapterTypesFor<SpecialPear>().First();

            // assert 
            Assert.AreEqual(typeof(DisguisePearAsApple), pearAdapter);
            Assert.AreEqual(typeof(DisguiseSpecialPearAsApple), specialPearAdapter);
        }

        [Test]
        public void RegisterAndFindSpecializedAdaptee2()
        {
            // arrange
            var registry = new TypeAdapterRegistry<IApple>();

            // act
            registry.Register<DisguisePearAsApple>();
            var pearAdapter = registry.FindAdapterTypesFor<Pear>().Single();
            var specialPearAdapter = registry.FindAdapterTypesFor<SpecialPear>().Single();

            // assert 
            Assert.AreEqual(typeof(DisguisePearAsApple), pearAdapter);
            Assert.AreEqual(typeof(DisguisePearAsApple), specialPearAdapter);
        }

        [Test]
        public void RegisterAndFindMultiPurposeAdapter()
        {
            // arrange
            var registry = new TypeAdapterRegistry<IApple>();

            // act
            registry.Register<DisguisePearOrOrangeAsApple>();
            var pearAdapter = registry.FindAdapterTypesFor<Pear>().Single();
            var orangeAdapter = registry.FindAdapterTypesFor<Orange>().Single();

            // assert 
            Assert.AreEqual(typeof(DisguisePearOrOrangeAsApple), pearAdapter);
            Assert.AreEqual(typeof(DisguisePearOrOrangeAsApple), orangeAdapter);
        }

        [Test]
        public void FindReturnsEveryTypeOnlyOnce1()
        {
            // arrange
            var registry = new TypeAdapterRegistry<IApple>();

            // act
            registry.Register<DisguiseSpecialPearAndPearAsApple>();
            var pearAdapters = registry.FindAdapterTypesFor<Pear>();

            // assert 
            Assert.AreEqual(typeof(DisguiseSpecialPearAndPearAsApple), pearAdapters.Single());
        }

        [Test]
        public void FindReturnsEveryTypeOnlyOnce2()
        {
            // arrange
            var registry = new TypeAdapterRegistry<IApple>();

            // act
            registry.Register<DisguiseSpecialPearAndPearAsApple>();
            var pearAdapters = registry.FindAdapterTypesFor<SpecialPear>();

            // assert 
            Assert.AreEqual(typeof(DisguiseSpecialPearAndPearAsApple), pearAdapters.Single());
        }

        /// <summary>
        /// Test the order in which types are found and returned
        /// </summary>
        /// <remarks>
        /// This is important because with this behavior, it is possible to override
        /// previous registrations with later ones.
        /// 
        /// The list should be ordered as follows:
        /// 
        /// 1. More specialized adapters first (i.e., their adaptee type argument 
        ///    is more specialized)
        /// 2. If two adapters are equally specialyzed, first comes the one that
        ///    was added last.
        /// </remarks>
        [Test]
        public void FindReturnsInReverseRegistrationOrder1() 
        {
            // arrange
            var registry = new TypeAdapterRegistry<IApple>();

            // act
            registry.Register<DisguisePearAsApple>();
            registry.Register<DisguiseSpecialPearAsApple>();
            registry.Register<DisguiseSpecialPearAndPearAsApple>();

            var pearAdapters = registry.FindAdapterTypesFor<Pear>().ToArray();
            var specialPearAdapters = registry.FindAdapterTypesFor<SpecialPear>().ToArray();

            // assert 
            Assert.AreEqual(2, pearAdapters.Length);
            Assert.AreEqual(typeof(DisguiseSpecialPearAndPearAsApple), pearAdapters[0]);
            Assert.AreEqual(typeof(DisguisePearAsApple), pearAdapters[1]);

            Assert.AreEqual(3, specialPearAdapters.Length);
            Assert.AreEqual(typeof(DisguiseSpecialPearAndPearAsApple), specialPearAdapters[0]);
            Assert.AreEqual(typeof(DisguiseSpecialPearAsApple), specialPearAdapters[1]);
            Assert.AreEqual(typeof(DisguisePearAsApple), specialPearAdapters[2]);
        }

        [Test]
        public void FindReturnsInReverseRegistrationOrder2() 
        {
            // arrange
            var registry = new TypeAdapterRegistry<IApple>();

            // act
            registry.Register<DisguiseSpecialPearAndPearAsApple>();
            registry.Register<DisguiseSpecialPearAsApple>();
            registry.Register<DisguisePearAsApple>();

            var pearAdapters = registry.FindAdapterTypesFor<Pear>().ToArray();
            var specialPearAdapters = registry.FindAdapterTypesFor<SpecialPear>().ToArray();

            // assert 
            Assert.AreEqual(2, pearAdapters.Length);
            Assert.AreEqual(typeof(DisguisePearAsApple), pearAdapters[0]);
            Assert.AreEqual(typeof(DisguiseSpecialPearAndPearAsApple), pearAdapters[1]);

            Assert.AreEqual(3, specialPearAdapters.Length);
            Assert.AreEqual(typeof(DisguiseSpecialPearAsApple), specialPearAdapters[0]);
            Assert.AreEqual(typeof(DisguiseSpecialPearAndPearAsApple), specialPearAdapters[1]);
            Assert.AreEqual(typeof(DisguisePearAsApple), specialPearAdapters[2]);
        }

        [Test]
        public void ExpectArgumentExceptionIfAdapteeCouldNotBeResolved()
        {
            // arrange
            var registry = new TypeAdapterRegistry<IApple>();

            try
            {
                // act
                registry.Register<DisguisePearAsBadApple>();

                // assert
                Assert.Fail("Expecting ArgumentException");
            }
            catch (ArgumentException)
            {
                // success :-)
            }
        }

        [Test]
        public void ExpectArgumentExceptionIfAdapterWrongType()
        {
            // arrange
            var registry = new TypeAdapterRegistry<IPear>();

            try
            {
                // act
                registry.Register(typeof(DisguisePearAsApple));

                // assert
                Assert.Fail("Expecting ArgumentException");
            }
            catch (ArgumentException)
            {
                // success :-)
            }
        }

    }
}


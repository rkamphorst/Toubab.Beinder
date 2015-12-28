using NUnit.Framework;
using Beinder.PropertyScanners;
using Beinder.Mocks;

namespace Beinder
{
    [TestFixture]
    public class PropertyTest
    {
        static void CloneYieldsIndependentPropertiesWithSameObject(IProperty property, object o)
        {
            // Arrange
            property.TrySetObject(o);

            // Act
            IProperty clone = property.Clone();

            // Assert
            Assert.AreNotSame(property, clone);
            Assert.AreSame(o, property.Object);
            Assert.AreSame(o, clone.Object);
        }

        static void ClonedPropertyCanChangeObjectIndependently(IProperty property, object o1, object o2)
        {
            // Arrange
            property.TrySetObject(o1);

            // Act
            IProperty clone = property.Clone();
            clone.TrySetObject(o2);

            // Assert
            Assert.AreSame(o1, property.Object);
            Assert.AreSame(o2, clone.Object);
        }

        [Test]
        public void NotifyPropertyChangedPropertyClone()
        {
            int cnt = 0;
            var props = new NotifyPropertyChangedPropertyScanner().Scan(typeof(NotifyPropertyChangedClass));
            foreach (var prop in props)
            {
                object o1 = new NotifyPropertyChangedClass();
                object o2 = new NotifyPropertyChangedClass();
                CloneYieldsIndependentPropertiesWithSameObject(prop, o1);
                ClonedPropertyCanChangeObjectIndependently(prop, o1, o2);
                ClonedPropertyCanChangeObjectIndependently(prop, o2, null);
                cnt++;
            }
            Assert.IsTrue(cnt > 0);
        }

        [Test]
        public void ReflectionPropertyClone()
        {
            int cnt = 0;
            var props = new ReflectionPropertyScanner().Scan(typeof(MockView));
            foreach (var prop in props)
            {
                object o1 = new MockView();
                object o2 = new MockView();
                CloneYieldsIndependentPropertiesWithSameObject(prop, o1);
                ClonedPropertyCanChangeObjectIndependently(prop, o1, o2);
                ClonedPropertyCanChangeObjectIndependently(prop, o2, null);
                cnt++;
            }
            Assert.IsTrue(cnt > 0);
        }

        [Test]
        public void AggregatePropertyClone()
        {
            int cnt = 0;
            var scanner = new AggregatePropertyScanner();
            scanner.AddScanner(new ReflectionPropertyScanner());
            scanner.AddScanner(new NotifyPropertyChangedPropertyScanner());
            var props = scanner.Scan(typeof(MockView));
            foreach (var prop in props)
            {
                object o1 = new MockView();
                object o2 = new MockView();
                CloneYieldsIndependentPropertiesWithSameObject(prop, o1);
                ClonedPropertyCanChangeObjectIndependently(prop, o1, o2);
                ClonedPropertyCanChangeObjectIndependently(prop, o2, null);
                cnt++;
            } 
            Assert.IsTrue(cnt > 0);
        }

    }
}


using NUnit.Framework;
using Toubab.Beinder.Scanner;
using Toubab.Beinder.Mock;
using Toubab.Beinder.Valve;

namespace Toubab.Beinder
{
    [TestFixture]
    public class PropertyTest
    {
        static void CloneYieldsIndependentPropertiesWithSameObject(IBindable property, object o)
        {
            // Arrange
            property.SetObject(o);

            // Act
            var clone = (IBindable) property.CloneWithoutObject();

            // Assert
            Assert.AreNotSame(property, clone);
            Assert.AreSame(o, property.Object);
            Assert.IsNull(clone.Object);
        }

        static void ClonedPropertyCanChangeObjectIndependently(IBindable property, object o1, object o2)
        {
            // Arrange
            property.SetObject(o1);

            // Act
            IBindableState clone = (IBindableState)property.CloneWithoutObject();
            clone.SetObject(o2);

            // Assert
            Assert.AreSame(o1, property.Object);
            Assert.AreSame(o2, clone.Object);
        }

        [Test]
        public void NotifyPropertyChangedPropertyClone()
        {
            int cnt = 0;
            var props = new NotifyPropertyChangedScanner().Scan(typeof(NotifyPropertyChangedClass));
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
            var props = new ReflectionScanner().Scan(typeof(MockView));
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
            var scanner = new CombinedScanner();
            scanner.Add(new ReflectionScanner());
            scanner.Add(new NotifyPropertyChangedScanner());
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


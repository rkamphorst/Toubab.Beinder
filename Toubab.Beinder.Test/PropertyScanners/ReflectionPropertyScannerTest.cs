using System;
using NUnit.Framework;
using Toubab.Beinder.Mocks;
using System.Linq;

namespace Toubab.Beinder.PropertyScanners
{

    [TestFixture]
    public class ReflectionPropertyScannerTest
    {
        [Test]
        public void ScanProperties() {
            // Arrange
            var scanner = new ReflectionPropertyScanner();

            // Act
            var result = scanner.Scan(typeof(ClassWithPropertyAndEvents)).ToArray();

            // Assert
            Assert.AreEqual(2, result.Length);
            Assert.IsTrue(result.Any(p => p.Path.Equals((PropertyPath) "property")));
            Assert.IsTrue(result.Any(p => p.Path.Equals((PropertyPath) new[] { "second", "property" })));
        }

        [Test]
        public void ValueIsSetAndEventIsRaised() 
        {
            // Arrange
            var scanner = new ReflectionPropertyScanner();
            object newValue = null;
            var property = scanner.Scan(typeof(ClassWithPropertyAndEvents))
                .FirstOrDefault(p => Equals(p.Path, (PropertyPath) "property"));
            var ob = new ClassWithPropertyAndEvents();
            ob.Property = "banaan";
            property.SetObject(ob);
            property.Broadcast += (sender, e) => newValue = e.Argument;

            // Act
            property.TryHandleBroadcast("fdsa");

            // Assert
            Assert.AreEqual("fdsa", ob.Property);
            Assert.AreEqual("fdsa", newValue);
        }
    }
}

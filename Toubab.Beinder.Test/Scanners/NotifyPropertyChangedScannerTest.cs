﻿namespace Toubab.Beinder.Scanners
{
    using System.Linq;
    using NUnit.Framework;
    using Bindables;
    using Mocks;
    using Paths;

    [TestFixture]
    public class NotifyPropertyChangedScannerTest
    {
        [Test]
        public void ScanProperties() {
            // Arrange
            var scanner = new NotifyPropertyChangedScanner();

            // Act
            var result = scanner.Scan(typeof(NotifyPropertyChangedClass)).ToArray();

            // Assert
            Assert.AreEqual(2, result.Length);
            Assert.IsTrue(result.Any(p => p.Name.Equals(new Fragment("property"))));
            Assert.IsTrue(result.Any(p => p.Name.Equals(new Fragment("second", "property"))));
        }

        [Test]
        public void ValueIsSetAndEventIsNotRaisedWhenHandlingBroadcast() 
        {
            // Arrange
            var scanner = new NotifyPropertyChangedScanner();
            object newValue = null;
            var property = (IProperty) scanner.Scan(typeof(NotifyPropertyChangedClass))
                .FirstOrDefault(p => Equals(p.Name, new Fragment("property")));
            var ob = new NotifyPropertyChangedClass();
            ob.Property = "banaan";
            property.SetObject(ob);
            property.SetBroadcastListener(payload => newValue = payload);

            // Act
            property.TryHandleBroadcastAsync(new object[] { "asdf" });

            // Assert
            Assert.AreEqual("asdf", ob.Property);
            Assert.AreEqual(null, newValue);
        }
    }

}


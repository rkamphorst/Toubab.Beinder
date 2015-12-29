﻿using System;
using NUnit.Framework;
using Beinder.Mocks;
using System.Linq;

namespace Beinder.PropertyScanners
{
    [TestFixture]
    public class NotifyPropertyChangedPropertyScannerTest
    {
        [Test]
        public void ScanProperties() {
            // Arrange
            var scanner = new NotifyPropertyChangedPropertyScanner();

            // Act
            var result = scanner.Scan(typeof(NotifyPropertyChangedClass)).ToArray();

            // Assert
            Assert.AreEqual(2, result.Length);
            Assert.IsTrue(result.Any(p => p.Path.Equals((PropertyPath) "property")));
            Assert.IsTrue(result.Any(p => p.Path.Equals((PropertyPath) new[] { "second", "property" })));
        }

        [Test]
        public void ValueIsSetAndEventIsRaised() 
        {
            // Arrange
            var scanner = new NotifyPropertyChangedPropertyScanner();
            object newValue = null;
            var property = scanner.Scan(typeof(NotifyPropertyChangedClass))
                .FirstOrDefault(p => Equals(p.Path, (PropertyPath) "property"));
            var ob = new NotifyPropertyChangedClass();
            ob.Property = "banaan";
            property.TrySetObject(ob);
            property.ValueChanged += (sender, e) => newValue = e.NewValue;

            // Act
            property.TrySetValue("asdf");

            // Assert
            Assert.AreEqual("asdf", ob.Property);
            Assert.AreEqual("asdf", newValue);
        }
    }

}


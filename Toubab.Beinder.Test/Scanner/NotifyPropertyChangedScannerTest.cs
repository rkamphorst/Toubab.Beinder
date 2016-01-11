﻿using System;
using NUnit.Framework;
using Toubab.Beinder.Mock;
using System.Linq;

namespace Toubab.Beinder.Scanner
{
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
            Assert.IsTrue(result.Any(p => p.Path.Equals((Path) "property")));
            Assert.IsTrue(result.Any(p => p.Path.Equals((Path) new[] { "second", "property" })));
        }

        [Test]
        public void ValueIsSetAndEventIsRaised() 
        {
            // Arrange
            var scanner = new NotifyPropertyChangedScanner();
            object newValue = null;
            var property = scanner.Scan(typeof(NotifyPropertyChangedClass))
                .FirstOrDefault(p => Equals(p.Path, (Path) "property"));
            var ob = new NotifyPropertyChangedClass();
            ob.Property = "banaan";
            property.SetObject(ob);
            property.Broadcast += (sender, e) => newValue = e.Argument;

            // Act
            property.TryHandleBroadcast("asdf");

            // Assert
            Assert.AreEqual("asdf", ob.Property);
            Assert.AreEqual("asdf", newValue);
        }
    }

}


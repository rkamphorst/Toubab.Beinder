namespace Toubab.Beinder.Scanner
{
    using System.Linq;
    using NUnit.Framework;
    using Bindable;
    using Mock;

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
            Assert.IsTrue(result.Any(p => p.Path.Equals((Path.Path) "property")));
            Assert.IsTrue(result.Any(p => p.Path.Equals((Path.Path) new[] { "second", "property" })));
        }

        [Test]
        public void ValueIsSetAndEventIsNotRaisedWhenHandlingBroadcast() 
        {
            // Arrange
            var scanner = new NotifyPropertyChangedScanner();
            object newValue = null;
            var property = (IProperty) scanner.Scan(typeof(NotifyPropertyChangedClass))
                .FirstOrDefault(p => Equals(p.Path, (Path.Path) "property"));
            var ob = new NotifyPropertyChangedClass();
            ob.Property = "banaan";
            property.SetObject(ob);
            property.Broadcast += (sender, e) => newValue = e.Payload;

            // Act
            property.TryHandleBroadcast(new object[] { "asdf" });

            // Assert
            Assert.AreEqual("asdf", ob.Property);
            Assert.AreEqual(null, newValue);
        }
    }

}


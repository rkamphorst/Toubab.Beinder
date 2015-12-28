using System;
using NUnit.Framework;
using Beinder.PropertyScanners;
using Beinder.Mocks;

namespace Beinder
{
    [TestFixture]
    public class BinderWithCustomExtensionsScannerTest
    {
        Valve[] _valves;

        [Test]
        public void BindSpecialPropertyOnExtensionsWithNotifyPropertyChanged()
        {
            // Arrange
            var bnd = new Binder();
            var customExtensionsScanner = new CustomExtensionsScanner(bnd.PropertyScanner);
            customExtensionsScanner.AdapterRegistry.Register<MockViewExtensions>();
            bnd.PropertyScanner.AddScanner(new NotifyPropertyChangedPropertyScanner());
            bnd.PropertyScanner.AddScanner(customExtensionsScanner);
            var ob1 = new MockView();
            var ob2 = new MockViewModel();
            _valves = bnd.Bind(new object[] { ob1, ob2 });

            // Act
            ob2.SpecialProperty = 666;

            // Assert
            Assert.AreEqual(666, ob1.GetSpecialProperty());
        }

        [Test]
        public void BindSpecialPropertyOnExtensionsWithReflection()
        {
            // Arrange
            var bnd = new Binder();
            var customExtensionsScanner = new CustomExtensionsScanner(bnd.PropertyScanner);
            customExtensionsScanner.AdapterRegistry.Register<MockViewExtensions2>();
            bnd.PropertyScanner.AddScanner(new NotifyPropertyChangedPropertyScanner());
            bnd.PropertyScanner.AddScanner(new ReflectionPropertyScanner());
            bnd.PropertyScanner.AddScanner(customExtensionsScanner);
            var ob1 = new MockView();
            var ob2 = new MockViewModel();
            _valves = bnd.Bind(new object[] { ob1, ob2 });

            // Act
            ob2.SpecialProperty2 = "666";

            // Assert
            Assert.AreEqual("666", ob1.GetSpecialProperty2());
        }
    }
}


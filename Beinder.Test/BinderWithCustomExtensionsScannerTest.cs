using System;
using NUnit.Framework;
using Beinder.PropertyScanners;
using Beinder.Mocks;

namespace Beinder
{
    [TestFixture]
    public class BinderWithCustomExtensionsScannerTest
    {
        IProperty[] _valves;

        [Test]
        public void BindSpecialPropertyOnExtensions()
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
    }
}


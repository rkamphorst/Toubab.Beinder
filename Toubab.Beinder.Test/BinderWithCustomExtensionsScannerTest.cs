using System;
using NUnit.Framework;
using Toubab.Beinder.Scanner;
using Toubab.Beinder.Mocks;
using System.Linq;

namespace Toubab.Beinder
{
    [TestFixture]
    public class BinderWithCustomExtensionsScannerTest
    {
        IBindings _bindings;

        [Test]
        public void BindSpecialPropertyOnExtensionsWithNotifyPropertyChanged()
        {
            // Arrange
            var bnd = new Binder();
            var customExtensionsScanner = new TypeExtensionsScanner(bnd.Scanner);
            customExtensionsScanner.AdapterRegistry.Register<MockViewExtensions>();
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            bnd.Scanner.Add(customExtensionsScanner);
            var ob1 = new MockView();
            var ob2 = new MockViewModel();
            _bindings = bnd.Bind(new object[] { ob1, ob2 });

            // Act
            ob2.SpecialProperty = 666;

            // Assert
            Assert.Greater(_bindings.Count(), 0);
            Assert.AreEqual(666, ob1.GetSpecialProperty());
        }

        [Test]
        public void BindSpecialPropertyOnExtensionsWithReflection()
        {
            // Arrange
            var bnd = new Binder();
            var customExtensionsScanner = new TypeExtensionsScanner(bnd.Scanner);
            customExtensionsScanner.AdapterRegistry.Register<MockViewExtensions2>();
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            bnd.Scanner.Add(new ReflectionScanner());
            bnd.Scanner.Add(customExtensionsScanner);
            var ob1 = new MockView();
            var ob2 = new MockViewModel();
            _bindings = bnd.Bind(new object[] { ob1, ob2 });

            // Act
            ob2.SpecialProperty2 = "666";

            // Assert
            Assert.Greater(_bindings.Count(), 0);
            Assert.AreEqual("666", ob1.GetSpecialProperty2());
        }
    }
}


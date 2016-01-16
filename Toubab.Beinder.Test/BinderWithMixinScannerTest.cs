﻿using System;
using NUnit.Framework;
using Toubab.Beinder.Scanner;
using Toubab.Beinder.Mock;
using System.Linq;

namespace Toubab.Beinder
{
    [TestFixture]
    public class BinderWithMixinScannerTest
    {
        IBindings _bindings;

        [Test]
        public void BindSpecialPropertyOnMixinWithNotifyPropertyChanged()
        {
            // Arrange
            var bnd = new Binder();
            var mixinScanner = new MixinScanner(bnd.Scanner);
            mixinScanner.AdapterRegistry.Register<MockViewMixin>();
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            bnd.Scanner.Add(mixinScanner);
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
        public void BindSpecialPropertyOnMixinWithReflection()
        {
            // Arrange
            var bnd = new Binder();
            var mixinScanner = new MixinScanner(bnd.Scanner);
            mixinScanner.AdapterRegistry.Register<MockViewMixin2>();
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            bnd.Scanner.Add(new ReflectionScanner());
            bnd.Scanner.Add(mixinScanner);
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


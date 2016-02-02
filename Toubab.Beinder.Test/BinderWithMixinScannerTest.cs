namespace Toubab.Beinder
{
    using System.Linq;
    using NUnit.Framework;
    using Mocks;
    using Scanners;
    using Mixins;

    [TestFixture]
    public class BinderWithMixinScannerTest
    {
        IBindings _bindings;

        [Test]
        public async void BindSpecialPropertyOnMixinWithNotifyPropertyChanged()
        {
            // Arrange
            var bnd = new Binder(new CombinedScanner());
            var mixinScanner = new CustomMixinScanner(bnd.Scanner);
            mixinScanner.AdapterRegistry.Register<MockViewMixin>();
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            bnd.Scanner.Add(mixinScanner);
            var ob1 = new MockView();
            var ob2 = new MockViewModel();
            _bindings = await bnd.Bind(new object[] { ob1, ob2 });

            // Act
            ob2.SpecialProperty = 666;

            // Assert
            Assert.Greater(_bindings.Count(), 0);
            Assert.AreEqual(666, ob1.GetSpecialProperty());
        }

        [Test]
        public async void BindSpecialPropertyOnMixinWithReflection()
        {
            // Arrange
            var bnd = new Binder(new CombinedScanner());
            var mixinScanner = new CustomMixinScanner(bnd.Scanner);
            mixinScanner.AdapterRegistry.Register<MockViewMixin2>();
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            bnd.Scanner.Add(new ReflectionScanner());
            bnd.Scanner.Add(mixinScanner);
            var ob1 = new MockView();
            var ob2 = new MockViewModel();
            _bindings = await bnd.Bind(new object[] { ob1, ob2 });

            // Act
            ob2.SpecialProperty2 = "666";

            // Assert
            Assert.Greater(_bindings.Count(), 0);
            Assert.AreEqual("666", ob1.GetSpecialProperty2());
        }

        [Test]
        public async void BogusCommandWithNullParameter()
        {
            // Arrange

            var bnd = new Binder(new CombinedScanner());
            var mixinScanner = new CustomMixinScanner(bnd.Scanner);
            mixinScanner.AdapterRegistry.Register<CommandMixin>();
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            bnd.Scanner.Add(new ReflectionScanner());
            bnd.Scanner.Add(mixinScanner);
            var ob1 = new MockView();
            var ob2 = new MockViewModel();
            _bindings = await bnd.Bind(new object[] { ob1, ob2 });
            ob2.BogusCount = 0;

            // Act
            ob1.BogusCommand.OnExecute(null);

            // Assert
            Assert.AreEqual(1, ob2.BogusCount);
        }

        [Test]
        public async void BogusCommandWithNonNullParameter()
        {
            // Arrange

            var bnd = new Binder(new CombinedScanner());
            var mixinScanner = new CustomMixinScanner(bnd.Scanner);
            mixinScanner.AdapterRegistry.Register<CommandMixin>();
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            bnd.Scanner.Add(new ReflectionScanner());
            bnd.Scanner.Add(mixinScanner);
            var ob1 = new MockView();
            var ob2 = new MockViewModel();
            _bindings = await bnd.Bind(new object[] { ob1, ob2 });
            ob2.BogusCount = 0;

            // Act
            ob1.BogusCommand.OnExecute(15);

            // Assert
            Assert.AreEqual(15, ob2.BogusCount);
        }

        [Test]
        public async void BogusCommandWithNullParameterCanExecute()
        {
            // Arrange

            var bnd = new Binder(new CombinedScanner());
            var mixinScanner = new CustomMixinScanner(bnd.Scanner);
            mixinScanner.AdapterRegistry.Register<CommandMixin>();
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            bnd.Scanner.Add(new ReflectionScanner());
            bnd.Scanner.Add(mixinScanner);
            var ob1 = new MockView();
            var ob2 = new MockViewModel();
            _bindings = await bnd.Bind(new object[] { ob1, ob2 });
            ob2.SpecialProperty2 = string.Empty;
            ob2.BogusCount = 0;

            // Act
            bool canExecute = ob1.BogusCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        [Test]
        public async void BogusCommandWithNullParameterCannotExecute()
        {
            // Arrange

            var bnd = new Binder(new CombinedScanner());
            var mixinScanner = new CustomMixinScanner(bnd.Scanner);
            mixinScanner.AdapterRegistry.Register<CommandMixin>();
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            bnd.Scanner.Add(new ReflectionScanner());
            bnd.Scanner.Add(mixinScanner);
            var ob1 = new MockView();
            var ob2 = new MockViewModel();
            _bindings = await bnd.Bind(new object[] { ob1, ob2 });
            ob2.SpecialProperty2 = "nonempty";
            ob2.BogusCount = 0;

            // Act
            bool canExecute = ob1.BogusCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [Test]
        public async void BogusCommandWithNonNullParameterCanExecute()
        {
            // Arrange

            var bnd = new Binder(new CombinedScanner());
            var mixinScanner = new CustomMixinScanner(bnd.Scanner);
            mixinScanner.AdapterRegistry.Register<CommandMixin>();
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            bnd.Scanner.Add(new ReflectionScanner());
            bnd.Scanner.Add(mixinScanner);
            var ob1 = new MockView();
            var ob2 = new MockViewModel();
            _bindings = await bnd.Bind(new object[] { ob1, ob2 });
            ob2.SpecialProperty2 = "nonempty";
            ob2.BogusCount = 0;

            // Act
            bool canExecute = ob1.BogusCommand.CanExecute(42);

            // Assert
            Assert.IsTrue(canExecute);
        }

        [Test]
        public async void BogusCommandWithNonNullParameterCannotExecute()
        {
            // Arrange

            var bnd = new Binder(new CombinedScanner());
            var mixinScanner = new CustomMixinScanner(bnd.Scanner);
            mixinScanner.AdapterRegistry.Register<CommandMixin>();
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            bnd.Scanner.Add(new ReflectionScanner());
            bnd.Scanner.Add(mixinScanner);
            var ob1 = new MockView();
            var ob2 = new MockViewModel();
            _bindings = await bnd.Bind(new object[] { ob1, ob2 });
            ob2.SpecialProperty2 = "nonempty";
            ob2.BogusCount = 0;

            // Act
            bool canExecute = ob1.BogusCommand.CanExecute(10);

            // Assert
            Assert.IsFalse(canExecute);
        }
    }
}


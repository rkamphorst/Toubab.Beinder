using NUnit.Framework;
using Beinder.PropertyScanners;
using Beinder.Mocks;

namespace Beinder
{
    [TestFixture]
    public class BinderTest
    {
        IProperty[] _valves;

        [Test]
        public void BindNotifyPropertyChangedObjects()
        {
            // Arrange
            var bnd = new Binder();
            bnd.PropertyScanner.AddScanner(new NotifyPropertyChangedPropertyScanner());
            var ob1 = new NotifyPropertyChangedClass();
            var ob2 = new NotifyPropertyChangedClass();
            _valves = bnd.Bind(new object[] { ob1, ob2 });

            // Act
            ob2.Property = "a";

            // Assert
            Assert.AreEqual("a", ob1.Property);
        }

        [Test]
        public void BindReflectedObjects()
        {
            // Arrange
            var bnd = new Binder();
            bnd.PropertyScanner.AddScanner(new ReflectionPropertyScanner());
            bnd.PropertyScanner.AddScanner(new NotifyPropertyChangedPropertyScanner());
            var ob1 = new ClassWithPropertyAndEvents();
            var ob2 = new ClassWithPropertyAndEvents();
            _valves = bnd.Bind(new object[] { ob1, ob2 });

            // Act
            ob2.Property = "a";

            // Assert
            Assert.AreEqual("a", ob1.Property);
        }

        [Test]
        public void BindObjectsOfDifferentTypesNotpropToReflected()
        {
            // Arrange
            var bnd = new Binder();
            bnd.PropertyScanner.AddScanner(new ReflectionPropertyScanner());
            bnd.PropertyScanner.AddScanner(new NotifyPropertyChangedPropertyScanner());
            var ob1 = new ClassWithPropertyAndEvents();
            var ob2 = new NotifyPropertyChangedClass();
            _valves = bnd.Bind(new object[] { ob1, ob2 });

            // Act
            ob2.Property = "a";

            // Assert
            Assert.AreEqual("a", ob1.Property);
        }

        [Test]
        public void BindObjectsOfDifferentTypesReflectedToNotprop()
        {
            // Arrange
            var bnd = new Binder();
            bnd.PropertyScanner.AddScanner(new ReflectionPropertyScanner());
            bnd.PropertyScanner.AddScanner(new NotifyPropertyChangedPropertyScanner());
            var ob1 = new NotifyPropertyChangedClass();
            var ob2 = new ClassWithPropertyAndEvents();
            _valves = bnd.Bind(new object[] { ob1, ob2 });

            // Act
            ob2.Property = "a";

            // Assert
            Assert.AreEqual("a", ob1.Property);
        }

        [Test]
        public void BindObjectsOfDifferentTypesResultsInPropertiesBeingSetOnce()
        {
            // Arrange
            int ob1cnt = 0, ob2cnt = 0;
            var bnd = new Binder();
            bnd.PropertyScanner.AddScanner(new ReflectionPropertyScanner());
            bnd.PropertyScanner.AddScanner(new NotifyPropertyChangedPropertyScanner());
            var ob1 = new NotifyPropertyChangedClass();
            var ob2 = new ClassWithPropertyAndEvents();
            ob1.PropertyChanged += delegate
            {
                ob1cnt++;
            };
            ob2.PropertyChanged += delegate
            {
                ob2cnt++;
            };
            _valves = bnd.Bind(new object[] { ob1, ob2 });

            // Act
            ob2.Property = "a";

            // Assert
            Assert.AreEqual(1, ob1cnt);
            Assert.AreEqual(1, ob2cnt);
        }

        [Test]
        public void BindViewmodelToView()
        {
            // Arrange
            var bnd = new Binder();
            bnd.PropertyScanner.AddScanner(new ReflectionPropertyScanner());
            bnd.PropertyScanner.AddScanner(new NotifyPropertyChangedPropertyScanner());
            var view = new MockView { Control = new MockControl("ViewOrig") };
            var vm = new MockViewModel();
            bnd.Bind(new object[] { vm, view });

            // Act
            vm.ControlText = "banaan";
            vm.ControlSize = 11;

            // Assert
            Assert.AreEqual("banaan", view.Control.Text);
            Assert.AreEqual(11, view.Control.Size);
        }

        [Test]
        public void BindViewToViewmodel()
        {
            // Arrange
            var bnd = new Binder();
            bnd.PropertyScanner.AddScanner(new ReflectionPropertyScanner());
            bnd.PropertyScanner.AddScanner(new NotifyPropertyChangedPropertyScanner());
            var view = new MockView { Control = new MockControl("ViewOrig") };
            var vm = new MockViewModel();
            var valves = bnd.Bind(new object[] { vm, view });

            // Act
            view.Control.Text = "banaan";
            view.Control.Size = 11;

            // Assert2
            Assert.AreEqual("banaan", vm.ControlText);
            Assert.AreEqual(11, vm.ControlSize);
        }

        [Test]
        public void BindViewToViewmodelReplaceParent()
        {
            // Arrange
            var bnd = new Binder();
            bnd.PropertyScanner.AddScanner(new ReflectionPropertyScanner());
            bnd.PropertyScanner.AddScanner(new NotifyPropertyChangedPropertyScanner());
            var view = new MockView { Control = new MockControl("ViewOrig") };
            var vm = new MockViewModel();
            var valves = bnd.Bind(new object[] { vm, view });

            // Act
            view.Control = new MockControl("ViewNew")
            {
                Text = "banaan",
                Size = 11
            };

            // Assert
            Assert.AreEqual("banaan", vm.ControlText);
            Assert.AreEqual(11, vm.ControlSize);
        }

        [Test]
        public void ReplaceParentChangesChildrenIfParentDifferentTypes()
        {
            // Arrange
            var bnd = new Binder();
            bnd.PropertyScanner.AddScanner(new ReflectionPropertyScanner());
            bnd.PropertyScanner.AddScanner(new NotifyPropertyChangedPropertyScanner());
            var view = new MockView { Control = new MockControl("ViewOrig") };
            var vm = new MockViewModel2 { Control = new MockControlViewModel ("VmOrig") };
            var valves = bnd.Bind(new object[] { vm, view });

            // Act
            view.Control = new MockControl("ViewNew")
                {
                    Text = "banaan",
                    Size = 11
                };

            // Assert
            Assert.AreEqual("banaan", vm.Control.Text);
            Assert.AreEqual(11, vm.Control.Size);
        }

        [Test]
        public void ReplaceParentWithSameTypeChangesParentAndLeavesChildrenAlone()
        {
            // Arrange
            var bnd = new Binder();
            bnd.PropertyScanner.AddScanner(new ReflectionPropertyScanner());
            bnd.PropertyScanner.AddScanner(new NotifyPropertyChangedPropertyScanner());
            var view = new MockView { Control = new MockControl("ViewOrig") { Text = "Appel", Size = 100 } };
            var viewCtl = view.Control;
            var vm = new MockView { Control = new MockControl("VmOrig") { Text = "Peer", Size = 200 } };
            var vmCtl = vm.Control;
            var valves = bnd.Bind(new object[] { vm, view });

            // Act
            view.Control = new MockControl("ViewNew")
                {
                    Text = "banaan",
                    Size = 11
                };

            // Assert
            Assert.AreSame(view.Control, vm.Control);
            Assert.AreEqual("Appel", viewCtl.Text);
            Assert.AreEqual(100, viewCtl.Size);
            Assert.AreEqual("Peer", vmCtl.Text);
            Assert.AreEqual(200, vmCtl.Size);
        }
    }


}


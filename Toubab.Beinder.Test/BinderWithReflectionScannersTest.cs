using NUnit.Framework;
using Toubab.Beinder.PropertyScanners;
using Toubab.Beinder.Mocks;

namespace Toubab.Beinder
{
    [TestFixture]
    public class BinderWithReflectionScannersTest
    {
        Valve[] _valves;

        [Test]
        public void BindTwoNotifyPropertyChangedObjects()
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
            Assert.Greater(_valves.Length, 0);
            Assert.AreEqual("a", ob1.Property);
        }

        [Test]
        public void BindThreeNotifyPropertyChangedObjects()
        {
            {
                // Arrange
                var bnd = new Binder();
                bnd.PropertyScanner.AddScanner(new NotifyPropertyChangedPropertyScanner());
                var ob1 = new NotifyPropertyChangedClass();
                var ob2 = new NotifyPropertyChangedClass();
                var ob3 = new NotifyPropertyChangedClass();
                _valves = bnd.Bind(new object[] { ob1, ob2, ob3 });

                // Act
                ob1.Property = "a";

                // Assert
                Assert.Greater(_valves.Length, 0);
                Assert.AreEqual("a", ob1.Property);
                Assert.AreEqual("a", ob2.Property);
                Assert.AreEqual("a", ob3.Property);
            }

            {
                // Arrange
                var bnd = new Binder();
                bnd.PropertyScanner.AddScanner(new NotifyPropertyChangedPropertyScanner());
                var ob1 = new NotifyPropertyChangedClass();
                var ob2 = new NotifyPropertyChangedClass();
                var ob3 = new NotifyPropertyChangedClass();
                _valves = bnd.Bind(new object[] { ob1, ob2, ob3 });

                // Act
                ob2.Property = "a";

                // Assert
                Assert.Greater(_valves.Length, 0);
                Assert.AreEqual("a", ob1.Property);
                Assert.AreEqual("a", ob2.Property);
                Assert.AreEqual("a", ob3.Property);
            }

            {
                // Arrange
                var bnd = new Binder();
                bnd.PropertyScanner.AddScanner(new NotifyPropertyChangedPropertyScanner());
                var ob1 = new NotifyPropertyChangedClass();
                var ob2 = new NotifyPropertyChangedClass();
                var ob3 = new NotifyPropertyChangedClass();
                _valves = bnd.Bind(new object[] { ob1, ob2, ob3 });

                // Act
                ob3.Property = "a";

                // Assert
                Assert.Greater(_valves.Length, 0);
                Assert.AreEqual("a", ob1.Property);
                Assert.AreEqual("a", ob2.Property);
                Assert.AreEqual("a", ob3.Property);
            }
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
            Assert.Greater(_valves.Length, 0);
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
            Assert.Greater(_valves.Length, 0);
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
            Assert.Greater(_valves.Length, 0);
            Assert.AreEqual("a", ob1.Property);
        }

        [Test]
        public void BindDifferentlyDistributedObjectHierarchiesChangeChild()
        {
            // Arrange
            var bnd = new Binder();
            bnd.PropertyScanner.AddScanner(new ReflectionPropertyScanner());
            bnd.PropertyScanner.AddScanner(new NotifyPropertyChangedPropertyScanner());
            var ob1 = new Abcd1();
            var ob2 = new Abcd2();
            var ob3 = new Abcd3();
            _valves = bnd.Bind(new object[] { ob1, ob2, ob3 });

            // Act
            ob1.Aaa.Bee.Cee.Dee.Eee = "x";


            // Assert
            Assert.Greater(_valves.Length, 0);
            Assert.AreEqual("x", ob2.AaaBee.CeeDee.Eee);
            Assert.AreEqual("x", ob3.Aaa.BeeCee.Dee.Eee);
        }

        [Test]
        public void BindDifferentlyDistributedObjectHierarchiesChangeParent()
        {
            // Arrange
            var bnd = new Binder();
            bnd.PropertyScanner.AddScanner(new ReflectionPropertyScanner());
            bnd.PropertyScanner.AddScanner(new NotifyPropertyChangedPropertyScanner());
            var ob1 = new Abcd1();
            var ob2 = new Abcd2();
            var ob3 = new Abcd3();
            ob2.AaaBee.CeeDee.Eee = "x";
            _valves = bnd.Bind(new object[] { ob1, ob2, ob3 });

            // Act
            Abcd2.AB oldAaaBee = ob2.AaaBee;
            ob2.AaaBee = new Abcd2.AB
            {
                CeeDee = new Abcd2.CD
                {
                    Eee = "y"
                }
            };


            // Assert
            Assert.Greater(_valves.Length, 0);
            Assert.AreEqual("y", ob1.Aaa.Bee.Cee.Dee.Eee);
            Assert.AreEqual("y", ob3.Aaa.BeeCee.Dee.Eee);
            Assert.AreEqual("x", oldAaaBee.CeeDee.Eee);
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
            ob1cnt = 0;
            ob2cnt = 0;

            // Act
            ob2.Property = "a";

            // Assert
            Assert.Greater(_valves.Length, 0);
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
            _valves = bnd.Bind(new object[] { vm, view });

            // Act
            vm.ControlText = "banaan";
            vm.ControlSize = 11;

            // Assert
            Assert.Greater(_valves.Length, 0);
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
            _valves = bnd.Bind(new object[] { vm, view });

            // Act
            view.Control.Text = "banaan";
            view.Control.Size = 11;

            // Assert
            Assert.Greater(_valves.Length, 0);
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
            _valves = bnd.Bind(new object[] { vm, view });

            // Act
            view.Control = new MockControl("ViewNew")
            {
                Text = "banaan",
                Size = 11
            };

            // Assert
            Assert.Greater(_valves.Length, 0);
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
            var vm = new MockViewModel2 { Control = new MockControlViewModel("VmOrig") };
            _valves = bnd.Bind(new object[] { vm, view });

            // Act
            view.Control = new MockControl("ViewNew")
            {
                Text = "banaan",
                Size = 11
            };

            // Assert
            Assert.Greater(_valves.Length, 0);
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
            _valves = bnd.Bind(new object[] { vm, view });

            // Act
            view.Control = new MockControl("ViewNew")
            {
                Text = "banaan",
                Size = 11
            };

            // Assert
            Assert.Greater(_valves.Length, 0);
            Assert.AreSame(view.Control, vm.Control);
            Assert.AreEqual("Appel", viewCtl.Text);
            Assert.AreEqual(100, viewCtl.Size);
            Assert.AreEqual("Peer", vmCtl.Text);
            Assert.AreEqual(200, vmCtl.Size);
        }
    }


}


namespace Toubab.Beinder
{
    using System.Linq;
    using NUnit.Framework;
    using Mocks;
    using Mocks.Abcd;
    using Scanners;

    [TestFixture]
    public class BinderWithReflectionScannersTest
    {
        IBindings _bindings;

        [Test]
        public async void BindTwoNotifyPropertyChangedObjects()
        {
            // Arrange
            var bnd = new Binder(new CombinedScanner());
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            var ob1 = new NotifyPropertyChangedClass();
            var ob2 = new NotifyPropertyChangedClass();
            _bindings = await bnd.Bind(new object[] { ob1, ob2 });

            // Act
            ob2.Property = "a";

            // Assert
            Assert.Greater(_bindings.Count, 0);
            Assert.AreEqual("a", ob1.Property);
        }

        [Test]
        public async void BindThreeNotifyPropertyChangedObjects()
        {
            {
                // Arrange
                var bnd = new Binder(new CombinedScanner());
                bnd.Scanner.Add(new NotifyPropertyChangedScanner());
                var ob1 = new NotifyPropertyChangedClass();
                var ob2 = new NotifyPropertyChangedClass();
                var ob3 = new NotifyPropertyChangedClass();
                _bindings = await bnd.Bind(new object[] { ob1, ob2, ob3 });

                // Act
                ob1.Property = "a";

                // Assert
                Assert.Greater(_bindings.Count, 0);
                Assert.AreEqual("a", ob1.Property);
                Assert.AreEqual("a", ob2.Property);
                Assert.AreEqual("a", ob3.Property);
            }

            {
                // Arrange
                var bnd = new Binder();
                bnd.Scanner.Add(new NotifyPropertyChangedScanner());
                var ob1 = new NotifyPropertyChangedClass();
                var ob2 = new NotifyPropertyChangedClass();
                var ob3 = new NotifyPropertyChangedClass();
                _bindings = await bnd.Bind(new object[] { ob1, ob2, ob3 });

                // Act
                ob2.Property = "a";

                // Assert
                Assert.Greater(_bindings.Count, 0);
                Assert.AreEqual("a", ob1.Property);
                Assert.AreEqual("a", ob2.Property);
                Assert.AreEqual("a", ob3.Property);
            }

            {
                // Arrange
                var bnd = new Binder(new CombinedScanner());
                bnd.Scanner.Add(new NotifyPropertyChangedScanner());
                var ob1 = new NotifyPropertyChangedClass();
                var ob2 = new NotifyPropertyChangedClass();
                var ob3 = new NotifyPropertyChangedClass();
                _bindings = await bnd.Bind(new object[] { ob1, ob2, ob3 });

                // Act
                ob3.Property = "a";

                // Assert
                Assert.Greater(_bindings.Count, 0);
                Assert.AreEqual("a", ob1.Property);
                Assert.AreEqual("a", ob2.Property);
                Assert.AreEqual("a", ob3.Property);
            }
        }

        [Test]
        public async void BindReflectedObjects()
        {
            // Arrange
            var bnd = new Binder(new CombinedScanner());
            bnd.Scanner.Add(new ReflectionScanner());
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            var ob1 = new ClassWithPropertyAndEvents();
            var ob2 = new ClassWithPropertyAndEvents();
            _bindings = await bnd.Bind(new object[] { ob1, ob2 });

            // Act
            ob2.Property = "a";

            // Assert
            Assert.Greater(_bindings.Count, 0);
            Assert.AreEqual("a", ob1.Property);
        }

        [Test]
        public async void BindObjectsOfDifferentTypesNotpropToReflected()
        {
            // Arrange
            var bnd = new Binder(new CombinedScanner());
            bnd.Scanner.Add(new ReflectionScanner());
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            var ob1 = new ClassWithPropertyAndEvents();
            var ob2 = new NotifyPropertyChangedClass();
            _bindings = await bnd.Bind(new object[] { ob1, ob2 });

            // Act
            ob2.Property = "a";

            // Assert
            Assert.Greater(_bindings.Count, 0);
            Assert.AreEqual("a", ob1.Property);
        }

        [Test]
        public async void BindObjectsOfDifferentTypesReflectedToNotprop()
        {
            // Arrange
            var bnd = new Binder(new CombinedScanner());
            bnd.Scanner.Add(new ReflectionScanner());
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            var ob1 = new NotifyPropertyChangedClass();
            var ob2 = new ClassWithPropertyAndEvents();
            _bindings = await bnd.Bind(new object[] { ob1, ob2 });

            // Act
            ob2.Property = "a";

            // Assert
            Assert.Greater(_bindings.Count, 0);
            Assert.AreEqual("a", ob1.Property);
        }

        [Test]
        public async void BindDifferentlyDistributedObjectHierarchiesChangeChild()
        {
            // Arrange
            var bnd = new Binder(new CombinedScanner());
            bnd.Scanner.Add(new ReflectionScanner());
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            var ob1 = new Abcd1();
            var ob2 = new Abcd2();
            var ob3 = new Abcd3();
            _bindings = await bnd.Bind(new object[] { ob1, ob2, ob3 });

            // Act
            ob1.Aaa.Bee.Cee.Dee.Eee = "x";


            // Assert
            Assert.Greater(_bindings.Count, 0);
            Assert.AreEqual("x", ob2.AaaBee.CeeDee.Eee);
            Assert.AreEqual("x", ob3.Aaa.BeeCee.Dee.Eee);
        }

        [Test]
        public async void BindDifferentlyDistributedObjectHierarchiesChangeParent()
        {
            // Arrange
            var bnd = new Binder(new CombinedScanner());
            bnd.Scanner.Add(new ReflectionScanner());
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            var ob1 = new Abcd1();
            var ob2 = new Abcd2();
            var ob3 = new Abcd3();
            _bindings = await bnd.Bind(new object[] { ob1, ob2, ob3 });
            ob2.AaaBee.CeeDee.Eee = "x";

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
            Assert.Greater(_bindings.Count, 0);
            Assert.AreEqual("y", ob1.Aaa.Bee.Cee.Dee.Eee);
            Assert.AreEqual("y", ob3.Aaa.BeeCee.Dee.Eee);
            Assert.AreEqual("x", oldAaaBee.CeeDee.Eee);
        }

        [Test]
        public async void BindObjectsOfDifferentTypesResultsInPropertiesBeingSetOnce()
        {
            // Arrange
            int ob1cnt = 0, ob2cnt = 0;
            var bnd = new Binder(new CombinedScanner());
            bnd.Scanner.Add(new ReflectionScanner());
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
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
            _bindings = await bnd.Bind(new object[] { ob1, ob2 });
            ob1cnt = 0;
            ob2cnt = 0;

            // Act
            ob2.Property = "a";

            // Assert
            Assert.Greater(_bindings.Count, 0);
            Assert.AreEqual(1, ob1cnt);
            Assert.AreEqual(1, ob2cnt);
        }

        [Test]
        public async void BindViewmodelToView()
        {
            // Arrange
            var bnd = new Binder(new CombinedScanner());
            bnd.Scanner.Add(new ReflectionScanner());
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            var view = new MockView { Control = new MockControl("ViewOrig") };
            var vm = new MockViewModel();
            _bindings = await bnd.Bind(new object[] { vm, view });

            // Act
            vm.ControlText = "banaan";
            vm.ControlSize = 11;

            // Assert
            Assert.Greater(_bindings.Count, 0);
            Assert.AreEqual("banaan", view.Control.Text);
            Assert.AreEqual(11, view.Control.Size);
        }

        [Test]
        public async void BindViewToViewmodel()
        {
            // Arrange
            var bnd = new Binder(new CombinedScanner());
            bnd.Scanner.Add(new ReflectionScanner());
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            var view = new MockView { Control = new MockControl("ViewOrig") };
            var vm = new MockViewModel();
            _bindings = await bnd.Bind(new object[] { vm, view });

            // Act
            view.Control.Text = "banaan";
            view.Control.Size = 11;

            // Assert
            Assert.Greater(_bindings.Count, 0);
            Assert.AreEqual("banaan", vm.ControlText);
            Assert.AreEqual(11, vm.ControlSize);
        }

        [Test]
        public async void BindViewToViewmodelReplaceParent()
        {
            // Arrange
            var bnd = new Binder(new CombinedScanner());
            bnd.Scanner.Add(new ReflectionScanner());
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            var view = new MockView { Control = new MockControl("ViewOrig") };
            var vm = new MockViewModel();
            _bindings = await bnd.Bind(new object[] { vm, view });

            // Act
            view.Control = new MockControl("ViewNew")
            {
                Text = "banaan",
                Size = 11
            };

            // Assert
            Assert.Greater(_bindings.Count, 0);
            Assert.AreEqual("banaan", vm.ControlText);
            Assert.AreEqual(11, vm.ControlSize);
        }

        [Test]
        public async void ReplaceParentChangesChildrenIfParentDifferentTypes()
        {
            // Arrange
            var bnd = new Binder(new CombinedScanner());
            bnd.Scanner.Add(new ReflectionScanner());
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            var view = new MockView { Control = new MockControl("ViewOrig") };
            var vm = new MockViewModel2 { Control = new MockControlViewModel("VmOrig") };
            _bindings = await bnd.Bind(new object[] { vm, view });

            // Act
            view.Control = new MockControl("ViewNew")
            {
                Text = "banaan",
                Size = 11
            };

            // Assert
            Assert.Greater(_bindings.Count, 0);
            Assert.AreEqual("banaan", vm.Control.Text);
            Assert.AreEqual(11, vm.Control.Size);
        }

        [Test]
        public async void ReplaceParentWithSameTypeChangesParentAndLeavesChildrenAlone()
        {
            // Arrange
            var bnd = new Binder(new CombinedScanner());
            bnd.Scanner.Add(new ReflectionScanner());
            bnd.Scanner.Add(new NotifyPropertyChangedScanner());
            var view = new MockView { Control = new MockControl("ViewOrig") { Text = "Appel", Size = 100 } };
            var viewCtl = view.Control;
            var vm = new MockView { Control = new MockControl("VmOrig") { Text = "Peer", Size = 200 } };
            var vmCtl = vm.Control;
            _bindings = await bnd.Bind(new object[] { vm, view });

            // Act
            view.Control = new MockControl("ViewNew")
            {
                Text = "banaan",
                Size = 11
            };

            // Assert
            Assert.Greater(_bindings.Count, 0);
            Assert.AreSame(view.Control, vm.Control);
            Assert.AreEqual("Appel", viewCtl.Text);
            Assert.AreEqual(100, viewCtl.Size);
            Assert.AreEqual("Peer", vmCtl.Text);
            Assert.AreEqual(200, vmCtl.Size);
        }

        [Test]
        public async void BindEventToMethd()
        {
            // Arrange
            var bnd = new Binder(new CombinedScanner());
            bnd.Scanner.Add(new ReflectionScanner());
            var view = new MockView();
            var vm = new MockViewModel();
            _bindings = await bnd.Bind(new object[] { vm, view });

            // Act
            view.OnClick(); // raises the Click event
            view.OnClick(); // raises the Click event
            view.OnClick(); // raises the Click event

            // Assert: click should have propagated to vm.Click(), which increments ClickCount
            Assert.AreEqual(3, vm.ClickCount);
        }

        [Test]
        public async void BindCommandToCommandSource() 
        {
            // Arrange
            var bnd = new Binder();
            var view = new MockView();
            var vm = new MockViewModel();
            _bindings = await bnd.Bind(new object[] { vm, view });


            // Act
            view.OnBogus();
            view.OnBogus();
            view.OnBogus();

            // Assert
            Assert.AreEqual(3, vm.BogusCount);
        }

    }

}


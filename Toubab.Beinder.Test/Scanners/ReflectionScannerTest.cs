namespace Toubab.Beinder.Scanners
{
    using System;
    using System.Linq;
    using NUnit.Framework;
    using Bindables;
    using Mocks;
    using Paths;

    [TestFixture]
    public class ReflectionScannerTest
    {
        [Test]
        public void ScanBindables()
        {
            // Arrange
            var scanner = new ReflectionScanner();

            // Act
            var result = scanner.Scan(typeof(ClassWithPropertyAndEvents)).ToArray();

            // Assert
            Assert.AreEqual(8, result.Length);
            Assert.IsTrue(result.Any(p => p is IProperty && p.Capabilities == BindingOperations.All && p.Path.Equals(new Path("property"))));
            Assert.IsTrue(result.Any(p => p is IProperty && p.Capabilities == BindingOperations.All && p.Path.Equals(new Path("second", "property" ))));
            Assert.IsTrue(result.Any(p => p is IEvent && p.Capabilities == BindingOperations.Broadcast && p.Path.Equals(new Path("property", "changed" ))));
            Assert.IsTrue(result.Any(p => p is IEvent && p.Capabilities == BindingOperations.Broadcast && p.Path.Equals(new Path("second", "property", "changed" ))));
            Assert.IsTrue(result.Any(p => p is IEvent && p.Capabilities == BindingOperations.Broadcast && p.Path.Equals(new Path("simple", "event" ))));
            Assert.IsTrue(result.Any(p => p is IEvent && p.Capabilities == BindingOperations.Broadcast && p.Path.Equals(new Path("complex", "event" ))));
            Assert.IsTrue(result.Any(p => p is IEventHandler && p.Capabilities == BindingOperations.HandleBroadcast && p.Path.Equals(new Path("on", "simple", "event" ))));
            Assert.IsTrue(result.Any(p => p is IEventHandler && p.Capabilities == BindingOperations.HandleBroadcast && p.Path.Equals(new Path("on", "complex", "event" ))));
        }

        [Test]
        public void PropertyValueIsSetButEventIsNotRaisedWhileHandlingBroadcast()
        {
            // Arrange
            var scanner = new ReflectionScanner();
            object newValue = null;
            var property = (IProperty)scanner.Scan(typeof(ClassWithPropertyAndEvents))
                .FirstOrDefault(p => Equals(p.Path, new Path("property")));
            var ob = new ClassWithPropertyAndEvents();
            ob.Property = "banaan";
            property.SetObject(ob);
            property.SetBroadcastListener(payload => newValue = payload);

            // Act
            property.TryHandleBroadcastAsync(new[] { "fdsa" });

            // Assert
            Assert.AreEqual("fdsa", ob.Property);
            Assert.AreEqual(null, newValue);
        }

        [Test]
        public void SimpleEventIsRaised()
        {
            // Arrange
            var scanner = new ReflectionScanner();
            object[] eventArgs = null;
            int eventHappened = 0;
            var evt = (IEvent)scanner.Scan(typeof(ClassWithPropertyAndEvents))
                .FirstOrDefault(p => Equals(p.Path, new Path("simple", "event")));
            var ob = new ClassWithPropertyAndEvents();
            evt.SetObject(ob);
            evt.SetBroadcastListener(payload => 
            {
                eventArgs = payload;
                eventHappened++;
            });

            // Act
            ob.OnSimpleEvent();

            // Assert
            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(2, eventArgs.Length);
            Assert.AreSame(ob, eventArgs[0]);
            Assert.AreEqual(1, eventHappened);
        }

        [Test]
        public void SimpleEventIsRaisedMultipleTimes()
        {
            // Arrange
            var scanner = new ReflectionScanner();
            object[] eventArgs = null;
            int eventHappened = 0;
            var evt = (IEvent)scanner.Scan(typeof(ClassWithPropertyAndEvents))
                .FirstOrDefault(p => Equals(p.Path, new Path("simple", "event" )));
            var ob = new ClassWithPropertyAndEvents();
            evt.SetObject(ob);
            evt.SetBroadcastListener(payload => 
                {
                    eventArgs = payload;
                    eventHappened++;
                });

            // Act
            ob.OnSimpleEvent();
            ob.OnSimpleEvent();
            ob.OnSimpleEvent();
            ob.OnSimpleEvent();

            // Assert
            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(2, eventArgs.Length);
            Assert.AreSame(ob, eventArgs[0]);
            Assert.AreEqual(4, eventHappened);
        }

        [Test]
        public void ComplexEventIsRaised()
        {
            // Arrange
            var scanner = new ReflectionScanner();
            object[] eventArgs = null;
            int eventHappened = 0;
            var evt = (IEvent)scanner.Scan(typeof(ClassWithPropertyAndEvents))
                .FirstOrDefault(p => Equals(p.Path, new Path("complex", "event" )));
            var ob = new ClassWithPropertyAndEvents();

            var cwpae = new ClassWithPropertyAndEvents();
            var vtfce = new ClassWithPropertyAndEvents.ValueTypeForComplexEvent
            {
                A = "xyz",
                B = 10
            };

            evt.SetObject(ob);
            evt.SetBroadcastListener(payload => 
                {
                    eventArgs = payload;
                    eventHappened++;
                });

            // Act

            ob.OnComplexEvent("a", 15, cwpae, vtfce, ob);

            // Assert
            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(5, eventArgs.Length);
            Assert.AreEqual("a", eventArgs[0]);
            Assert.AreEqual(15, eventArgs[1]);
            Assert.AreSame(cwpae, eventArgs[2]);
            Assert.AreEqual(vtfce, eventArgs[3]);
            Assert.AreSame(ob, eventArgs[4]);
            Assert.AreEqual(1, eventHappened);
        }

        [Test]
        public void ComplexEventIsRaisedMultipleTimes()
        {
            // Arrange
            var scanner = new ReflectionScanner();
            object[] eventArgs = null;
            int eventHappened = 0;
            var evt = (IEvent)scanner.Scan(typeof(ClassWithPropertyAndEvents))
                .FirstOrDefault(p => Equals(p.Path, new Path("complex", "event" )));
            var ob = new ClassWithPropertyAndEvents();

            var cwpae = new ClassWithPropertyAndEvents();
            var vtfce = new ClassWithPropertyAndEvents.ValueTypeForComplexEvent
            {
                A = "xyz",
                B = 10
            };

            evt.SetObject(ob);
            evt.SetBroadcastListener(payload => 
                {
                    eventArgs = payload;
                    eventHappened++;
                });

            // Act
            ob.OnComplexEvent("a", 15, cwpae, vtfce, ob);
            ob.OnComplexEvent("b", 14, cwpae, vtfce, ob);
            ob.OnComplexEvent("c", 13, cwpae, vtfce, ob);
            ob.OnComplexEvent("d", 12, cwpae, vtfce, ob);
            ob.OnComplexEvent("e", 11, cwpae, vtfce, ob);

            // Assert
            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(5, eventArgs.Length);
            Assert.AreEqual("e", eventArgs[0]);
            Assert.AreEqual(11, eventArgs[1]);
            Assert.AreSame(cwpae, eventArgs[2]);
            Assert.AreEqual(vtfce, eventArgs[3]);
            Assert.AreSame(ob, eventArgs[4]);
            Assert.AreEqual(5, eventHappened);
        }


        [Test]
        public void SimpleEventIsHandled()
        {
            // Arrange
            var scanner = new ReflectionScanner();
            int eventHappened = 0;
            var method = (IEventHandler)scanner.Scan(typeof(ClassWithPropertyAndEvents))
                .FirstOrDefault(p => Equals(p.Path, new Path("on", "simple", "event" )));
            var ob = new ClassWithPropertyAndEvents();
            ob.SimpleEvent += (sender, e) => eventHappened++;
            method.SetObject(ob);

            // Act
            method.TryHandleBroadcastAsync(new object[0]);

            // Assert
            Assert.AreEqual(1, eventHappened);
        }

        [Test]
        public void SimpleEventIsHandledMultipleTimes()
        {
            // Arrange
            var scanner = new ReflectionScanner();
            int eventHappened = 0;
            var method = (IEventHandler)scanner.Scan(typeof(ClassWithPropertyAndEvents))
                .FirstOrDefault(p => Equals(p.Path, new Path("on", "simple", "event" )));
            var ob = new ClassWithPropertyAndEvents();
            ob.SimpleEvent += (sender, e) => eventHappened++;
            method.SetObject(ob);

            // Act
            method.TryHandleBroadcastAsync(new object[0]);
            method.TryHandleBroadcastAsync(new object[0]);
            method.TryHandleBroadcastAsync(new object[0]);
            method.TryHandleBroadcastAsync(new object[0]);

            // Assert
            Assert.AreEqual(4, eventHappened);
        }

        [Test]
        public void ComplexEventIsHandled()
        {
            // Arrange
            var scanner = new ReflectionScanner();
            int eventHappened = 0;
            object[] eventArgs = null;
            var method = (IEventHandler)scanner.Scan(typeof(ClassWithPropertyAndEvents))
                .FirstOrDefault(p => Equals(p.Path, new Path("on", "complex", "event")));
            var ob = new ClassWithPropertyAndEvents();

            var cwpae = new ClassWithPropertyAndEvents();
            var vtfce = new ClassWithPropertyAndEvents.ValueTypeForComplexEvent
                {
                    A = "xyz",
                    B = 10
                };
            var bcArgs = new object[] {
                "a", 15, cwpae, vtfce, ob
            };

            ob.ComplexEvent += (arg1, arg2, arg3, arg4, arg5) => {
                eventHappened++;
                eventArgs = new object[] { arg1, arg2, arg3, arg4, arg5 };
            };
            method.SetObject(ob);

            // Act
            method.TryHandleBroadcastAsync(bcArgs);

            // Assert
            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(bcArgs, eventArgs);
            Assert.AreEqual(1, eventHappened);
        }

        [Test]
        public void ComplexEventIsHandledMultipleTimes()
        {
            // Arrange
            var scanner = new ReflectionScanner();
            int eventHappened = 0;
            object[] eventArgs = null;
            var method = (IEventHandler)scanner.Scan(typeof(ClassWithPropertyAndEvents))
                .FirstOrDefault(p => Equals(p.Path, new Path("on", "complex", "event")));
            var ob = new ClassWithPropertyAndEvents();

            var cwpae = new ClassWithPropertyAndEvents();
            var vtfce = new ClassWithPropertyAndEvents.ValueTypeForComplexEvent
                {
                    A = "xyz",
                    B = 10
                };
            var bcArgs = new object[] {
                "a", 15, cwpae, vtfce, ob
            };

            ob.ComplexEvent += (arg1, arg2, arg3, arg4, arg5) => {
                eventHappened++;
                eventArgs = new object[] { arg1, arg2, arg3, arg4, arg5 };
            };
            method.SetObject(ob);

            // Act
            method.TryHandleBroadcastAsync(bcArgs);
            method.TryHandleBroadcastAsync(bcArgs);
            method.TryHandleBroadcastAsync(bcArgs);
            method.TryHandleBroadcastAsync(bcArgs);

            // Assert
            Assert.IsNotNull(eventArgs);
            Assert.AreEqual(bcArgs, eventArgs);
            Assert.AreEqual(4, eventHappened);
        }
    }
}

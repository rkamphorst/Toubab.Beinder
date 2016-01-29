using System;
using NUnit.Framework;

namespace Toubab.Beinder.Tools
{
    [TestFixture]
    public class CommandSourceTest
    {
        [Test]
        public void CanExecuteQueryWithOneHandler()
        {
            bool canExecute;
            var cmds = new CommandSource((bool b) => {});

            bool handler1Called = false;
            bool handler1CanExecute = false;
            Action<CommandCanExecuteArgs> handler1 = (args) =>
            {
                handler1Called = true;      
                args.Respond(handler1CanExecute);
            };


            cmds.CanExecuteQuery += handler1;

            // Act
            canExecute = cmds.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
            Assert.IsTrue(handler1Called);

            // Arrange
            canExecute = false;
            handler1Called = false;
            handler1CanExecute = true;

            // Act
            canExecute = cmds.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
            Assert.IsTrue(handler1Called);
        }

        [Test]
        public void CanExecuteQueryWithMultipleHandlers()
        {
            var cmds = new CommandSource((bool b) => {});

            bool handler1Called = false;
            bool handler1CanExecute = false;
            Action<CommandCanExecuteArgs> handler1 = (args) =>
                {
                    handler1Called = true;      
                    args.Respond(handler1CanExecute);
                };

            bool handler2Called = false;
            bool handler2CanExecute = false;
            Action<CommandCanExecuteArgs> handler2 = (args) =>
                {
                    handler2Called = true;
                    args.Respond(handler2CanExecute);
                };

            bool handler3Called = false;
            bool handler3CanExecute = false;
            Action<CommandCanExecuteArgs> handler3 = (args) =>
                {
                    handler3Called = true;
                    args.Respond(handler3CanExecute);
                };

            bool canExecute = false;

            cmds.CanExecuteQuery += handler1;
            cmds.CanExecuteQuery += handler2;
            cmds.CanExecuteQuery += handler3;

            // Act
            canExecute = cmds.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
            Assert.IsTrue(handler1Called);
            Assert.IsTrue(handler2Called);
            Assert.IsTrue(handler3Called);

            // Arrange
            handler2CanExecute = true;
            canExecute = false;
            handler1Called = false;
            handler2Called = false;
            handler3Called = false;

            // Act
            canExecute = cmds.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
            Assert.IsTrue(handler1Called);
            Assert.IsTrue(handler2Called);
            Assert.IsTrue(handler3Called);
        }

    }
}


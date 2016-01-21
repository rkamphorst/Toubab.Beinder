namespace Toubab.Beinder.Tools
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Mock;

    [TestFixture]
    public class CommandTest
    {
        [Test]
        public void CanExecuteCallsBack()
        {
            // Arrange
            bool called = false;
            bool expectResult = false;
            var cmd = new Command(
                (o) => {},
                (o) => { called = true; return expectResult; }, 
                new object[0]
            );

            // Act
            var result = cmd.CanExecute(null);

            // Assert
            Assert.AreEqual(expectResult, result);
            Assert.IsTrue(called);

            // Arrange
            expectResult = false;
            called = false;

            // Act
            result = cmd.CanExecute(null);

            // Assert
            Assert.AreEqual(expectResult, result);
            Assert.IsTrue(called);
        }

        [Test]
        public void ExecuteCallsBack()
        {
            // Arrange
            bool called = false;
            object expectedParam  = new object();
            object parameter = null;
            var cmd = new Command(
                (o) => { called = true; parameter = o; },
                (o) => true, 
                new object[0]
            );

            // Act
            cmd.Execute(expectedParam);

            // Assert
            Assert.AreEqual(expectedParam, parameter);
            Assert.IsTrue(called);
        }

        [Test]
        public async void CanExecuteChangesToFalseDuringExecuteAndThenChangesBack()
        {
            // Arrange
            // create a command that awaits a task completion source when it executes
            var tcs = new TaskCompletionSource<int>();
            var cmd = new Command(
                async (o) => { await tcs.Task; },
                (o) => true, 
                new object[0]
            );
            bool canExecuteChanged = false;
            cmd.CanExecuteChanged += (o, e) => canExecuteChanged = true;

            // Assert: cmd.CanExecute(anything) should be true, 
            // canExecuteChanged is false
            Assert.IsFalse(canExecuteChanged);
            Assert.IsTrue(cmd.CanExecute(null));
            Assert.IsTrue(cmd.CanExecute(new object()));
            Assert.IsTrue(cmd.CanExecute("asdf"));
                
            // Act: start executing the task, *but don't await it*
            Task execTask = cmd.ExecuteAsync(null);

            // Assert: cmd.CanExecute(anything) should be false,
            // canExecuteChanged is true.
            Assert.IsTrue(canExecuteChanged);
            Assert.IsFalse(cmd.CanExecute(null));
            Assert.IsFalse(cmd.CanExecute(new object()));
            Assert.IsFalse(cmd.CanExecute("asdf"));

            // Arrange: let the command finish
            canExecuteChanged = false;
            tcs.SetResult(0);
            await execTask;

            // Assert: cm.CanExecute(anything) should be true again,
            // canExecuteChanged should also be true
            Assert.IsTrue(canExecuteChanged);
            Assert.IsTrue(cmd.CanExecute(null));
            Assert.IsTrue(cmd.CanExecute(new object()));
            Assert.IsTrue(cmd.CanExecute("asdf"));

        }

        [Test]
        public async void ExecuteIsMutuallyExclusive()
        {
            int cnt = 0;
            var cmdTcs = new TaskCompletionSource<int>();
            var procTcs = new TaskCompletionSource<int>();
            var cmd = new Command(
                async (o) => { 
                    cnt++; // increment count
                    cmdTcs.SetResult(0); // signal that count has been incremented
                    await procTcs.Task; // wait for signal that new count has been processed
                    procTcs = new TaskCompletionSource<int>(); // create a new task completion source
                },
                (o) => true, 
                new object[0]
            );

            // now, execute the command a large number of times. 
            // execution will occur asynchronously in this case because of the 
            // async callback in command (which contains an await)
            for (int i = 0; i < 100; i++) 
            {
                cmd.Execute(null);
            }

            // now, if all commands would execute simultaneously, 
            // cnt would increase uncrontrollably. 
            // here, we show that at each time we check the count, we can predict it 
            // because command executions occur sequentially.
            for (int i = 0; i < 100; i++) {
                await cmdTcs.Task; // wait for signal that count has been incremented
                Assert.AreEqual(i + 1, cnt); // check that count has been incremented
                cmdTcs = new TaskCompletionSource<int>(); // create new task completion source for command execution
                procTcs.SetResult(0); // signal command execution that count has been processed
            }
        }

        [Test]
        public void CanExecuteChangedFiresWhenNotifierFires()
        {
            // Arrange
            bool canExecute = false;
            var viewModel = new MockViewModel();
            var cmd = new Command(
                (o) => { },
                (o) => Equals(viewModel.ControlText, "banaan"), 
                new [] { viewModel }
            );
            cmd.CanExecuteChanged += (s, e) => canExecute = cmd.CanExecute(null);

            // Act
            viewModel.ControlText = "banaan";

            // Assert
            Assert.IsTrue(canExecute);

            // Act
            viewModel.ControlText = "appel";

            // Assert
            Assert.IsFalse(canExecute);

        }

        [Test]
        public void CanExecuteChangedDoesNotFireWhenNotifierFiresAndNoCanExecuteCallback()
        {
            // Arrange
            bool didCallBack = false;
            var viewModel = new MockViewModel();
            var cmd = new Command(
                (o) => { },
                null,
                new [] { viewModel }
            );
            cmd.CanExecuteChanged += (s, e) => didCallBack = true;

            // Act
            viewModel.ControlText = "banaan";

            // Assert
            Assert.IsFalse(didCallBack);

        }

    }
}


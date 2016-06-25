namespace Toubab.Beinder.Poc
{
    using System;
    using System.Runtime.Remoting.Messaging;
    using NUnit.Framework;
    using System.Threading.Tasks;

    [TestFixture]
    public class CallContextPoc
    {
        readonly string _key = Guid.NewGuid().ToString();
        readonly string _parentContext = "banaan";
        readonly string _childContext = "appel";

        [Test]
        public async void MainCall()
        {
            AssertContextIs(null);
            CallContext.LogicalSetData(_key, _parentContext);
            AssertContextIs(_parentContext);

            await NestedCall();

            AssertContextIs(_parentContext);

            CallContext.FreeNamedDataSlot(_key);

            AssertContextIs(null);
        }

        public async Task NestedCall()
        {
            await Task.Delay(TimeSpan.FromMilliseconds(10));
            AssertContextIs(_parentContext);

            CallContext.LogicalSetData(_key, _childContext);
            AssertContextIs(_childContext);

            await NestedNestedCall();
        }

        public Task NestedNestedCall() 
        {
            AssertContextIs(_childContext);
            return Task.FromResult(0);
        }

        void AssertContextIs(string context)
        {
            var data = CallContext.LogicalGetData(_key);
            Assert.AreEqual(context, data);
        }
    }
}


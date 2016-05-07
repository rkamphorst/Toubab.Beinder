namespace Toubab.Beinder
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class UsingPoc
    {
        public class Banaan
        {

            public string State { get; set; }

            class DisposableThing : IDisposable
            {
                readonly Action _callback;

                public DisposableThing(Action callback)
                {
                    _callback = callback;
                }

                public void Dispose()
                {
                    _callback();
                    Console.WriteLine("BANAAN: Dispose was called, resetting State!");
                }
            }

            public IDisposable GetDisposableToken()
            {
                Console.WriteLine("BANAAN: Creating disposable token, setting State!");
                string oldState = State;
                State = "Special State";
                return new DisposableThing(() =>
                    {
                        State = oldState;
                    });
            }
        }

        [Test]
        public void TestUsing()
        {
            var banaan = new Banaan();
            banaan.State = "State before using";
            Console.WriteLine("Before the the using, banaan.State is: {0}", banaan.State);
            using (banaan.GetDisposableToken())
            {
                Console.WriteLine("Within the using, banaan.State is: {0}", banaan.State);
            }
            Console.WriteLine("After the using, banaan.State is: {0}", banaan.State);
        }
    }
}


namespace Toubab.Beinder.Valves
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Bindables;
    using Paths;
    using Tools;

    public class Valve2 : IDisposable
    {
        readonly Fixture _fixture;
        readonly ICollection<int> _broadcastingTo;
        readonly ICollection<Valve2> _childValves;

        public Valve2(Fixture fixture)
        {
            _fixture = fixture;
            _broadcastingTo = new LinkedList<int>();
            _childValves = new List<Valve2>();
        }

        public virtual async Task InitializeAsync(int familyToActivate)
        {
            foreach (var sender in _fixture.Conduits)
                TrySetBroadcastListener(sender, CreateBroadcastListenerCallback(sender));

            var conduitToActivate = 
                Fixture.Conduits.FirstOrDefault(c => c.Family == familyToActivate);

            await BroadcastAsync(conduitToActivate, null);
        }

        public Fixture Fixture { get { return _fixture; } }

        #region Broadcast

        async Task BroadcastAsync(Conduit sender, object[] payload)
        {
            if (sender == null)
                return;

            var prop = sender.Bindable as IProperty;
            if (prop != null)
            {
                using (var attachment = sender.Attach())
                {
                    payload = new[] { prop.Value };
                }
            }

            if (payload == null)
                return;

            var handleTasks = new List<Task>();
            lock (_fixture)
            {
                foreach (var receiver in _fixture.Conduits)
                    handleTasks.Add(SendBroadcastPayloadAsync(sender, receiver, payload));
            }

            await Task.WhenAll(handleTasks);
            await UpdateChildValvesAsync(sender.Family);
        }

        async Task SendBroadcastPayloadAsync(Conduit sender, Conduit receiver, object[] payload)
        {
            if (Equals(sender.Family, receiver.Family))
                return;
            
            var eventHandler = receiver.Bindable as IEventHandler;
            if (eventHandler == null)
                return;
            
            bool areParamsCompatible =
                sender != null
                ? eventHandler.ValueTypes.AreAssignableFromTypes(sender.Bindable.ValueTypes)
                : eventHandler.ValueTypes.AreAssignableFromObjects(payload);
            if (!areParamsCompatible)
                return;
            
            using (var attachment = receiver.Attach())
            {
                if (attachment != null)
                {
                    try
                    {
                        lock (_broadcastingTo)
                        {
                            _broadcastingTo.Add(receiver.Family);
                        }
                        await eventHandler.TryHandleBroadcastAsync(payload);
                    }
                    finally
                    {
                        lock (_broadcastingTo)
                        {
                            _broadcastingTo.Remove(receiver.Family);
                        }
                    }
                }
            }
        }

        void TrySetBroadcastListener(Conduit sender, Action<object[]> listenerCallback)
        {
            var @event = sender.Bindable as IEvent;
            if (@event != null)
            {
                using (var attachment = sender.Attach())
                {
                    if (attachment != null)
                    {
                        @event.SetBroadcastListener(listenerCallback);
                    }
                }
            }
        }

        Action<object[]> CreateBroadcastListenerCallback(Conduit sender)
        {
            return async payload =>
            {
                lock (_broadcastingTo)
                {
                    if (_broadcastingTo.Contains(sender.Family))
                        return;
                }
                await BroadcastAsync(sender, payload);
            };
        }

        #endregion

        async Task UpdateChildValvesAsync(int familyToActivate)
        {
            foreach (var valve in _childValves)
                valve.Dispose();
            _childValves.Clear();

            Fixture.UpdateChildFixtures();

            // create child valves
            var tasks = new List<Task>();
            foreach (var valve in Fixture.ChildFixtures.Select(f => new Valve2(f)))
            {
                _childValves.Add(valve);
                tasks.Add(valve.InitializeAsync(familyToActivate));
            }
            await Task.WhenAll(tasks);
        }

        public void Dispose()
        {
            foreach (var conduit in _fixture.Conduits)
                TrySetBroadcastListener(conduit, null);
            
            foreach (var valve in _childValves)
                valve.Dispose();
        }

        public override string ToString()
        {
            Path path = _fixture.Conduits.First?.Value.AbsolutePath;
            return string.Format("{0}: {1}", GetType().Name, path?.ToString() ?? "..."); 
        }


    }
    
}

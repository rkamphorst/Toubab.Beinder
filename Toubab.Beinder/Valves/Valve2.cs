using System.Threading;

namespace Toubab.Beinder.Valves
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Bindables;
    using Paths;
    using Tools;

    public class Valve2 : IDisposable
    {
        readonly Fixture _fixture;

        public Valve2(Fixture fixture)
        {
            _fixture = fixture;
            SetBroadcastListeners();
        }

        public Fixture Fixture { get { return _fixture; } }

        #region Dispose

        bool _disposed = false;

        /// <summary>
        /// Disposes of this instance.
        /// </summary>
        /// <remarks>
        /// Consists of the following parts:
        /// 
        /// 1. Unregistering of broadcast event handlers from <see cref="IEvent"/> instances in the valve
        /// 2. Calling <see cref="Dispose(bool)"/> with a <c>true</c> parameter.
        /// 
        /// Just before these steps, the <see cref="Disposing"/> event is raised.
        /// 
        /// Note to subclass implementors: use <see cref="Dispose(bool)"/> to implement disposal.
        /// </remarks>
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                SetBroadcastListeners(makeNull: true);
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~Valve2()
        {
            Dispose(false);
        }

        /// <summary>
        /// Overridable dispose
        /// </summary>
        /// <remarks>
        /// Override this method to add disposal logic in a subclass.
        /// </remarks>
        /// <param name="disposing">If set to <c>true</c> disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Throw an exception if this instance is disposed.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException">If this object is disposed.</exception>
        protected void AssertNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        #endregion

        #region Broadcast

        protected virtual async Task<bool> BroadcastAsync(Conduit sender, object[] payload)
        {
            var handleTasks = new List<Task<bool>>();
            lock (_fixture)
            {
                foreach (var receiver in _fixture.Conduits)
                    handleTasks.Add(ConduitTryHandleBroadcast(sender, receiver, payload));
            }
            return (await Task.WhenAll(handleTasks)).Any();
        }

        static async Task<bool> ConduitTryHandleBroadcast(Conduit sender, Conduit receiver, object[] payload)
        {
            if (Equals(sender.Tag, receiver.Tag))
                return false;
            
            var cons = receiver.Bindable as IEventHandler;
            if (cons == null)
                return false;
            
            bool areParamsCompatible =
                sender != null
                ? cons.ValueTypes.AreAssignableFromTypes(sender.Bindable.ValueTypes)
                : cons.ValueTypes.AreAssignableFromObjects(payload);
            if (!areParamsCompatible)
                return false;
            
            using (var attachment = receiver.Attach())
            {
                if (attachment != null)
                    return await cons.TryHandleBroadcast(payload);
            }
            return false;
        }

        bool TrySetBroadcastListener(Conduit sender, bool makeNull = false)
        {
            var @event = sender.Bindable as IEvent;
            if (@event != null)
            {
                using (var attachment = sender.Attach())
                {
                    if (attachment != null)
                    {
                        if (makeNull)
                        {
                            @event.SetBroadcastListener(null);

                        }
                        else
                        {
                            @event.SetBroadcastListener(async payload => await BroadcastAsync(sender, payload));
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        void SetBroadcastListeners(bool makeNull = false)
        {
            foreach (var conduit in _fixture.Conduits)
                TrySetBroadcastListener(conduit, makeNull);
        }

        #endregion

        /// <inheritdoc/>
        public override string ToString()
        {
            Path path = _fixture.Conduits.First?.Value.AbsolutePath;
            return string.Format("{0}: {1}", GetType().Name, path?.ToString() ?? "..."); 
        }


    }
    
}

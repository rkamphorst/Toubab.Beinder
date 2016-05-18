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

    public class Valve : IDisposable, IGrouping<Path, Conduit.Attachment>
    {
        readonly LinkedList<Conduit> _conduits =
            new LinkedList<Conduit>();

        /// <summary>
        /// Add a bindable to the valve.
        /// </summary>
        public void Add(Conduit conduit)
        {
            AssertNotDisposed();
            lock (_conduits)
            {             
                using (conduit.Attach())
                {
                    var @event = conduit.Bindable as IEvent;
                    if (@event != null)
                        @event.SetBroadcastListener(payload => HandleBroadcast(@event, payload));
                }
                _conduits.AddLast(conduit);
            }
        }

        async void HandleBroadcast(IEvent sender, object[] e)
        {
            await HandleBroadcastAsync(sender, e);
        }

        protected virtual async Task HandleBroadcastAsync(IEvent sender, object[] e)
        {
            await Push(sender, e);
        }

        /// <summary>
        /// Event that is raised just before the <see cref="Valve"/> is disposed.
        /// </summary>
        public event EventHandler Disposing;

        bool _disposed;

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
            if (_disposed)
                return;
            _disposed = true;
            var evt = Disposing;

            if (evt != null)
                evt(this, EventArgs.Empty);
            Disposing = null;

            foreach (var conduit in _conduits)
            {
                using (var attachment = conduit.Attach())
                {
                    if (attachment != null)
                    {
                        var @event = attachment.Conduit.Bindable as IEvent;
                        if (@event != null)
                            @event.SetBroadcastListener(null);
                    }
                }
            }

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~Valve()
        {
            if (_disposed)
                return;
            _disposed = true;
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

        /// <inheritdoc/>
        public IEnumerator<Conduit.Attachment> GetEnumerator()
        {
            return EnumerateLiveRefsAndRemoveDefuncts().GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected virtual async Task<bool> Push(IEvent source, object[] payload)
        {
            var handleTasks = new List<Task<bool>>();
            lock (_conduits)
            {
                foreach (var conduit in _conduits)
                    handleTasks.Add(ConduitTryHandleBroadcast(source, conduit, payload));
            }
            return (await Task.WhenAll(handleTasks)).Any();
        }

        static async Task<bool> ConduitTryHandleBroadcast(IBindable source, Conduit conduit, object[] payload)
        {
            var cons = conduit.Bindable as IEventHandler;
            if (cons != null && !ReferenceEquals(source, cons))
            {
                bool areParamsCompatible =
                    source != null
                    ? cons.ValueTypes.AreAssignableFromTypes(source.ValueTypes)
                    : cons.ValueTypes.AreAssignableFromObjects(payload);
                if (areParamsCompatible)
                {
                    using (var attachment = conduit.Attach())
                    {
                        if (attachment != null)
                            return await cons.TryHandleBroadcast(payload);
                    }
                }
            }
            return false;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            Path path = _conduits.First?.Value.AbsolutePath;
            return string.Format("{0}: {1}", GetType().Name, path?.ToString() ?? "..."); 
        }

        /// <summary>
        /// Enumerate the weak references that are still alive, and remove the ones that are "dead"
        /// </summary>
        protected IEnumerable<Conduit.Attachment> EnumerateLiveRefsAndRemoveDefuncts()
        {
            var conduits = _conduits;
            if (conduits == null)
                yield break;
            var node = conduits.First;
            while (node != null)
            {
                var attachment = node.Value.Attach();
                var prev = node;
                node = node.Next;
                if (attachment == null)
                    conduits.Remove(prev);
                else
                    yield return attachment;
            }
        }

        /// <summary>
        /// Key of the grouping that this <see cref="Valve"/> represents.
        /// </summary>
        Path IGrouping<Path, Conduit.Attachment>.Key
        {
            get
            {
                return this.FirstOrDefault()?.Conduit.AbsolutePath;
            }
        }
    }
    
}

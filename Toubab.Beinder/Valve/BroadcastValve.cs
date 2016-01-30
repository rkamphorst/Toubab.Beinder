namespace Toubab.Beinder.Valve
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Bindable;
    using Tools;

    public class BroadcastValve : IDisposable, IGrouping<Path.Path, IBindable>
    {
        readonly LinkedList<WeakReference<IBindable>> _bindables = 
            new LinkedList<WeakReference<IBindable>>();

        public void Add(IBindable prop)
        {
            AssertNotDisposed();
            lock (_bindables)
            {             
                _bindables.AddLast(new WeakReference<IBindable>(prop));
                var prod = prop as IEvent;
                if (prod != null)
                    prod.Broadcast += HandleBroadcast;
            }
        }

        protected virtual void HandleBroadcast(object sender, BroadcastEventArgs e)
        {
            Push(sender, e.Payload);
        }

        /// <summary>
        /// Event that is raised just before the <see cref="BroadcastValve"/> is disposed.
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

            foreach (var bindable in EnumerateLiveRefsAndRemoveDefuncts(_bindables))
            {
                var prod = bindable as IEvent;
                if (prod != null)
                {
                    prod.Broadcast -= HandleBroadcast;
                }
            }

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~BroadcastValve()
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
        public IEnumerator<IBindable> GetEnumerator()
        {
            return EnumerateLiveRefsAndRemoveDefuncts(_bindables).GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected virtual bool Push(object source, object[] payload)
        {
            var srcBindable = source as IBindable;
            lock (_bindables)
            {
                bool valueWasBroadcast = false;
                foreach (var bnd in EnumerateLiveRefsAndRemoveDefuncts(_bindables))
                {
                    var cons = bnd as IEventHandler;
                    if (cons != null && !ReferenceEquals(source, cons))
                    {
                        bool areParamsCompatible =
                            srcBindable != null
                            ? cons.ValueTypes.AreAssignableFromTypes(srcBindable.ValueTypes)
                                : cons.ValueTypes.AreAssignableFromObjects(payload);
                        if (areParamsCompatible)
                        {
                            valueWasBroadcast |= cons.TryHandleBroadcast(payload);
                        }
                    }
                }
                return valueWasBroadcast;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var first = this.FirstOrDefault();

            return string.Format("{0}: {1}->{2} ({3})", 
                GetType().Name,
                first == null || first.Object == null ? "[?]" : first.Object.GetType().Name, 
                first == null ? "?" : first.Path, 
                first == null ? "(?)" : string.Join(",", first.ValueTypes.Select(vt => vt.Name))
            );
        }

        /// <summary>
        /// Enumerate the weak references that are still alive, and remove the ones that are "dead"
        /// </summary>
        protected static IEnumerable<T> EnumerateLiveRefsAndRemoveDefuncts<T>(LinkedList<WeakReference<T>> list) 
            where T : class
        {
            if (list == null)
                yield break;
            var node = list.First;
            while (node != null)
            {
                T target;
                while (!node.Value.TryGetTarget(out target))
                {
                    if (node.Next == null)
                    {
                        list.Remove(node);
                        node = null;
                    }
                    else
                    {
                        node = node.Next;
                        list.Remove(node.Previous);
                    }
                    if (node == null)
                        yield break;
                }
                yield return target;
                node = node.Next;
            }


        }

        /// <summary>
        /// Key of the grouping that this <see cref="BroadcastValve"/> represents.
        /// </summary>
        Path.Path IGrouping<Path.Path, IBindable>.Key
        {
            get
            {
                return this.FirstOrDefault()?.Path;
            }
        }
    }
    
}

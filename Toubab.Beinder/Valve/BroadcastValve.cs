using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Toubab.Beinder.Bindable;
using Toubab.Beinder.Tools;

namespace Toubab.Beinder.Valve
{

    public class BroadcastValve : IDisposable, IEnumerable<IBindable>
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

        #region IDisposable implementation

        public event EventHandler Disposing;

        bool _disposed;

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

        ~BroadcastValve()
        {
            if (_disposed)
                return;
            _disposed = true;
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        protected void AssertNotDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        #endregion

        #region IEnumerable implementation

        public IEnumerator<IBindable> GetEnumerator()
        {
            return EnumerateLiveRefsAndRemoveDefuncts(_bindables).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

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
    }
    
}

using System;
using System.Collections.Generic;
using System.Reflection;

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
                var prod = prop as IBindableBroadcastProducer;
                if (prod != null)
                    prod.Broadcast += HandleBroadcast;
            }
        }

        protected virtual void HandleBroadcast(object sender, BindableBroadcastEventArgs e)
        {
            Push(sender, e.Argument);
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
                var prod = bindable as IBindableBroadcastProducer;
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

        protected virtual bool Push(object source, object payload)
        {
            var srcProp = source as IBindableState;
            var payloadType = srcProp != null 
                ? srcProp.ValueType.GetTypeInfo()
                : payload == null ? null : payload.GetType().GetTypeInfo();

            lock (_bindables)
            {
                bool valueWasBroadcast = false;
                foreach (var prop in EnumerateLiveRefsAndRemoveDefuncts(_bindables))
                {
                    var cons = prop as IBindableBroadcastConsumer;
                    if (cons != null && !ReferenceEquals(source, cons))
                    {
                        var propValueType = cons.ValueType.GetTypeInfo();
                        if (payloadType == null || propValueType.IsAssignableFrom(payloadType))
                        {
                            valueWasBroadcast |= cons.TryHandleBroadcast(payload);
                        }
                    }
                }
                return valueWasBroadcast;
            }
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
                    node = node.Next;
                    list.Remove(node.Previous);
                    if (node == null)
                        yield break;
                }
                yield return target;
                node = node.Next;
            }
        }
    }
    
}
namespace Toubab.Beinder.Valves
{
    using System;
    using System.Collections.Generic;
    using Bindables;
    using Paths;

    public class Outlet
    {
        readonly WeakReference<object> _objectReference;
        readonly Path _absolutePath;
        readonly int _tag;

        IBindable _bindable;

        public Outlet(IBindable bindable, object obj, Path basePath = null, int tag = -1)
        {
            _objectReference = new WeakReference<object>(obj);
            _absolutePath = basePath == null ? bindable.Path : new Path(basePath, bindable.Path);
            _bindable = (IBindable) bindable.CloneWithoutObject();
            _tag = tag;
        }

        public int Tag { get { return _tag; } }

        public Path AbsolutePath { get { return _absolutePath; } }

        public IBindable Bindable { get { return _bindable; } }

        #region Attach, Detach

        public Attachment Attach()
        {
            object obj;
            if (_objectReference.TryGetTarget(out obj))
            {
                _bindable.SetObject(obj);
                return new Attachment(this);
            }
            return null;
        }

        public void Detach()
        {
            _bindable.SetObject(null);
        }

        public class Attachment : IDisposable
        {
            readonly Outlet _outlet;

            public Attachment(Outlet outlet)
            {
                _outlet = outlet;
            }

            public Outlet Outlet { get { return _outlet; } }

            public void Dispose()
            {
                _outlet.Detach();
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}: {1}", GetType().Name, AbsolutePath);
        }
    }
}
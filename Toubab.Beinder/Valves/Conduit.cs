namespace Toubab.Beinder.Valves
{
    using System;
    using System.Collections.Generic;
    using Bindables;
    using Paths;

    public class Conduit
    {
        readonly WeakReference<object> _objectReference;
        readonly Path _absolutePath;
        readonly int _tag;
        readonly IBindable _bindable;

        public static Conduit Create(IBindable bindable, object obj, Path basePath = null, int tag = -1)
        {
            return new Conduit(
                (IBindable)bindable.CloneWithoutObject(),
                new WeakReference<object>(obj),
                basePath == null ? bindable.Path : new Path(basePath, bindable.Path),
                tag
            );
        }

        protected Conduit(IBindable bindable,  WeakReference<object> objectReference, Path absolutePath, int tag)
        {
            _objectReference = objectReference;
            _absolutePath = absolutePath;
            _bindable = bindable;
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
            readonly Conduit _outlet;

            public Attachment(Conduit outlet)
            {
                _outlet = outlet;
            }

            public Conduit Outlet { get { return _outlet; } }

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
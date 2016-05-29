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
        readonly int _family;
        readonly IBindable _bindable;

        public static Conduit Create(IBindable bindable, object obj, Path basePath, int family)
        {
            return new Conduit(
                (IBindable)bindable.CloneWithoutObject(),
                new WeakReference<object>(obj),
                basePath == null ? bindable.Path : new Path(basePath, bindable.Path),
                family
            );
        }

        protected Conduit(IBindable bindable,  WeakReference<object> objectReference, Path absolutePath, int family)
        {
            _objectReference = objectReference;
            _absolutePath = absolutePath;
            _bindable = bindable;
            _family = family;
        }

        public int Family { get { return _family; } }

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

        public class Attachment : IDisposable
        {
            readonly Conduit _conduit;

            public Attachment(Conduit conduit)
            {
                _conduit = conduit;
            }

            public Conduit Conduit { get { return _conduit; } }

            public void Dispose()
            {
                _conduit._bindable.SetObject(null);
            }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0}: {1}", GetType().Name, AbsolutePath);
        }
    }
}
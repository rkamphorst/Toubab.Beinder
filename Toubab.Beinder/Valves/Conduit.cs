namespace Toubab.Beinder.Valves
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Bindables;
    using Paths;

    public class Conduit
    {
        readonly WeakReference<object> _objectReference;
        readonly Path _absolutePath;
        readonly Path _basePath;
        readonly int _family;
        readonly int _generation;
        readonly IBindable _bindable;

        public static Conduit Create(IBindable bindable, object obj, Path basePath, int family, int generation)
        {
            return new Conduit(
                (IBindable)bindable.CloneWithoutObject(),
                new WeakReference<object>(obj),
                basePath,
                new Path(basePath, bindable.NameSyllables),
                family, generation
            );
        }

        protected Conduit(IBindable bindable,  WeakReference<object> objectReference, Path basePath, Path absolutePath, int family, int generation)
        {
            _objectReference = objectReference;
            _absolutePath = absolutePath;
            _basePath = basePath;
            _bindable = bindable;
            _family = family;
            _generation = generation;
        }

        public int Family { get { return _family; } }

        public int Generation { get { return _generation; } }

        public Path AbsolutePath { get { return _absolutePath; } }

        public Path BasePath { get { return _absolutePath; } }

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
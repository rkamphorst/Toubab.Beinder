namespace Toubab.Beinder.Droid.Sample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Android.App;
    using Android.Views;
    using Bindables;
    using Mixins;
    using Paths;
    using Scanners;

    public class AndroidScanner : IScanner
    {
        IPathParser _pathParser = new UnderscorePathParser();

        public IPathParser PathParser
        { 
            get { return _pathParser; }
            set { _pathParser = value; }
        }

        public IEnumerable<IBindable> Scan(object obj)
        {
            var act = obj as Activity;
            if (act != null)
            {
                return ScanView(act.FindViewById(Android.Resource.Id.Content));
            }
            var frag = obj as Fragment;
            if (frag != null)
            {
                return ScanView(frag.View);
            }

            return Enumerable.Empty<IBindable>();
        }

        IEnumerable<IBindable> ScanView(View view)
        {
            var resId = view.Id;
            if (resId > 0)
            {
                Path path = _pathParser.Parse(view.Resources.GetResourceEntryName(resId));
                Type type = view.GetType();
                yield return new ViewProperty(path, resId, type);
            }

            var viewGroup = view as ViewGroup;
            if (viewGroup != null)
            {
                for (int i = 0; i < viewGroup.ChildCount; i++)
                {
                    foreach (var child in ScanView(viewGroup.GetChildAt(i)))
                    {
                        yield return child;
                    }
                }
            }
        }

        class ViewProperty : Bindable, IProperty
        {
            readonly int _resId;
            readonly Type _valueType;

            public ViewProperty(Path path, int resId, Type valueType)
                : base(path)
            {
                _resId = resId;
                _valueType = valueType;
            }

            protected ViewProperty(ViewProperty toCopy)
                : this(toCopy.Path, toCopy._resId, toCopy._valueType)
            {
            }

            public override IMixin CloneWithoutObject()
            {
                return new ViewProperty(this);
            }

            public override Type[] ValueTypes
            {
                get
                {
                    return new [] { _valueType };
                }
            }

            public event EventHandler<BroadcastEventArgs> Broadcast;

            public Task<bool> TryHandleBroadcast(object[] payload) 
            {
                return Task.FromResult(false);
            }

            public object[] Values
            {
                get
                {
                    var obj = Object;
                    var act = obj as Activity;
                    if (act != null)
                    {
                        return new object[] { act.FindViewById(_resId) };
                    }
                    var frag = obj as Fragment;
                    if (frag != null)
                    {
                        return new object[] { frag.View.FindViewById(_resId) };
                    }
                    return new object[] { null };
                }
            }
        }
    }
}


namespace Toubab.Beinder.Paths
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections;
    using Tools;

    public class Path : IFormattable
    {
        public static readonly IComparer<Path> FragmentComparer = new PathFragmentComparer();
        public static readonly IComparer<Path> SyllableComparer = new PathSyllableComparer();

        readonly Fragment _fragment;
        readonly Path _basePath;
        readonly bool _isEmpty;

        static readonly Path _emptyPath = new Path(null, null);

        public static Path Empty
        {
            get
            {
                return _emptyPath;
            }
        }

        public Path(Path basePath, Fragment fragment)
        {
            if (basePath != null && fragment == null)
            {
                _basePath = basePath._basePath;
                _fragment = basePath._fragment;
                _isEmpty = basePath._isEmpty;
            }
            else
            {
                _basePath = basePath == null || basePath._isEmpty ? null : basePath;
                _fragment = fragment;
                _isEmpty = _basePath == null && _fragment == null;
            }
        }

        public Path(Fragment fragment)
            : this(null, fragment)
        {
        }

        public bool IsEmpty
        {
            get
            { 
                return _isEmpty;
            }
        }

        public bool StartsWith(IEnumerable<string> syllables)
        {
            if (_isEmpty)
                return !syllables.Any();
            return Syllables.StartsWith(syllables);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Path;
            if (other != null)
            {
                return FragmentComparer.Compare(this, other) == 0;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                foreach (var frag in Syllables)
                    hash = hash * 23 + frag.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return ToString(Fragment.FormatSlash, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Join(".", Fragments.Select(f => f.ToString(format, formatProvider)));
        }


        public IEnumerable<string> Syllables
        {
            get
            {
                foreach (var frag in Fragments)
                    foreach (var syl in frag.Syllables)
                        yield return syl;
            }
        }

        public IEnumerable<Fragment> Fragments
        {
            get
            {
                if (_basePath != null)
                    foreach (var frag in _basePath.Fragments)
                        yield return frag;
                if (_fragment != null)
                    yield return _fragment;
            }
        }


        class PathFragmentComparer : IComparer<Path>
        {
            #region IComparer implementation

            public int Compare(Path x, Path y)
            {
                if (ReferenceEquals(x, y))
                    return 0;
                if (x._isEmpty && !y._isEmpty)
                    return -1;
                if (!x.IsEmpty && y.IsEmpty)
                    return 1;
                return x.Fragments.SequenceCompareTo(y.Fragments);
            }

            #endregion
        }

        class PathSyllableComparer : IComparer<Path>
        {
            #region IComparer implementation

            public int Compare(Path x, Path y)
            {
                if (ReferenceEquals(x, y))
                    return 0;
                if (x._isEmpty && !y._isEmpty)
                    return -1;
                if (!x.IsEmpty && y.IsEmpty)
                    return 1;
                return x.Syllables.SequenceCompareTo(y.Syllables);
            }

            #endregion
        }

    }

}

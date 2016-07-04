namespace Toubab.Beinder.Paths
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections;
    using Tools;

    public class Path : IComparable<Path>, IFormattable
    {
        const string FORMAT_CAMELCASE = "C";
        const string FORMAT_PASCALCASE = "P";
        const string FORMAT_UNDERSCORE = "_";
        const string FORMAT_SLASH = "/";

        readonly Syllables _syllables;
        readonly Path _basePath;
        readonly bool _isEmpty;

        static readonly Path _emptyPath = new Path();

        public static Path Empty
        {
            get
            {
                return _emptyPath;
            }
        }

        Path()
        {
            _basePath = null;
            _syllables = null;
            _isEmpty = true;
        }

        public Path(Path basePath, Syllables syllables)
        {
            _basePath = basePath;
            _syllables = syllables;
        }

        public Path(Syllables syllables)
            : this(null, syllables)
        {
        }

        public bool IsEmpty
        {
            get
            { 
                return _isEmpty;
            }
        }

        public bool StartsWith(Path other)
        {
            if (_isEmpty)
                return other._isEmpty;
            return Syllables.StartsWith(other.Syllables);
        }

        public int CompareTo(Path other)
        {
            return Compare(this, other);
        }

        public static int Compare(Path path1, Path path2)
        {
            if (ReferenceEquals(path1, path2))
                return 0;
            if (path1._isEmpty && !path2._isEmpty)
                return -1;
            if (!path1.IsEmpty && path2.IsEmpty)
                return 1;
            return path1.Syllables.SequenceCompareTo(path2.Syllables);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Path;
            if (other != null)
            {
                return CompareTo(other) == 0;
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
            return ToString(FORMAT_SLASH, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            switch (format)
            {
                case FORMAT_CAMELCASE:
                    return string.Join(".",
                        Paths.Select(
                            p => string.Join(string.Empty,
                                p.Syllables.Select((f, i) => i == 0 ? f.ToLowerInvariant() : f.Substring(0, 1).ToUpperInvariant() + f.Substring(1).ToLowerInvariant())
                            )
                        )
                    );
                case FORMAT_UNDERSCORE:
                    return string.Join(".", Paths.Select(p => string.Join("_", p.Syllables.Select(f => f.ToLowerInvariant()))));
                case FORMAT_SLASH:
                    return string.Join(".", Paths.Select(p => string.Join("/", p.Syllables.Select(f => f.ToLowerInvariant()))));
                default:
                    return string.Join(".",
                        Paths.Select(
                            p => string.Join(string.Empty,
                                p.Syllables.Select(f => f.Substring(0, 1).ToUpperInvariant() + f.Substring(1).ToLowerInvariant())
                            )
                        )
                    );
            }
        }

        public int FragmentCount
        {
            get
            {
                return Paths.Sum(p => p._syllables.Count);
            }
        }

        public int PathCount
        {
            get
            {
                return Paths.Count();
            }
        }

        public IEnumerable<string> Syllables
        {
            get { return new EnumerableSyllables(this); }
        }

        public IEnumerable<Path> Paths
        {
            get { return new EnumerablePaths(this); }
        }

        class EnumerableSyllables : IEnumerable<string>
        {
            readonly Path _path;

            public EnumerableSyllables(Path path)
            {
                _path = path;
            }

            public IEnumerator<string> GetEnumerator()
            {
                var syllables = _path._syllables;
                var basePath = _path._basePath;
                if (basePath != null)
                    foreach (var syllable in new EnumerableSyllables(basePath))
                        yield return syllable;
                foreach (var syllable in syllables)
                    yield return syllable;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        class EnumerablePaths : IEnumerable<Path>
        {
            readonly Path _path;

            public EnumerablePaths(Path path)
            {
                _path = path;
            }

            public IEnumerator<Path> GetEnumerator()
            {
                var basePath = _path._basePath;

                if (basePath != null)
                    foreach (var partpath in new EnumerablePaths(basePath))
                        yield return partpath;
                
                yield return _path;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }


    }

}

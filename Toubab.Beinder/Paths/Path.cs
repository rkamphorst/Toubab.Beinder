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
        const string FORMAT_UNDERSCORE = "U";
        const string FORMAT_SLASH = "/";

        readonly string[] _fragments;
        readonly Path[] _paths;

        public Path(params Path[] paths)
        {
            _paths = paths.SelectMany(p => p.Paths).ToArray();
        }

        public Path(params string[] fragments)
        {
            _fragments = fragments;
        }

        public bool StartsWith(Path other) 
        {
            return Fragments.StartsWith(other.Fragments);
        }

        public Path RelativeTo(Path other)
        {
            var frags = Fragments.SkipIfStartsWith(other.Fragments);
            return frags == null ? null : new Path(frags.ToArray());
        }

        public int CompareTo(Path other)
        {
            return Compare(this, other);
        }

        public static int Compare(Path path1, Path path2)
        {
            return path1.Fragments.SequenceCompareTo(path2.Fragments);
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
                foreach (var frag in Fragments)
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
                                p.Fragments.Select((f, i) => i == 0 ? f.ToLowerInvariant() : f.Substring(0, 1).ToUpperInvariant() + f.Substring(1).ToLowerInvariant())
                                )
                            )
                         );
                case FORMAT_UNDERSCORE:
                    return string.Join(".", Paths.Select(p => string.Join("_", p.Fragments.Select(f => f.ToLowerInvariant()))));
                case FORMAT_SLASH:
                    return string.Join(".", Paths.Select(p => string.Join("/", p.Fragments.Select(f => f.ToLowerInvariant()))));
                case FORMAT_PASCALCASE:
                default:
                    return string.Join(".",
                        Paths.Select(
                            p => string.Join(string.Empty,
                                p.Fragments.Select(f => f.Substring(0, 1).ToUpperInvariant() + f.Substring(1).ToLowerInvariant())
                                )
                            )
                         );
            }
        }

        public int FragmentCount
        {
            get
            {
                return Paths.Sum(p => p._fragments.Length);
            }
        }

        public int PathCount
        {
            get
            {
                return Paths.Count();
            }
        }

        public IEnumerable<string> Fragments
        {
            get { return new EnumerableFragments(this); }
        }

        public IEnumerable<Path> Paths
        {
            get { return new EnumerablePaths(this); }
        }

        class EnumerableFragments : IEnumerable<string>
        {
            readonly Path _path;

            public EnumerableFragments(Path path)
            {
                _path = path;
            }

            public IEnumerator<string> GetEnumerator()
            {
                var frags = _path._fragments;
                var paths = _path._paths;
                if (frags != null)
                    foreach (var frag in frags)
                        yield return frag;
                else if (paths != null)
                    foreach (var path in paths)
                        foreach (var frag in new EnumerableFragments(path))
                            yield return frag;
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
                var paths = _path._paths;
                if (paths != null)
                    foreach (var path in paths)
                        foreach (var partpath in new EnumerablePaths(path))
                            yield return partpath;
                else
                    yield return _path;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }


    }

}

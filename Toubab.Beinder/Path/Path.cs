namespace Toubab.Beinder.Path
{
    using System;
    using System.Linq;

    public class Path : IComparable<Path>
    {
        readonly string[] _fragments;

        public Path(params Path[] paths)
        {
            _fragments = paths.SelectMany(p => p._fragments).ToArray();
        }

        public Path(params string[] fragments)
        {
            _fragments = fragments;
        }

        public bool MatchesStartOf(Path other)
        {
            return CalculateMatchSize(this, other) == _fragments.Length;
        }

        public Path RelativeTo(Path other)
        {
            if (other.MatchesStartOf(this))
            {
                return new Path(_fragments.Skip(other._fragments.Length).ToArray());
            }
            else
            {
                return null;
            }
        }

        public int CompareTo(Path other)
        {
            return Compare(this, other);
        }

        public static int Compare(Path path1, Path path2)
        {
            int len1 = path1._fragments.Length;
            int len2 = path2._fragments.Length;
            int len =
                len1 == len2 ? len1 : len1 < len2 ? len1 : len2;
            for (int i = 0; i < len; i++)
            {
                int cmp = string.Compare(path1._fragments[i], path2._fragments[i], StringComparison.Ordinal);
                if (cmp != 0)
                    return cmp;
            }
            if (len1 > len2)
                return 1;
            if (len2 > len1)
                return -1;
            return 0;
        }

        static int CalculateMatchSize(Path path1, Path path2)
        {
            // fIdx: fragment index
            int fIdx;
            var frags1 = path1._fragments;
            var frags2 = path2._fragments;
            for (fIdx = 0; fIdx < frags1.Length && fIdx < frags2.Length; fIdx++)
            {
                if (!Equals(frags1[fIdx], frags2[fIdx]))
                    break;
            }
            return fIdx;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Path;
            if (other != null)
            {
                return this.CompareTo(other) == 0;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + _fragments.Length.GetHashCode();
                for (int i = 0; i < _fragments.Length; i++)
                {
                    hash = hash * 23 + _fragments[i].GetHashCode();
                }
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Join("/", _fragments);
        }

        public static Path Add(Path path1, Path path2)
        {
            return new Path(path1, path2);
        }

        public static Path operator +(Path path1, Path path2)
        {
            return Add(path1, path2);
        }

        public static implicit operator Path(string[] fragments)
        {
            return new Path(fragments);
        }

        public static implicit operator Path(string fragment)
        {
            return new Path(fragment);
        }
    }

}

namespace Toubab.Beinder.Paths
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Tools;

    public class Fragment : IComparable<Fragment>, IFormattable
    {
        public const string FormatCamelCase = "C";
        public const string FormatPascalCase = "P";
        public const string FormatUnderscore = "_";
        public const string FormatSlash = "/";
        
        readonly string[] _syllables;

        public Fragment(IEnumerable<string> syllables)
        {
            _syllables = syllables.Select(s => s.ToLowerInvariant()).ToArray();
            if (_syllables.Length == 0)
                throw new ArgumentException("Zero syllables not allowed");
        }

        public Fragment(params string[] syllables) 
            : this((IEnumerable<string>)syllables) { }

        public IEnumerator<string> GetEnumerator()
        {
            foreach (var syllable in _syllables)
                yield return syllable;
        }

        public string[] Syllables { get { return _syllables; } }

        public int CompareTo(Fragment other)
        {
            if (ReferenceEquals(this, other))
                return 0;
            return _syllables.SequenceCompareTo(other._syllables);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Fragment;
            return other != null && CompareTo(other) == 0;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                foreach (var syl in _syllables)
                    hash = hash * 23 + syl.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return ToString(FormatSlash, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            switch (format)
            {
                case FormatCamelCase:
                    return string.Join(string.Empty,
                        _syllables.Select((s, i) => i == 0 ? s.ToLowerInvariant() : s.Substring(0, 1).ToUpperInvariant() + s.Substring(1).ToLowerInvariant())
                    );
                case FormatUnderscore:
                    return string.Join("_", _syllables.Select(s => s.ToLowerInvariant()));
                case FormatSlash:
                    return string.Join("/", _syllables.Select(s => s.ToLowerInvariant()));
                default:
                    return string.Join(string.Empty,
                        _syllables.Select(s => s.Substring(0, 1).ToUpperInvariant() + s.Substring(1).ToLowerInvariant())
                    );
            }
        }
    }
}


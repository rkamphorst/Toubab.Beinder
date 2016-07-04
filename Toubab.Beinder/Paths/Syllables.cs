using System;
using System.Collections.Generic;
using System.Linq;
using Toubab.Beinder.Tools;
using System.Collections;

namespace Toubab.Beinder.Paths
{
    public class Syllables : IEnumerable<string>, IComparable<Syllables>, IFormattable
    {
        const string FORMAT_CAMELCASE = "C";
        const string FORMAT_PASCALCASE = "P";
        const string FORMAT_UNDERSCORE = "_";
        const string FORMAT_SLASH = "/";
        
        readonly string[] _syllables;

        public Syllables(IEnumerable<string> syllables)
        {
            _syllables = syllables.Select(s => s.ToLowerInvariant()).ToArray();
            if (_syllables.Length == 0)
                throw new ArgumentException("Zero syllables not allowed");
        }

        public Syllables(params string[] syllables) 
            : this((IEnumerable<string>)syllables) { }

        public int Count 
        {
            get { return _syllables.Length; }
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach (var syllable in _syllables)
                yield return syllable;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int CompareTo(Syllables other)
        {
            if (ReferenceEquals(this, other))
                return 0;
            return _syllables.SequenceCompareTo(other._syllables);
        }

        public override bool Equals(object obj)
        {
            var other = obj as Syllables;
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
            return ToString(FORMAT_SLASH, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            switch (format)
            {
                case FORMAT_CAMELCASE:
                    return string.Join(string.Empty,
                        _syllables.Select((s, i) => i == 0 ? s.ToLowerInvariant() : s.Substring(0, 1).ToUpperInvariant() + s.Substring(1).ToLowerInvariant())
                    );
                case FORMAT_UNDERSCORE:
                    return string.Join("_", _syllables.Select(s => s.ToLowerInvariant()));
                case FORMAT_SLASH:
                    return string.Join("/", _syllables.Select(s => s.ToLowerInvariant()));
                default:
                    return string.Join(string.Empty,
                        _syllables.Select(s => s.Substring(0, 1).ToUpperInvariant() + s.Substring(1).ToLowerInvariant())
                    );
            }
        }
    }
}


using System;
using System.Collections.Generic;

namespace Toubab.Beinder
{
    public class NamespaceAffinityComparer : Comparer<string>
    {
        readonly string[] _relativeNs;

        public NamespaceAffinityComparer(string relativeNs)
        {
            _relativeNs = relativeNs.Split('.');
        }

        public override int Compare(string x, string y)
        {
            return CalculateAffinity(x) - CalculateAffinity(y);
        }

        int CalculateAffinity(string ns)
        {
            var parts = ns.Split('.');
            int result = 0;
            while (
                parts.Length > result &&
                _relativeNs.Length > result &&
                Equals(parts[result], _relativeNs[result]))
                result++;
            return result;
        }
    }


}


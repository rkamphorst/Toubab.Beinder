using System;
using System.Collections.Generic;

namespace Beinder
{
    public class NamespacePreferenceComparer : Comparer<string>
    {

        readonly NamespaceAffinityComparer _affinityComparer;
        readonly NamespaceSpecializationComparer _specializationComparer;

        public NamespacePreferenceComparer(string relativeNs)
        {
            _affinityComparer = new NamespaceAffinityComparer(relativeNs);
            _specializationComparer = new NamespaceSpecializationComparer();
        }

        public override int Compare(string x, string y)
        {
            int cmp = _affinityComparer.Compare(x, y);
            return cmp != 0 ? cmp : _specializationComparer.Compare(x, y);
            
        }
    }

}

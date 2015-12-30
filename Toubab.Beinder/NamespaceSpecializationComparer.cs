using System;
using System.Collections.Generic;

namespace Toubab.Beinder
{

    public class NamespaceSpecializationComparer : Comparer<string>
    {
        public override int Compare(string x, string y)
        {
            string[] xparts = x.Split('.');
            string[] yparts = y.Split('.');
            int i;
            for (i = 0; 
                 i < xparts.Length && i < yparts.Length &&
                    Equals(xparts[i], yparts[i]);
                 i++)
            {
            }

            if (i < xparts.Length && i == yparts.Length)
                return 1;
            if (i < yparts.Length && i == xparts.Length)
                return -1;
            return 0;
        }

    }

}

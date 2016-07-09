using System;
using System.Collections.Generic;

namespace Toubab.Beinder.Tools
{
    public class LambdaComparer<T> : IComparer<T>
    {
        readonly Func<T,T,int> _compare;

        public LambdaComparer(Func<T, T, int> compare)
        {
            _compare = compare;
        }

        public int Compare(T x, T y)
        {
            return _compare(x, y);
        }
    }
}


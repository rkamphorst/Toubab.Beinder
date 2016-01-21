using System;
using System.Collections.Generic;
using System.Linq;

namespace Toubab.Beinder.Tools
{
    
    public static class EnumerableUtilities
    {
        /// <summary>
        /// Transpose a nested enumerable, and use padding to fill the holes
        /// </summary>
        /// <remarks>
        /// Say you have a list of enumerables, for example (each line is an enumerable)
        ///
        /// a, b, c, d
        /// e, f, g
        /// h, i, j, k, l, m
        /// n, o 
        /// 
        /// This method transposes it into (where P is the padding):
        /// 
        /// a, e, h, n
        /// b, f, i, o
        /// c, g, j, P
        /// d, P, k, P
        /// P, P, l, P
        /// P, P, m, P
        /// 
        /// Every enumerable *r* in the result contains the elements of the original enumerables
        /// at position *x*, where *x* is the position of *r* in the result. If any of the original 
        /// enumerables does not have a position *x* (i.e., it has a <c>Count()</c> less than *x + 1*),
        /// the <paramref name="padding"/> is inserted at that position in *r*.
        /// </remarks>
        public static IEnumerable<IEnumerable<T>> TransposeWithPadding<T>(this IEnumerable<IEnumerable<T>> source, T padding = default(T))
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var enumerators = source.Select(x => x.GetEnumerator()).ToArray();
            try
            {
                bool oneHasMoved;
                do
                {
                    oneHasMoved = false;
                    var result = enumerators.Select(x =>
                        {
                            var moved = x.MoveNext();
                            oneHasMoved |= moved;
                            return moved ? x.Current : padding;
                        }).ToArray();
                    if (oneHasMoved)
                        yield return result;
                } while (oneHasMoved);
            }
            finally
            {
                foreach (var enumerator in enumerators)
                    enumerator.Dispose();
            }
        }

        /// <summary>
        /// Get the first value of an enumerable, and if enumerable is empty,
        /// the given default value.
        /// </summary>
        public static TSource FirstOr<TSource>(
            this IEnumerable<TSource> source, 
            TSource defaultValue
        )
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            using (IEnumerator<TSource> iterator = source.GetEnumerator())
            {
                return iterator.MoveNext() ? iterator.Current : defaultValue;
            }
        }
            
    }
}


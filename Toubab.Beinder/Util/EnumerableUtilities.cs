using System;
using System.Collections.Generic;
using System.Linq;

namespace Toubab.Beinder.Util
{
    public static class EnumerableUtilities
    {
        public static IEnumerable<IEnumerable<T>> TransposeWithPadding<T>(this IEnumerable<IEnumerable<T>> source, T padding = default(T))
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var enumerators = source.Select(x => x.GetEnumerator()).ToArray();
            try
            {
                bool oneHasMoved;
                do {
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


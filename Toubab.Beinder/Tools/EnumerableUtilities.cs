namespace Toubab.Beinder.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Utility (extension) methods for <see cref="IEnumerable{T}"/> instances.
    /// </summary>
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

        public static LinkedList<T> MergeIntoSortedLinkedList<T, U>(this IEnumerable<T> enumerableToMerge, LinkedList<T> sortedListToMergeInto, Func<T, U> keySelector)
        {
            var comparer = Comparer<U>.Default;
            var sortedEnumerableToMerge = enumerableToMerge.OrderBy(keySelector, comparer);

            if (sortedListToMergeInto.Count == 0)
            {
                foreach (var b in sortedEnumerableToMerge)
                    sortedListToMergeInto.AddLast(b);
            }
            else
            {
                var sortedListToMerge = new LinkedList<T>(sortedEnumerableToMerge);
                LinkedListNode<T> nodeToMerge, mergeBeforeNode;
                mergeBeforeNode = sortedListToMergeInto.First;
                while ((nodeToMerge = sortedListToMerge.First) != null)
                {
                    sortedListToMerge.RemoveFirst();

                    while (
                        mergeBeforeNode != null
                        && comparer.Compare(keySelector(nodeToMerge.Value), keySelector(mergeBeforeNode.Value)) > 0)
                        mergeBeforeNode = mergeBeforeNode.Next;

                    if (mergeBeforeNode != null)
                        sortedListToMergeInto.AddBefore(mergeBeforeNode, nodeToMerge);
                    else
                        sortedListToMergeInto.AddLast(nodeToMerge);
                }
            }

            return sortedListToMergeInto;
        }

        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> self, params T[] items)
        {
            foreach (var item in items)
                yield return item;
            foreach (var item in self)
                yield return item;
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> self, params T[] items)
        {
            foreach (var item in self)
                yield return item;
            foreach (var item in items)
                yield return item;
        }

        public static bool StartsWith<T>(this IEnumerable<T> self, IEnumerable<T> other, Func<T, T, bool> equals = null)
        {
            IEnumerable<T> rest = null;
            try
            {
                rest = SkipIfStartsWith(self, other, equals);
                return rest != null;
            }
            finally
            {
                var disp = rest as IDisposable;
                if (disp != null) disp.Dispose();
            }
        }

        public static IEnumerable<T> SkipIfStartsWith<T>(this IEnumerable<T> self, IEnumerable<T> other, Func<T, T, bool> equals = null)
        {
            var selfEnu = self.GetEnumerator();
            bool returnNull = false;
            foreach (var item in other)
            {
                if (!selfEnu.MoveNext())
                {
                    returnNull = true;
                    break;
                }

                if (equals != null)
                {
                    if (!equals(item, selfEnu.Current))
                    {
                        returnNull = true;
                        break;
                    }
                }
                else
                {
                    if (!Equals(item, selfEnu.Current))
                    {
                        returnNull = true;
                        break;
                    }
                }
            }

            if (returnNull)
            {
                var disp = selfEnu as IDisposable;
                if (disp != null)
                    disp.Dispose();
                return null;
            }
            else
            {
                return selfEnu.ToEnumerable(false);
            }
        }

        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> self, bool yieldCurrent)
        {
            try
            {
                if (yieldCurrent)
                    yield return self.Current;
                while (self.MoveNext())
                    yield return self.Current;
            }
            finally
            {
                var disp = self as IDisposable;
                if (disp != null)
                    disp.Dispose();
            }
        }

        public static int SequenceCompareTo<T>(this IEnumerable<T> self, IEnumerable<T> other, Func<T, T, int> compare = null)
        {
            if (compare == null)
            {
                var cmp = Comparer<T>.Default;
                compare = (a, b) => cmp.Compare(a, b);
            }

            var selfEnu = self.GetEnumerator();
            var otherEnu = other.GetEnumerator();
            try
            {
                bool selfHasCurrent, otherHasCurrent;
                while (
                    (selfHasCurrent = selfEnu.MoveNext()) |
                    (otherHasCurrent = otherEnu.MoveNext()))
                {
                    if (selfHasCurrent && !otherHasCurrent)
                        return 1;
                    if (!selfHasCurrent && otherHasCurrent)
                        return -1;
                    var cmpResult = compare(selfEnu.Current, otherEnu.Current);
                    if (cmpResult != 0)
                        return cmpResult;
                }

                return 0;
            }
            finally
            {
                var disp = selfEnu as IDisposable;
                if (disp != null) disp.Dispose();
                disp = otherEnu as IDisposable;
                if (disp != null) disp.Dispose();
            }
        }

    }
}


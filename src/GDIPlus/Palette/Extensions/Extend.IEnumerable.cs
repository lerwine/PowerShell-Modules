using System;
using System.Collections.Generic;
using System.Linq;

namespace Erwine.Leonard.T.GDIPlus.Palette.Extensions
{
    public static partial class Extend
    {
        /// <summary>
        /// Selects distinct items by a given selector.
        /// </summary>
        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> items, Func<T, TKey> selector)
        {
            HashSet<TKey> keys = new HashSet<TKey>();
            return items.Where(item => keys.Add(selector(item)));
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            return source.MaxBy(selector, Comparer<TKey>.Default);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static T MaxBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector, IComparer<TKey> comparer)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            using (IEnumerator<T> sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext()) throw new InvalidOperationException("Sequence was empty");
                
                T max = sourceIterator.Current;
                TKey maxKey = selector.Invoke(max);

                while (sourceIterator.MoveNext())
                {
                    T candidate = sourceIterator.Current;
                    TKey candidateProjected = selector.Invoke(candidate);

                    if (comparer.Compare(candidateProjected, maxKey) > 0)
                    {
                        max = candidate;
                        maxKey = candidateProjected;
                    }
                }

                return max;
            }
        }
    }
}

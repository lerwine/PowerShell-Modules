#if PSLEGACY
using System;
using System.Collections.Generic;

namespace XmlUtilityCLR
{
    public static class LinqEmul
    {
        public class TargetContext<TTarget>
        {
            private TTarget _target;

            public TargetContext(TTarget obj) { this._target = obj; }

            public bool ReferenceEqualsPredicate<TItem>(TItem item)
            {
                if ((object)(this._target) == null)
                    return (object)item == null;

                return (object)item != null && Object.ReferenceEquals(this._target, item);
            }

            public bool NotReferenceEqualsPredicate<TItem>(TItem item)
            {
                if ((object)(this._target) == null)
                    return (object)item != null;

                return (object)item == null || !Object.ReferenceEquals(this._target, item);
            }
        }

        public static bool AnyObjectReferenceEquals<T1, T2>(IEnumerable<T1> collection, T2 item)
        {
            TargetContext<T2> context = new TargetContext<T2>(item);
            return Any<T1>(collection, context.ReferenceEqualsPredicate<T1>);
        }

        public static IEnumerable<T1> SkipWhileNotObjectReferenceEquals<T1, T2>(IEnumerable<T1> collection, T2 item)
        {
            TargetContext<T2> context = new TargetContext<T2>(item);
            return SkipWhile<T1>(collection, context.NotReferenceEqualsPredicate<T1>);
        }

        public static bool Any<T>(IEnumerable<T> collection, Predicate<T> predicate)
        {
            foreach (T item in collection)
            {
                if (predicate(item))
                    return true;
            }

            return false;
        }

        public static int Count<T>(IEnumerable<T> collection)
        {
            int count = 0;
            using (IEnumerator<T> enumerator = collection.GetEnumerator())
            {
                while (enumerator.MoveNext())
                    count++;
            }
            return count;
        }

        public static IEnumerable<T> SkipWhile<T>(IEnumerable<T> collection, Predicate<T> predicate)
        {
            bool hasCurrent = true;
            using (IEnumerator<T> enumerator = collection.GetEnumerator())
            {
                while (hasCurrent)
                {
                    hasCurrent = enumerator.MoveNext();
                    if (hasCurrent && !predicate(enumerator.Current))
                        break;
                }
                if (hasCurrent)
                {
                    do
                    {
                        yield return enumerator.Current;
                    } while (enumerator.MoveNext());
                }
            }
        }
    }
}
#endif
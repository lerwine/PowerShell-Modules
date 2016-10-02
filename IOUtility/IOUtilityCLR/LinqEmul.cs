#if PSLEGACY
using System;
using System.Collections.Generic;
using System.Text;

namespace IOUtilityCLR
{
    public class LinqEmul
    {
		public static bool ObjectIsNotNullPredicate<T>(T value) where T : class { return value != null; }
		
		public static bool StringNotNullOrEmpty(string value) { return !String.IsNullOrEmpty(value); }
		
		public static T[] ToArray<T>(IEnumerable<T> collection)
		{
            if (collection == null)
                throw new ArgumentNullException("collection");

			List<T> list = new List<T>(collection);
			return list.ToArray();
		}
		
		#region Any
		
        private static bool _Any<T>(IEnumerable<T> collection, Func<T, bool> predicate)
        {
            foreach (T item in collection)
            {
                if (predicate == null || predicate(item))
                    return true;
            }

            return false;
        }

        public static bool Any<T>(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            return LinqEmul._Any<T>(collection, null);
        }

        public static bool Any<T>(IEnumerable<T> collection, Func<T, bool> predicate)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return LinqEmul._Any<T>(collection, null);
        }

		#endregion
		
		#region Select
		
        private static IEnumerable<TResult> _Select<TSource, TState, TResult>(IEnumerator<TSource> enumerator, TState state, Func<TSource, TState, TResult> selector)
        {
            while (enumerator.MoveNext())
				yield return selector(enumerator.Current, state);
        }

        private static IEnumerable<TResult> _Select<TSource, TResult>(IEnumerator<TSource> enumerator, Func<TSource, TResult> selector)
        {
            while (enumerator.MoveNext())
				yield return selector(enumerator.Current);
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(IEnumerator<TSource> enumerator, Func<TSource, TResult> selector)
        {
            if (enumerator == null)
                throw new ArgumentNullException("enumerator");

            if (selector == null)
                throw new ArgumentNullException("selector");

			return LinqEmul._Select<TSource, TResult>(enumerator, selector);
		}

        private static IEnumerable<TResult> _Select<TSource, TState, TResult>(IEnumerable<TSource> collection, TState state, Func<TSource, TState, TResult> selector)
        {
            IEnumerator<TSource> enumerator = collection.GetEnumerator();
            try
            {
                return LinqEmul._Select<TSource, TState, TResult>(enumerator, state, selector);
            }
            catch
            {
                throw;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        private static IEnumerable<TResult> _Select<TSource, TResult>(IEnumerable<TSource> collection, Func<TSource, TResult> selector)
        {
            IEnumerator<TSource> enumerator = collection.GetEnumerator();
            try
            {
                return LinqEmul._Select<TSource, TResult>(enumerator, selector);
            }
            catch
            {
                throw;
            }
            finally
            {
                enumerator.Dispose();
            }
        }
		
        public static IEnumerable<TResult> Select<TSource, TState, TResult>(IEnumerable<TSource> collection, TState state, Func<TSource, TState, TResult> selector)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (selector == null)
                throw new ArgumentNullException("selector");

            return LinqEmul._Select<TSource, TState, TResult>(collection, state, selector);
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(IEnumerable<TSource> collection, Func<TSource, TResult> selector)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (selector == null)
                throw new ArgumentNullException("selector");

            return LinqEmul._Select<TSource, TResult>(collection, selector);
        }

		#endregion
		
		#region Where
		
        private static IEnumerable<T> _Where<T>(IEnumerator<T> enumerator, Func<T, bool> predicate)
        {
            while (enumerator.MoveNext())
            {
                if (predicate(enumerator.Current))
                    yield return enumerator.Current;
            }
        }

        public static IEnumerable<T> Where<T>(IEnumerator<T> enumerator, Func<T, bool> predicate)
        {
            if (enumerator == null)
                throw new ArgumentNullException("enumerator");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

			return LinqEmul._Where<T>(enumerator, predicate);
		}

        private static IEnumerable<T> _Where<T>(IEnumerable<T> collection, Func<T, bool> predicate)
        {
            IEnumerator<T> enumerator = collection.GetEnumerator();
            try
            {
                return LinqEmul._Where<T>(enumerator, predicate);
            }
            catch
            {
                throw;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        public static IEnumerable<T> Where<T>(IEnumerable<T> collection, Func<T, bool> predicate)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return LinqEmul._Where<T>(collection, predicate);
        }

		#endregion
		
		#region SkipWhile
		
        private static IEnumerable<T> _SkipWhile<T>(IEnumerator<T> enumerator, Func<T, bool> predicate)
        {
            bool matched = false;
            while (enumerator.MoveNext())
            {
                if (!predicate(enumerator.Current))
                {
                    matched = true;
                    yield return enumerator.Current;
                    break;
                }
            }
            if (matched)
            {
                while (enumerator.MoveNext())
                    yield return enumerator.Current;
            }
        }

        public static IEnumerable<T> SkipWhile<T>(IEnumerator<T> enumerator, Func<T, bool> predicate)
        {
            if (enumerator == null)
                throw new ArgumentNullException("enumerator");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

			return LinqEmul._SkipWhile<T>(enumerator, predicate);
        }

        private static IEnumerable<T> _SkipWhile<T>(IEnumerable<T> collection, Func<T, bool> predicate)
        {
            IEnumerator<T> enumerator = collection.GetEnumerator();
            try
            {
                return LinqEmul._SkipWhile<T>(enumerator, predicate);
            }
            catch
            {
                throw;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

        public static IEnumerable<T> SkipWhile<T>(IEnumerable<T> collection, Func<T, bool> predicate)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return LinqEmul._SkipWhile<T>(collection, predicate);
        }

		#endregion
		
		#region Count
		
        private static int _Count<T>(IEnumerator<T> enumerator, Func<T, bool> predicate)
        {
            int count = 0;
            if (predicate == null)
            {
                while (enumerator.MoveNext())
                    count++;
            }
            else
            {
                while (enumerator.MoveNext())
                {
                    if (predicate(enumerator.Current))
                        count++;
                }
            }

            return count;
        }

        public static int Count<T>(IEnumerator<T> enumerator, Func<T, bool> predicate)
        {
            if (enumerator == null)
                throw new ArgumentNullException("enumerator");

            return LinqEmul._Count<T>(enumerator, predicate);
        }

        public static int Count<T>(IEnumerable<T> collection, Func<T, bool> predicate)
        {
            IEnumerator<T> enumerator = collection.GetEnumerator();
            try
            {
                return LinqEmul._Count<T>(enumerator, predicate);
            }
            catch
            {
                throw;
            }
            finally
            {
                enumerator.Dispose();
            }
        }

		#endregion
	}
    
	public class Tuple<T1, T2>
	{
		private T1 _item1;
		private T2 _item2;
		
		public T1 Item1 { get { return _item1; } }
		public T2 Item2 { get { return _item2; } }
		
		public Tuple(T1 item1, T2 item2)
		{
			_item1 = item1;
			_item2 = item2;
		}
	}
	
	public delegate void Action();
	public delegate void Action<TArg>(TArg arg);
	public delegate void Action<TArg1, TArg2>(TArg1 arg1, TArg2 arg2);
	public delegate void Action<TArg1, TArg2, TArg3>(TArg1 arg1, TArg2 arg2, TArg3 arg3);
    public delegate TResult Func<TResult>();
    public delegate TResult Func<TArg, TResult>(TArg arg);
    public delegate TResult Func<TArg1, TArg2, TResult>(TArg1 arg1, TArg2 arg2);
    public delegate TResult Func<TArg1, TArg2, TArg3, TResult>(TArg1 arg1, TArg2 arg2, TArg3 arg3);
}

#endif
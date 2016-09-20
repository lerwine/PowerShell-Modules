#if PS20

using System;
using System.Collections.Generic;

namespace System.Collections.ObjectModel
{
	public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		public const string ReadOnlyErrorMessage = "Dictionary is read-only";
		
		private IDictionary<TKey, TValue> _innerDictionary;
		protected IDictionary<TKey, TValue> InnerDictionary { get { return _innerDictionary; } }
		
		public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
		{
			if (dictionary == null)
				throw new ArgumentNullException("dictionary");
			_innerDictionary = dictionary;
		}
		
		#region IDictionary<TKey, TValue> members
		
		public TValue this[TKey key] { get { return _innerDictionary[key]; } }
		
		TValue IDictionary<TKey, TValue>.this[TKey key]
		{
			get { return _innerDictionary[key]; }
			set { throw new NotSupportedException(ReadOnlyErrorMessage); }
		}
		
		public ICollection<TKey> Keys { get { return _innerDictionary.Keys; } }
		
		public ICollection<TValue> Values { get { return _innerDictionary.Values; } }
		
		void IDictionary<TKey, TValue>.Add(TKey key, TValue value) { throw new NotSupportedException(ReadOnlyErrorMessage); }
		
		public bool ContainsKey(TKey key) { return _innerDictionary.ContainsKey(key); }
		
		bool IDictionary<TKey, TValue>.Remove(TKey key) { throw new NotSupportedException(ReadOnlyErrorMessage); }
		
		public bool TryGetValue(TKey key, out TValue value) { return _innerDictionary.TryGetValue(key, out value); }
		
		#endregion
		
		#region ICollection<KeyValuePair<TKey, TValue>> members
		
		public int Count { get { return _innerDictionary.Count; } }
		
		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly { get { return true; } }
		
		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) { throw new NotSupportedException(ReadOnlyErrorMessage); }
		
		void ICollection<KeyValuePair<TKey, TValue>>.Clear() { throw new NotSupportedException(ReadOnlyErrorMessage); }
		
		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) { return _innerDictionary.Contains(item); }
		
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) { _innerDictionary.CopyTo(array, arrayIndex); }
		
		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) { throw new NotSupportedException(ReadOnlyErrorMessage); }
		
		#endregion
		
		#region IEnumerable members
		
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() { return _innerDictionary.GetEnumerator(); }
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return (_innerDictionary as System.Collections.IEnumerable).GetEnumerator(); }
		
		#endregion
	}
}

#endif

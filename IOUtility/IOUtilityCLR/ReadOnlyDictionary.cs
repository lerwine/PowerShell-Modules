using System;
using System.Collections.Generic;

namespace System.Collections.ObjectModel
{
#if PSLegacy
    /// <summary>
    /// A read-only dictionary intended to be forward-compatible with later .NET versions.
    /// </summary>
    /// <typeparam name="TKey">Type of key used by the wrapped dictionary.</typeparam>
    /// <typeparam name="TValue">Type of value associated with the keys in the wrapped dictionary.</typeparam>
    public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private const string ReadOnlyErrorMessage = "Dictionary is read-only";
		
		private IDictionary<TKey, TValue> _innerDictionary;
		
		/// <summary>
		/// Gets the dictionary that is wrapped by this <see cref="ReadOnlyDictionary&lt;TKey,TValue&gt;" /> object.
		/// </summary>
		protected IDictionary<TKey, TValue> InnerDictionary { get { return _innerDictionary; } }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlyDictionary&lt;TKey,TValue&gt;" /> class that is a wrapper around the specified dictionary.
		/// </summary>
        /// <param name="dictionary">The dictionary to wrap.</param>
		public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
		{
			if (dictionary == null)
				throw new ArgumentNullException("dictionary");
			_innerDictionary = dictionary;
		}
		
#region IDictionary<TKey, TValue> members
		
		/// <summary>
		/// Gets the element that has the specified key.
		/// </summary>
        /// <param name="key">The key of the element to get.</param>
		public TValue this[TKey key] { get { return _innerDictionary[key]; } }
		
		TValue IDictionary<TKey, TValue>.this[TKey key]
		{
			get { return _innerDictionary[key]; }
			set { throw new NotSupportedException(ReadOnlyErrorMessage); }
		}
		
		/// <summary>
		/// Gets a key collection that contains the keys of the dictionary.
		/// </summary>
		public ICollection<TKey> Keys { get { return _innerDictionary.Keys; } }
		
		/// <summary>
		/// Gets a collection that contains the values in the dictionary.
		/// </summary>
		public ICollection<TValue> Values { get { return _innerDictionary.Values; } }
		
		void IDictionary<TKey, TValue>.Add(TKey key, TValue value) { throw new NotSupportedException(ReadOnlyErrorMessage); }
		
		/// <summary>
		/// Determines whether the dictionary contains an element that has the specified key.
		/// </summary>
        /// <param name="key">The key to locate in the dictionary.</param>
		public bool ContainsKey(TKey key) { return _innerDictionary.ContainsKey(key); }
		
		bool IDictionary<TKey, TValue>.Remove(TKey key) { throw new NotSupportedException(ReadOnlyErrorMessage); }
		
		/// <summary>
		/// Retrieves the value that is associated with the specified key.
		/// </summary>
        /// <param name="key">The key whose value will be retrieved.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found;
		/// otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
		public bool TryGetValue(TKey key, out TValue value) { return _innerDictionary.TryGetValue(key, out value); }
		
#endregion
		
#region ICollection<KeyValuePair<TKey, TValue>> members
		
		
		/// <summary>
		/// Gets the number of items in the dictionary.
		/// </summary>
		public int Count { get { return _innerDictionary.Count; } }
		
		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly { get { return true; } }
		
		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) { throw new NotSupportedException(ReadOnlyErrorMessage); }
		
		void ICollection<KeyValuePair<TKey, TValue>>.Clear() { throw new NotSupportedException(ReadOnlyErrorMessage); }
		
		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) { return _innerDictionary.Contains(item); }
		
		/// <summary>
		/// Copies the elements of the dictionary to an array, starting at the specified array index.
		/// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the elements copied from the dictionary. The array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) { _innerDictionary.CopyTo(array, arrayIndex); }
		
		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) { throw new NotSupportedException(ReadOnlyErrorMessage); }
		
#endregion
		
#region IEnumerable members
		
		/// <summary>
		/// Returns an enumerator that iterates through the <see cref="ReadOnlyDictionary&lt;TKey,TValue&gt;" />.
		/// </summary>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() { return _innerDictionary.GetEnumerator(); }
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return (_innerDictionary as System.Collections.IEnumerable).GetEnumerator(); }
		
#endregion
	}
    
 #endif
}
using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Task;

namespace Erwine.Leonard.T.GDIPlus
{
    /// <summary>
    /// Base object for a list-based dictionary that supports asserted key values.
    /// </summary>
	/// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
	/// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
	[Serializable()]
    public class ListDictionaryBase<TKey, TValue> : IDictionary<TKey, TValue>, IList<TValue>, IList
		where TKey : IComparible
    {
		#region Fields
			
		private object _syncRoot = new object();
		private Dictionary<TKey, Collection<int>> _keyMapping;
		private KeyCollection _allKeys;
		private IList<TValue> _allValues = new List<TValue>();
	
		#endregion
		
		#region Events
		
        /// <summary>
        /// Defines the delegate method which gets the value of an asserted dictionary key.
        /// </summary>
        /// <param name="key">Value of asserted key.</param>
        /// <returns>The <typeparamref name="TValue" /> associated with the <typeparamref name="TKey" /> value.</param>
		protected internal delegate TValue GetAssertedValueDelegate(TKey key);
		
        /// <summary>
        /// Occurs when an asserted <typeparamref name="TKey" /> does not exist in the inner dictionary and a value needs to be returned.
        /// </summary>
		/// <remarks>This gets invoked from the <see cref="OnGetAssertedValue(TKey)" /> method.</remarks>
		public event GetAssertedValueDelegate GetAssertedValue;
		
		#endregion
		
		#region Properties
		
        /// <summary>
		/// Gets a collection containing the keys in the <see cref="ListDictionaryBase{TKey, TValue}" />.
        /// </summary>
		public ICollection<TKey> Keys { get { return _allKeys; } }
		
        /// <summary>
		/// Gets a collection containing the keys explicitly populated in the <see cref="ListDictionaryBase{TKey, TValue}" />.
        /// </summary>
		public ICollection<TKey> ExplicitKeys { get { return _keyMapping.Keys; } }
		
        /// <summary>
		/// Gets a collection containing the keys that will always exist in the <see cref="ListDictionaryBase{TKey, TValue}" />.
        /// </summary>
		public ICollection<TKey> AssertedKeys { get { return _allKeys.AssertedKeyNames; } }
		
        /// <summary>
		/// Gets a value indicating whether the <see cref="ListDictionaryBase{TKey, TValue}" /> has a fixed size.
        /// </summary>
		protected virtual bool IsFixedSize { get { return false; } }
		
        /// <summary>
		/// Gets a value indicating whether the <see cref="ListDictionaryBase{TKey, TValue}" /> is read-only.
        /// </summary>
		protected virtual bool IsReadOnly { get { return false; } }
				
        /// <summary>
		/// Gets the number of <typeparamref name="TValue" /> elements contained in the <see cref="ListDictionaryBase{TKey, TValue}" />.
        /// </summary>
		public int Count { get { return _allValues.Count; } }
		
		/// <summary>
		/// Gets a value indicating whether access to the <see cref="ListDictionaryBase{TKey, TValue}" /> is synchronized (thread safe).
        /// </summary>
		protected virtual bool IsSynchronized { get { return true; } }
		
		/// <summary>
		/// Gets an object that can be used to synchronize access to the <see cref="ListDictionaryBase{TKey, TValue}" />.
        /// </summary>
		protected virtual object SyncRoot { get { return false; } }
		
		#region Explicit Members
		
		TValue IList.this[int index]
		{
			get { return BaseGet(index); }
			set { BaseSet(index, value); }
		}
		
		TValue IDictionary<TKey, TValue>.this[TKey name]
		{
			get { return BaseGet(name); }
			set { BaseSet(name, value); }
		}
		
		ICollection<TValue> IDictionary<TKey, TValue>.Values { get { return _keyMapping.Values; } }
		bool IList.IsFixedSize { get { return IsFixedSize; } }
		bool IList.IsReadOnly { get { return IsReadOnly; } }
		
		#endregion
		
		#endregion
		
		#region Constructors
		
		/// <summary>
		/// Initializes a new <see cref="ListDictionaryBase{TKey, TValue}" /> the default <seealso cref="IEqualityComparer{TKey}" /> and no asserted key values.
        /// </summary>
		public ListDictionaryBase() : this(null, null as IEnumerable<TKey>) { }
		
		/// <summary>
		/// Initializes a new <see cref="ListDictionaryBase{TKey, TValue}" /> the specified asserted <typeparamref name="TKey" /> values and the default <seealso cref="IEqualityComparer{TKey}" />.
        /// </summary>
        /// <param name="assertedKeyValues">Values of keys that will always exist in the dictionary.</param>
		/// <remarks>Override the <see cref="OnGetAssertedValue(TKey)" /> method or use the <see cref="GetAssertedValue" /> event to provide values for asserted keys.</remarks>
		public ListDictionaryBase(params TKey[] assertedKeyValues) : this(assertedKeyValues as IEnumerable<TKey>) { }
		
		/// <summary>
		/// Initializes a new <see cref="ListDictionaryBase{TKey, TValue}" /> the specified asserted <typeparamref name="TKey" /> values and the default <seealso cref="IEqualityComparer{TKey}" />.
        /// </summary>
        /// <param name="assertedKeyValues">Values of keys that will always exist in the dictionary.</param>
		/// <remarks>Override the <see cref="OnGetAssertedValue(TKey)" /> method or use the <see cref="GetAssertedValue" /> event to provide values for asserted keys.</remarks>
		public ListDictionaryBase(IEnumerable<TKey> assertedKeyValues) : this(assertedKeyValues, null as IEnumerable<TKey>) { }
		
		/// <summary>
		/// Initializes a new <see cref="ListDictionaryBase{TKey, TValue}" /> the specified <seealso cref="IEqualityComparer{TKey}" /> and no asserted key values.
        /// </summary>
        /// <param name="assertedKeyValues"><seealso cref="IEqualityComparer{TKey}" /> which will be used to compare <typeparamref name="TKey" /> values.</param>
		public ListDictionaryBase(IEqualityComparer<TKey> equalityComparer) : this(null, equalityComparer) { }
		
		/// <summary>
		/// Initializes a new <see cref="ListDictionaryBase{TKey, TValue}" /> the specified <seealso cref="IEqualityComparer{TKey}" /> and the asserted <typeparamref name="TKey" /> values.
        /// </summary>
        /// <param name="assertedKeyValues"><seealso cref="IEqualityComparer{TKey}" /> which will be used to compare <typeparamref name="TKey" /> values.</param>
        /// <param name="assertedKeyValues">Values of keys that will always exist in the dictionary.</param>
		/// <remarks>Override the <see cref="OnGetAssertedValue(TKey)" /> method or use the <see cref="GetAssertedValue" /> event to provide values for asserted keys.</remarks>
		public ListDictionaryBase(IEqualityComparer<TKey> equalityComparer, params TKey[] assertedKeyValues) : this(equalityComparer, assertedKeyValues as IEnumerable<TKey>) { }
		
		/// <summary>
		/// Initializes a new <see cref="ListDictionaryBase{TKey, TValue}" /> the specified <seealso cref="IEqualityComparer{TKey}" /> and the asserted <typeparamref name="TKey" /> values.
        /// </summary>
        /// <param name="assertedKeyValues"><seealso cref="IEqualityComparer{TKey}" /> which will be used to compare <typeparamref name="TKey" /> values.</param>
        /// <param name="assertedKeyValues">Values of keys that will always exist in the dictionary.</param>
		/// <remarks>Override the <see cref="OnGetAssertedValue(TKey)" /> method or use the <see cref="GetAssertedValue" /> event to provide values for asserted keys.</remarks>
		public ListDictionaryBase(IEqualityComparer<TKey> equalityComparer, IEnumerable<TKey> assertedKeyValues)
		{
			if (equalityComparer == null)
				equalityComparer = EqualityComparer<TKey>.Default;
			List
			_keyMapping = new Dictionary<TKey, Collection<int>>(equalityComparer);
			_allKeys = new KeyCollection(_keyMapping, assertedKeyValues, equalityComparer);
		}
		
		#endregion
		
		#region Methods
		
		#region Get
		
        /// <summary>
        /// Gets the value of an asserted dictionary key.
        /// </summary>
        /// <param name="key">Value of asserted key.</param>
        /// <returns>The <typeparamref name="TValue" /> associated with the <typeparamref name="TKey" /> value.</param>
		/// <remarks>This invokes the <see cref="GetAssertedValue" /> event or returns the default value if no event handlers are attached.</remarks>
		protected virtual TValue OnGetAssertedValue(TKey key)
		{
			GetAssertedValueDelegate getAssertedValue = GetAssertedValue;
			if (getAssertedValue != null)
				return getAssertedValue(key);
			return default(TKey);
		}
		
        /// <summary>
        /// Gets the <typeparamref name="TValue" /> elements(s) associated with a <typeparamref name="TKey" /> value.
        /// </summary>
        /// <param name="key">Value of key.</param>
        /// <returns>The <typeparamref name="TValue" /> element(s) associated with the <paramref name="key" /> value.</param>
		/// <remarks>If more than one element has the same key, the resulting array will have multiple <typeparamref name="TValue" /> elements or an empty array if no elements are associated with <paramref name="key" />.</remarks>
		protected virtual TValue[] BaseGet(TKey key)
		{
			TValue[] value;
			if (BaseTryGet(key, out value))
				return value;
			return new TValue[0];
		}
		
        /// <summary>
        /// Gets the <typeparamref name="TValue" /> element at a particular index.
        /// </summary>
        /// <param name="index">Zero-based index of element to return.</param>
        /// <returns>The <typeparamref name="TValue" /> element at the specified index.</param>
		protected virtual TValue BaseGet(int index)
		{
			Monitor.Enter(_syncRoot);
			try { return _allValues[index]; }
			finally { Monitor.Exit(_syncRoot); }
		}
		
        /// <summary>
        /// Gets the nth <typeparamref name="TValue" /> element associated with a <typeparamref name="TKey" /> value.
        /// </summary>
        /// <param name="key">Value of key.</param>
        /// <param name="subIndex">Zero-based index of element which is associated with the <paramref name="key" /> value.</param>
        /// <returns>The nth <typeparamref name="TValue" /> element associated with the <paramref name="key" /> value or null if no elements are associated with <paramref name="key" />.</param>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="subIndex" /> is less than zero or there are not at least (<paramref name="subIndex" /> + 1) elements associated with <paramref name="key" />.</exception>
		protected virtual TValue BaseGet(TKey key, int subIndex)
		{
			TValue value;
			if (BaseTryGet(key, subIndex, out value))
				return value;
			return default(TValue);
		}
		
        /// <summary>
        /// Gets the nth <typeparamref name="TKey" /> associated with the element at the specified index.
        /// </summary>
        /// <param name="subIndex">Zero-based index of associated element.</param>
        /// <returns>The <typeparamref name="TKey" /> element associated with the element at the specified <paramref name="index" /> or null if <paramref name="index" /> was out of range of the collection.</param>
		protected TKey BaseGetKey(int index)
		{
			Monitor.Enter(_syncRoot);
			try { return _keyMapping.Keys.FirstOrDefault(k => _keyMapping[k].Contains(index)); }
			finally { Monitor.Exit(_syncRoot); }
		}
		
		#endregion
		
		#region Set
		
        /// <summary>
        /// Associates a <typeparamref name="TValue" /> with a <typeparamref name="TKey" />, replacing any existing elements(s) that were associated with the <typeparamref name="TKey" /> value.
        /// </summary>
        /// <param name="key">Value of key.</param>
        /// <param name="value">New value of element.</param>
        /// <returns>The zero-based index where the <paramref name="value" /> was appended or replaced int.</param>
		/// <remarks>If more than any existing element hav the same key, this will assume the same index position as the first associated element, otherwise, it will be appended to the end of the list.</remarks>
		protected virtual int BaseSet(TKey key, TValue value)
		{
			Monitor.Enter(_syncRoot);
			try
			{
				int index;
				Collection<int> indices;
				if (!_keyMapping.ContainsKey(key))
				{
					indices = new Collection<int>();
					index = _allValues.Count;
					_allValues.Add(value);
					indices.Add(index);
					return index;
				}
				indices = _keyMapping[key];
				index = indices[0];
				_allValues[index] = value;
				if (indices.Count == 1)
				{
					indices.Add(index);
					return index;
				}
				int[] removedIndices = indices.Skip(1).ToArray();
				indices.Clear();
				foreach (TKey k in _keyMapping.Keys)
				{
					indices = _keyMapping[k];
					for (int n = 0; n < indices.Count; n++)
					{
						int x = indices[n];
						int c = removedIndices.Count(i => i < x);
						if (c > 0)
							indices[n] = x - c;
					}
				}
				foreach (int i in removedIndices.Reverse())
					_allValues.RemoveAt(i);
				indices.Add(index);
				return index;
			}
			finally { Monitor.Exit(_syncRoot); }
		}
		
        /// <summary>
        /// Replaces a <typeparamref name="TValue" /> in the current list.
        /// </summary>
        /// <param name="index">Zero-based index of item to replace.</param>
        /// <param name="key">New value of element.</param>
        /// <returns>The <typeparamref name="TKey" /> to be associated with the <paramref name="key" /> value.</param>
		/// <remarks>If more than any existing element hav the same key, this will assume the same index position as the first associated element, otherwise, it will be appended to the end of the list.</remarks>
		protected virtual TKey BaseSet(int index, TValue value)
		{
			Monitor.Enter(_syncRoot);
			try
			{
				_allValues[index] = value;
				return _keyMapping.Keys.FirstOrDefault(k => _keyMapping[k].Contains(index));
			}
			finally { Monitor.Exit(_syncRoot); }
		}
		
		protected virtual int BaseSet(TKey key, int subIndex, TValue value)
		{
			Monitor.Enter(_syncRoot);
			try
			{
				int index;
				if (_keyMapping.ContainsKey(key))
				{
					Collection<int> indices = _keyMapping[key];
					index = indices[subIndex];
					_allValues[index] = value;
					return index;
				}
				
				if (subIndex != 0)
					throw new ArgumentOutOfRangeException("subIndex");
		
				Collection<int> indices = new Collection<int>();
				index = _allValues.Count;
				indices.Add(index);
				_allValues.Add(value);
				return index;
			}
			finally { Monitor.Exit(_syncRoot); }
		}

		#endregion
		
		#region Add
		
		protected virtual int BaseAdd(TKey key, TValue value)
		{
			Monitor.Enter(_syncRoot);
			try
			{
				int index = _allValues.Count;
				_allValues.Add(value);
				Collection<int> indices;
				if (_keyMapping.ContainsKey(key))
					indices = _keyMapping[key];
				else
				{
					indices = new Collection<int>();
					_keyMapping.Add(key, indices);
				}
				indices.Add(index);
				return index;
			}
			finally { Monitor.Exit(_syncRoot); }
		}
		
		#endregion
		
		#region Insert
		
		protected virtual void BaseInsert(int index, TKey key, TValue value)
		{
			Monitor.Enter(_syncRoot);
			try
			{
				Collection<int> indices;
				_allValues.Insert(index, value);
				foreach (TKey k in _keyMapping.Keys)
				{
					indices = _keyMapping[k];
					for (int n = 0; n < indices.Count; n++)
					{
						if (indices[n] <= index)
							indices[n]++;
					}
				}
				if (!_keyMapping.ContainsKey(key))
				{
					indices = new Collection<int>();
					_keyMapping.Add(key, indices);
					indices.Add(index);
					return;
				}
				indices = _keyMapping[key];
				int i = indices.TakeWhile(i => i < index).Count();
				if (i < indices.Count)
					indices.Insert(i, index);
				else
					indices.Add(index);
			}
			finally { Monitor.Exit(_syncRoot); }
		}
		
		#endregion
		
		#region Remove
		
		protected virtual bool BaseRemove(TKey key, TValue value)
		{
			Monitor.Enter(_syncRoot);
			try
			{
				if (!_keyMapping.ContainsKey(key))
					return false;
				Collection<int> indices = _keyMapping[key];
				int index = indices.Select(i => _allValues[i]).ToList().IndexOf(value);
				if (index < 0)
					return false;
				int removedIndex = indices[index];
				if (indices.Count == 1)
					_keyMapping.Remove(key);
				else
					indices.RemoveAt(index);
				_allValues.RemoveAt(removedIndex);
				foreach (TKey k in _keyMapping.Keys)
				{
					indices = _keyMapping[k];
					for (int n = 0; n < indices.Count; n++)
					{
						if (indices[n] >= removedIndex)
							indices[n]--;
					}
				}
			}
			finally { Monitor.Exit(_syncRoot); }
		}
		
		protected virtual bool BaseRemove(TKey key)
		{
			Monitor.Enter(_syncRoot);
			try
			{
				if (!_keyMapping.ContainsKey(key))
					return false;
				Collection<int> removedIndices = _keyMapping[key];
				_keyMapping.Remove(key);
				foreach (TKey k in _keyMapping.Keys)
				{
					indices = _keyMapping[k];
					for (int n = 0; n < indices.Count; n++)
					{
						int x = indices[n];
						int c = removedIndices.Count(i => i >= x);
						if (c > 0)
							indices[n] = x - c;
					}
				}
				foreach (int i in removedIndices.Reverse())
					_allValues.RemoveAt(i);
			}
			finally { Monitor.Exit(_syncRoot); }
		}
		
		protected virtual bool BaseRemove(TValue value)
		{
			Monitor.Enter(_syncRoot);
			try
			{
				int removedIndex = _allValues.IndexOf(value);
				if (removedIndex < 0)
					return false;
				_allValues.RemoveAt(removedIndex);
				IEnumerable<TKey> keyMatches = _keyMapping.Keys.Where(k => _keyMapping[k].Contains(removedIndex));
				if (keyMatches.Any())
				{
					TKey key = keyMatches.First();
					Collection<int> indices = _keyMapping[key];
					if (indices.Count == 1)
						_keyMapping.Remove(key);
					else
						_keyMapping.Remove(removedIndex);
				}
				foreach (TKey key in _keyMapping.Keys)
				{
					indices = _keyMapping[key];
					for (int n = 0; n < indices.Count; n++)
					{
						if (indices[n] >= removedIndex)
							indices[n]--;
					}
				}
			}
			finally { Monitor.Exit(_syncRoot); }
		}
		
		#endregion
		
		#region RemoveAt
		
		protected virtual bool BaseRemoveAt(TKey key, int subIndex)
		{
			if (subIndex < 0)
				return false;
			
			Monitor.Enter(_syncRoot);
			try
			{
				if (!_keyMapping.ContainsKey(key))
					return false;
				Collection<int> indices = _keyMapping[key];
				if (subIndex >= indices.Count)
					return false;
				int removedIndex = indices[subIndex];
				if (indices.Count == 1)
					_keyMapping.Remove(key);
				else
					indices.RemoveAt(subIndex);
				_allValues.RemoveAt(removedIndex);
				foreach (TKey k in _keyMapping.Keys)
				{
					indices = _keyMapping[k];
					for (int n = 0; n < indices.Count; n++)
					{
						if (indices[n] >= removedIndex)
							indices[n]--;
					}
				}
			}
			finally { Monitor.Exit(_syncRoot); }
		}
		
		protected virtual void BaseRemoveAt(int index)
		{
			Monitor.Enter(_syncRoot);
			try
			{
				_allValues.RemoveAt(index);
				IEnumerable<TKey> keyMatches = _keyMapping.Keys.FirstOrDefault(k => _keyMapping[k].Contains(index));
				if (keyMatches.Any())
				{
					TKey key = keyMatches.First();
					Collection<int> indices = _keyMapping[key];
					if (indices.Count == 1)
						_keyMapping.Remove(key);
					else
						_keyMapping.Remove(index);
				}
				foreach (TKey key in _keyMapping.Keys)
				{
					indices = _keyMapping[key];
					for (int n = 0; n < indices.Count; n++)
					{
						if (indices[n] >= index)
							indices[n]--;
					}
				}
			}
			finally { Monitor.Exit(_syncRoot); }
		}
		
		#endregion
		
		#region Clear
		
		protected virtual void BaseClear()
		{
			Monitor.Enter(_syncRoot);
			try
			{
				_keyMapping.Clear();
				_allValues.Clear();
			}
			finally { Monitor.Exit(_syncRoot); }
		}
		
		#endregion
		
		#region TryGet
		
		protected virtual bool BaseTryGet(TKey key, out TValue value)
		{
			Monitor.Enter(_syncRoot);
			try
			{
				if (!_keyMapping.ContainsKey(key))
				{
					value = default(TValue);
					return false;
				}
				Collection<int> indices = _keyMapping[key];
				if (indices.Count == 1)
					value = _allValues[indices[0]];
				else
					value = indices.Select(i => _allValues[i]).ToArray();
			}
			catch
			{
				value = default(TValue);
				return false;
			}
			finally { Monitor.Exit(_syncRoot); }
			return true;
		}
		
		protected virtual bool BaseTryGet(TKey key, int subIndex, out TValue value)
		{
			if (subIndex < 0)
			{
				value = default(TValue);
				return false;
			}
			
			Monitor.Enter(_syncRoot);
			try
			{
				if (!_keyMapping.ContainsKey(key))
				{
					value = default(TValue);
					return false;
				}
				Collection<int> indices = _keyMapping[key];
				if (subIndex >= indices.Count)
				{
					value = default(TValue);
					return false;
				}
				value = _allValues[_keyMapping[key][subIndex]];
			}
			finally { Monitor.Exit(_syncRoot); }
			return true;
		}
		
		#endregion
		
		#region Interface Implementation Members
		
		public bool ContainsKey(TKey key) { return _keyMapping.ContainsKey(key); }
		
		public void Remove(TKey key) { BaseRemove(key); }
		
		public void Clear() { BaseClear(); }
		
		public bool Contains(TValue value) { return _allValues.Contains(value); }
		
		public int IndexOf(TValue value) { return _allValues.IndexOf(value); }
		
		public void RemoveItem(TValue value) { BaseRemove(value); }
		
		public void RemoveAt(int index) { BaseRemoveAt(index); }
	  
		public void CopyTo(TValue[] array, int index) { _allValues.CopyTo(array, index); }
			
        /// <summary>
        /// Returns an enumerator that iterates through each <typeparamref name="TValue" /> of the collection.
        /// </summary>
		/// <returns>An <seealso cref="IEnumerator{TValue}" /> that iterates through the collection.</param>
		public IEnumerator<TValue> GetEnumerator() { return _allValues.GetEnumerator(); }
		
		#region Explicit Members
		
		void IDictionary<TKey, TValue>.Add(TKey key, TValue value) { BaseAdd(key, value); }
		
		bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value) { return BaseTryGet(key, out value); }
	  
		bool ICollection<KeyValuePair<TKey, TValue>>.Count { get { return _keyMapping.Count; } }
		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly { get { return IsReadOnly; } }
		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) { BaseAdd(item.Key, item.Value); }
		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) { }
		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index) { }
		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) { }
		int IList.Add(TValue value) { throw new NotSupportedException(); }
		void IList.Insert(int index, TValue value) { throw new NotSupportedException(); }
		void IList.Remove(TValue value) { BaseRemove(value); }
		bool ICollection.IsSynchronized { get { return IsSynchronized; } }
		object ICollection.SyncRoot { get { return SyncRoot; } }
		void ICollection.CopyTo(Array array, int index) { _allValues.ToArray().CopyTo(array, index); }
		
		IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
		{
			return _keyMapping.Keys.SelectMany(k => _keyMapping[k].Select(v => new KeyValuePair<TKey, TValue>(k, v))).GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator() { return allValues.ToArray().GetEnumerator(); }
		
		#endregion
		
		#endregion
		
		#endregion
		
		#region Nested Classes
		
		protected internal sealed class KeyCollection : ICollection<TKey>, IEnumerable<TKey>, ICollection, IReadOnlyCollection<TKey>
		{
			#region Fields
			
			private ListDictionaryBase<TKey, TValue> _owner;
			private ReadOnlyCollection<TKey> _assertedKeyValues;
			private IEqualityComparer<TKey> _equalityComparer;
			
			#endregion
			
			#region Properties
			
			public int Count
			{
				get
				{
					int count = _owner.ExplicitKeys.Count;
					if (count == 0)
						return _assertedKeyValues.Count;
					if (_assertedKeyValues.Count == 0)
						return count;
					return count + _assertedKeyValues.Count(k => !_owner.ExplicitKeys.Contains(k, _equalityComparer));
				}
			}
			
			internal ReadOnlyCollection<TKey> AssertedKeyNames { get { return _assertedKeyValues; } }
			
			internal ListDictionaryBase<TKey, TValue> Owner { get { return _owner; } }
			
			#region Explicit Members
			
			bool ICollection<TKey>.IsReadOnly { get { return true; } }
			bool ICollection.IsSynchronized { get { return true; } }
			object ICollection.SyncRoot { get { return _owner.SyncRoot; } }
			
			#endregion
			
			#endregion
			
			#region Constructors
			
			internal KeyCollection(ListDictionaryBase<TKey, TValue> owner, IEnumerable<TKey> assertedKeyValues, IEqualityComparer<TKey> equalityComparer)
			{
				_owner = owner;
				_assertedKeyValues = new ReadOnlyCollection<TKey>((assertedKeyValues == null) ? new TKey[0] : assertedKeyValues.Distinct(equalityComparer).ToArray());
				_equalityComparer = equalityComparer;
			}
			
			#endregion
			
			#region Methods
			
			public IEnumerable<TKey> AsEnumerable()
			{
				if (_assertedKeyValues.Count == 0)
					return _owner.ExplicitKeys;
				if (_owner.ExplicitKeys.Count == 0)
					return _assertedKeyValues;
				
				return _owner.ExplicitKeys.Concat(_assertedKeyValues.Where(k => !_owner.ExplicitKeys.Contains(k, _equalityComparer)));
			}
			
			public void CopyTo(TKey[] array, int index) { AsEnumerable().ToList().CopyTo(array, index); }
			
			public IEnumerator<TKey> GetEnumerator() { return AsEnumerable().GetEnumerator(); }
			
			internal bool Contains(TKey value) { return AsEnumerable().Contains(value, _equalityComparer); }
			
			#region Explicit Members
			
			bool ICollection<TKey>.Contains(TKey value) { return AsEnumerable().Contains(value, _equalityComparer); }
			void ICollection.CopyTo(Array array, int arrayIndex) { AsEnumerable().ToArray().CopyTo(array, arrayIndex); }
			IEnumerator IEnumerable.GetEnumerator() { return AsEnumerable().ToArray().GetEnumerator(); }
			
			void ICollection<TKey>.Add(TKey] value) { throw new NotSupportedException(); }
			void ICollection<TKey>.Clear() { throw new NotSupportedException(); 
			bool ICollection<TKey>.Remove(TKey value) { throw new NotSupportedException(); }

			#endregion

			#endregion
		}
		
		#endregion
	}
}
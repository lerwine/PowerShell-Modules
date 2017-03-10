using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Erwine.Leonard.T.GDIPlus
{
    /// <summary>
    /// Base object for a list-based dictionary that supports asserted key values.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    [Serializable()]
    public class ListDictionaryBase<TKey, TValue> : IDictionary<TKey, TValue>, IList<TValue>, IList
        where TKey : IComparable
    {
        #region Fields
            
        private object _syncRoot = new object();
        private Dictionary<TKey, Collection<int>> _keyMapping;
        private KeyCollection _keyCollection;
        private List<TValue> _innerList = new List<TValue>();
        private ReadOnlyCollection<TValue> _values;
        private IEqualityComparer<TValue> _valueComparer = EqualityComparer<TValue>.Default;

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
        protected internal event GetAssertedValueDelegate GetAssertedValue;
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Gets a collection containing the keys in the <see cref="ListDictionaryBase{TKey, TValue}" />.
        /// </summary>
        public ICollection<TKey> Keys { get { return _keyCollection; } }
        
        /// <summary>
        /// Gets a collection containing the keys explicitly populated in the <see cref="ListDictionaryBase{TKey, TValue}" />.
        /// </summary>
        public ICollection<TKey> ExplicitKeys { get { return _keyMapping.Keys; } }
        
        /// <summary>
        /// Gets a collection containing the keys that will always exist in the <see cref="ListDictionaryBase{TKey, TValue}" />.
        /// </summary>
        public ICollection<TKey> AssertedKeys { get { return _keyCollection.AssertedKeyNames; } }
        
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
        public int Count { get { return _innerList.Count; } }
        
        /// <summary>
        /// Gets a value indicating whether access to the <see cref="ListDictionaryBase{TKey, TValue}" /> is synchronized (thread safe).
        /// </summary>
        protected virtual bool IsSynchronized { get { return true; } }
        
        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="ListDictionaryBase{TKey, TValue}" />.
        /// </summary>
        protected virtual object SyncRoot { get { return false; } }

        #region Explicit Members

        TValue IList<TValue>.this[int index]
        {
            get { return BaseGet(index); }
            set { BaseSet(index, value); }
        }

        TValue IDictionary<TKey, TValue>.this[TKey name]
        {
            get { return BaseGet(name)[0]; }
            set { BaseSet(name, value); }
        }

        object IList.this[int index]
        {
            get { return BaseGet(index); }
            set { BaseSet(index, (TValue)value); }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values { get { return _values; } }

        bool IList.IsFixedSize { get { return IsFixedSize; } }

        bool ICollection<TValue>.IsReadOnly { get { return IsReadOnly; } }
        
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
        public ListDictionaryBase(IEnumerable<TKey> assertedKeyValues) : this(null, assertedKeyValues) { }
        
        /// <summary>
        /// Initializes a new <see cref="ListDictionaryBase{TKey, TValue}" /> the specified <seealso cref="IEqualityComparer{TKey}" /> and no asserted key values.
        /// </summary>
        /// <param name="assertedKeyValues"><seealso cref="IEqualityComparer{TKey}" /> which will be used to compare <typeparamref name="TKey" /> values.</param>
        public ListDictionaryBase(IEqualityComparer<TKey> equalityComparer) : this(equalityComparer, null as IEnumerable<TKey>) { }
        
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
            _keyMapping = new Dictionary<TKey, Collection<int>>(equalityComparer);
            _keyCollection = new KeyCollection(this, assertedKeyValues, equalityComparer);
            _values = new ReadOnlyCollection<TValue>(_innerList);
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
            return default(TValue);
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
            try { return _innerList[index]; }
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
                    index = _innerList.Count;
                    _innerList.Add(value);
                    indices.Add(index);
                    return index;
                }
                indices = _keyMapping[key];
                index = indices[0];
                _innerList[index] = value;
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
                    _innerList.RemoveAt(i);
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
                _innerList[index] = value;
                return _keyMapping.Keys.FirstOrDefault(k => _keyMapping[k].Contains(index));
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        /// <summary>
        /// Replaces a <typeparamref name="TValue" /> matching key in the current list.
        /// </summary>
        /// <param name="key">Key value associated with the <paramref name="value"/>.</param>
        /// <param name="subIndex">nth <typeparamref name="TValue"/> associated with <paramref name="key"/>.</param>
        /// <param name="value"><typeparamref name="TValue"/> to remove.</param>
        /// <returns>Returns true if <paramref name="value"/> was removed; otherwise, false.</returns>
        protected virtual int BaseSet(TKey key, int subIndex, TValue value)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                int index;
                Collection<int> indices;
                if (_keyMapping.ContainsKey(key))
                {
                    indices = _keyMapping[key];
                    index = indices[subIndex];
                    _innerList[index] = value;
                    return index;
                }
                
                if (subIndex != 0)
                    throw new ArgumentOutOfRangeException("subIndex");

                indices = new Collection<int>();
                index = _innerList.Count;
                indices.Add(index);
                _innerList.Add(value);
                return index;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        #endregion

        #region Add

        /// <summary>
        /// Adds <typeparamref name="TValue"/> to the current <see cref="ListDictionaryBase{TKey, TValue}"/>, associating it with a <typeparamref name="TKey"/> value.
        /// </summary>
        /// <param name="key"><typeparamref name="TKey"/> to associate with <paramref name="value"/>.</param>
        /// <param name="value"><typeparamref name="TValue"/> to be added to the current <see cref="ListDictionaryBase{TKey, TValue}"/>.</param>
        /// <returns>Zero-based index at which <paramref name="value"/> was added.</returns>
        protected virtual int BaseAdd(TKey key, TValue value)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                int index = _innerList.Count;
                _innerList.Add(value);
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

        /// <summary>
        /// Inserts <typeparamref name="TValue"/> into the current <see cref="ListDictionaryBase{TKey, TValue}"/> at a specified index, associating it with a <typeparamref name="TKey"/> value.
        /// </summary>
        /// <param name="index">Zero-based index where <paramref name="value"/> is to be inserted.</param>
        /// <param name="key"><typeparamref name="TKey"/> to associate with <paramref name="value"/>.</param>
        /// <param name="value"><typeparamref name="TValue"/> to be inserted into the current <see cref="ListDictionaryBase{TKey, TValue}"/>.</param>
        protected virtual void BaseInsert(int index, TKey key, TValue value)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                Collection<int> indices;
                _innerList.Insert(index, value);
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
                int i = indices.TakeWhile(n => n < index).Count();
                if (i < indices.Count)
                    indices.Insert(i, index);
                else
                    indices.Add(index);
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        #endregion

        #region Remove

        /// <summary>
        /// Removes a specifc value from the current <see cref="ListDictionaryBase{TKey, TValue}"/> which is also associated with a <typeparamref name="TKey"/> value.
        /// </summary>
        /// <param name="key"><typeparamref name="TKey"/> associated with <paramref name="value"/>.</param>
        /// <param name="value"><typeparamref name="TValue"/> to be removed from the current <see cref="ListDictionaryBase{TKey, TValue}"/>.</param>
        /// <returns>true if <paramref name="value"/> was found and associated with <paramref name="key"/> and was removed; otherwise, false.</returns>
        protected virtual bool BaseRemove(TKey key, TValue value)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (!_keyMapping.ContainsKey(key))
                    return false;
                Collection<int> indices = _keyMapping[key];
                int index = indices.Select(i => _innerList[i]).ToList().IndexOf(value);
                if (index < 0)
                    return false;
                int removedIndex = indices[index];
                if (indices.Count == 1)
                    _keyMapping.Remove(key);
                else
                    indices.RemoveAt(index);
                _innerList.RemoveAt(removedIndex);
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
            return true;
        }

        /// <summary>
        /// Removes all values from the current <see cref="ListDictionaryBase{TKey, TValue}"/> which are associated with a <typeparamref name="TKey"/> value.
        /// </summary>
        /// <param name="key"><typeparamref name="TKey"/> associated with <typeparamref name="TValue"/> elements to be moved.</param>
        /// <returns>True if any elements were found which were associated with <paramref name="key"/> and were removed.</returns>
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
                    Collection<int> indices = _keyMapping[k];
                    for (int n = 0; n < indices.Count; n++)
                    {
                        int x = indices[n];
                        int c = removedIndices.Count(i => i >= x);
                        if (c > 0)
                            indices[n] = x - c;
                    }
                }
                foreach (int i in removedIndices.Reverse())
                    _innerList.RemoveAt(i);
            }
            finally { Monitor.Exit(_syncRoot); }
            return true;
        }

        /// <summary>
        /// Removes a specific <typeparamref name="TValue"/> element from the current <see cref="ListDictionaryBase{TKey, TValue}"/>.
        /// </summary>
        /// <param name="value"><typeparamref name="TValue"/> to be removed.</param>
        /// <returns>True if a matching <typeparamref name="TValue"/> was found and removed; otherwise, false.</returns>
        protected virtual bool BaseRemove(TValue value)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                int removedIndex = _innerList.IndexOf(value);
                if (removedIndex < 0)
                    return false;
                _innerList.RemoveAt(removedIndex);
                IEnumerable<TKey> keyMatches = _keyMapping.Keys.Where(k => _keyMapping[k].Contains(removedIndex));
                if (keyMatches.Any())
                {
                    TKey key = keyMatches.First();
                    Collection<int> indices = _keyMapping[key];
                    if (indices.Count == 1)
                        _keyMapping.Remove(key);
                    else
                        _keyMapping[key].Remove(removedIndex);
                }
                foreach (TKey key in _keyMapping.Keys)
                {
                    Collection<int> indices = _keyMapping[key];
                    for (int n = 0; n < indices.Count; n++)
                    {
                        if (indices[n] >= removedIndex)
                            indices[n]--;
                    }
                }
            }
            finally { Monitor.Exit(_syncRoot); }
            return true;
        }
        
        #endregion
        
        #region RemoveAt
        
        /// <summary>
        /// Removes the nth <typeparamref name="TValue"/> associated with a <typeparamref name="TKey"/> value.
        /// </summary>
        /// <param name="key"><typeparamref name="TKey"/> associated with the element to remove.</param>
        /// <param name="subIndex">Zero-based index of element assocated with <paramref name="key"/>, relative to any other elements associated with that <paramref name="key"/>.</param>
        /// <returns></returns>
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
                _innerList.RemoveAt(removedIndex);
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
            return true;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        protected virtual void BaseRemoveAt(int index)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                _innerList.RemoveAt(index);
                Collection<int> indices;
                IEnumerable<TKey> keyMatches = _keyMapping.Keys.Where(k => _keyMapping[k].Contains(index));
                if (keyMatches.Any())
                {
                    TKey key = keyMatches.First();
                    indices = _keyMapping[key];
                    if (indices.Count == 1)
                        _keyMapping.Remove(key);
                    _innerList.RemoveAt(index);
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
        
        /// <summary>
        /// 
        /// </summary>
        protected virtual void BaseClear()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                _keyMapping.Clear();
                _innerList.Clear();
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        
        #endregion
        
        #region TryGet
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual bool BaseTryGet(TKey key, out TValue[] value)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_keyMapping.ContainsKey(key))
                {
                    value = _keyMapping[key].Select(i => _innerList[i]).ToArray();
                    return true;
                }
            }
            finally { Monitor.Exit(_syncRoot); }
            value = new TValue[0];
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="subIndex"></param>
        /// <param name="value"></param>
        /// <returns></returns>
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
                value = _innerList[_keyMapping[key][subIndex]];
            }
            finally { Monitor.Exit(_syncRoot); }
            return true;
        }
        
        #endregion
        
        #region Interface Implementation Members
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key) { return _keyMapping.ContainsKey(key); }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key) { return BaseRemove(key); }
        
        /// <summary>
        /// 
        /// </summary>
        public void Clear() { BaseClear(); }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(TValue value) { return _innerList.Contains(value); }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int IndexOf(TValue value) { return _innerList.IndexOf(value); }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void RemoveItem(TValue value) { BaseRemove(value); }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index) { BaseRemoveAt(index); }
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(TValue[] array, int index) { _innerList.CopyTo(array, index); }
            
        /// <summary>
        /// Returns an enumerator that iterates through each <typeparamref name="TValue" /> of the collection.
        /// </summary>
        /// <returns>An <seealso cref="IEnumerator{TValue}" /> that iterates through the collection.</param>
        public IEnumerator<TValue> GetEnumerator() { return _innerList.GetEnumerator(); }

        #region Explicit Members

        int ICollection<KeyValuePair<TKey, TValue>>.Count { get { return _keyMapping.Count; } }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly { get { return IsReadOnly; } }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value) { BaseAdd(key, value); }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) { BaseAdd(item.Key, item.Value); }

        int IList.IndexOf(object value) { return IndexOf((TValue)value); }

        bool IList.Contains(object value) { return Contains((TValue)value); }

        bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            TValue[] values;
            if (BaseTryGet(key, out values))
            {
                value = values[0];
                return true;
            }
            value = default(TValue);
            return false;
        }
      
        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue[] value;
            return BaseTryGet(item.Key, out value) && value.Contains(item.Value, _valueComparer);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            _keyMapping.Keys.SelectMany(k => _keyMapping[k].Select(v => new KeyValuePair<TKey, TValue>(k, _innerList[v]))).ToList().CopyTo(array, index);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            TValue[] value;
            return BaseTryGet(item.Key, out value) && value.Contains(item.Value, _valueComparer) && BaseRemove(item.Value);
        }

        void ICollection<TValue>.Add(TValue item) { throw new NotSupportedException(); }

        int IList.Add(object value) { throw new NotSupportedException(); }

        void IList<TValue>.Insert(int index, TValue value) { throw new NotSupportedException(); }

        void IList.Insert(int index, object value) { throw new NotSupportedException(); }

        bool ICollection<TValue>.Remove(TValue value) { return BaseRemove(value); }

        void IList.Remove(object value) { BaseRemove((TValue)value); }

        bool ICollection.IsSynchronized { get { return IsSynchronized; } }

        object ICollection.SyncRoot { get { return SyncRoot; } }

        void ICollection.CopyTo(Array array, int index) { _innerList.ToArray().CopyTo(array, index); }
        
        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return _keyMapping.Keys.SelectMany(k => _keyMapping[k].Select(v => new KeyValuePair<TKey, TValue>(k, _innerList[v]))).GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator() { return _innerList.ToArray().GetEnumerator(); }
        
        #endregion
        
        #endregion
        
        #endregion
        
        #region Nested Classes
        
        /// <summary>
        /// 
        /// </summary>
        protected internal sealed class KeyCollection : ICollection<TKey>, IEnumerable<TKey>, ICollection
        {
            #region Fields
            
            private ListDictionaryBase<TKey, TValue> _owner;
            private ReadOnlyCollection<TKey> _assertedKeyValues;
            private IEqualityComparer<TKey> _equalityComparer;
            
            #endregion
            
            #region Properties
            
            /// <summary>
            /// 
            /// </summary>
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
            
            internal IEqualityComparer<TKey> EqualityComparer { get { return _equalityComparer; } }

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
            
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public IEnumerable<TKey> AsEnumerable()
            {
                if (_assertedKeyValues.Count == 0)
                    return _owner.ExplicitKeys;
                if (_owner.ExplicitKeys.Count == 0)
                    return _assertedKeyValues;
                
                return _owner.ExplicitKeys.Concat(_assertedKeyValues.Where(k => !_owner.ExplicitKeys.Contains(k, _equalityComparer)));
            }
            
            /// <summary>
            /// 
            /// </summary>
            /// <param name="array"></param>
            /// <param name="index"></param>
            public void CopyTo(TKey[] array, int index) { AsEnumerable().ToList().CopyTo(array, index); }
            
            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public IEnumerator<TKey> GetEnumerator() { return AsEnumerable().GetEnumerator(); }
            
            internal bool Contains(TKey value) { return AsEnumerable().Contains(value, _equalityComparer); }
            
            #region Explicit Members
            
            bool ICollection<TKey>.Contains(TKey value) { return AsEnumerable().Contains(value, _equalityComparer); }

            void ICollection.CopyTo(Array array, int arrayIndex) { AsEnumerable().ToArray().CopyTo(array, arrayIndex); }

            IEnumerator IEnumerable.GetEnumerator() { return AsEnumerable().ToArray().GetEnumerator(); }
            
            void ICollection<TKey>.Add(TKey value) { throw new NotSupportedException(); }

            void ICollection<TKey>.Clear() { throw new NotSupportedException(); }
                 
            bool ICollection<TKey>.Remove(TKey value) { throw new NotSupportedException(); }

            #endregion

            #endregion
        }
        
        #endregion
    }
}
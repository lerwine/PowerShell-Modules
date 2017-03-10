using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;

/// <summary>
/// 
/// </summary>
namespace Erwine.Leonard.T.GDIPlus
{
    public class CrawlComponentCollection<TKey> : IList<ICrawledComponent<TKey>> //, IList, INotifyCollectionChanged
        where TKey : IComparable
    {
        private object _syncRoot = new object();
        private IEqualityComparer<TKey> _keyComparer;
        private IList<ICrawledComponent<TKey>> _innerList = new List<ICrawledComponent<TKey>>();
        private ICrawlComponentContainer<TKey> _owner;

        #region IList<ICrawledComponent<TKey>> Implementation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ICrawledComponent<TKey> this[int index]
        {
            get { return _innerList[index]; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                ICrawledComponent<TKey> oldItem;
                Monitor.Enter(_syncRoot);
                try
                {
                    int k = _IndexOfKey(value.Key);
                    if (k > -1 && index != k)
                        throw new ArgumentOutOfRangeException("index", value.Key, "Another item with that key already exist.");
                    oldItem = _SetComponent(index, value);
                } finally { Monitor.Exit(_syncRoot); }
                if (oldItem != null)
                    RaiseItemReplaced(oldItem, value, index);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(ICrawledComponent<TKey> item) { return _innerList.IndexOf(item); }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, ICrawledComponent<TKey> item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            Monitor.Enter(_syncRoot);
            try
            {
                if (_IndexOfKey(item.Key) > -1)
                    throw new ArgumentOutOfRangeException("index", item.Key, "Another item with that key already exist.");
                _InsertComponent(index, item);
            } finally { Monitor.Exit(_syncRoot); }
            RaiseItemAdded(item, index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            ICrawledComponent<TKey> item;
            Monitor.Enter(_syncRoot);
            try
            {
                item = _RemoveComponent(index);
            } finally { Monitor.Exit(_syncRoot); }
            RaiseItemRemoved(item, index);
        }
        
        #endregion
        
        #region ICollection<ICrawledComponent<TKey>> Implementation
        
        /// <summary>
        /// 
        /// </summary>
        public int Count { get { return _innerList.Count; } }

        bool ICollection<ICrawledComponent<TKey>>.IsReadOnly { get { return false; } }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(ICrawledComponent<TKey> item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            int index;
            Monitor.Enter(_syncRoot);
            try
            {
                if (_IndexOfKey(item.Key) > -1)
                    throw new ArgumentOutOfRangeException("index", item.Key, "Another item with that key already exist.");
                index = _AddComponent(item);
            } finally { Monitor.Exit(_syncRoot); }
            RaiseItemAdded(item, index);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            ICrawledComponent<TKey>[] removedItems;
            Monitor.Enter(_syncRoot);
            try
            {
                if (_innerList.Count == 0)
                    return;
                removedItems = _innerList.ToArray();
                _innerList.Clear();
            } finally { Monitor.Exit(_syncRoot); }
            foreach (ICrawledComponent<TKey> item in removedItems)
                item.Parent = null;
            RaiseItemsRemoved(removedItems, 0);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(ICrawledComponent<TKey> item) { return item != null && _innerList.Contains(item); }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(ICrawledComponent<TKey>[] array, int arrayIndex) { _innerList.CopyTo(array, arrayIndex); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(ICrawledComponent<TKey> item)
        {
            if (item == null)
                return false;
            int index;
            Monitor.Enter(_syncRoot);
            try
            {
                if ((index = _innerList.IndexOf(item)) < 0)
                    return false;
                _RemoveComponent(index);
            } finally { Monitor.Exit(_syncRoot); }
            RaiseItemRemoved(item, index);
            return true;
        }
        
        #endregion
        
        #region IEnumerable Implementations
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ICrawledComponent<TKey>> GetEnumerator() { return _innerList.GetEnumerator(); }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return (_innerList as System.Collections.IEnumerable).GetEnumerator(); }

        #endregion
        
        /*
        public ICrawledComponent<TKey> this[TKey key]
        {
            get { return _innerList; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                if (!_keyComparer.Equals(value, key))
                    throw new ArgumentOutOfRangeException("key", key, "Key does not match the item being added.");

                Monitor.Enter(_syncRoot);
                try
                {
                    int index = _IndexOfKey(key);
                    if (index < 0)
                        _AddComponent(value);
                    else
                        _SetComponent(index, value);
                } finally { Monitor.Exit(_syncRoot); }
            }
        }
        */

        private void _InsertComponent(int index, ICrawledComponent<TKey> item)
        {
            _innerList.Insert(index, item);
            if (item.Parent != null && !ReferenceEquals(item.Parent, _owner))
                _innerList[index] = item.Clone(_owner);
            else
                _innerList[index].Parent = _owner;
        }

        private int _AddComponent(ICrawledComponent<TKey> item)
        {
            int index = _innerList.Count;
            _innerList.Add(item);
            if (item.Parent != null && !ReferenceEquals(item.Parent, _owner))
                _innerList[index] = item.Clone(_owner);
            else
                _innerList[index].Parent = _owner;
            return index;
        }

        private  ICrawledComponent<TKey> _RemoveComponent(int index)
        {
            ICrawledComponent<TKey> oldItem = _innerList[index];
            _innerList.RemoveAt(index);
            oldItem.Parent = null;
            return oldItem;
        }

        private ICrawledComponent<TKey> _SetComponent(int index, ICrawledComponent<TKey> item)
        {
            ICrawledComponent<TKey> oldItem = _innerList[index];
            if (ReferenceEquals(oldItem, item))
                return null;
            int k = _IndexOfKey(item.Key);
            if (k > -1 && k != index)
                throw new ArgumentOutOfRangeException("item", item.Key, "Another item withthe same key already exists.");
            _innerList[index] = item;
            if (item.Parent != null && !ReferenceEquals(item.Parent, _owner))
                _innerList[index] = item.Clone(_owner);
            else
                _innerList[index].Parent = _owner;
            oldItem.Parent = null;
            return oldItem;
        }

        private int _IndexOfKey(TKey key)
        {
            for (int i = 0; i < _innerList.Count; i++)
            {
                if (_keyComparer.Equals(_innerList[i].Key, key))
                    return i;
            }
            return -1;
        }

        private void RaiseItemAdded(ICrawledComponent<TKey> newItem, int index)
        {
            throw new NotImplementedException();
        }

        private void RaiseItemReplaced(ICrawledComponent<TKey> oldItem, ICrawledComponent<TKey> newItem, int index)
        {
            throw new NotImplementedException();
        }

        private void RaiseItemRemoved(ICrawledComponent<TKey> oldItem, int index)
        {
            throw new NotImplementedException();
        }

        private void RaiseItemsRemoved(ICrawledComponent<TKey>[] oldItems, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="existingItems"></param>
        protected virtual void OnItemsInitialized(ICrawledComponent<TKey>[] existingItems)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public CrawlComponentCollection() : this(null, null) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="keyComparer"></param>
        public CrawlComponentCollection(ICrawlComponentContainer<TKey> owner, IEqualityComparer<TKey> keyComparer)
        {
            _owner = owner;
            _keyComparer = (keyComparer == null) ? EqualityComparer<TKey>.Default : keyComparer;
        }

        internal ICrawledComponent<TKey>[] Initialize(ICrawlComponentContainer<TKey> owner, IEqualityComparer<TKey> keyComparer)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");

            List<ICrawledComponent<TKey>> existingItems = new List<ICrawledComponent<TKey>>();
            Monitor.Enter(_syncRoot);
            try
            {
                if (!ReferenceEquals(owner.ItemCollection, this))
                    throw new InvalidOperationException("Component container does not own this collection.");
                
                if (_owner != null)
                {
                    if (!ReferenceEquals(_owner, owner))
                        throw new InvalidOperationException("Collection owner cannot be changed once it is initialized.");
                    _keyComparer = (keyComparer == null) ? EqualityComparer<TKey>.Default : keyComparer;
                }
                else if (keyComparer != null)
                    _keyComparer = keyComparer;
                _owner = owner;
                existingItems.AddRange(_innerList);
            } finally { Monitor.Exit(_syncRoot); }
            if (existingItems.Count > 0)
            {
                for (int i = 0; i < existingItems.Count; i++)
                {
                    try
                    {
                        int index = _innerList.IndexOf(existingItems[i]);
                        if (index < 0)
                        {
                            existingItems.RemoveAt(i);
                            i--;
                            continue;
                        }
                        if (existingItems[i].Parent != null)
                        {
                            if (ReferenceEquals(existingItems[i].Parent, _owner))
                            {
                                existingItems.RemoveAt(i);
                                i--;
                            }
                            else
                            {
                                existingItems[i] = _innerList[index].Clone(_owner);
                                _innerList[index] = existingItems[i];
                            }
                            continue;
                        }
                    } finally { Monitor.Exit(_syncRoot); }
                    existingItems[i].Parent = _owner;
                }
            }
            OnItemsInitialized(existingItems.ToArray());
            return existingItems.ToArray();
        }
    }
}

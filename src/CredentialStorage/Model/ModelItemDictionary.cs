using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;

namespace CredentialStorage.Model
{
    public class ModelItemDictionary<TChild> : IDictionary<Guid, TChild>, IList<TChild>, ICollection<KeyValuePair<Guid, TChild>>, IDictionary, IList
        where TChild : class, IModelItem
    {
        private object _syncRoot = new object();
        private Dictionary<Guid, TChild> _innerDictionary = new Dictionary<Guid, TChild>();
        private List<TChild> _innerList = new List<TChild>();

        public TChild this[int index]
        {
            get
            {
                TChild result;
                Monitor.Enter(_syncRoot);
                try { result = _innerList[index]; }
                finally { Monitor.Exit(_syncRoot); }
                return result;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                Monitor.Enter(_syncRoot);
                try
                {
                    TChild removed = _innerList[index];
                    if (removed.ID.Equals(value.ID))
                    {
                        if (ReferenceEquals(value, removed))
                            return;
                        OnAddingItem(value, index);
                        _innerDictionary[value.ID] = value;
                    }
                    else
                    {
                        OnAddingItem(value, index);
                        _innerDictionary.Remove(removed.ID);
                        _innerDictionary.Add(value.ID, value);    
                    }
                    _innerList[index] = value;
                    OnItemsRemoved(new TChild[] { removed });
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public TChild this[Guid key]
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_innerDictionary.ContainsKey(key))
                        return _innerDictionary[key];
                }
                finally { Monitor.Exit(_syncRoot); }
                return null;
            }
        }

        TChild IDictionary<Guid, TChild>.this[Guid key]
        {
            get { return this[key]; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                Set(key, value);
            }
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                Monitor.Enter(_syncRoot);
                try
                {
                    TChild added = (TChild)value;
                    TChild removed = _innerList[index];
                    if (removed.ID.Equals(added.ID))
                    {
                        if (ReferenceEquals(value, removed))
                            return;
                        OnAddingItem(added, index);
                        _innerDictionary[added.ID] = added;
                    }
                    else
                    {
                        OnAddingItem(added, index);
                        _innerDictionary.Remove(removed.ID);
                        _innerDictionary.Add(added.ID, added);    
                    }
                    _innerList[index] = added;
                    OnItemsRemoved(new TChild[] { removed });
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                if (key != null && key is Guid)
                    return this[(Guid)key];
                return null;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                Set((Guid)key, (TChild)value);
            }
        }

        public int Count { get { return _innerList.Count; } }

        bool IList.IsFixedSize { get { return false; } }

        bool IDictionary.IsFixedSize { get { return false; } }

        bool ICollection<TChild>.IsReadOnly { get { return false; } }

        bool IList.IsReadOnly { get { return false; } }

        bool IDictionary.IsReadOnly { get { return false; } }

        bool ICollection<KeyValuePair<Guid, TChild>>.IsReadOnly { get { return false; } }

        bool ICollection.IsSynchronized { get { return true; } }

        public ICollection<Guid> Keys { get { return _innerDictionary.Keys; } }

        ICollection IDictionary.Keys { get { return ((IDictionary)_innerDictionary).Keys; } }

        public object SyncRoot { get { return _syncRoot; } }

        ICollection<TChild> IDictionary<Guid, TChild>.Values { get { return _innerDictionary.Values; } }
        
        ICollection IDictionary.Values { get { return ((IDictionary)_innerDictionary).Values; } }

        public void Add(TChild item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            Monitor.Enter(_syncRoot);
            try
            {
                OnAddingItem(item, _innerList.Count);
                _innerDictionary.Add(item.ID, item);
                _innerList.Add(item);
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        int IList.Add(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            TChild item = (TChild)value;
            int index;
            Monitor.Enter(_syncRoot);
            try
            {
                index = _innerList.Count;
                OnAddingItem(item, index);
                _innerDictionary.Add(item.ID, item);
                _innerList.Add(item);
            }
            finally { Monitor.Exit(_syncRoot); }
            return index;
        }

        private void _Add(Guid key, TChild value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (!value.ID.Equals(key))
                throw new InvalidOperationException("Key must match child ID");

            Monitor.Enter(_syncRoot);
            try
            {
                OnAddingItem(value, _innerList.Count);
                _innerDictionary.Add(value.ID, value);
                _innerList.Add(value);
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        void IDictionary<Guid, TChild>.Add(Guid key, TChild value) { _Add(key, value); }
        void IDictionary.Add(object key, object value) { _Add((Guid)key, (TChild)value); }

        void ICollection<KeyValuePair<Guid, TChild>>.Add(KeyValuePair<Guid, TChild> item) { _Add(item.Key, item.Value); }

        public void Clear()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                TChild[] removedItems = _innerList.ToArray();
                if (removedItems.Length > 0)
                {
                    _innerDictionary.Clear();
                    _innerList.Clear();
                    OnItemsRemoved(removedItems);
                }
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool Contains(TChild item)
        {
            if (item == null)
                return false;
            Monitor.Enter(_syncRoot);
            try
            {
                return _innerDictionary.ContainsKey(item.ID) && ReferenceEquals(item, _innerDictionary[item.ID]);
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        bool IList.Contains(object value)
        {
            if (value == null || !(value is TChild))
                return false;
            Monitor.Enter(_syncRoot);
            try
            {
                Guid id = ((TChild)value).ID;
                return _innerDictionary.ContainsKey(id) && ReferenceEquals(value, _innerDictionary[id]);
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        bool ICollection<KeyValuePair<Guid, TChild>>.Contains(KeyValuePair<Guid, TChild> item)
        {
            if (item.Value == null || !item.Key.Equals(item.Value.ID))
                return false;
            Monitor.Enter(_syncRoot);
            try
            {
                return _innerDictionary.ContainsKey(item.Key) && ReferenceEquals(item.Value, _innerDictionary[item.Key]);
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool ContainsKey(Guid key)
        {
            Monitor.Enter(_syncRoot);
            try { return _innerDictionary.ContainsKey(key); }
            finally { Monitor.Exit(_syncRoot); }
        }

        bool IDictionary.Contains(object key)
        {
            if (key == null || !(key is Guid))
                return false;
            Monitor.Enter(_syncRoot);
            try { return _innerDictionary.ContainsKey((Guid)key); }
            finally { Monitor.Exit(_syncRoot); }
        }

        public void CopyTo(TChild[] array, int arrayIndex)
        {
            Monitor.Enter(_syncRoot);
            try { _innerList.CopyTo(array, arrayIndex); }
            finally { Monitor.Exit(_syncRoot); }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            Monitor.Enter(_syncRoot);
            try { ((IList)_innerList).CopyTo(array, index); }
            finally { Monitor.Exit(_syncRoot); }
        }

        void ICollection<KeyValuePair<Guid, TChild>>.CopyTo(KeyValuePair<Guid, TChild>[] array, int arrayIndex)
        {
            Monitor.Enter(_syncRoot);
            try { ((IDictionary<Guid, TChild>)_innerDictionary).CopyTo(array, arrayIndex); }
            finally { Monitor.Exit(_syncRoot); }
        }

        public IEnumerator<TChild> GetEnumerator()
        {
            Monitor.Enter(_syncRoot);
            try { return _innerList.GetEnumerator(); }
            finally { Monitor.Exit(_syncRoot); }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            Monitor.Enter(_syncRoot);
            try { return ((IList)_innerList).GetEnumerator(); }
            finally { Monitor.Exit(_syncRoot); }
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            Monitor.Enter(_syncRoot);
            try { return ((IDictionary)_innerDictionary).GetEnumerator(); }
            finally { Monitor.Exit(_syncRoot); }
        }

        IEnumerator<KeyValuePair<Guid, TChild>> IEnumerable<KeyValuePair<Guid, TChild>>.GetEnumerator()
        {
            Monitor.Enter(_syncRoot);
            try { return ((IDictionary<Guid, TChild>)_innerDictionary).GetEnumerator(); }
            finally { Monitor.Exit(_syncRoot); }
        }

        public int IndexOf(TChild item)
        {
            Monitor.Enter(_syncRoot);
            try { return _innerList.IndexOf(item); }
            finally { Monitor.Exit(_syncRoot); }
        }

        int IList.IndexOf(object value)
        {
            if (value == null || !(value is TChild))
                return -1;
            Monitor.Enter(_syncRoot);
            try { return _innerList.IndexOf((TChild)value); }
            finally { Monitor.Exit(_syncRoot); }
        }

        public void Insert(int index, TChild item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            Monitor.Enter(_syncRoot);
            try
            {
                OnAddingItem(item, _innerList.Count);
                _innerDictionary.Add(item.ID, item);
                _innerList.Insert(index, item);
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        void IList.Insert(int index, object value)
        {
            if (value == null)
                throw new ArgumentNullException("item");

            TChild item = (TChild)value;
            Monitor.Enter(_syncRoot);
            try
            {
                OnAddingItem(item, _innerList.Count);
                _innerDictionary.Add(item.ID, item);
                _innerList.Insert(index, item);
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        protected virtual void OnAddingItem(TChild item, int index) { }

        protected virtual void OnItemsRemoved(TChild[] removedItems) { }

        public bool Remove(TChild item)
        {
            if (item == null)
                return false;

            Monitor.Enter(_syncRoot);
            try
            {
                int index = _innerList.IndexOf(item);
                if (index < 0)
                    return false;
                item = _innerList[index];
                _innerList.RemoveAt(index);
                _innerDictionary.Remove(item.ID);
                OnItemsRemoved(new TChild[] { item });
            }
            finally { Monitor.Exit(_syncRoot); }
            return true;
        }

        public bool Remove(Guid key)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (!_innerDictionary.ContainsKey(key))
                    return false;
                TChild item = _innerDictionary[key];
                int index = _innerList.IndexOf(item);
                _innerList.RemoveAt(index);
                _innerDictionary.Remove(key);
                OnItemsRemoved(new TChild[] { item });
            }
            finally { Monitor.Exit(_syncRoot); }
            return true;
        }

        public void RemoveAt(int index)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                TChild item = _innerList[index];
                _innerList.RemoveAt(index);
                _innerDictionary.Remove(item.ID);
                OnItemsRemoved(new TChild[] { item });
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        void IList.Remove(object value)
        {
            if (value == null || !(value is TChild))
                return;
                
            Monitor.Enter(_syncRoot);
            try
            {
                int index = _innerList.IndexOf((TChild)value);
                if (index < 0)
                    return;
                TChild item = _innerList[index];
                _innerList.RemoveAt(index);
                _innerDictionary.Remove(item.ID);
                OnItemsRemoved(new TChild[] { item });
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        void IDictionary.Remove(object key)
        {
            if (key == null || !(key is Guid))
                return;
            Monitor.Enter(_syncRoot);
            try
            {
                Guid g = (Guid)key;
                if (!_innerDictionary.ContainsKey(g))
                    return;
                TChild item = _innerDictionary[g];
                int index = _innerList.IndexOf(item);
                _innerList.RemoveAt(index);
                _innerDictionary.Remove(g);
                OnItemsRemoved(new TChild[] { item });
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        bool ICollection<KeyValuePair<Guid, TChild>>.Remove(KeyValuePair<Guid, TChild> item)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (item.Value == null || !_innerDictionary.ContainsKey(item.Key) || !ReferenceEquals(item.Value, _innerDictionary[item.Key]))
                    return false;
                int index = _innerList.IndexOf(item.Value);
                _innerList.RemoveAt(index);
                _innerDictionary.Remove(item.Key);
                OnItemsRemoved(new TChild[] { item.Value });
            }
            finally { Monitor.Exit(_syncRoot); }
            return true;
        }

        private void Set(Guid key, TChild value)
        {
            if (!value.ID.Equals(key))
                throw new InvalidOperationException("Key must match item ID");

            Monitor.Enter(_syncRoot);
            try
            {
                if (_innerDictionary.ContainsKey(key))
                {
                    TChild removed = _innerDictionary[key];
                    if (ReferenceEquals(removed, value))
                        return;
                    int index = _innerList.IndexOf(removed);
                    OnAddingItem(value, index);
                    _innerDictionary[key] = value;
                    _innerList[index] = value;
                    OnItemsRemoved(new TChild[] { removed });
                }
                else
                {
                    OnAddingItem(value, _innerList.Count);
                    _innerDictionary.Add(value.ID, value);
                    _innerList.Add(value);
                }
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool TryGetValue(Guid key, out TChild value)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_innerDictionary.ContainsKey(key))
                {
                    value = _innerDictionary[key];
                    return true;
                }
            }
            finally { Monitor.Exit(_syncRoot); }
            value = default(TChild);
            return false;
        }
    }
}
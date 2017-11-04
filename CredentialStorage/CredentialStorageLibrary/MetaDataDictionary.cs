using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using System.Xml.Serialization;

namespace CredentialStorageLibrary
{
    public class MetaDataDictionary : IList<MetaDataItem>, IDictionary<string, object>, IList, IDictionary
    {
        private InnerList _innerList = new InnerList();

        public ICollection<string> Keys { get { return _innerList.Keys; } }

        public ICollection<object> Values
        {
            get
            {
                return _dictionary_Value.Values;
            }
        }

        public int Count
        {
            get
            {
                return _dictionary_Value.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return _dictionary_Value.IsReadOnly;
            }
        }

        public bool IsFixedSize
        {
            get
            {
                return _items_Value2.IsFixedSize;
            }
        }

        public object SyncRoot
        {
            get
            {
                return _items_Value2.SyncRoot;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return _items_Value2.IsSynchronized;
            }
        }

        ICollection IDictionary.Keys
        {
            get
            {
                return _dictionary_Value2.Keys;
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                return _dictionary_Value2.Values;
            }
        }

        public object this[object key]
        {
            get
            {
                return _dictionary_Value2[key];
            }

            set
            {
                _dictionary_Value2[key] = value;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return _items_Value2[index];
            }

            set
            {
                _items_Value2[index] = value;
            }
        }

        public MetaDataItem this[int index]
        {
            get
            {
                return _innerList[index];
            }

            set
            {
                _innerList[index] = value;
            }
        }

        public object this[string key]
        {
            get
            {
                return _dictionary_Value[key];
            }

            set
            {
                _dictionary_Value[key] = value;
            }
        }

        public MetaDataDictionary()
        {
        }
        
        sealed class InnerList : KeyedCollection<NormalizingText, MetaDataItem>, IDictionary<NormalizingText, MetaDataItem>
        {
            private object _syncRoot = new object();

            public object SyncRoot { get { return _syncRoot; } }

            public ICollection<NormalizingText> Keys { get { return Dictionary.Keys; } }

            public ICollection<MetaDataItem> Values { get { return Dictionary.Values; } }

            bool ICollection<KeyValuePair<NormalizingText, MetaDataItem>>.IsReadOnly { get { return false; } }
            
            public InnerList() : base(NormalizingText.DefaultComparer) { }

            ~InnerList()
            {
                foreach (MetaDataItem item in Items)
                    item.UnLockKey(_syncRoot);
            }

            protected override NormalizingText GetKeyForItem(MetaDataItem item)
            {
                if (item == null)
                    return new NormalizingText();
                return (item == null) ? new NormalizingText() : new NormalizingText(item.Key);
            }
            
            protected override void ClearItems()
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    try
                    {
                        foreach (MetaDataItem item in Items)
                            item.UnLockKey(_syncRoot);
                        base.ClearItems();
                    }
                    finally
                    {
                        foreach (MetaDataItem item in Items)
                            item.LockKey(_syncRoot);
                    }
                }
                finally { Monitor.Exit(_syncRoot); }
            }

            protected override void InsertItem(int index, MetaDataItem item)
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    try
                    {
                        if (!Items.Any(i => ReferenceEquals(i, item)))
                            item.LockKey(_syncRoot);
                        base.InsertItem(index, item);
                    }
                    finally
                    {
                        if (!Items.Any(i => ReferenceEquals(i, item)))
                            item.UnLockKey(_syncRoot);
                    }
                }
                finally { Monitor.Exit(_syncRoot); }
            }

            protected override void RemoveItem(int index)
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    MetaDataItem removed = Items[index];
                    try
                    {
                        base.RemoveItem(index);
                    }
                    finally
                    {
                        if (!Items.Any(i => ReferenceEquals(i, removed)))
                            removed.UnLockKey(_syncRoot);
                    }
                }
                finally { Monitor.Exit(_syncRoot); }
            }

            protected override void SetItem(int index, MetaDataItem item)
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    MetaDataItem replaced = Items[index];
                    if (ReferenceEquals(item, replaced))
                        return;
                    try
                    {
                        if (!Items.Any(i => ReferenceEquals(i, item)))
                            item.LockKey(_syncRoot);
                        base.SetItem(index, item);
                    }
                    finally
                    {
                        if (!Items.Any(i => ReferenceEquals(i, replaced)))
                            replaced.UnLockKey(_syncRoot);
                        if (!Items.Any(i => ReferenceEquals(i, item)))
                            item.UnLockKey(_syncRoot);
                    }
                }
                finally { Monitor.Exit(_syncRoot); }
            }

            public bool ContainsKey(NormalizingText key) { return Dictionary.ContainsKey(key); }

            void IDictionary<NormalizingText, MetaDataItem>.Add(NormalizingText key, MetaDataItem value)
            {
                if (key == null)
                    throw new ArgumentNullException("key");
                if (value == null)
                    throw new ArgumentNullException("value");
                if (!value.Key.Equals(key))
                    value.Key = key;
                Add(value);
            }

            bool IDictionary<NormalizingText, MetaDataItem>.Remove(NormalizingText key)
            {
                throw new NotImplementedException();
            }

            bool IDictionary<NormalizingText, MetaDataItem>.TryGetValue(NormalizingText key, out MetaDataItem value)
            {
                throw new NotImplementedException();
            }

            void ICollection<KeyValuePair<NormalizingText, MetaDataItem>>.Add(KeyValuePair<NormalizingText, MetaDataItem> item)
            {
                throw new NotImplementedException();
            }

            void ICollection<KeyValuePair<NormalizingText, MetaDataItem>>.Clear()
            {
                throw new NotImplementedException();
            }

            bool ICollection<KeyValuePair<NormalizingText, MetaDataItem>>.Contains(KeyValuePair<NormalizingText, MetaDataItem> item)
            {
                throw new NotImplementedException();
            }

            void ICollection<KeyValuePair<NormalizingText, MetaDataItem>>.CopyTo(KeyValuePair<NormalizingText, MetaDataItem>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            bool ICollection<KeyValuePair<NormalizingText, MetaDataItem>>.Remove(KeyValuePair<NormalizingText, MetaDataItem> item)
            {
                throw new NotImplementedException();
            }

            IEnumerator<KeyValuePair<NormalizingText, MetaDataItem>> IEnumerable<KeyValuePair<NormalizingText, MetaDataItem>>.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }

        public bool ContainsKey(string key)
        {
            return _dictionary_Value.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            _dictionary_Value.Add(key, value);
        }

        public bool Remove(string key)
        {
            return _dictionary_Value.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return _dictionary_Value.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            _dictionary_Value.Add(item);
        }

        public void Clear()
        {
            _dictionary_Value.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return _dictionary_Value.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            _dictionary_Value.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return _dictionary_Value.Remove(item);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _dictionary_Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary_Value.GetEnumerator();
        }

        public int IndexOf(MetaDataItem item)
        {
            return _innerList.IndexOf(item);
        }

        public void Insert(int index, MetaDataItem item)
        {
            _innerList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _innerList.RemoveAt(index);
        }

        public void Add(MetaDataItem item)
        {
            _innerList.Add(item);
        }

        public bool Contains(MetaDataItem item)
        {
            return _innerList.Contains(item);
        }

        public void CopyTo(MetaDataItem[] array, int arrayIndex)
        {
            _innerList.CopyTo(array, arrayIndex);
        }

        public bool Remove(MetaDataItem item)
        {
            return _innerList.Remove(item);
        }

        IEnumerator<MetaDataItem> IEnumerable<MetaDataItem>.GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        public int Add(object value)
        {
            return _items_Value2.Add(value);
        }

        public bool Contains(object value)
        {
            return _items_Value2.Contains(value);
        }

        public int IndexOf(object value)
        {
            return _items_Value2.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            _items_Value2.Insert(index, value);
        }

        public void Remove(object value)
        {
            _items_Value2.Remove(value);
        }

        public void CopyTo(Array array, int index)
        {
            _items_Value2.CopyTo(array, index);
        }

        public void Add(object key, object value)
        {
            _dictionary_Value2.Add(key, value);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return _dictionary_Value2.GetEnumerator();
        }
    }
}
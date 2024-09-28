using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;
using System.Threading;

namespace NetworkUtility
{
    public class UriQueryList : IList<UriQueryItem>, IDictionary<string, string>, IList, IDictionary, INotifyCollectionChanged, INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public const string PropertyName_Keys = "Keys";

        public const string PropertyName_Values = "Values";
        
        public const string PropertyName_Count = "Count";

        private static StringComparer _keyComparer = StringComparer.InvariantCultureIgnoreCase;
        private object _syncRoot = new object();
        private int _count = 0;
        private Collection<UriQueryItem> _allItems = new Collection<UriQueryItem>();
        private Collection<int> _keyItemIndexes = new Collection<int>();
        private KeyCollection _keys;
        private ValueCollection _values;
        private Queue<Tuple<Delegate, object[]>> _queuedEvents = null;
        
        public int Count
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return _count; }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        int ICollection<KeyValuePair<string, string>>.Count
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return _keyItemIndexes.Count; }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        bool IList.IsFixedSize { get { return false; } }

        bool IDictionary.IsFixedSize { get { return false; } }

        bool ICollection<UriQueryItem>.IsReadOnly { get { return false; } }

        bool ICollection<KeyValuePair<string, string>>.IsReadOnly { get { return false; } }

        bool IList.IsReadOnly { get { return false; } }

        bool IDictionary.IsReadOnly { get { return false; } }

        bool ICollection.IsSynchronized { get { return true; } }

        public ICollection<string> Keys { get { return _keys; } }

        ICollection IDictionary.Keys { get { return _keys; } }

        object ICollection.SyncRoot { get { return _syncRoot; } }

        public ICollection<string> Values { get { return _values; } }

        ICollection IDictionary.Values { get { return _values; } }

        private T GetSafe<T>(Func<T> func) { return EventQueueManager.Get(_syncRoot, func); }

        private void InvokeSafe(Action method) { EventQueueManager.Invoke(_syncRoot, method); }

        public UriQueryItem this[int index]
        {
            get { return GetSafe<UriQueryItem>(() => _allItems[index]); }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                if (index < 0)
                    throw new IndexOutOfRangeException();
                InvokeSafe(() =>
                {
                    if (index == _allItems.Count)
                    {
                        Add(value);
                        return;
                    }
                    UriQueryItem oldItem = _allItems[index];
                    if (ReferenceEquals(oldItem, value))
                        return;
                    for (int i = 0; i < _allItems.Count; i++)
                    {
                        if (ReferenceEquals(_allItems[i], value))
                        {
                            MoveItem(i, index);
                            return;
                        }
                    }
                    oldItem.PropertyChanged -= Item_PropertyChanged;
                    _allItems[index] = value;
                    value.PropertyChanged += Item_PropertyChanged;
                    if (_keyComparer.Equals(oldItem.Key, value.Key))
                    {
                        if (IndexOf(oldItem.Key) == index)
                        {
                            if (oldItem.Key != value.Key)
                                EventQueueManager.Raise(PropertyName_Keys, RaisePropertyChanged);
                            if ((oldItem.Value == null) ? value.Value != null : (value.Value == null || oldItem.Value != value.Value))
                                EventQueueManager.Raise(PropertyName_Values, RaisePropertyChanged);
                        }
                    }
                    else
                    {
                        int keyIndex = IndexOf(value.Key);
                        if (keyIndex > index)
                            _keyItemIndexes.Remove(keyIndex);
                        else if (keyIndex > -1)
                            _keyItemIndexes.Remove(index);
                        for (int i = index + 1; i < _allItems.Count; i++)
                        {
                            if (_keyComparer.Equals(_allItems[i].Key, oldItem.Key))
                            {
                                _keyItemIndexes.Add(i);
                                break;
                            }
                        }
                        EventQueueManager.Raise(PropertyName_Keys, RaisePropertyChanged);
                        EventQueueManager.Raise(PropertyName_Values, RaisePropertyChanged);
                    }
                });
            }
        }

        object IList.this[int index] { get { return this[index]; } set { this[index] = AssertItemValue(value); } }

        public string this[string key]
        {
            get
            {
                if (key == null)
                    return null;
                return GetSafe<string>(() =>
                {
                    int index = IndexOf(key);
                    if (index < 0)
                        return null;
                    return _allItems[index].Value;
                });
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException();

                InvokeSafe(() =>
                {
                    int index = IndexOf(key);
                    if (index < 0)
                    {
                        index = _allItems.Count;
                        _allItems.Add(new UriQueryItem(key, value));
                        _keyItemIndexes.Add(index);
                        EventQueueManager.Raise(PropertyName_Keys, RaisePropertyChanged);
                        EventQueueManager.Raise(PropertyName_Values, RaisePropertyChanged);
                        EventQueueManager.Raise(PropertyName_Count, RaisePropertyChanged);
                    }
                    else
                    {
                        UriQueryItem item = _allItems[index];
                        if ((item.Value == null) ? value == null : value != null && item.Value == value)
                            return;
                        item.Value = value;
                        EventQueueManager.Raise(PropertyName_Values, RaisePropertyChanged);
                    }
                });
            }
        }

        object IDictionary.this[object key] { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public UriQueryList()
        {
            _keys = new KeyCollection(this);
            _values = new ValueCollection(this);
        }

        private static UriQueryItem AssertItemValue(object obj)
        {
            try
            {
                if (!(obj == null || obj is UriQueryItem))
                {
                    if (obj is PSObject)
                        return AssertItemValue(((PSObject)obj).BaseObject);
                    if (obj is IConvertible)
                        return (UriQueryItem)(((IConvertible)obj).ToType(typeof(UriQueryItem), System.Globalization.CultureInfo.CurrentCulture));
                }
            }
            catch 
            {
                try { return (UriQueryItem)obj; }
                catch { }
                throw;
            }

            return (UriQueryItem)obj;
        }

        private static string AssertStringValue(object obj)
        {
            if (obj == null)
                return null;
            
            if (obj is PSObject)
                return AssertStringValue(((PSObject)obj).BaseObject);

            if (obj is IConvertible)
            {
                try { return (string)(((IConvertible)obj).ToString(System.Globalization.CultureInfo.CurrentCulture)); }
                catch 
                {
                    try { return (string)obj; } catch { }
                    throw;
                }
            }

            return (string)obj;
        }

        private static string AsStringValue(object obj)
        {
            if (obj == null)
                return null;
            
            if (obj is PSObject)
                return AsStringValue(((PSObject)obj).BaseObject);

            if (obj is IConvertible)
            {
                try { return (string)(((IConvertible)obj).ToString(System.Globalization.CultureInfo.CurrentCulture)); }
                catch { }
            }

            try { return (string)obj; } catch { }

            return obj as string;
        }

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void RaisePropertyChanged(string propertyName)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(UriQueryItem item)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(string key)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, UriQueryItem item)
        {
            throw new NotImplementedException();
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void MoveItem(int i, int index)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Add(UriQueryItem item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(UriQueryItem item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(UriQueryItem[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(UriQueryItem item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<UriQueryItem> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public void Add(string key, string value)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out string value)
        {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        int IList.Add(object value)
        {
            throw new NotImplementedException();
        }

        bool IList.Contains(object value)
        {
            throw new NotImplementedException();
        }

        int IList.IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        void IList.Remove(object value)
        {
            throw new NotImplementedException();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        bool IDictionary.Contains(object key)
        {
            throw new NotImplementedException();
        }

        void IDictionary.Add(object key, object value)
        {
            throw new NotImplementedException();
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        void IDictionary.Remove(object key)
        {
            throw new NotImplementedException();
        }

#warning Not implemented

        public int CompareTo(UriQueryList other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj) { return CompareTo(obj as UriQueryList); }

        public bool Equals(UriQueryList other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj) { return Equals(obj as UriQueryList); }
		
		public override int GetHashCode() { return ToString().GetHashCode(); }
		
		public override string ToString()
		{
			Monitor.Enter(_syncRoot);
			try
			{
				throw new NotImplementedException();
			}
			finally { Monitor.Exit(_syncRoot); }
		}

        class KeyCollection : ICollection<string>, ICollection
        {
            private object _syncRoot;
            UriQueryList _parent;

            internal KeyCollection(UriQueryList parent)
            {
                _parent = parent;
                _syncRoot = parent._syncRoot;
            }

            public int Count
            {
                get
                {
                    Monitor.Enter(_syncRoot);
                    try { return _parent._keyItemIndexes.Count; }
                    finally { Monitor.Exit(_syncRoot); }
                }
            }

            bool ICollection<string>.IsReadOnly { get { return true; } }

            object ICollection.SyncRoot { get { return _syncRoot; } }


            bool ICollection.IsSynchronized { get { return true; } }

            void ICollection<string>.Add(string item) { throw new NotSupportedException(); }

            void ICollection<string>.Clear() { throw new NotSupportedException(); }

            public bool Contains(string item)
            {
                if (item == null)
                    return false;
                Monitor.Enter(_syncRoot);
                try { return GetKeys().Any(k => _keyComparer.Equals(item, k)); }
                finally { Monitor.Exit(_syncRoot); }
            }

            void ICollection<string>.CopyTo(string[] array, int arrayIndex) { GetKeys().ToList().CopyTo(array, arrayIndex); }

            void ICollection.CopyTo(Array array, int index) { GetKeys().ToArray().CopyTo(array, index); }

            public IEnumerator<string> GetEnumerator()
            {
                Monitor.Enter(_syncRoot);
                try { return GetKeys().GetEnumerator(); }
                finally { Monitor.Exit(_syncRoot); }
            }

            IEnumerator IEnumerable.GetEnumerator() { return GetKeys().ToArray().GetEnumerator(); }

            private IEnumerable<string> GetKeys()
            {
                foreach (int index in _parent._keyItemIndexes)
                    yield return _parent._allItems[index].Key;
            }

            bool ICollection<string>.Remove(string item) { throw new NotSupportedException(); }
        }

        class ValueCollection : ICollection<string>, ICollection
        {
            private object _syncRoot;
            UriQueryList _parent;

            internal ValueCollection(UriQueryList parent)
            {
                _parent = parent;
                _syncRoot = parent._syncRoot;
            }

            public int Count
            {
                get
                {
                    Monitor.Enter(_syncRoot);
                    try { return _parent._keyItemIndexes.Count; }
                    finally { Monitor.Exit(_syncRoot); }
                }
            }

            bool ICollection<string>.IsReadOnly { get { return true; } }

            object ICollection.SyncRoot { get { return _syncRoot; } }


            bool ICollection.IsSynchronized { get { return true; } }

            void ICollection<string>.Add(string item) { throw new NotSupportedException(); }

            void ICollection<string>.Clear() { throw new NotSupportedException(); }

            public bool Contains(string item)
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (item == null)
                        return GetValues().Any(v => v == null);
                    return GetValues().Any(v => v != null && v == item);
                }
                finally { Monitor.Exit(_syncRoot); }
            }

            void ICollection<string>.CopyTo(string[] array, int arrayIndex) { GetValues().ToList().CopyTo(array, arrayIndex); }

            void ICollection.CopyTo(Array array, int index) { GetValues().ToArray().CopyTo(array, index); }

            public IEnumerator<string> GetEnumerator()
            {
                Monitor.Enter(_syncRoot);
                try { return GetValues().GetEnumerator(); }
                finally { Monitor.Exit(_syncRoot); }
            }

            IEnumerator IEnumerable.GetEnumerator() { return GetValues().ToArray().GetEnumerator(); }

            private IEnumerable<string> GetValues()
            {
                foreach (int index in _parent._keyItemIndexes)
                    yield return _parent._allItems[index].Value;
            }

            bool ICollection<string>.Remove(string item) { throw new NotSupportedException(); }
        }
    }
}
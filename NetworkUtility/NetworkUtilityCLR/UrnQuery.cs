using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace NetworkUtilityCLR
{
    [Serializable]
    public class UrnQuery : IList<UrnQueryItem>, IList, INotifyPropertyChanged, INotifyCollectionChanged, IEquatable<UrnQuery>, IEquatable<string>, IComparable<UrnQuery>, IComparable<string>, IComparable, IConvertible
    {
        private object _syncRoot = new object();
        private int _count = 0;
        private List<UrnQueryItem> _innerList = new List<UrnQueryItem>();
        [NonSerialized]
        private string _stringValue = null;
        private StringComparer _keyComparer;

        private const string ErrMsg_ItemRefExists = "";

        public UrnQueryItem this[int index]
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return _innerList[index]; }
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                UrnQueryItem oldItem;
                Monitor.Enter(_syncRoot);
                try
                {
                    if (ReferenceEquals(_innerList[index], value))
                        return;

                    if (_innerList.Any(i => ReferenceEquals(value, i)))
                        throw new ArgumentException(ErrMsg_ItemRefExists, "value");
                    oldItem = _innerList[index];
                    oldItem.PropertyChanged -= Item_PropertyChanged;
                    _innerList[index] = value;
                    value.PropertyChanged += Item_PropertyChanged;
                    RaiseCollectionChanged(NotifyCollectionChangedAction.Replace, value, oldItem, index);
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (UrnQueryItem)value; }
        }

        public int Count
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return _count; }
                finally { Monitor.Exit(_syncRoot); }
            }
            private set
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_count == value)
                        return;
                    _count = value;
                    RaisePropertyChanged("Count");
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        bool ICollection<UrnQueryItem>.IsReadOnly { get { return false; } }

        bool IList.IsReadOnly { get { return false; } }

        bool IList.IsFixedSize { get { return false; } }

        object ICollection.SyncRoot { get { return _syncRoot; } }

        bool ICollection.IsSynchronized { get { return true; } }

        public UrnQuery(string query, StringComparer keyComparer, StringComparer valueComparer)
        {
            _keyComparer = (keyComparer == null) ? StringComparer.InvariantCultureIgnoreCase : keyComparer;
            if (String.IsNullOrEmpty(query))
                return;
            int startIndex = 0;
            if (query[0] == '?')
            {
                if (query.Length == 1)
                    return;
                startIndex = 1;
            }
            int nextIndex;
            while ((nextIndex = query.IndexOfAny(new char[] { '=', '&' }, startIndex)) >= 0)
            {
                char c = query[nextIndex];
                string key = Uri.UnescapeDataString(query.Substring(startIndex, nextIndex - startIndex));
                if (c == '&')
                {
                    _innerList.Add(new UrnQueryItem(key));
                    if ((startIndex = nextIndex + 1) >= query.Length)
                        break;
                }
                else
                {
                    nextIndex++;
                    if (nextIndex < query.Length && (startIndex = query.IndexOfAny(new char[] { '&' }, nextIndex)) >= 0)
                    {
                        _innerList.Add(new UrnQueryItem(key, Uri.UnescapeDataString(query.Substring(nextIndex, startIndex - nextIndex))));
                        startIndex++;
                        if (startIndex >= query.Length)
                            break;
                    }
                    else
                    {
                        _innerList.Add(new UrnQueryItem(key, Uri.UnescapeDataString(query.Substring(nextIndex))));
                        startIndex = query.Length;
                        break;
                    }
                }
            }

            if (startIndex < query.Length)
                _innerList.Add(new UrnQueryItem(Uri.UnescapeDataString(query.Substring(startIndex))));
        }

        public UrnQuery(string query, StringComparison keyComparison, StringComparison valueComparison) : this(query, UrnInfo.ToComparer(keyComparison), UrnInfo.ToComparer(valueComparison)) { }

        public UrnQuery(string query, StringComparer comparer) : this(query, comparer, comparer) { }

        public UrnQuery(string query, StringComparison comparison) : this(query, UrnInfo.ToComparer(comparison)) { }

        public UrnQuery(StringComparer keyComparer, StringComparer valueComparer) : this(null, keyComparer, valueComparer) { }

        public UrnQuery(StringComparison keyComparison, StringComparison valueComparison) : this(null, keyComparison, valueComparison) { }

        public UrnQuery(StringComparer comparer) : this(null as string, comparer, comparer) { }

        public UrnQuery(StringComparison comparison) : this(null as string, comparison) { }

        public UrnQuery(string query) : this(query, StringComparer.InvariantCultureIgnoreCase) { }

        public UrnQuery() : this(null as string) { }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args) { }

        protected void RaisePropertyChanged(string propertyName)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
                try { OnPropertyChanged(args); }
                finally { PropertyChanged?.Invoke(this, args); }
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args) { }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action) { RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action)); }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, UrnQueryItem changedItem) { RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItem)); }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, UrnQueryItem[] changedItems) { RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItems)); }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, UrnQueryItem changedItem, int index) { RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItem, index)); }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, UrnQueryItem[] changedItems, int startingIndex) { RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItems, startingIndex)); }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, UrnQueryItem newItem, UrnQueryItem oldItem) { RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem)); }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, UrnQueryItem[] newItems, UrnQueryItem[] oldItems) { RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItems, oldItems)); }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, UrnQueryItem newItem, UrnQueryItem oldItem, int index) { RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index)); }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, UrnQueryItem[] newItems, UrnQueryItem[] oldItems, int startingIndex) { RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItems, oldItems, startingIndex)); }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, UrnQueryItem changedItem, int index, int oldIndex) { RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItem, index, oldIndex)); }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, UrnQueryItem[] changedItems, int index, int oldIndex) { RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItems, index, oldIndex)); }

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                try
                {
                    _stringValue = null;
                    try { Count = _innerList.Count; }
                    finally { OnCollectionChanged(args); }
                }
                finally { CollectionChanged?.Invoke(this, args); }
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        #endregion
        
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                _stringValue = null;
                OnItemPropertyChanged((UrnQueryItem)sender, e);
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        protected virtual void OnItemPropertyChanged(UrnQueryItem sender, PropertyChangedEventArgs e) { }

        public bool Equals(UrnQuery other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            Monitor.Enter(_syncRoot);
            try
            {
                Monitor.Enter(other._syncRoot);
                try { return Equals(other._innerList); }
                finally { Monitor.Exit(other._syncRoot); }
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        private bool Equals(IEnumerable<UrnQueryItem> other)
        {
            using (IEnumerator<UrnQueryItem> enumerator = other.GetEnumerator())
            {
                for (int i = 0; i < _innerList.Count; i++)
                {
                    if (!enumerator.MoveNext() || enumerator.Current == null || !_innerList[i].Equals(enumerator.Current))
                        return false;
                }
                if (enumerator.MoveNext())
                    return false;
            }
            return true;
        }

        private bool Equals(IEnumerable<KeyValuePair<string, string>> other)
        {
            using (IEnumerator<KeyValuePair<string, string>> enumerator = other.GetEnumerator())
            {
                for (int i = 0; i < _innerList.Count; i++)
                {
                    if (!enumerator.MoveNext() || !_innerList[i].Equals(enumerator.Current))
                        return false;
                }
                if (enumerator.MoveNext())
                    return false;
            }
            return true;
        }

        public bool Equals(string other)
        {
            if (other == null)
                return false;

            Monitor.Enter(_syncRoot);
            try { return ToString() == other; }
            finally { Monitor.Exit(_syncRoot); }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is UrnQuery)
                return Equals((UrnQuery)obj);

            if (obj is IEnumerable<UrnQueryItem>)
                return Equals((IEnumerable<UrnQueryItem>)obj);

            if (obj is IEnumerable<KeyValuePair<string, string>>)
                return Equals((IEnumerable<KeyValuePair<string, string>>)obj);
            
            if (!(obj is string))
            {
                if (obj is IEquatable<string>)
                    return ((IEquatable<string>)obj).Equals(ToString());

                if (obj is IComparable<string>)
                    return ((IComparable<string>)obj).CompareTo(ToString()) == 0;

                if (obj is IComparable)
                    return ((IComparable)obj).CompareTo(ToString()) == 0;

                try { obj = obj.ToString(); } catch { return false; }

                if (obj == null)
                    return false;
            }

            return Equals((string)obj);
        }

        public int CompareTo(UrnQuery other)
        {
            if (other == null)
                return 1;

            if (ReferenceEquals(this, other))
                return 0;

            Monitor.Enter(_syncRoot);
            try
            {
                Monitor.Enter(other._syncRoot);
                try { return CompareTo(other._innerList); }
                finally { Monitor.Exit(other._syncRoot); }
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        private int CompareTo(IEnumerable<UrnQueryItem> other)
        {
            using (IEnumerator<UrnQueryItem> enumerator = other.GetEnumerator())
            {
                for (int i = 0; i < _innerList.Count; i++)
                {
                    if (!enumerator.MoveNext() || enumerator.Current == null)
                        return 1;
                    int n = _innerList[i].CompareTo(enumerator.Current);
                    if (n != 0)
                        return n;
                }
                if (enumerator.MoveNext())
                    return -1;
            }
            return 0;
        }

        private int CompareTo(IEnumerable<KeyValuePair<string, string>> other)
        {
            using (IEnumerator<KeyValuePair<string, string>> enumerator = other.GetEnumerator())
            {
                for (int i = 0; i < _innerList.Count; i++)
                {
                    if (!enumerator.MoveNext())
                        return 1;
                    int n = _innerList[i].CompareTo(enumerator.Current);
                    if (n != 0)
                        return n;
                }
                if (enumerator.MoveNext())
                    return -1;
            }
            return 0;
        }

        public int CompareTo(string other)
        {
            if (other == null)
                return 1;

            Monitor.Enter(_syncRoot);
            try { return ToString().CompareTo(other); }
            finally { Monitor.Exit(_syncRoot); }
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            if (obj is UrnQuery)
                return CompareTo((UrnQuery)obj);

            if (obj is IEnumerable<UrnQueryItem>)
                return CompareTo((IEnumerable<UrnQueryItem>)obj);

            if (obj is IEnumerable<KeyValuePair<string, string>>)
                return CompareTo((IEnumerable<KeyValuePair<string, string>>)obj);

            if (!(obj is string))
            {
                if (obj is IComparable<string>)
                    return 0 - ((IComparable<string>)obj).CompareTo(ToString());

                if (obj is IComparable)
                    return 0 - ((IComparable)obj).CompareTo(ToString());

                try { obj = obj.ToString(); } catch { return -1; }

                if (obj == null)
                    return -1;
            }

            return CompareTo((string)obj);
        }

        public override int GetHashCode() { return ToString().GetHashCode(); }

        public override string ToString()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_stringValue == null)
                {
                    if (_innerList.Count == 0)
                        _stringValue = "";
                    else if (_innerList.Count == 1)
                        _stringValue = _innerList[0].ToString();
                    else
                    {
                        StringBuilder result = new StringBuilder(_innerList[0].ToString());
                        for (int i = 1; i < _innerList.Count; i++)
                            result.Append(_innerList[i].ToString());
                        _stringValue = result.ToString();
                    }
                }
                return _stringValue;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public void Add(UrnQueryItem item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(UrnQueryItem item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(UrnQueryItem[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<UrnQueryItem> GetEnumerator()
        {
            throw new NotImplementedException();
        }
        
        public int IndexOf(UrnQueryItem item)
        {
            throw new NotImplementedException();
        }

        void IList<UrnQueryItem>.Insert(int index, UrnQueryItem item)
        {
            throw new NotImplementedException();
        }

        bool ICollection<UrnQueryItem>.Remove(UrnQueryItem item)
        {
            throw new NotImplementedException();
        }

        void IList<UrnQueryItem>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        #region IConvertible Members

        TypeCode IConvertible.GetTypeCode() { return TypeCode.String; }

        bool IConvertible.ToBoolean(IFormatProvider provider) { throw new NotSupportedException(); }

        char IConvertible.ToChar(IFormatProvider provider) { throw new NotSupportedException(); }

        sbyte IConvertible.ToSByte(IFormatProvider provider) { throw new NotSupportedException(); }

        byte IConvertible.ToByte(IFormatProvider provider) { throw new NotSupportedException(); }

        short IConvertible.ToInt16(IFormatProvider provider) { throw new NotSupportedException(); }

        ushort IConvertible.ToUInt16(IFormatProvider provider) { throw new NotSupportedException(); }

        int IConvertible.ToInt32(IFormatProvider provider) { throw new NotSupportedException(); }

        uint IConvertible.ToUInt32(IFormatProvider provider) { throw new NotSupportedException(); }

        long IConvertible.ToInt64(IFormatProvider provider) { throw new NotSupportedException(); }

        ulong IConvertible.ToUInt64(IFormatProvider provider) { throw new NotSupportedException(); }

        float IConvertible.ToSingle(IFormatProvider provider) { throw new NotSupportedException(); }

        double IConvertible.ToDouble(IFormatProvider provider) { throw new NotSupportedException(); }

        decimal IConvertible.ToDecimal(IFormatProvider provider) { throw new NotSupportedException(); }

        DateTime IConvertible.ToDateTime(IFormatProvider provider) { throw new NotSupportedException(); }

        string IConvertible.ToString(IFormatProvider provider) { return ToString(); }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == null || conversionType.IsAssignableFrom(typeof(string)))
                return ToString();

            if (conversionType.IsAssignableFrom(GetType()))
                return this;

            throw new NotSupportedException();
        }

        int IList.Add(object value)
        {
            throw new NotImplementedException();
        }

        bool IList.Contains(object value)
        {
            throw new NotImplementedException();
        }

        void IList.Clear()
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

        void IList.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
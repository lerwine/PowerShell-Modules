using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace NetworkUtilityCLR
{
    public class UrnPath : IList<string>, IList, INotifyPropertyChanged, INotifyCollectionChanged, IEquatable<UrnPath>, IEquatable<string>, IComparable<UrnPath>, IComparable<string>, IComparable, IConvertible
    {
        private object _syncRoot = new object();
        private List<string> _innerList = new List<string>();

        public string this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
            private set
            {
                throw new NotImplementedException();
            }
        }

        bool ICollection<string>.IsReadOnly { get { return false; } }

        bool IList.IsReadOnly { get { return false; } }

        bool IList.IsFixedSize { get { return false; } }

        object ICollection.SyncRoot { get { return _syncRoot; } }

        bool ICollection.IsSynchronized { get { return true; } }

        object IList.this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

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

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, string changedItem) { RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItem)); }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, string[] changedItems) { RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItems)); }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, string changedItem, int index) { RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItem, index)); }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, string[] changedItems, int startingIndex) { RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItems, startingIndex)); }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, string newItem, string oldItem) { RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem)); }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, string[] newItems, string[] oldItems) { RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItems, oldItems)); }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, string newItem, string oldItem, int index) { RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index)); }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, string[] newItems, string[] oldItems, int startingIndex) { RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItems, oldItems, startingIndex)); }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, string changedItem, int index, int oldIndex) { RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItem, index, oldIndex)); }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, string[] changedItems, int index, int oldIndex) { RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItems, index, oldIndex)); }

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                try { OnCollectionChanged(args); }
                finally { CollectionChanged?.Invoke(this, args); }
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        #endregion

        internal static UrnPath Parse(string path)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(string other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(UrnPath other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(string other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(UrnPath other)
        {
            throw new NotImplementedException();
        }

        void ICollection<string>.Add(string item)
        {
            throw new NotImplementedException();
        }

        void ICollection<string>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<string>.Contains(string item)
        {
            throw new NotImplementedException();
        }

        void ICollection<string>.CopyTo(string[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
        
        int IList<string>.IndexOf(string item)
        {
            throw new NotImplementedException();
        }

        void IList<string>.Insert(int index, string item)
        {
            throw new NotImplementedException();
        }

        bool ICollection<string>.Remove(string item)
        {
            throw new NotImplementedException();
        }

        void IList<string>.RemoveAt(int index)
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
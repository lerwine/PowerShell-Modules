using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

namespace NetworkUtility
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class UriQueryItem : IEquatable<UriQueryItem>, IComparable<UriQueryItem>, IEquatable<KeyValuePair<string, string>>
    {
        private static StringComparer _comparer = StringComparer.InvariantCultureIgnoreCase;
        private object _syncRoot = new object();
        private string _key;
        private string _value;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Key
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return _key; }
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public string Value
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return _value; }
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public UriQueryItem(string key, string value)
        {
            _key = (key == null) ? "" : key;
            _value = value;
        }

        public UriQueryItem(string key) : this(key, null) { }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args) { }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventArgs args = new PropertyChangedEventArgs(propertyName);
            try { OnPropertyChanged(args); }
            finally
            {
                PropertyChangedEventHandler propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                    propertyChanged(this, args);
            }
        }

        public bool Equals(UriQueryItem other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(KeyValuePair<string, string> other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(UriQueryItem other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
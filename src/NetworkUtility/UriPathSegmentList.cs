using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading;

namespace NetworkUtility
{
    public class UriPathSegmentList : IList<string>, IList, INotifyPropertyChanged, INotifyCollectionChanged, IEquatable<UriPathSegmentList>, IComparable<UriPathSegmentList>, IComparable
    {
        private static StringComparer _comparer = StringComparer.InvariantCultureIgnoreCase;

        private object _syncRoot = new object();
        private List<string> _segments = null;
        private List<char> _separators = new List<char>();
        private int _count = 0;
        private string _fullPath = "";
        private string _encodedFullPath = "";

        public event PropertyChangedEventHandler PropertyChanged;
        
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public string FullPath
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return _fullPath; }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public string EncodedFullPath
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return _encodedFullPath; }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public bool IsEmpty
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return _segments == null; }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public bool IsPathRooted
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return _segments != null && _separators.Count == _segments.Count; }
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (value)
                    {
                        if (_separators == null)
                            _separators = new List<char>();
                        else if (_separators.Count < _segments.Count)
                            _separators.Insert(0, _separators.DefaultIfEmpty('/').First());
                    }
                    else if (_separators != null)
                    {
                        if (_separators.Count == 0)
                            _separators = null;
                        else
                            _separators.RemoveAt(0);
                    }
                }
                finally { Monitor.Exit(_syncRoot); } 
            }
        }
        
        public string this[int index]
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return _segments[index]; }
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                string oldValue;
                Monitor.Enter(_syncRoot);
                try
                {
                    if (value == null)
                        throw new ArgumentNullException();
                    if (_segments[index] == value)
                        return;
                    oldValue = _segments[index];
                    _segments[index] = value;
                }
                finally { Monitor.Exit(_syncRoot); }
                UpdateFullPath(() => RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, oldValue, index)));
            }
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set
            {
                object obj = value;
                if (obj != null && obj is PSObject)
                    obj = (obj as PSObject).BaseObject;
                this[index] = (string)obj;
            }
        }

        public int Count
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return _count; }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        bool ICollection<string>.IsReadOnly { get { return false; } }

        bool IList.IsReadOnly { get { return false; } }

        bool IList.IsFixedSize { get { return false; } }

        object ICollection.SyncRoot { get { return _syncRoot; } }

        bool ICollection.IsSynchronized { get { return true; } }

        public static string EscapePathSegment(string value)
        {
            if (String.IsNullOrEmpty(value))
                return "";
            
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                switch (c)
                {
                    case '%':
                        sb.Append("%25");
                        break;
                    case '\\':
                        sb.Append("%5C");
                        break;
                    case '#':
                        sb.Append("%23");
                        break;
                    case '/':
                        sb.Append("%2F");
                        break;
                    case ':':
                        sb.Append("%3A");
                        break;
                    case '?':
                        sb.Append("%3F");
                        break;
                    default:
                        if (c < ' ' || c > 126)
                            sb.Append(Uri.HexEscape(c));
                        else
                            sb.Append(c);
                        break;
                }
            }

            return sb.ToString();
        }

        public static string EncodePathSegment(string value)
        {
            if (String.IsNullOrEmpty(value))
                return "";
            
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                switch (c)
                {
                    case ' ':
                        sb.Append("%20");
                        break;
                    case '"':
                        sb.Append("%22");
                        break;
                    case '%':
                        sb.Append("%25");
                        break;
                    case '<':
                        sb.Append("%3C");
                        break;
                    case '>':
                        sb.Append("%3E");
                        break;
                    case '\\':
                        sb.Append("%5C");
                        break;
                    case '^':
                        sb.Append("%5E");
                        break;
                    case '`':
                        sb.Append("%60");
                        break;
                    case '{':
                        sb.Append("%7B");
                        break;
                    case '|':
                        sb.Append("%7C");
                        break;
                    case '}':
                        sb.Append("%7D");
                        break;
                    case '#':
                        sb.Append("%23");
                        break;
                    case '+':
                        sb.Append("%2B");
                        break;
                    case '/':
                        sb.Append("%2F");
                        break;
                    case ':':
                        sb.Append("%3A");
                        break;
                    case '?':
                        sb.Append("%3F");
                        break;
                    default:
                        if (c < ' ' || c > 126)
                            sb.Append(Uri.HexEscape(c));
                        else
                            sb.Append(c);
                        break;
                }
            }

            return sb.ToString();
        }

        private void EnsureCount(Action action)
        {
            try { action(); }
            finally { EnsureCount(); }
        }

        private void EnsureCount()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_count == _segments.Count)
                    return;

                _count = _segments.Count;
            }
            finally { Monitor.Exit(_syncRoot); }
            RaisePropertyChanged("Count");
        }

        private void UpdateFullPath(Action action)
        {
            try { action(); }
            finally { UpdateFullPath(); }
        }

        private void UpdateFullPath()
        {
            string encodedFullPath, escapedFullPath;
            Monitor.Enter(_syncRoot);
            try
            {
                if (_segments.Count == 0)
                {
                    encodedFullPath = "";
                    escapedFullPath = "";
                }
                else
                {
                    StringBuilder encoded = new StringBuilder();
                    StringBuilder escaped = new StringBuilder();
                    if (_segments.Count == _separators.Count)
                    {
                        for (int i = 0; i < _segments.Count; i++)
                        {
                            char c = _separators[i];
                            encoded.Append(c);
                            escaped.Append(c);
                            string s = _segments[i];
                            encoded.Append(EncodePathSegment(s));
                            escaped.Append(EscapePathSegment(s));
                        }
                    }
                    else
                    {
                        encoded.Append(EncodePathSegment(_segments[0]));
                        for (int i = 1; i < _segments.Count; i++)
                        {
                            char c = _separators[i - 1];
                            encoded.Append(c);
                            encoded.Append(c);
                            string s = _segments[i];
                            encoded.Append(EncodePathSegment(s));
                            escaped.Append(EscapePathSegment(s));
                        }
                    }
                    encodedFullPath = encoded.ToString();
                    escapedFullPath = escaped.ToString();
                }
                if (_encodedFullPath == encodedFullPath)
                    encodedFullPath = null;
                else
                    _encodedFullPath = encodedFullPath;
                if (_fullPath == escapedFullPath)
                    escapedFullPath = null;
                else
                    _fullPath = escapedFullPath;
            }
            finally { Monitor.Exit(_syncRoot); }
            if (escapedFullPath != null)
                RaisePropertyChanged("FullPath");
            if (encodedFullPath != null)
                RaisePropertyChanged("EncodedFullPath");
        }

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

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args) { }

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            try { OnCollectionChanged(args); }
            finally
            {
                NotifyCollectionChangedEventHandler collectionChanged = CollectionChanged;
                if (collectionChanged != null)
                    collectionChanged(this, args);
            }
        }

        public int Add(string item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            Monitor.Enter(_syncRoot);
            int index;
            try
            {
                index = _segments.Count;
                if (_separators == null)
                    _separators = new List<char>();
                else
                    _separators.Add(_separators.DefaultIfEmpty('/').Last());
                _segments.Add(item);
            }
            finally { Monitor.Exit(_syncRoot); }
            EnsureCount();
            UpdateFullPath();
            return index;
        }

        void ICollection<string>.Add(string item) { Add(item); }

        int IList.Add(object value)
        {
            object obj = value;
            if (obj != null && obj is PSObject)
                obj = (obj as PSObject).BaseObject;
            return Add((string)obj);
        }

        public void Clear()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_segments.Count == 0)
                    return;
                if (_separators.Count == _segments.Count)
                    _separators.Clear();
                else
                    _separators = null;
                _segments.Clear();
            }
            finally { Monitor.Exit(_syncRoot); }
            EnsureCount();
            UpdateFullPath();
        }

        public bool Contains(string item)
        {
            if (item == null)
                return false;
            Monitor.Enter(_syncRoot);
            try
            {
                if (_segments.Count == 0)
                    return false;
                for (int i = 0; i < _segments.Count; i++)
                {
                    if (_comparer.Equals(_segments[i], item))
                        return true;
                }
            }
            finally { Monitor.Exit(_syncRoot); }
            return false;
        }

        bool IList.Contains(object value)
        {
            object obj = value;
            if (obj != null && obj is PSObject)
                obj = (obj as PSObject).BaseObject;
            return Contains(obj as string);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            Monitor.Enter(_syncRoot);
            try { _segments.CopyTo(array, arrayIndex); }
            finally { Monitor.Exit(_syncRoot); }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            Monitor.Enter(_syncRoot);
            try { _segments.ToArray().CopyTo(array, index); }
            finally { Monitor.Exit(_syncRoot); }
        }

        public IEnumerator<string> GetEnumerator() { return _segments.GetEnumerator(); }

        public char? GetSeparator(int index)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (index < 0 || index >= _segments.Count)
                    throw new ArgumentOutOfRangeException("index");
                if (_separators != null && _separators.Count == _segments.Count)
                    return _separators[index];
                if (index == 0)
                    return null;
                return _separators[index - 1];
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public int IndexOf(string item)
        {
            if (item == null)
                return -1;
            Monitor.Enter(_syncRoot);
            try
            {
                if (_segments.Count == 0)
                    return -1;
                int index = _segments.IndexOf(item);
                if (index < 0)
                {
                    for (int i = 0; i < _segments.Count; i++)
                    {
                        if (_comparer.Equals(_segments[i], item))
                            return i;
                    }
                }
                return index;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        int IList.IndexOf(object value)
        {
            object obj = value;
            if (obj != null && obj is PSObject)
                obj = (obj as PSObject).BaseObject;
            return IndexOf(obj as string);
        }

        public void Insert(int index, string item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            Monitor.Enter(_syncRoot);
            try
            {
                if (index < 0 || index > _segments.Count)
                    throw new ArgumentOutOfRangeException("index");
                if (index == _segments.Count)
                {
                    _segments.Add(item);
                    if (_separators == null)
                        _separators = new List<char>();
                    else
                        _separators.Add(_separators.DefaultIfEmpty('/').Last());
                }
                else
                {
                    if (_separators.Count == _segments.Count)
                        _separators.Insert(index, _separators[index]);
                    else if (index == _separators.Count)
                        _separators.Add(_separators.DefaultIfEmpty('/').Last());
                    else if (index == 0)
                        _separators.Insert(0, _separators.DefaultIfEmpty('/').First());
                    else
                        _separators.Insert(index - 1, _separators[index - 1]);
                    _segments.Insert(index, item);
                }
            }
            finally { Monitor.Exit(_syncRoot); }
            EnsureCount();
            UpdateFullPath();
        }

        void IList.Insert(int index, object value)
        {
            object obj = value;
            if (obj != null && obj is PSObject)
                obj = (obj as PSObject).BaseObject;
            Insert(index, (string)obj);
        }

        public bool Remove(string item)
        {
            if (item == null)
                return false;
            Monitor.Enter(_syncRoot);
            try
            {
                int index = IndexOf(item);
                if (index < 0)
                    return false;
                if (_segments.Count == _separators.Count)
                    _separators.RemoveAt(index);
                else
                    _separators.RemoveAt((index > 0) ? index - 1 : 0);
                _segments.RemoveAt(index);
            }
            finally { Monitor.Exit(_syncRoot); }
            EnsureCount();
            UpdateFullPath();
            return true;
        }

        void IList.Remove(object value)
        {
            object obj = value;
            if (obj != null && obj is PSObject)
                obj = (obj as PSObject).BaseObject;
            Remove(obj as string);
        }

        public void RemoveAt(int index)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (index < 0 || index >= _segments.Count)
                    throw new ArgumentOutOfRangeException("index");
                if (_segments.Count == _separators.Count)
                    _separators.RemoveAt(index);
                else
                    _separators.RemoveAt((index > 0) ? index - 1 : 0);
                _segments.RemoveAt(index);
            }
            finally { Monitor.Exit(_syncRoot); }
            EnsureCount();
            UpdateFullPath();
        }

        public void SetSeparator(int index, char separator)
        {
            if (!(separator == ':' || separator == '/' || separator == '\\'))
                throw new ArgumentException("Invalid separator character", "separator");
            Monitor.Enter(_syncRoot);
            try
            {
                if (index < 0 || index >= _segments.Count)
                    throw new ArgumentOutOfRangeException("index");
                if (_separators.Count == _segments.Count)
                    _separators[index] = separator;
                else if (index == 0)
                    _separators.Insert(0, separator);
                else
                    _separators[index - 1] = separator;
            }
            finally { Monitor.Exit(_syncRoot); }
            UpdateFullPath();
        }

        IEnumerator IEnumerable.GetEnumerator() { return _segments.ToArray().GetEnumerator(); }

#warning Not implemented

        public int CompareTo(UriPathSegmentList other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj) { return CompareTo(obj as UriPathSegmentList); }

        public bool Equals(UriPathSegmentList other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj) { return Equals(obj as UriPathSegmentList); }
		
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
    }
}
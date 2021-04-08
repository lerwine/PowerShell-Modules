using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace NetworkUtility
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class UriHierarchy : INotifyPropertyChanged, IEquatable<UriHierarchy>, IComparable<UriHierarchy>, IComparable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private object _syncRoot = new object();
        private UriPathSegmentList _path = new UriPathSegmentList();
        private string _userName = null;
        private string _password = null;
        private string _host = "";
        private int _port = -1;
		
        public string UserName
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return (_userName == null && _password != null) ? "" : _userName; }
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try
                {
					if (_password == null)
					{
						if ((_userName == null) ? value == null : value != null && _userName == value)
							return;
					}
					else if (String.IsNullOrEmpty(_userName) ? String.IsNullOrEmpty(value) : !String.IsNullOrEmpty(value) && _userName == value)
					{
						_userName = value;
						return;
					}
                    _userName = value;
                    RaisePropertyChanged("UserName");
					if (_host == null)
						RaisePropertyChanged("Host");
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public string Password
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return _password; }
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if ((_password == null) ? value == null : value != null && _password == value)
                        return;
                    _password = value;
                    RaisePropertyChanged("Password");
					if (_userName == null)
					{
						RaisePropertyChanged("UserName");
						if (_host == null)
							RaisePropertyChanged("Host");
					}
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public string Host
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return (_host == null && (_userName != null || _password != null || _port >= 0)) ? "" : _host; }
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try
                {
					if (_password == null && _userName == null && _port < 0)
					{
						if ((_host == null) ? value == null : value != null && _host == value)
							return;
					}
					else if ((String.IsNullOrEmpty(_host)) ? String.IsNullOrEmpty(value) : !String.IsNullOrEmpty(value) && _host == value)
					{
						_host = value;
						return;
					}
                    _host = value;
                    RaisePropertyChanged("Host");
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public int Port
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return _port; }
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try
                {
					int port = (value < 0) ? -1 : value;
					if (_port == port)
						return;
                    _port = port;
                    RaisePropertyChanged("Port");
					if (_host == null)
						RaisePropertyChanged("Host");
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public UriPathSegmentList Path
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try
				{
					if (_path == null)
					{
						_path = new UriPathSegmentList();
						_path.CollectionChanged += Path_CollectionChanged;
					}
					return _path;
				}
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try
				{
					if (value == null)
					{
						if (_path == null)
							return;
					}
					else
					{
						if (_path != null)
						{
							if (ReferenceEquals(_path, value))
								return;
							_path.CollectionChanged -= Path_CollectionChanged;
						}
						value.CollectionChanged += Path_CollectionChanged;
					}
                    _path = value;
                    RaisePropertyChanged("Path");
				}
                finally { Monitor.Exit(_syncRoot); }
            }
        }
		
		private void Path_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) { RaisePropertyChanged("Path"); }
		
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

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args) { }

#warning Not implemented

        public int CompareTo(UriHierarchy other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj) { return CompareTo(obj as UriHierarchy); }

        public bool Equals(UriHierarchy other)
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj) { return Equals(obj as UriHierarchy); }
		
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
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
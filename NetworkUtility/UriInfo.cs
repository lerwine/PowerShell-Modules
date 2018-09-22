using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace NetworkUtility
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    [Serializable]
    public class UriInfo : INotifyPropertyChanged, IEquatable<UriInfo>, IEquatable<string>, IComparable<UriInfo>, IComparable<string>, IComparable, IConvertible
    {
        private object _syncRoot = new object();
        private string _scheme = null;
        private UriHierarchy _hierarchy = null;
        private UriQueryList _query = null;
        private string _fragment = null;

        public string Scheme
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return _scheme; }
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if ((_scheme == null) ? value == null : value != null && _scheme == value)
                        return;
                    _scheme = value;
                    RaisePropertyChanged("Scheme");
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public string UserName
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return (_hierarchy == null) ? null : _hierarchy.UserName; }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public string Password
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return (_hierarchy == null) ? null : _hierarchy.Password; }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public string Host
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return (_hierarchy == null) ? null : _hierarchy.Host; }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public int Port
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return (_hierarchy == null) ? -1 : _hierarchy.Port; }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public UriPathSegmentList Path
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return (_hierarchy == null) ? null : _hierarchy.Path; }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public UriHierarchy Hierarchy
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return _hierarchy; }
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if ((_hierarchy == null) ? value == null : value != null && ReferenceEquals(_hierarchy, value))
                        return;
					string oldUserName = UserName, oldPassword = Password, oldHost = Host;
					int oldPort = Port;
					UriPathSegmentList oldPath = Path;
					if (_hierarchy != null)
						_hierarchy.PropertyChanged -= Hierarchy_PropertyChanged;
                    _hierarchy = value;
					if (value != null)
						value.PropertyChanged += Hierarchy_PropertyChanged;
					
                    RaisePropertyChanged("Hierarchy");
					if ((oldUserName == null) ? UserName != null : UserName == null || oldUserName != UserName)
						RaisePropertyChanged("UserName");
					if ((oldPassword == null) ? Password != null : Password == null || oldPassword != Password)
						RaisePropertyChanged("Password");
					if ((oldHost == null) ? Host != null : Host == null || oldHost != Host)
						RaisePropertyChanged("Host");
					if (oldPort != Port)
						RaisePropertyChanged("Port");
					if ((oldPath == null) ? Path != null : Path == null || !ReferenceEquals(oldPath, Path))
						RaisePropertyChanged("Path");
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }
        
        public UriQueryList Query
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { return _query; }
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if ((_query == null) ? value == null : value != null && ReferenceEquals(_query, value))
                        return;
					string oldUserName = UserName, oldPassword = Password, oldHost = Host;
					int oldPort = Port;
					UriPathSegmentList oldPath = Path;
					if (_query != null)
						_query.CollectionChanged -= Query_CollectionChanged;
                    _query = value;
					if (value != null)
						value.CollectionChanged += Query_CollectionChanged;
					
                    RaisePropertyChanged("Query");
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }
        
        public string Fragment { get; set; }
        
		private void Hierarchy_PropertyChanged(object sender, PropertyChangedEventArgs e) { RaisePropertyChanged(e.PropertyName); }
		
		private void Query_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) { RaisePropertyChanged("Query"); }
		
        public event PropertyChangedEventHandler PropertyChanged;

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

        public int CompareTo(UriInfo other)
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

        public bool Equals(UriInfo other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(string other)
        {
            throw new NotImplementedException();
        }

        TypeCode IConvertible.GetTypeCode()
        {
            throw new NotImplementedException();
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public static class Patterns
        {
            public const string Hex = @"%[a-fA-F\d]{2}";
            public const string Safe = @"[$\-_@.&]+";
            public const string Extra = @"[!*""'|,]+";
            public const string Reserved = @"[=;/#?: ]";
            public const string NotReserved = @"[^=;/#?: ]";
            public const string AlphaNum2 = @"[a-zA-Z\d\-_.+]+";
            public const string XAlpha_Fast = @"a-zA-Z\d$\-_@.&[!*""'|,%]+";
            public const string XAlpha_Strict = @"(?(?=%)%[a-fA-F\d]{2}|[a-zA-Z\d$\-_@.&[!*""'|,]+)+";
            public const string IAlpha_Strict = @"[A-Za-z](?(?=%)%[a-fA-F\d]{2}|[a-zA-Z\d$\-_@.&[!*""'|,]+)*";
            public const string XPAlpha_Strict = @"(?(?=%)%[a-fA-F\d]{2}|[a-zA-Z\d$\-_@.&[!*""'|,+]+)+";
            public const string Path_Fast = @"[a-zA-Z\d$\-_@.&[!*""'|,+/%]+";
            public const string Path_Strict = @"(?(?=%)%[a-fA-F\d]{2}|[a-zA-Z\d$\-_@.&[!*""'|,+/]+)+";
            public const string PathAlt_Strict = @"(?(?=%)%[a-fA-F\d]{2}|[a-zA-Z\d$\-_@.&[!*""'|,+/\\]+)+";
            public const string HostName_Strict = @"[a-zA-Z](?(?=\.)[a-zA-Z]|(?(?=%)%[a-fA-F\d]{2}|[a-zA-Z\d$\-_@.&[!*""'|,]+)+)*";
            public const string HostPort_Fast = @"(?<host>[a-zA-Z\d$\-_@&!*""'|,%](\.[a-zA-Z\d$\-_@&!*""'|,%])*)(:(?<port>\d+))?";
            public const string Port_Strict = @"(?(?=0+$)0+|0*(?=\d)(6(5(5(3[0-5]?|[012]\d?)?|[0-4]?\d{0,2})?|[0-4]?\d{0,3})?|[1-5]?\d{0,4}))";
            public const string ByteVal_Strict = @"(?(?=0+$)0+|0*(?=\d)(2(5[0-5]?|[0-4]?\d?)?|1?\d{0,2}))";
            public const string IPAddress_Strict = @"(?(?=0+$)0+|0*(?=\d)(2(5[0-5]?|[0-4]?\d?)?|1?\d{0,2}))(\.(?(?=0+$)0+|0*(?=\d)(2(5[0-5]?|[0-4]?\d?)?|1?\d{0,2}))){3}";
            public const string Uri_Parse = @"(((?<scheme>urn):)((?<nid>[^=;\\/#?:\[\]]*):)?|((?<scheme>[^=;\\/#?:\[\]]*):)?((?<urlSep>[\\/]{1,2})?(((?<user>[^=;\\/#?:\[\]]*)(:(?<password>[^=;\\/#?:\[\]]*))?@)?(?<host>[^=;\\/#?:\[\]]+)(:(?<port>\d{1,5}))?)?))(?<path>[^#?]+)?(\?(?<query>[^#]*))?(\#(?<fragment>.*))?";
            public const string Path_Parse = @"[\\/][^\\/]*|^[^\\/]+";
            public const string Path_RemoveTrailingSlashAndEmpty = @"^[^\\/]?[^\\/]*((?=\s*[\\/]+\s*[^\\/\s])\s*[\\/]+[^\\/]*(\s+[^\\/]+)*)*";
            public const string Query_Parse = @"(?<key>[^&=]*)=(?<value>[^&]*)|(?<key>[^&=]+)";
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace PSModuleInstallUtil
{
    public class UriPathBuilder : IEquatable<UriPathBuilder>, IComparable<UriPathBuilder>, IConvertible, ICloneable, ISerializable, IXmlSerializable, INotifyPropertyChanged
    {
        public const string PropertyName_Host = "Host";
        public const string PropertyName_IsAbsoluteUri = "IsAbsoluteUri";
        public const string PropertyName_IsDefaultPort = "IsDefaultPort";
        public const string PropertyName_IsFile = "IsFile";
        public const string PropertyName_IsLoopback = "IsLoopback";
        public const string PropertyName_IsUnc = "IsUnc";
        public const string PropertyName_LocalPath = "LocalPath";
        public const string PropertyName_OriginalString = "OriginalString";
        public const string PropertyName_Password = "Password";
        public const string PropertyName_Scheme = "Scheme";
        public const string PropertyName_UserName = "UserName";
        public const string PropertyName_Segments = "Segments";
        public const string PropertyName_Query = "Query";

        private UriBuilder _uriBuilder;
        private string _originalString;
        private UriSegmentCollection _segments = null;
        private UrlQueryCollection _query = null;
        private bool _querySync = true;
        private bool _segmentSync = true;

        // TODO: Implement method to look for changes and populate a string array of property changed events to raise.
        public event PropertyChangedEventHandler PropertyChanged;

        public UriPathBuilder() { }

        public UriPathBuilder(Uri uri) { InitializeFrom(uri); }

        private void InitializeFrom(Uri uri)
        {
            _originalString = uri.OriginalString;
            if (uri.IsAbsoluteUri)
                _uriBuilder = new UriBuilder(uri);
            else
            {
                if (_originalString.StartsWith("\\"))
                    _uriBuilder = new UriBuilder("http://localhost/" + _originalString.Substring(1));
                else if (_originalString.StartsWith("/"))
                    _uriBuilder = new UriBuilder("http://localhost" + _originalString);
                else
                    _uriBuilder = new UriBuilder("http://localhost/" + _originalString);
                _uriBuilder.Host = "";
                _uriBuilder.Scheme = "";
                _uriBuilder.Port = -1;
            }
        }

        public UriPathBuilder(UriPathBuilder toCopyFrom)
        {
            toCopyFrom.EnsureQuerySync();
            toCopyFrom.EnsureSegmentSync();
            throw new NotImplementedException();
        }

        protected UriPathBuilder(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public UriPathBuilder Clone() { return new UriPathBuilder(this); }

        object ICloneable.Clone() { return Clone(); }

        public string Host
        {
            get { return _uriBuilder.Host; }
            set { _uriBuilder.Host = value; }
        }

        public bool IsAbsoluteUri { get { return _uriBuilder.Uri != null && _uriBuilder.Uri.IsAbsoluteUri; } }

        public bool IsDefaultPort { get { return _uriBuilder.Uri != null && _uriBuilder.Uri.IsDefaultPort; } }

        public bool IsFile
        {
            get
            {
                EnsureSegmentSync();
                return _uriBuilder.Uri != null && _uriBuilder.Uri.IsFile;
            }
        }

        public bool IsLoopback
        {
            get
            {
                EnsureSegmentSync();
                return _uriBuilder.Uri != null && _uriBuilder.Uri.IsLoopback;
            }
        }

        public bool IsUnc
        {
            get
            {
                EnsureSegmentSync();
                return _uriBuilder.Uri != null && _uriBuilder.Uri.IsUnc;
            }
        }

        public string LocalPath
        {
            get
            {
                EnsureSegmentSync();
                return (_uriBuilder.Uri != null) ? _uriBuilder.Uri.LocalPath : "";
            }
        }

        private void EnsureSegmentSync()
        {
            if (_segmentSync)
                return;

            if (_segments == null)
                _segments = UriSegmentCollection.Parse(_uriBuilder.Path);
            else
                _uriBuilder.Path = _segments.ToString();
            _segmentSync = true;
        }

        private void EnsureQuerySync()
        {
            if (_querySync)
                return;

            if (_query == null)
                _query = UrlQueryCollection.Parse(_uriBuilder.Query);
            else
                _uriBuilder.Query = _query.ToString();
            _querySync = true;
        }

        public string OriginalString
        {
            get
            {
                if (_originalString == null)
                    _originalString = (_uriBuilder.Uri == null) ? _uriBuilder.ToString() : _uriBuilder.Uri.OriginalString;
                return _originalString;
            }
        }

        public string Password
        {
            get { return _uriBuilder.Password; }
            set { _uriBuilder.Password = value; }
        }

        public string Scheme
        {
            get { return _uriBuilder.Scheme; }
            set { _uriBuilder.Scheme = value; }
        }

        public string UserName
        {
            get { return _uriBuilder.UserName; }
            set { _uriBuilder.UserName = value; }
        }

        public UriSegmentCollection Segments
        {
            get
            {
                if (_segments == null)
                {
                    _segments = UriSegmentCollection.Parse(_uriBuilder.Query);
                    _segmentSync = true;
                }
                return _segments;
            }
            set
            {
                UriSegmentCollection o = _segments;
                if (value == null)
                    _segments = new UriSegmentCollection();
                else
                {
                    if (ReferenceEquals(_segments, value))
                        return;
                    _segments = value;
                }
                o.CollectionChanged -= Segments_CollectionChanged;
                _segments.CollectionChanged += Segments_CollectionChanged;
                _segmentSync = false;
            }
        }

        private void Segments_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _segmentSync = false;
        }

        public UrlQueryCollection Query
        {
            get
            {
                if (_query == null)
                {
                    _query = UrlQueryCollection.Parse(_uriBuilder.Query);
                    _querySync = true;
                }
                return _query;
            }
            set
            {
                UrlQueryCollection o = _query;
                if (value == null)
                    _query = new UrlQueryCollection();
                else
                {
                    if (ReferenceEquals(_query, value))
                        return;
                    _query = value;
                }
                o.CollectionChanged -= Query_CollectionChanged;
                _query.CollectionChanged += Query_CollectionChanged;
                _querySync = false;
            }
        }

        private void Query_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _querySync = false;
        }

        public Uri CreateUri()
        {
            EnsureQuerySync();
            EnsureSegmentSync();
            throw new NotImplementedException();
        }

        public bool Equals(UriPathBuilder other) { return other != null && (ReferenceEquals(this, other) || this.ToString().Equals(other.ToString())); }

        public override bool Equals(object obj) { return Equals(obj as UriPathBuilder); }

        public override int GetHashCode() { return ToString().GetHashCode(); }

        public int CompareTo(UriPathBuilder other)
        {
            if (other == null)
                return 1;

            if (ReferenceEquals(this, other))
                return 0;

            string x = ToString();
            string y = other.ToString();

            int result = String.Compare(x, y, true);
            if (result != 0)
                return result;
            return String.Compare(x, y, false);
        }

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

        public override string ToString()
        {
            EnsureQuerySync();
            EnsureSegmentSync();

            return _uriBuilder.ToString();
        }

        string IConvertible.ToString(IFormatProvider provider) { return ToString(); }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == null || conversionType == typeof(string))
                return ToString();

            if (conversionType == typeof(Uri))
                return CreateUri();

            throw new NotSupportedException();
        }

        XmlSchema IXmlSerializable.GetSchema() { return null; }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            if (reader.IsEmptyElement)
            {
                // TODO: Reset all properties
                return;
            }

            throw new NotImplementedException();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer) { writer.WriteString(ToString()); }
    }
}
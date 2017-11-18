using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;

namespace NetworkUtilityCLR
{
    [Serializable]
    public class UriInfo : INotifyPropertyChanged, IEquatable<UriInfo>, IEquatable<string>,
        IComparable<UriInfo>, IComparable<string>, IComparable, IConvertible
    {
        // scheme:[//[user[:password]@]host[:port]][/path][?query][#fragment]
        public string Scheme { get; set; }
        public UriHierarchy Hierarchy { get; set; }
        public UriQueryNode Query { get; set; }
        public string Fragment { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

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

        public class UriHierarchy
        {
            public UriAuthority Authority { get; set; }
            public UriPathNode RootPathNode { get; set; }
        }

        public class UriPathNode
        {

        }

        public class UriQueryNode
        {

        }

        public class UriAuthority
        {
            public UriAuthentication Authentication { get; set; }
            public string Host { get; set; }
            public int Port { get; set; }
        }

        public class UriAuthentication
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }
    }
}
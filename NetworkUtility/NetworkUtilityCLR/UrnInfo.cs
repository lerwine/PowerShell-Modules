using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkUtilityCLR
{
    public class UrnInfo : INotifyPropertyChanged, IEquatable<UrnInfo>, IEquatable<Uri>, IEquatable<string>, IComparable<UrnInfo>, IComparable<Uri>, IComparable<string>, IComparable, IConvertible
    {
        private object _syncRoot = new object();
        private int _port = -1;

        public string Scheme
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public string OriginalString
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public string UserName
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public string Password
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public string Host
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public int Port
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(_syncRoot); }
            }
        }


        public UrnPath Path
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        internal static StringComparer ToComparer(StringComparison comparison)
        {
            switch (comparison)
            {
                case StringComparison.CurrentCultureIgnoreCase:
                    return StringComparer.CurrentCultureIgnoreCase;
                case StringComparison.InvariantCulture:
                    return StringComparer.InvariantCulture;
                case StringComparison.InvariantCultureIgnoreCase:
                    return StringComparer.InvariantCultureIgnoreCase;
                case StringComparison.Ordinal:
                    return StringComparer.Ordinal;
                case StringComparison.OrdinalIgnoreCase:
                    return StringComparer.OrdinalIgnoreCase;
            }
            return StringComparer.CurrentCulture;
        }

        public UrnQuery Query
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public string Fragment
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(_syncRoot); }
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public UrnType Type
        {
            get
            {
                Monitor.Enter(_syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(_syncRoot); }
            }
            private set
            {
                Monitor.Enter(_syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(_syncRoot); }
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

        private static void ParseString(UrnQuery target, string uri)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            Monitor.Enter(_syncRoot);
            try { throw new NotImplementedException(); }
            finally { Monitor.Exit(_syncRoot); }
        }

        public Uri ToUri()
        {
            Monitor.Enter(_syncRoot);
            try { throw new NotImplementedException(); }
            finally { Monitor.Exit(_syncRoot); }
        }

        public override int GetHashCode()
        {
            Monitor.Enter(_syncRoot);
            try { throw new NotImplementedException(); }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool Equals(UrnInfo other)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                Monitor.Enter(other._syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(other._syncRoot); }
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool Equals(Uri other)
        {
            Monitor.Enter(_syncRoot);
            try { throw new NotImplementedException(); }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool Equals(string other)
        {
            Monitor.Enter(_syncRoot);
            try { throw new NotImplementedException(); }
            finally { Monitor.Exit(_syncRoot); }
        }

        public int CompareTo(UrnInfo other)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                Monitor.Enter(other._syncRoot);
                try { throw new NotImplementedException(); }
                finally { Monitor.Exit(other._syncRoot); }
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public int CompareTo(Uri other)
        {
            Monitor.Enter(_syncRoot);
            try { throw new NotImplementedException(); }
            finally { Monitor.Exit(_syncRoot); }
        }

        public int CompareTo(string other)
        {
            Monitor.Enter(_syncRoot);
            try { throw new NotImplementedException(); }
            finally { Monitor.Exit(_syncRoot); }
        }

        public int CompareTo(object obj)
        {
            Monitor.Enter(_syncRoot);
            try { throw new NotImplementedException(); }
            finally { Monitor.Exit(_syncRoot); }
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

            if (conversionType.AssemblyQualifiedName == (typeof(Uri)).AssemblyQualifiedName)
                return ToUri();

            if (conversionType.IsAssignableFrom(GetType()))
                return this;

            throw new NotSupportedException();
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace NetworkUtilityCLR
{
    [Serializable]
    public class UrnQueryItem : INotifyPropertyChanged, IEquatable<UrnQueryItem>, IEquatable<KeyValuePair<string, string>>, IEquatable<string>, IComparable<UrnQueryItem>, IComparable<KeyValuePair<string, string>>, IComparable<string>, IComparable, IConvertible
    {
        #region Fields

        private object _syncRoot = new object();
        private string _key = "";
        private string _value = null;

        #endregion

        #region Properties

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
                try
                {
                    string key = value ?? "";
                    if (key == _key)
                        return;
                    _key = key;
                    RaisePropertyChanged("Key");
                }
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
                try
                {
                    if ((_value == null) ? value == null : value != null && value == _value)
                        return;
                    _value = value;
                    RaisePropertyChanged("Value");
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        #endregion

        public UrnQueryItem(string key, string value)
        {
            _key = key ?? "";
            _value = value;
        }

        public UrnQueryItem(string key)
        {
            _key = key ?? "";
        }

        public UrnQueryItem() { }

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

        public KeyValuePair<string, string> ToKeyValuePair()
        {
            Monitor.Enter(_syncRoot);
            try { return new KeyValuePair<string, string>(_key, _value); }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool Equals(UrnQueryItem other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            Monitor.Enter(_syncRoot);
            try
            {
                Monitor.Enter(other._syncRoot);
                try { return _key == other._key && ((_value == null) ? other._value == null : other._value != null && _value == other._value); }
                finally { Monitor.Exit(other._syncRoot); }
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public bool Equals(KeyValuePair<string, string> other)
        {
            Monitor.Enter(_syncRoot);
            try { return _key == other.Key && ((_value == null) ? other.Value == null : other.Value != null && _value == other.Value); }
            finally { Monitor.Exit(_syncRoot); }
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

            if (obj is UrnQueryItem)
                return Equals((UrnQueryItem)obj);

            if (obj is KeyValuePair<string, string>)
                return Equals((KeyValuePair<string, string>)obj);

            if (obj is IEquatable<KeyValuePair<string, string>>)
                return ((IEquatable<KeyValuePair<string, string>>)obj).Equals(ToKeyValuePair());

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

        public int CompareTo(UrnQueryItem other)
        {
            if (other == null)
                return 1;

            if (ReferenceEquals(this, other))
                return 0;

            Monitor.Enter(_syncRoot);
            try
            {
                Monitor.Enter(other._syncRoot);
                try
                {
                    int i = _key.CompareTo(other._key);
                    if (i != 0)
                        return i;

                    return (_value == null) ? ((other._value == null) ? 0 : -1) : ((other._value == null) ? 1 : _value.CompareTo(other._value)); }
                finally { Monitor.Exit(other._syncRoot); }
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public int CompareTo(KeyValuePair<string, string> other)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                int i = _key.CompareTo(other.Key);
                if (i != 0)
                    return i;

                return (_value == null) ? ((other.Value == null) ? 0 : -1) : ((other.Value == null) ? 1 : _value.CompareTo(other.Value));
            }
            finally { Monitor.Exit(_syncRoot); }
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

            if (obj is UrnQueryItem)
                return CompareTo((UrnQueryItem)obj);

            if (obj is KeyValuePair<string, string>)
                return CompareTo((KeyValuePair<string, string>)obj);

            if (obj is IComparable<KeyValuePair<string, string>>)
                return 0 - ((IComparable<KeyValuePair<string, string>>)obj).CompareTo(ToKeyValuePair());

            if (!(obj is string))
            {
                if (obj is IComparable<string>)
                    return 0 - ((IComparable<string>)obj).CompareTo(ToString());
                
                if (obj is IComparable)
                    return 0 - ((IComparable)obj).CompareTo(ToString());

                try { obj = obj.ToString(); } catch { return -1; }

                if (obj == null)
                    return 1;
            }

            return CompareTo((string)obj);
        }

        public override int GetHashCode() { return ToString().GetHashCode(); }

        public override string ToString()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_value == null)
                    return (_key.Length == 0) ? "" : Uri.EscapeDataString(_key);
                return ((_key.Length == 0) ? "" : Uri.EscapeDataString(_key)) + "=" + ((_value.Length == 0) ? "" : Uri.EscapeDataString(_value));
            }
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

            if (conversionType.IsAssignableFrom(GetType()))
                return this;

            throw new NotSupportedException();
        }

        #endregion
    }
}
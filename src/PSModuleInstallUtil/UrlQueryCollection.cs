using System;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace PSModuleInstallUtil
{
    [Serializable]
    public class UrlQueryCollection : NameValueCollection, IEquatable<UrlQueryCollection>, IComparable<UrlQueryCollection>, IConvertible, ICloneable, IXmlSerializable, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        
        private void DeserializeFromString(string text)
        {
            if (Count > 0)
                Clear();

            if (String.IsNullOrEmpty(text))
                return;

            foreach (string item in ((text.StartsWith("?")) ? text.Substring(1) : text).Split('&'))
            {
                if (item.Length == 0)
                    continue;

                int i = item.IndexOf('=');
                if (i < 0)
                    Add(Uri.UnescapeDataString(item), null);
                else
                    Add(Uri.UnescapeDataString(item.Substring(0, i)), Uri.UnescapeDataString(item.Substring(i + 1)));
            }
        }

        public static UrlQueryCollection Parse(string query)
        {
            UrlQueryCollection urlQueryCollection = new UrlQueryCollection();
            urlQueryCollection.DeserializeFromString(query);
            return urlQueryCollection;
        }
        
        public UrlQueryCollection() : base() { }

        public UrlQueryCollection(NameValueCollection coll) : base(coll) { }

        protected UrlQueryCollection(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public UrlQueryCollection Clone() { return new UrlQueryCollection(this); }

        object ICloneable.Clone() { return Clone(); }
        private object _syncRoot = new object();

        private int _IndexOfName(string name)
        {
            for (int i = 0; i < Count; i++)
            {
                if (String.Compare(GetKey(i), name, false) == 0)
                    return i;
            }

            for (int i = 0; i < Count; i++)
            {
                if (String.Compare(GetKey(i), name, true) == 0)
                    return i;
            }

            return -1;
        }

        public override void Add(string name, string value)
        {
            int index;
            string[] newValue;
            lock (_syncRoot)
            {
                base.Add(name, value);
                index = _IndexOfName(name);
                newValue = GetValues(index);
            }
            
            RaiseCollectionChanged(NotifyCollectionChangedAction.Add, index, newValue);
        }

        public override void Remove(string name)
        {
            int index;
            string[] oldValue;
            lock (_syncRoot)
            {
                index = _IndexOfName(name);
                if (index < 0)
                    return;
                oldValue = GetValues(index);
                base.Remove(name);
            }

            RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, index, oldValue);
        }

        public override void Set(string name, string value)
        {
            int index;
            string[] newValue;
            string[] oldValue = null;
            lock (_syncRoot)
            {
                index = _IndexOfName(name);
                oldValue = (index < 0) ? new string[0] : GetValues(index);
                if (oldValue.Length == 1 && (oldValue[0] == null) ? value == null : value != null && oldValue[0] == value)
                    return;
                base.Set(name, value);
                newValue = GetValues(index);
            }

            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue as object, oldValue as object, index));
        }

        public override void Clear()
        {
            base.Clear();
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, int index, string[] value)
        {
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, value as object, index));
        }

        protected void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            try { OnCollectionChanged(args); }
            finally { CollectionChanged?.Invoke(this, args); }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args) { }

        public bool Equals(UrlQueryCollection other) { return other != null && (ReferenceEquals(this, other) || this.ToString().Equals(other.ToString())); }

        public override bool Equals(object obj) { return Equals(obj as UrlQueryCollection); }

        public override int GetHashCode() { return ToString().GetHashCode(); }

        public int CompareTo(UrlQueryCollection other)
        {
            if (other == null)
                return 1;

            if (ReferenceEquals(this, other))
                return 0;

            if (Count == 0)
                return (other.Count == 0) ? 0 : -1;

            if (other.Count == 0)
                return 1;

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
            if (Count == 0)
                return "";

            return String.Join("&", Keys.OfType<string>().SelectMany(k => GetValues(k).Select(v => (v == null) ?
                Uri.EscapeDataString(k) : Uri.EscapeDataString(k) + "=" + Uri.EscapeDataString(v))));
        }

        string IConvertible.ToString(IFormatProvider provider) { return ToString(); }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == null || conversionType == typeof(string))
                return ToString();

            throw new NotSupportedException();
        }

        XmlSchema IXmlSerializable.GetSchema() { return null; }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            if (reader.IsEmptyElement)
            {
                if (Count > 0)
                    Clear();
                return;
            }

            DeserializeFromString(reader.ReadContentAsString().Trim());
        }

        void IXmlSerializable.WriteXml(XmlWriter writer) { writer.WriteString(ToString()); }
    }
}
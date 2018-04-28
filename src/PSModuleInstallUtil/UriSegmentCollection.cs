using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace PSModuleInstallUtil
{
    public class UriSegmentCollection : IList<string>, IEquatable<UriSegmentCollection>, IComparable<UriSegmentCollection>, IConvertible, ICloneable, ISerializable, IXmlSerializable, INotifyCollectionChanged
    {
        private List<string> _innerList;
        private object _syncRoot = new object();

        public string this[int index]
        {
            get { return _innerList[index]; }
            set
            {
                string s, o;
                lock (_syncRoot)
                {
                    s = value ?? "";
                    o = _innerList[index];
                    if (o == s)
                        return;

                    _innerList[index] = s;
                }
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, s, o, index));
            }
        }

        public int Count { get { return _innerList.Count; } }

        bool ICollection<string>.IsReadOnly { get { return false; } }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public static UriSegmentCollection Parse(string path)
        {
            UriSegmentCollection result = new UriSegmentCollection();
            result.DeserializeFromString(path);
            return result;
        }

        private void DeserializeFromString(string path)
        {
            lock (_syncRoot)
            {
                int startIndex = 0;
                int index;
                _innerList.Clear();
                while ((index = path.IndexOfAny(new char[] { '/', '\\' })) > -1)
                {
                    _innerList.Add(Uri.UnescapeDataString(path.Substring(startIndex, index - startIndex)));
                    if ((startIndex = index + 1) == path.Length)
                        break;
                }

                if (startIndex < path.Length)
                    _innerList.Add(Uri.UnescapeDataString(path.Substring(startIndex)));
            }

            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public UriSegmentCollection() { _innerList = new List<string>(); }

        public UriSegmentCollection(IEnumerable<string> segments)
        {
            if (segments == null)
                _innerList = new List<string>();
            else
                _innerList = segments.Select(s => s ?? "").ToList();
        }

        protected const string ElementName_Segments = "Segments";

        protected UriSegmentCollection(SerializationInfo info, StreamingContext context)
        {
            _innerList = (info.GetValue(ElementName_Segments, typeof(string[])) as string[]).ToList();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(ElementName_Segments, _innerList.ToArray(), typeof(string[]));
        }

        public UriSegmentCollection Clone() { return new UriSegmentCollection(this); }

        object ICloneable.Clone() { return Clone(); }

        protected void RaiseCollectionChanged(NotifyCollectionChangedAction action, int index, string value)
        {
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(action, value as object, index));
        }

        protected void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            try { OnCollectionChanged(args); }
            finally { CollectionChanged?.Invoke(this, args); }
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args) { }
        
        public bool Equals(UriSegmentCollection other) { return other != null && (ReferenceEquals(this, other) || this.ToString().Equals(other.ToString())); }    

        public override bool Equals(object obj) { return Equals(obj as UriSegmentCollection); }

        public override int GetHashCode() { return ToString().GetHashCode(); }

        public int CompareTo(UriSegmentCollection other)
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

            return String.Join("/", _innerList.Select(s => Uri.EscapeDataString(s)));
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

        public int IndexOf(string item)
        {
            return _innerList.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            lock (_syncRoot)
                _innerList.Insert(index, item);

            RaiseCollectionChanged(NotifyCollectionChangedAction.Add, index, item);
        }

        public void RemoveAt(int index)
        {
            string item;

            lock (_syncRoot)
            {
                item = _innerList[index];
                _innerList.RemoveAt(index);
            }

            RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, index, item);
        }

        public void Add(string item)
        {
            int index;
            lock (_syncRoot)
            {
                _innerList.Add(item);
                index = _innerList.Count;
            }

            RaiseCollectionChanged(NotifyCollectionChangedAction.Add, index, item);
        }

        public void Clear()
        {
            _innerList.Clear();
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(string item)
        {
            return _innerList.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            _innerList.CopyTo(array, arrayIndex);
        }

        public bool Remove(string item)
        {
            int index;
            lock (_syncRoot)
            {
                index = _innerList.IndexOf(item);
                if (index < 0)
                    return false;
                _innerList.RemoveAt(index);
            }

            RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, index, item);
            return true;
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (_innerList as IEnumerable).GetEnumerator();
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CredentialStorageLibrary
{
    public class NormalizingText : IEnumerable<char>, IEquatable<NormalizingText>, IEquatable<string>, IComparable<NormalizingText>, IComparable<string>, ICloneable,
        IComparable, IConvertible
    {
        public class Comparer : IEqualityComparer<NormalizingText>, IComparer<NormalizingText>, IEqualityComparer, IComparer
        {
            public int Compare(NormalizingText x, NormalizingText y)
            {
                if (x == null)
                    return (y == null) ? 0 : 1;
                return x.CompareTo(y);
            }

            public int Compare(object x, object y)
            {
                if ((x == null || x is NormalizingText) && (y == null || y is NormalizingText))
                    return Compare(x as NormalizingText, y as NormalizingText);
                return ValueComparer.Compare(x, y);
            }

            public bool Equals(NormalizingText x, NormalizingText y)
            {
                if (x == null)
                    return y == null;

                return x.Equals(y);
            }

            public new bool Equals(object x, object y)
            {
                if ((x == null || x is NormalizingText) && (y == null || y is NormalizingText))
                    return Equals(x as NormalizingText, y as NormalizingText);
                return ValueComparer.Equals(x, y);
            }

            public int GetHashCode(NormalizingText obj) { return ValueComparer.GetHashCode((obj == null) ? "" : obj.ToString()); }

            public int GetHashCode(object obj)
            {
                if (obj == null || obj is NormalizingText)
                    return GetHashCode(obj as NormalizingText);
                return ValueComparer.GetHashCode(obj);
            }
        }

        public static readonly StringComparer ValueComparer = StringComparer.InvariantCultureIgnoreCase;
        public static readonly Comparer DefaultComparer = new Comparer();
        private readonly string _value;
        private string _normalizedValue = null;
        private Func<string> _getNormalizedValue;

        public NormalizingText() : this(null as string) { }

        public NormalizingText(string value)
        {
            _value = value;
            _getNormalizedValue = _BuildNormalizedValue;
        }

        public NormalizingText(NormalizingText source)
        {
            _value = source._value;
            _normalizedValue = source.ToString();
            _getNormalizedValue = _GetNormalizedValue;
        }

        public static implicit operator NormalizingText(string value) { return new NormalizingText(value); }

        public static implicit operator String(NormalizingText value) { return (value == null) ? null : value.ToString(); }
        
        public static NormalizingText Join(string separator, params NormalizingText[] source)
        {
            if (source == null)
                return null;

            string normalizedValue;
            if (String.IsNullOrEmpty(separator))
                normalizedValue = String.Join("", source.Where(s => s != null).Select(s => s._getNormalizedValue()).ToArray());
            else
            {
                string ns = ToNormalizedString(separator);
                if (ns.Length == 0)
                    ns = " ";
                else
                {
                    if (Char.IsWhiteSpace(separator[0]))
                        ns = " " + ns;
                    if (Char.IsWhiteSpace(separator[separator.Length - 1]))
                        ns += " ";
                }
                normalizedValue = String.Join(ns, source.Where(s => s != null).Select(s => s._getNormalizedValue()).ToArray());
            }

            NormalizingText result = new NormalizingText(normalizedValue);
            result._normalizedValue = normalizedValue;
            result._getNormalizedValue = result._GetNormalizedValue;
            return result;
        }

        public NormalizingText Clone() { return new NormalizingText(this); }

        object ICloneable.Clone() { return Clone(); }

        public string AsOriginalValue() { return _value; }

        public override string ToString() { return _getNormalizedValue(); }

        private string _BuildNormalizedValue()
        {
            _normalizedValue = ToNormalizedString(_value);
            _getNormalizedValue = _GetNormalizedValue;
            return _normalizedValue;
        }

        private string _GetNormalizedValue() { return _normalizedValue; }

        static IEnumerable<char> ToNormalizedChars(IEnumerable<char> source)
        {
            using (IEnumerator<char> enumerator = source.GetEnumerator())
            {
                do
                {
                    if (!enumerator.MoveNext())
                        yield break;
                } while (Char.IsWhiteSpace(enumerator.Current));

                yield return enumerator.Current;

                while (enumerator.MoveNext())
                {
                    while (!Char.IsWhiteSpace(enumerator.Current))
                    {
                        yield return enumerator.Current;
                        if (!enumerator.MoveNext())
                            yield break;
                    }
                    do
                    {
                        if (!enumerator.MoveNext())
                            yield break;
                    } while (Char.IsWhiteSpace(enumerator.Current));
                    yield return ' ';
                    yield return enumerator.Current;
                }
            }
        }

        public static string ToNormalizedString(string source) { return new string(ToNormalizedChars(source).ToArray()); }

        public IEnumerator<char> GetEnumerator() { return _getNormalizedValue().GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator() { return _getNormalizedValue().GetEnumerator(); }

        public bool Equals(string other) { return other != null && ValueComparer.Equals(_getNormalizedValue(), ToNormalizedString(other)); }

        public bool Equals(NormalizingText other) { return other != null && ValueComparer.Equals(_getNormalizedValue(), other._getNormalizedValue()); }

        public override bool Equals(object obj)
        {
            if (obj == null || obj is string)
                return Equals(obj as string);

            return (obj is NormalizingText) ? Equals(obj as NormalizingText) : ValueComparer.Equals(_getNormalizedValue(), obj);
        }

        public override int GetHashCode() { return ValueComparer.GetHashCode(_getNormalizedValue()); }

        public int CompareTo(string other) { return (other == null) ? 1 : ValueComparer.Compare(_getNormalizedValue(), ToNormalizedString(other)); }

        public int CompareTo(NormalizingText other) { return (other == null) ? 1 : ValueComparer.Compare(_getNormalizedValue(), other._getNormalizedValue()); }

        int IComparable.CompareTo(object obj)
        {
            if (obj == null || obj is string)
                return CompareTo(obj as string);

            return (obj is NormalizingText) ? CompareTo(obj as NormalizingText) : ValueComparer.Compare(_getNormalizedValue(), obj);
        }

        TypeCode IConvertible.GetTypeCode() { return TypeCode.String; }

        bool IConvertible.ToBoolean(IFormatProvider provider) { return (_getNormalizedValue() as IConvertible).ToBoolean(provider); }

        char IConvertible.ToChar(IFormatProvider provider) { return (_getNormalizedValue() as IConvertible).ToChar(provider); }

        sbyte IConvertible.ToSByte(IFormatProvider provider) { return (_getNormalizedValue() as IConvertible).ToSByte(provider); }

        byte IConvertible.ToByte(IFormatProvider provider) { return (_getNormalizedValue() as IConvertible).ToByte(provider); }

        short IConvertible.ToInt16(IFormatProvider provider) { return (_getNormalizedValue() as IConvertible).ToInt16(provider); }

        ushort IConvertible.ToUInt16(IFormatProvider provider) { return (_getNormalizedValue() as IConvertible).ToUInt16(provider); }

        int IConvertible.ToInt32(IFormatProvider provider) { return (_getNormalizedValue() as IConvertible).ToInt32(provider); }

        uint IConvertible.ToUInt32(IFormatProvider provider) { return (_getNormalizedValue() as IConvertible).ToUInt32(provider); }

        long IConvertible.ToInt64(IFormatProvider provider) { return (_getNormalizedValue() as IConvertible).ToInt64(provider); }

        ulong IConvertible.ToUInt64(IFormatProvider provider) { return (_getNormalizedValue() as IConvertible).ToUInt64(provider); }

        float IConvertible.ToSingle(IFormatProvider provider) { return (_getNormalizedValue() as IConvertible).ToSingle(provider); }

        double IConvertible.ToDouble(IFormatProvider provider) { return (_getNormalizedValue() as IConvertible).ToDouble(provider); }

        decimal IConvertible.ToDecimal(IFormatProvider provider) { return (_getNormalizedValue() as IConvertible).ToDecimal(provider); }

        DateTime IConvertible.ToDateTime(IFormatProvider provider) { return (_getNormalizedValue() as IConvertible).ToDateTime(provider); }

        string IConvertible.ToString(IFormatProvider provider) { return (_getNormalizedValue() as IConvertible).ToString(provider); }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider) { return (_getNormalizedValue() as IConvertible).ToType(conversionType, provider); }
    }
}
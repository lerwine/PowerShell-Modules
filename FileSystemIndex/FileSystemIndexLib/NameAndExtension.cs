using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Erwine.Leonard.T.GDIPlus
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable()]
    public struct NameAndExtension : IEquatable<NameAndExtension>, IEquatable<string>, IComparable<NameAndExtension>, IComparable<string>, IConvertible, IComparable
    {
        private string _baseName;
        private string _extension;

        /// <summary>
        /// 
        /// </summary>
        public string BaseName { get { return _baseName; } }

        /// <summary>
        /// 
        /// </summary>
        public string Extension { get { return _extension; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string ParseNameAndExtension(string name, out string extension)
        {
            if (String.IsNullOrEmpty(name))
            {
                extension = "";
                return "";
            }
            int index = name.LastIndexOf(".");
            if (index < 1)
            {
                extension = "";
                return name;
            }
            
            extension = name.Substring(index);
            return name.Substring(0, index);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="extension"></param>
        public static void NormalizeNameAndExtension(ref string baseName, ref string extension)
        {

            if (String.IsNullOrEmpty(baseName))
            {
                if (String.IsNullOrEmpty(extension))
                {
                    baseName = "";
                    extension = "";
                    return;
                }
                baseName = ParseNameAndExtension(extension, out extension);
            }
            else if (String.IsNullOrEmpty(extension))
                baseName = ParseNameAndExtension(baseName, out extension);
            else if (extension.StartsWith("."))
                baseName = ParseNameAndExtension(baseName + extension, out extension);
            else
                baseName = ParseNameAndExtension(baseName + "." + extension, out extension);
        }
        public NameAndExtension(string name) { _baseName = ParseNameAndExtension(name, out _extension); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="extension"></param>
        public NameAndExtension(string baseName, string extension)
        {
            NormalizeNameAndExtension(ref baseName, ref extension);
            _baseName = baseName;
            _extension = extension;
        }
        public static implicit operator NameAndExtension(string name) { return new NameAndExtension(name); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(NameAndExtension x, NameAndExtension y) { return x.Equals(y); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(NameAndExtension x, string y) { return x.Equals(y); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(string x, NameAndExtension y) { return y.Equals(x); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(NameAndExtension x, NameAndExtension y) { return !x.Equals(y); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(NameAndExtension x, string y) { return !x.Equals(y); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(string x, NameAndExtension y) { return !y.Equals(x); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator <(NameAndExtension x, NameAndExtension y) { return x.CompareTo(y) < 0; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator <(NameAndExtension x, string y) { return x.CompareTo(y) < 0; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator <(string x, NameAndExtension y) { return y.CompareTo(x) > 0; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator <=(NameAndExtension x, NameAndExtension y) { return x.CompareTo(y) <= 0; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator <=(NameAndExtension x, string y) { return x.CompareTo(y) <= 0; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator <=(string x, NameAndExtension y) { return y.CompareTo(x) >= 0; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator >(NameAndExtension x, NameAndExtension y) { return x.CompareTo(y) > 0; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator >(NameAndExtension x, string y) { return x.CompareTo(y) > 0; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator >(string x, NameAndExtension y) { return y.CompareTo(x) < 0; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator >=(NameAndExtension x, NameAndExtension y) { return x.CompareTo(y) >= 0; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator >=(NameAndExtension x, string y) { return x.CompareTo(y) >= 0; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator >=(string x, NameAndExtension y) { return y.CompareTo(x) <= 0; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(NameAndExtension other)
        {
            int result = String.Compare(_baseName, other.BaseName, StringComparison.OrdinalIgnoreCase);
            if (result == 0 && (result = String.Compare(_extension, other.Extension, StringComparison.OrdinalIgnoreCase)) == 0 &&
                    (result = String.Compare(_baseName, other.BaseName, StringComparison.InvariantCultureIgnoreCase)) == 0 && (result = String.Compare(_extension, other.Extension, StringComparison.InvariantCultureIgnoreCase)) == 0 &&
                    (result = String.Compare(_baseName, other.BaseName, StringComparison.Ordinal)) == 0 && (result = String.Compare(_extension, other.Extension, StringComparison.Ordinal)) == 0 &&
                    (result = String.Compare(_baseName, other.BaseName, StringComparison.InvariantCulture)) == 0)
                return String.Compare(_extension, other.Extension, StringComparison.InvariantCulture);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="caseSenstitive"></param>
        /// <returns></returns>
        public int CompareTo(NameAndExtension other, bool caseSenstitive)
        {
            if (!caseSenstitive)
                return CompareTo(other);

            int result = String.Compare(_baseName, other.BaseName, false);
            if (result == 0)
                return String.Compare(_extension, other.Extension, false);
            return result;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public int CompareTo(NameAndExtension other, StringComparison comparisonType)
        {
            int result = String.Compare(_baseName, other.BaseName, comparisonType);
            if (result == 0)
                return String.Compare(_extension, other.Extension, comparisonType);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(string other)
        {
            if (other == null)
                return 1;
            string name = ToString();
            int result = String.Compare(name, other, StringComparison.OrdinalIgnoreCase);
            if (result == 0 && (result = String.Compare(name, other, StringComparison.InvariantCultureIgnoreCase)) == 0 &&
                    (result = String.Compare(name, other, StringComparison.Ordinal)) == 0)
                return String.Compare(name, other, StringComparison.InvariantCulture);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(object other)
        {
            if (other != null && other is NameAndExtension)
                return CompareTo((NameAndExtension)other);
            
            return CompareTo(other as string);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(NameAndExtension other)
        {
            return String.Compare(_baseName, other.BaseName, StringComparison.InvariantCultureIgnoreCase) == 0 && String.Compare(_extension, other.Extension, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(string other) { return other != null && String.Compare(ToString(), other, StringComparison.InvariantCultureIgnoreCase) == 0; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj != null && obj is NameAndExtension)
                return Equals((NameAndExtension)obj);
            
            return Equals(obj as string);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() { return ToString().GetHashCode(); }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return (_extension.Length == 0) ? _baseName : _baseName + _extension; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        string IConvertible.ToString(IFormatProvider provider) { return ToString(); }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == null || conversionType.Equals(typeof(string)))
                return ToString();
            if (conversionType.Equals(typeof(NameAndExtension)))
                return this;
            return Convert.ChangeType(this, conversionType);
        }

        TypeCode IConvertible.GetTypeCode() { return TypeCode.String; }

        bool IConvertible.ToBoolean(IFormatProvider provider) { throw new NotSupportedException(); }

        byte IConvertible.ToByte(IFormatProvider provider) { throw new NotSupportedException(); }

        char IConvertible.ToChar(IFormatProvider provider) { throw new NotSupportedException(); }

        DateTime IConvertible.ToDateTime(IFormatProvider provider) { throw new NotSupportedException(); }

        decimal IConvertible.ToDecimal(IFormatProvider provider) { throw new NotSupportedException(); }

        double IConvertible.ToDouble(IFormatProvider provider) { throw new NotSupportedException(); }

        short IConvertible.ToInt16(IFormatProvider provider) { throw new NotSupportedException(); }

        int IConvertible.ToInt32(IFormatProvider provider) { throw new NotSupportedException(); }

        long IConvertible.ToInt64(IFormatProvider provider) { throw new NotSupportedException(); }

        SByte IConvertible.ToSByte(IFormatProvider provider) { throw new NotSupportedException(); }

        float IConvertible.ToSingle(IFormatProvider provider) { throw new NotSupportedException(); }

        ushort IConvertible.ToUInt16(IFormatProvider provider) { throw new NotSupportedException(); }

        uint IConvertible.ToUInt32(IFormatProvider provider) { throw new NotSupportedException(); }

        ulong IConvertible.ToUInt64(IFormatProvider provider) { throw new NotSupportedException(); }
    }
}
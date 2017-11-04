using System;
using System.Collections.Specialized;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace NetworkUtilityCLR
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName_mediaType)]
    public class MediaType : IEquatable<MediaType>, IEquatable<string>, IComparable<MediaType>, IComparable<string>, IConvertible
    {
        /// <summary>
        /// 
        /// </summary>
        public const string AttributeName_topLevel = "topLevel";

        /// <summary>
        /// 
        /// </summary>
        public const string AttributeName_subType = "subType";

        /// <summary>
        /// 
        /// </summary>
        public const string ElementName_mediaType = "mediaType";

        /// <summary>
        /// 
        /// </summary>
        public const string ElementName_parameters = "parameters";

        /// <summary>
        /// 
        /// </summary>
        public const string TopLevelType_Text = "text";

        /// <summary>
        /// 
        /// </summary>
        public const string TopLevelType_Image = "image";

        /// <summary>
        /// 
        /// </summary>
        public const string TopLevelType_Audio = "audio";

        /// <summary>
        /// 
        /// </summary>
        public const string TopLevelType_Video = "video";

        /// <summary>
        /// 
        /// </summary>
        public const string TopLevelType_Application = "application";

        /// <summary>
        /// 
        /// </summary>
        public const string TopLevelType_Multipart = "multipart";

        /// <summary>
        /// 
        /// </summary>
        public const string TopLevelType_Message = "message";
        
        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex SpecialCharsRegex = new Regex(@"\(\)\<\>@,;:\\""/\[\]\?\.=");

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex TokenRegex = new Regex(@"[^()<>@,;:\\""/\[\]?.=\s/p{C}]+");

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex LooseRegex = new Regex(@"^(?<topLevel>[^()<>@,;:\\""/\[\]?.=\s/p{C}]+)(/(?<subType>[^()<>@,;:\\""/\[\]?.=\s/p{C}]+))?\s*;(?<parameters>.+)?$");

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex ContentTypeRegex = new Regex(@"^(?<topLevel>[^()<>@,;:\\""/\[\]?.=\s/p{C}]+)/(?<subType>[^()<>@,;:\\""/\[\]?.=\s/p{C}]+)\s*;(?<parameters>.+)?$");

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex MediaTypeRegex = new Regex(@"^(?<topLevel>[^()<>@,;:\\""/\[\]?.=\s/p{C}]+)/(?<subType>[^()<>@,;:\\""/\[\]?.=\s/p{C}]+)$");
        
        private string _topLevelType = "";
        private string _subType = "";
        private StringDictionary _parameters = new StringDictionary();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediaTypeString"></param>
        /// <returns></returns>
        public static MediaType Parse(string mediaTypeString) { return new MediaType(mediaTypeString); }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediaTypeString"></param>
        /// <param name="mediaType"></param>
        /// <returns></returns>
        public static bool TryParse(string mediaTypeString, out MediaType mediaType)
        {
            mediaType = null;
            if (!String.IsNullOrEmpty(mediaTypeString))
            {
                Match m = MediaTypeRegex.Match(mediaTypeString);
                if (!m.Success)
                {
                    m = ContentTypeRegex.Match(mediaTypeString);
                    if (m.Success)
                    {
                        if (m.Groups["parameters"].Success && m.Groups["parameters"].Value.Trim().Length > 0)
                        {
                            try
                            {
                                ContentType contentType = new ContentType(mediaTypeString);
                                mediaType = new MediaType(contentType);
                            } catch { }
                        }
                        else
                            mediaType = new MediaType(m.Groups["topLevel"].Value, m.Groups["subType"].Value);
                    }
                    if (mediaType == null)
                    {
                        m = LooseRegex.Match(mediaTypeString);
                        if (m.Success)
                        {
                            if (m.Groups["parameters"].Success && m.Groups["parameters"].Value.Trim().Length > 0)
                            {
                                try
                                {
                                    ContentType contentType = new ContentType(MediaTypeNames.Application.Octet + "; " + m.Groups["parameters"].Value);
                                    if (m.Groups["subType"].Success)
                                        mediaType = new MediaType(m.Groups["topLevel"].Value, m.Groups["subType"].Value, contentType.Parameters);
                                    else
                                        mediaType = new MediaType(m.Groups["topLevel"].Value, null, contentType.Parameters);
                                } catch { }
                            } else if (m.Groups["subType"].Success)
                                mediaType = new MediaType(m.Groups["topLevel"].Value, m.Groups["subType"].Value);
                            else
                                mediaType = new MediaType(m.Groups["topLevel"].Value, null);
                        }
                    }
                }
                else
                    mediaType = new MediaType(m.Groups["topLevel"].Value, m.Groups["subType"].Value);
            }
            
            return mediaType != null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediaTypeString"></param>
        /// <returns></returns>
        public static bool Validate(string mediaTypeString)
        {
            if (String.IsNullOrEmpty(mediaTypeString))
                return false;
            if (MediaTypeRegex.IsMatch(mediaTypeString))
                return true;
            
            Match m = ContentTypeRegex.Match(mediaTypeString);
            if (!m.Success)
                return false;
            if (m.Groups["parameters"].Success && m.Groups["parameters"].Value.Trim().Length > 0)
                try { new ContentType(mediaTypeString); } catch { return false; }
            return true;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool ValidateToken(string token) { return !String.IsNullOrEmpty(token) && token.Trim() == token && TokenRegex.IsMatch(token); }
        
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName_topLevel)]
        public string TopLevelType
        {
            get { return _topLevelType; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    _topLevelType = "";
                    return;
                }
                
                if (!ValidateToken(value))
                    throw new FormatException("Invalid token");
                
                _topLevelType = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(AttributeName_subType)]
        public string SubType
        {
            get { return _subType; }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    _subType = "";
                    return;
                }
                
                if (!ValidateToken(value))
                    throw new FormatException("Invalid token");
                
                _subType = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [XmlArray(ElementName_parameters)]
        public StringDictionary Parameters { get { return _parameters; } set { _parameters = (value == null) ? new StringDictionary() : value; } }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediaTypeString"></param>
        public MediaType(string mediaTypeString)
        {
            if (String.IsNullOrEmpty(mediaTypeString))
                return;
    
            Match m = MediaTypeRegex.Match(mediaTypeString);
            ContentType contentType = null;
            if (!m.Success)
            {
                m = ContentTypeRegex.Match(mediaTypeString);
                bool success = m.Success;
                if (!success)
                {
                    m = LooseRegex.Match(mediaTypeString);
                    success = m.Success;
                    if (m.Success && m.Groups["parameters"].Success && m.Groups["parameters"].Value.Trim().Length > 0)
                        contentType = new ContentType(MediaTypeNames.Application.Octet + "; " + m.Groups["parameters"].Value);
                }
                else if (m.Groups["parameters"].Success && m.Groups["parameters"].Value.Trim().Length > 0)
                {
                    contentType = new ContentType(mediaTypeString);
                    Match m2 = MediaTypeRegex.Match(contentType.MediaType);
                    if (m2.Success)
                        m = m2;
                }
                if (!success)
                    throw new FormatException("Invalid media type.");
            }
            _topLevelType = m.Groups["topLevel"].Value;
            _subType = m.Groups["subType"].Value;
            if (contentType != null)
            {
                foreach (string key in contentType.Parameters)
                    _parameters.Add(key, contentType.Parameters[key]);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentType"></param>
        public MediaType(ContentType contentType)
        {
            if (contentType == null)
                return;
            
            Match m = MediaTypeRegex.Match(contentType.MediaType);
            if (m.Success)
                _subType = m.Groups["subType"].Value;
            else
            {
                m = LooseRegex.Match(contentType.MediaType);
                if (!m.Success)
                    throw new FormatException("Invalid content type.");
                if (m.Groups["subType"].Success)
                    _subType = m.Groups["subType"].Value;
            }
            _topLevelType = m.Groups["topLevel"].Value;
            foreach (string key in contentType.Parameters)
                _parameters.Add(key, contentType.Parameters[key]);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="topLevelType"></param>
        /// <param name="subType"></param>
        public MediaType(string topLevelType, string subType)
        {
            if (!String.IsNullOrEmpty(topLevelType))
            {
                if (!ValidateToken(topLevelType))
                    throw new FormatException("Invalid top-level type token string.");
                _topLevelType = topLevelType;
            }
            
            if (String.IsNullOrEmpty(subType))
                return;

            if (!ValidateToken(subType))
                throw new FormatException("Invalid sub-type token string.");
            _subType = subType;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="topLevelType"></param>
        /// <param name="subType"></param>
        /// <param name="parameters"></param>
        public MediaType(string topLevelType, string subType, StringDictionary parameters)
            : this(topLevelType, subType)
        {
            if (parameters == null)
                return;
            
            foreach (string key in parameters)
                _parameters.Add(key, parameters[key]);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public static implicit operator MediaType(string name) { return new MediaType(name); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(MediaType x, MediaType y) { return (x == null) ? y == null : y != null && x.Equals(y); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(MediaType x, string y) { return (x == null) ? y == null : y != null && x.Equals(y); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(string x, MediaType y) { return (x == null) ? y == null : y != null && y.Equals(x); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(MediaType x, MediaType y) { return (x == null) ? y != null : y == null || !x.Equals(y); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(MediaType x, string y) { return (x == null) ? y != null : y == null || !x.Equals(y); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(string x, MediaType y) { return (x == null) ? y != null : y == null || !y.Equals(x); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator <(MediaType x, MediaType y) { return (x == null) ? (y != null) : (y != null && x.CompareTo(y) < 0) ; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator <(MediaType x, string y) { return (x == null) ? (y != null) : (y != null && x.CompareTo(y) < 0); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator <(string x, MediaType y) { return (x == null) ? (y != null) : (y != null && y.CompareTo(x) > 0); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator <=(MediaType x, MediaType y) { return x == null || (y != null && x.CompareTo(y) <= 0); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator <=(MediaType x, string y) { return x == null || (y != null && x.CompareTo(y) <= 0); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator <=(string x, MediaType y) { return x == null || (y != null && y.CompareTo(x) >= 0); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator >(MediaType x, MediaType y) { return x != null && (y == null || x.CompareTo(y) > 0); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator >(MediaType x, string y) { return x != null && (y == null || x.CompareTo(y) > 0); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator >(string x, MediaType y) { return y.CompareTo(x) < 0; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator >=(MediaType x, MediaType y) { return (x == null) ? (y == null) : (y == null || x.CompareTo(y) >= 0); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator >=(MediaType x, string y) { return (x == null) ? (y == null) : (y == null || x.CompareTo(y) >= 0); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator >=(string x, MediaType y) { return (x == null) ? (y == null) : (y == null || y.CompareTo(x) <= 0); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(MediaType other)
        {
            if (other == null)
                return 1;
            
            int result = String.Compare(_topLevelType, other._topLevelType, StringComparison.OrdinalIgnoreCase);
            if (result == 0 && (result = String.Compare(_subType, other._subType, StringComparison.OrdinalIgnoreCase)) == 0 &&
                    (result = String.Compare(_topLevelType, other._topLevelType, StringComparison.InvariantCultureIgnoreCase)) == 0 && (result = String.Compare(_subType, other._subType, StringComparison.InvariantCultureIgnoreCase)) == 0 &&
                    (result = String.Compare(_topLevelType, other._topLevelType, StringComparison.Ordinal)) == 0 && (result = String.Compare(_subType, other._subType, StringComparison.Ordinal)) == 0 &&
                    (result = String.Compare(_topLevelType, other._topLevelType, StringComparison.InvariantCulture)) == 0)
                return String.Compare(_subType, other._subType, StringComparison.InvariantCulture);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="caseSenstitive"></param>
        /// <returns></returns>
        public int CompareTo(MediaType other, bool caseSenstitive)
        {
            if (other == null)
                return 1;
            
            if (!caseSenstitive)
                return CompareTo(other);

            int result = String.Compare(_topLevelType, other._topLevelType, false);
            if (result == 0)
                return String.Compare(_subType, other._subType, false);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <param name="comparisonType"></param>
        /// <returns></returns>
        public int CompareTo(MediaType other, StringComparison comparisonType)
        {
            if (other == null)
                return 1;
            
            int result = String.Compare(_topLevelType, other._topLevelType, comparisonType);
            if (result == 0)
                return String.Compare(_subType, other._subType, comparisonType);
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
            if (other != null && other is MediaType)
                return CompareTo((MediaType)other);
            
            return CompareTo(other as string);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MediaType other)
        {
            if (other == null)
                return false;

            return String.Compare(_topLevelType, other._topLevelType, StringComparison.InvariantCultureIgnoreCase) == 0 && String.Compare(_subType, other._subType, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(string other)
        {
            if (other == null)
                return false;
            
            return other != null && String.Compare(ToString(), other, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj != null && obj is MediaType)
                return Equals((MediaType)obj);
            
            return Equals(obj as string);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ContentType ToContentType()
        {
            ContentType contentType = new ContentType(((_topLevelType.Length == 0) ? "undefined" : _topLevelType + "/") + ((_subType.Length == 0) ? "undefined" : _subType));
            foreach (string key in _parameters.Keys)
                contentType.Parameters.Add(key, _parameters[key]);
            return contentType;
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
        public override string ToString()
        {
            if (_parameters.Count == 0)
                return (_subType.Length == 0) ? _topLevelType : _topLevelType + "/" + _subType;
        
            if (_topLevelType.Length > 0 && _subType.Length > 0)
                try { return ToContentType().ToString(); } catch { }
            
            ContentType contentType = new ContentType();
            foreach (string key in _parameters.Keys)
                contentType.Parameters.Add(key, _parameters[key]);
            string result = contentType.ToString();
            return ((_subType.Length == 0) ? _topLevelType : _topLevelType + "/" + _subType) + result.Substring(result.IndexOf(";"));
        }
        
        string IConvertible.ToString(IFormatProvider provider) { return ToString(); }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            if (conversionType == null || conversionType.Equals(typeof(string)))
                return ToString();
            if (conversionType.Equals(typeof(ContentType)))
                return ToContentType();
            if (conversionType.Equals(typeof(MediaType)))
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

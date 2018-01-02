using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CredentialStorageLibrary
{
    public class MetaDataItem : IXmlSerializable
    {
        public const string Xmlns_Serialization = "http://schemas.microsoft.com/2003/10/Serialization/";

        public const string Xmlns_System = "http://schemas.datacontract.org/2004/07/System";

        public const string Xmlns_XmlSchemaInstance = "http://www.w3.org/2001/XMLSchema-instance";

        public const string Xmlns_XmlSchema = "http://www.w3.org/2001/XMLSchema";

        public const string Xmlns_Xmlns = "http://www.w3.org/2000/xmlns/";

        private object _syncRoot = new object();
        private NormalizingText _key = null;
        private List<Tuple<NormalizingText, KeyedCollection<NormalizingText, MetaDataItem>>> _keyOverrides = new List<Tuple<NormalizingText, KeyedCollection<NormalizingText, MetaDataItem>>>();
        private object _value = null;
        
        public MetaDataItem(NormalizingText key, object value)
        {
            Value = value;
            Key = key;
        }

        public object SyncRoot { get { return _syncRoot; } }
        
        public string Key
        {
            get
            {
                if (_key == null)
                    _key = new NormalizingText();
                return _key.ToString();
            }
            set
            {
                Monitor.Enter(_syncRoot);
                try
                {
                    if (_keyOverrides.Count > 0)
                        throw new InvalidOperationException("Key cannot be changed while it is a member of a collection.");
                    _key = value;
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }

        public static bool IsConvertibleType(object obj)
        {
            if (obj != null && obj is IConvertible)
            {
                IConvertible convertible = obj as IConvertible;
                try
                {
                    switch (convertible.GetTypeCode())
                    {
                        case TypeCode.DBNull:
                        case TypeCode.Empty:
                        case TypeCode.Object:
                            return false;
                    }
                    return true;
                }
                catch { }
            }

            return false;

        }
        public static bool IsSupportedType(object obj)
        {
            if (obj == null || obj is string || obj is byte[] || obj is DateTime || obj is DateTimeOffset || obj is decimal || obj is Guid || obj is TimeSpan || obj is Uri || obj is Version || obj is XmlDocument || obj is XmlElement || obj is IXmlSerializable)
                return true;

            if (!(obj is IEnumerable || obj is IDictionary))
            {
                Type t = obj.GetType();
                if (!(t.IsGenericType || t.IsPointer || !t.IsPublic) && (t.IsPrimitive || (t.IsEnum && t.IsPublic)))
                    return true;
            }

            return IsConvertibleType(obj);
        }

        public object Value
        {
            get { return _value; }
            set
            {
                if (IsSupportedType(value))
                {
                    _value = value;
                    return;
                }

                if (!IsConvertibleType(value))
                    throw new NotSupportedException("Type " + LanguagePrimitives.ConvertTypeNameToPSTypeName(value.GetType().FullName) + " is not supported.");
                IConvertible convertible = value as IConvertible;
                try
                {
                    TypeCode typeCode = convertible.GetTypeCode();
                    try
                    {
                        switch (convertible.GetTypeCode())
                        {
                            case TypeCode.Boolean:
                                _value = convertible.ToBoolean(CultureInfo.CurrentCulture);
                                break;
                            case TypeCode.Byte:
                                _value = convertible.ToByte(CultureInfo.CurrentCulture);
                                break;
                            case TypeCode.Char:
                                _value = convertible.ToChar(CultureInfo.CurrentCulture);
                                break;
                            case TypeCode.DateTime:
                                _value = convertible.ToDateTime(CultureInfo.CurrentCulture);
                                break;
                            case TypeCode.Decimal:
                                _value = convertible.ToDecimal(CultureInfo.CurrentCulture);
                                break;
                            case TypeCode.Double:
                                _value = convertible.ToDouble(CultureInfo.CurrentCulture);
                                break;
                            case TypeCode.Int16:
                                _value = convertible.ToInt16(CultureInfo.CurrentCulture);
                                break;
                            case TypeCode.Int32:
                                _value = convertible.ToInt32(CultureInfo.CurrentCulture);
                                break;
                            case TypeCode.Int64:
                                _value = convertible.ToInt64(CultureInfo.CurrentCulture);
                                break;
                            case TypeCode.SByte:
                                _value = convertible.ToSByte(CultureInfo.CurrentCulture);
                                break;
                            case TypeCode.Single:
                                _value = convertible.ToSingle(CultureInfo.CurrentCulture);
                                break;
                            case TypeCode.UInt16:
                                _value = convertible.ToUInt16(CultureInfo.CurrentCulture);
                                break;
                            case TypeCode.UInt32:
                                _value = convertible.ToUInt32(CultureInfo.CurrentCulture);
                                break;
                            case TypeCode.UInt64:
                                _value = convertible.ToUInt64(CultureInfo.CurrentCulture);
                                break;
                            default:
                                _value = convertible.ToString(CultureInfo.CurrentCulture);
                                break;
                        }

                    }
                    catch (Exception exc)
                    {
                        throw new NotSupportedException("Type " + LanguagePrimitives.ConvertTypeNameToPSTypeName(value.GetType().FullName) + " cannot be converted to " + typeCode.ToString("F") + ".");
                    }
                }
                catch (Exception e)
                {
                    throw new NotSupportedException("Error converting " + LanguagePrimitives.ConvertTypeNameToPSTypeName(value.GetType().FullName) + ": " + e.Message, e);
                }
            }
        }

        public IEnumerable<KeyedCollection<NormalizingText, MetaDataItem>> GetParentCollections() { return _keyOverrides.Select(i => i.Item2).Where(i => i != null); }

        public IEnumerable<KeyedCollection<NormalizingText, MetaDataItem>> RemoveFromAll()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                foreach (KeyedCollection<NormalizingText, MetaDataItem> removeFrom in GetParentCollections().ToArray())
                {
                    if (removeFrom.Remove(this))
                        yield return removeFrom;
                }
            }
            finally { Monitor.Exit(_syncRoot); }

        }

        public bool RemoveFrom(KeyedCollection<NormalizingText, MetaDataItem> collection)
        {
            if (collection == null)
                return false;

            Monitor.Enter(_syncRoot);
            try
            {
                int index = -1;
                for (int i = 0; i < _keyOverrides.Count - 1; i++)
                {
                    if (ReferenceEquals(_keyOverrides[i].Item2, collection))
                    {
                        index = i;
                        break;
                    }
                }
                if (index > -1)
                    return collection.Remove(this);
            }
            finally { Monitor.Exit(_syncRoot); }

            return false;
        }

        internal void SetKey(NormalizingText value, KeyedCollection<NormalizingText, MetaDataItem> collection)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (collection == null)
                throw new ArgumentNullException("collection");
            Monitor.Enter(_syncRoot);
            try
            {
                if (_keyOverrides.Count == 0)
                {
                    _keyOverrides.Add(new Tuple<NormalizingText, KeyedCollection<NormalizingText, MetaDataItem>>(value, collection));
                    _keyOverrides.Add(new Tuple<NormalizingText, KeyedCollection<NormalizingText, MetaDataItem>>(_key, collection));
                }
                else
                {
                    int index = -1;
                    for (int i = 0; i < _keyOverrides.Count - 1; i++)
                    {
                        if (ReferenceEquals(_keyOverrides[i].Item2, collection))
                        {
                            index = i;
                            break;
                        }
                    }
                    if (index < 0)
                        _keyOverrides.Insert(0, new Tuple<NormalizingText, KeyedCollection<NormalizingText, MetaDataItem>>(value, collection));
                    else if (index > 0)
                    {
                        _keyOverrides.RemoveAt(index);
                        _keyOverrides.Insert(0, new Tuple<NormalizingText, KeyedCollection<NormalizingText, MetaDataItem>>(value, collection));
                    }
                }
                _key = _keyOverrides[0].Item1;
            } finally { Monitor.Exit(_syncRoot); }
        }

        internal void ReleaseKey(KeyedCollection<NormalizingText, MetaDataItem> collection)
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_keyOverrides.Count == 0)
                    return;
                int index = -1;
                for (int i = 0; i < _keyOverrides.Count - 1; i++)
                {
                    if (ReferenceEquals(_keyOverrides[i].Item2, collection))
                    {
                        index = i;
                        break;
                    }
                }
                if (index < 0)
                    return;
                _keyOverrides.RemoveAt(index);
                _key = _keyOverrides[0].Item1;
                if (_keyOverrides.Count == 1)
                    _keyOverrides.Clear();
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        XmlSchema IXmlSerializable.GetSchema() { return null; }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            bool isNil = false;
            string xsdTypeName = null;
            string clrTypeName = null;
            string key = null;
            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    if (reader.NamespaceURI == Xmlns_XmlSchemaInstance)
                    {
                        if (reader.LocalName == "nil")
                        {
                            try
                            {
                                if (XmlConvert.ToBoolean(reader.Value))
                                    isNil = true;
                            }
                            catch { /* okay to ignore */ }
                        }
                        else if (reader.LocalName == "type")
                            xsdTypeName = reader.Value;
                    }
                    else if (reader.NamespaceURI == Xmlns_Serialization && reader.LocalName == "Type")
                        clrTypeName = reader.Value;
                    else if (reader.NamespaceURI.Length == 0 && reader.LocalName == "Key")
                        key = reader.Value;
                } while (reader.MoveToNextAttribute());
                reader.MoveToElement();
                if (reader.IsEmptyElement)
                    _value = new object();
                else if (xsdTypeName != null)
                {
                    string[] p = xsdTypeName.Split(':');
                    xsdTypeName = p[p.Length - 1];
                    switch (xsdTypeName)
                    {
                        case "base64Binary":
                            _value = Convert.FromBase64String(reader.ReadContentAsString());
                            break;
                        case "boolean":
                            _value = reader.ReadContentAsBoolean();
                            break;
                        case "unsignedByte":
                            _value = (byte)(reader.ReadContentAsInt());
                            break;
                        case "byte":
                            _value = (sbyte)(reader.ReadContentAsInt());
                            break;
                        case "short":
                            _value = (short)(reader.ReadContentAsInt());
                            break;
                        case "unsignedShort":
                            _value = (ushort)(reader.ReadContentAsInt());
                            break;
                        case "int":
                            _value = reader.ReadContentAsInt();
                            break;
                        case "unsignedInt":
                            _value = (uint)(reader.ReadContentAsLong());
                            break;
                        case "long":
                            _value = reader.ReadContentAsLong();
                            break;
                        case "unsignedLong":
                            _value = XmlConvert.ToUInt64(reader.ReadContentAsString());
                            break;
                        case "float":
                            _value = reader.ReadContentAsFloat();
                            break;
                        case "double":
                            _value = reader.ReadContentAsDouble();
                            break;
                        case "decimal":
                            _value = reader.ReadContentAsDecimal();
                            break;
                        case "anyURI":
                            _value = new Uri(reader.ReadContentAsString(), UriKind.RelativeOrAbsolute);
                            break;
                        case "dateTime":
                            _value = reader.ReadContentAsDateTime();
                            break;
                        case "duration":
                            _value = XmlConvert.ToTimeSpan(reader.ReadContentAsString());
                            break;
                        case "guid":
                            _value = XmlConvert.ToGuid(reader.ReadContentAsString());
                            break;
                        case "char":
                            XmlConvert.ToChar(reader.ReadContentAsString());
                            break;
                        case "anyType":
                            break;
                        default:
                            _value = reader.ReadContentAsString();
                            break;
                    }
                    return;
                }
                if (string.IsNullOrEmpty(clrTypeName))
                    _value = reader.ReadContentAsString();
                else
                {
                    if (clrTypeName == "Version")
                        _value = new Version(reader.ReadContentAsString());
                    else
                    {
                        Type t = Type.GetType(clrTypeName, false);
                        if (t == null)
                            _value = reader.ReadContentAsString();
                        else if (t.IsEnum)
                            _value = Enum.Parse(t, reader.ReadContentAsString());
                        else
                        {
                            IXmlSerializable serializable = (IXmlSerializable)(Activator.CreateInstance(t));
                            _value = serializable;
                            serializable.ReadXml(reader);
                        }
                    }
                }
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Key", Key);
            object value = _value;
            if (value == null)
            {
                writer.WriteAttributeString("nil", Xmlns_XmlSchemaInstance, "true");
                return;
            }

            string xsPrefix = writer.LookupPrefix(Xmlns_XmlSchema);
            if (String.IsNullOrEmpty(xsPrefix))
            {
                xsPrefix = "xs";
                writer.WriteAttributeString("xs", Xmlns_Xmlns, Xmlns_XmlSchema);
            }
            string serPrefix = writer.LookupPrefix(Xmlns_Serialization);
            if (String.IsNullOrEmpty(serPrefix))
            {
                serPrefix = "ser";
                writer.WriteAttributeString("ser", Xmlns_Xmlns, Xmlns_Serialization);
            }
                
            Type t = value.GetType();
            if (value is string)
            {
                writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":string");
                writer.WriteCData(value as string);
            }
            else if (value is byte[])
            {
                writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":base64Binary");
                byte[] b = value as byte[];
                writer.WriteBase64(b, 0, b.Length);
            }
            else if (value is bool)
            {
                writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":boolean");
                writer.WriteString(XmlConvert.ToString((bool)value));
            }
            else if (value is byte)
            {
                writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":unsignedByte");
                writer.WriteString(XmlConvert.ToString((byte)value));
            }
            else if (value is sbyte)
            {
                writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":byte");
                writer.WriteString(XmlConvert.ToString((sbyte)value));
            }
            else if (value is short)
            {
                writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":short");
                writer.WriteString(XmlConvert.ToString((short)value));
            }
            else if (value is ushort)
            {
                writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":unsignedShort");
                writer.WriteString(XmlConvert.ToString((ushort)value));
            }
            else if (value is int)
            {
                writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":int");
                writer.WriteString(XmlConvert.ToString((int)value));
            }
            else if (value is uint)
            {
                writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":unsignedInt");
                writer.WriteString(XmlConvert.ToString((uint)value));
            }
            else if (value is long)
            {
                writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":long");
                writer.WriteString(XmlConvert.ToString((long)value));
            }
            else if (value is ulong)
            {
                writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":unsignedLong");
                writer.WriteString(XmlConvert.ToString((ulong)value));
            }
            else if (value is float)
            {
                writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":float");
                writer.WriteString(XmlConvert.ToString((float)value));
            }
            else if (value is double)
            {
                writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":double");
                writer.WriteString(XmlConvert.ToString((double)value));
            }
            else if (value is decimal)
            {
                writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":decimal");
                writer.WriteString(XmlConvert.ToString((decimal)value));
            }
            else if (value is Uri)
            {
                writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":anyURI");
                writer.WriteString(value.ToString());
            }
            else if (value is DateTime)
            {
                writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":dateTime");
                writer.WriteString(XmlConvert.ToString((DateTime)value, "yyyy-MM-ddTHH:mm:ss.ffffffzzzzzz"));
            }
            else if (value is TimeSpan)
            {
                writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":duration");
                writer.WriteString(XmlConvert.ToString((decimal)value));
            }
            else if (value is Guid)
            {
                writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, serPrefix + ":guid");
                writer.WriteString(XmlConvert.ToString((Guid)value));
            }
            else if (value is char)
            {
                writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, serPrefix + ":char");
                writer.WriteString(XmlConvert.ToString((char)value));
            }
            else if (value is XmlDocument || value is XmlElement)
            {
                writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":anyType");
                (value as XmlNode).WriteTo(writer);
            }
            else
            {
                writer.WriteAttributeString("Type", Xmlns_Serialization, t.FullName);
                if (value is Version)
                {
                    Version v = value as Version;
                    writer.WriteString(v.ToString((v.Revision > 0) ? 4 : ((v.Build > 0) ? 3 : 2)));
                }
                else if (t.IsEnum)
                    writer.WriteString(Enum.Format(t, value, "G"));
                else if (value is IXmlSerializable)
                    (value as IXmlSerializable).WriteXml(writer);
                else if (value is IConvertible)
                {
                    IConvertible convertible = value as IConvertible;
                    switch ((value as IConvertible).GetTypeCode())
                    {
                        case TypeCode.Boolean:
                            writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":boolean");
                            writer.WriteString(XmlConvert.ToString(convertible.ToBoolean(CultureInfo.CurrentCulture)));
                            break;
                        case TypeCode.Byte:
                            writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":unsignedByte");
                            writer.WriteString(XmlConvert.ToString(convertible.ToByte(CultureInfo.CurrentCulture)));
                            break;
                        case TypeCode.Char:
                            writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, serPrefix + ":char");
                            writer.WriteString(XmlConvert.ToString(convertible.ToChar(CultureInfo.CurrentCulture)));
                            break;
                        case TypeCode.DateTime:
                            writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":dateTime");
                            writer.WriteString(XmlConvert.ToString(convertible.ToDateTime(CultureInfo.CurrentCulture), "yyyy-MM-ddTHH:mm:ss.ffffffzzzzzz"));
                            break;
                        case TypeCode.Decimal:
                            writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":decimal");
                            writer.WriteString(XmlConvert.ToString(convertible.ToDecimal(CultureInfo.CurrentCulture)));
                            break;
                        case TypeCode.Double:
                            writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":double");
                            writer.WriteString(XmlConvert.ToString(convertible.ToDouble(CultureInfo.CurrentCulture)));
                            break;
                        case TypeCode.Int16:
                            writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":short");
                            writer.WriteString(XmlConvert.ToString(convertible.ToInt16(CultureInfo.CurrentCulture)));
                            break;
                        case TypeCode.Int32:
                            writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":int");
                            writer.WriteString(XmlConvert.ToString(convertible.ToInt32(CultureInfo.CurrentCulture)));
                            break;
                        case TypeCode.Int64:
                            writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":long");
                            writer.WriteString(XmlConvert.ToString(convertible.ToInt64(CultureInfo.CurrentCulture)));
                            break;
                        case TypeCode.SByte:
                            writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":byte");
                            writer.WriteString(XmlConvert.ToString(convertible.ToSByte(CultureInfo.CurrentCulture)));
                            break;
                        case TypeCode.Single:
                            writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":float");
                            writer.WriteString(XmlConvert.ToString(convertible.ToSingle(CultureInfo.CurrentCulture)));
                            break;
                        case TypeCode.UInt16:
                            writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":unsignedShort");
                            writer.WriteString(XmlConvert.ToString(convertible.ToUInt16(CultureInfo.CurrentCulture)));
                            break;
                        case TypeCode.UInt32:
                            writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":unsignedInt");
                            writer.WriteString(XmlConvert.ToString(convertible.ToUInt32(CultureInfo.CurrentCulture)));
                            break;
                        case TypeCode.UInt64:
                            writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":unsignedLong");
                            writer.WriteString(XmlConvert.ToString(convertible.ToUInt64(CultureInfo.CurrentCulture)));
                            break;
                        default:
                            writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":string");
                            writer.WriteCData(convertible.ToString(CultureInfo.CurrentCulture));
                            break;
                    }
                }
                else
                {
                    writer.WriteAttributeString("type", Xmlns_XmlSchemaInstance, xsPrefix + ":string");
                    writer.WriteCData(value.ToString());
                }
            }
        }
    }
}
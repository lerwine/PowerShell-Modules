using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace XmlUtility
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class SchemaLoadResult
    {
        private string _path;
        private XmlSchemaSet _schemas;
        private Collection<ValidationErrorInfo> _innerValidationError = new Collection<ValidationErrorInfo>();
        private ReadOnlyCollection<ValidationErrorInfo> _validationError;
        public XmlSchemaSet Schemas { get { return _schemas; } }
        public ReadOnlyCollection<ValidationErrorInfo> ValidationError { get { return _validationError; } }
        private void SchemaLoad_ValidationEventHandler(object sender, ValidationEventArgs e) { _innerValidationError.Add(new ValidationErrorInfo(e, _path)); }

        public SchemaLoadResult(string path, XmlReaderSettings settings, bool doNotCompile)
        {
            _path = path;
            XmlSchema schema;
            _schemas = new XmlSchemaSet();
            _schemas.ValidationEventHandler += SchemaLoad_ValidationEventHandler;
            using (XmlReader reader = XmlReader.Create(path, settings))
                schema = XmlSchema.Read(reader, SchemaLoad_ValidationEventHandler);
            if (schema != null)
            {
                _schemas.Add(schema);
                if (!doNotCompile)
                    _schemas.Compile();
            }
            _validationError = new ReadOnlyCollection<ValidationErrorInfo>(_innerValidationError);
        }
        
        public class ValidationErrorInfo : IXmlLineInfo, IEquatable<ValidationErrorInfo>, IComparable<ValidationErrorInfo>, IComparable
        {
            private string _message;
            private bool _isWarning;
            private int _lineNumber;
            private int _linePosition;
            private string _source;
            private bool _hasLineInfo;
            private string _toString = null;
            private Exception _exception;
            
            public string Message { get { return _message; } }
            
            public bool IsWarning { get { return _isWarning; } }

            public int LineNumber { get { return _lineNumber; } }

            public int LinePosition { get { return _linePosition; } }

            public string Source { get { return _message; } }
            
            public Exception Exception { get { return _exception; } }

            internal ValidationErrorInfo(ValidationEventArgs e, string defaultSourcePath)
            {
                _message = e.Message;
                _isWarning = e.Severity != XmlSeverityType.Error;
                _exception = e.Exception;
                if (_exception != null)
                {
                    if (!String.IsNullOrEmpty(e.Message) && (String.IsNullOrEmpty(_message) || e.Message.Trim() != _message.Trim()))
                        _message = e.Message;
                    Initialize(e.Exception, defaultSourcePath);
                }
                else
                {
                    _hasLineInfo = false;
                    _lineNumber = 0;
                    _linePosition = 0;
                }

                if (_message == null)
                    _message = "";
            }

            internal ValidationErrorInfo(Exception ex, string psPath)
            {
                _exception = ex;
                _message = ex.Message;
                if (ex is XmlSchemaException)
                    Initialize((XmlSchemaException)ex, psPath);
                else
                {
                    _source = (String.IsNullOrEmpty(ex.Source)) ? ((psPath == null) ? "" : psPath) : ex.Source;
                    if (ex is IXmlLineInfo)
                    {
                        IXmlLineInfo xmlLineInfo = (IXmlLineInfo)ex;
                        _hasLineInfo = xmlLineInfo.HasLineInfo();
                        if (_hasLineInfo)
                        {
                            _lineNumber = xmlLineInfo.LineNumber;
                            _linePosition = xmlLineInfo.LinePosition;
                            return;
                        }
                    }
                    _hasLineInfo = false;
                    _lineNumber = 0;
                    _linePosition = 0;
                }
            }

            public bool HasLineInfo() { return _hasLineInfo; }

            private void Initialize(XmlSchemaException ex, string defaultSourcePath)
            {
                _hasLineInfo = true;
                _lineNumber = ex.LineNumber;
                _linePosition = ex.LinePosition;
                if (String.IsNullOrEmpty(ex.SourceUri))
                    _source = (String.IsNullOrEmpty(ex.Source)) ? ((defaultSourcePath == null) ? "" : defaultSourcePath) : ex.Source;
                else
                    _source = ex.SourceUri;
            }

            public bool Equals(ValidationErrorInfo other) { return other != null && ReferenceEquals(other, this); }

            public override bool Equals(object obj)
            {
                if (obj == null || obj is ValidationErrorInfo)
                    return Equals(obj as ValidationErrorInfo);
                return (obj is string && StringComparer.CurrentCulture.Equals(ToString(), (string)obj));
            }

            public override int GetHashCode() { return ToString().GetHashCode(); }

            public override string ToString()
            {
                if (_toString != null)
                    return _toString;
                
                StringBuilder sb = new StringBuilder((_isWarning) ? "Warning" : "Error");
                string message = _source.Trim();
                if (message.Length > 0)
                    sb.Append(" ").Append(message);
                if (_hasLineInfo)
                    sb.Append(" (").Append(_lineNumber.ToString()).Append(", ").Append(_linePosition.ToString()).Append(")");
                
                message = _message.Trim();
                if (message.Length > 0)
                    sb.Append(":").Append(message);
                if (Exception == null)
                    return sb.ToString();
                for (Exception e = Exception; e != null; e = e.InnerException)
                {
                    sb.AppendLine().Append("\t").Append(e.GetType().Name);
                    string m = (e.Message == null) ? "" : e.Message.Trim();
                    if (e is XmlSchemaException)
                    {
                        XmlSchemaException x = (XmlSchemaException)e;
                        if (!_hasLineInfo || x.LineNumber != _lineNumber || x.LinePosition != _linePosition)
                        {
                            sb.Append(" (").Append(x.LineNumber.ToString()).Append(", ").Append(x.LinePosition.ToString());
                            if (m.Length > 0)
                                sb.Append("): ").Append(m);
                            else
                                sb.Append(")");
                        }
                        else if (m.Length > 0 && m != _message)
                            sb.Append(": ").Append(m);
                    }
                    else if (!String.IsNullOrEmpty(m) && _hasLineInfo || m != _message)
                        sb.Append(": ").Append(m);
                }
                _toString = sb.ToString();
                return _toString;
            }

            public int CompareTo(ValidationErrorInfo other)
            {
                if (other == null)
                    return -1;
                if (ReferenceEquals(this, other))
                    return 0;
                int result = StringComparer.InvariantCultureIgnoreCase.Compare(_source, other._source);
                if (result == 0)
                    result = StringComparer.CurrentCulture.Compare(_source, other._source);
                if (result != 0)
                    return result;
                if (_hasLineInfo)
                {
                    if (!other._hasLineInfo)
                        return 1;
                    if ((result = _lineNumber.CompareTo(other._lineNumber)) != 0 || (result = _linePosition.CompareTo(other._linePosition)) != 0)
                        return result;
                }
                else if (other._hasLineInfo)
                    return -1;
                if (_isWarning)
                {
                    if (!other._isWarning)
                        return 1;
                }
                else if (!other.IsWarning)
                    return -1;
                string x = ToString();
                string y = other.ToString();
                result = StringComparer.InvariantCultureIgnoreCase.Compare(x, y);
                if (result == 0)
                    return StringComparer.CurrentCulture.Compare(x, y);
                return result;
            }

            public int CompareTo(object obj)
            {
                if (obj == null || obj is ValidationErrorInfo)
                    return CompareTo(obj as ValidationErrorInfo);
                if (obj is string)
                {
                    string x = ToString();
                    string y = (string)obj;
                    int result = StringComparer.InvariantCultureIgnoreCase.Compare(x, y);
                    if (result == 0)
                        return StringComparer.CurrentCulture.Compare(x, y);
                    return result;
                }
                return -1;
            }
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}

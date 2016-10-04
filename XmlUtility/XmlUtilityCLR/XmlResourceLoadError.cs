using System;
using System.Collections.Generic;
#if !PSLEGACY
using System.Linq;
#endif
using System.Text;
#if !PSLEGACY
using System.Threading.Tasks;
#endif
using System.Xml;
using System.Xml.Schema;

namespace XmlUtilityCLR
{
    public class XmlResourceLoadError : ResourceLoadError
    {
#if PSLEGACY
        private XmlSeverityType _severity = default(XmlSeverityType);
        private int _lineNumber = 0;
        private int _linePosition = 0;
        private string _sourceUri = null;

#endif
        public override bool IsWarning { get { return Severity == XmlSeverityType.Warning; } }

#if PSLEGACY
        public XmlSeverityType Severity { get { return _severity; } private set { _severity = value; } }
#else
        public XmlSeverityType Severity { get; private set; }
#endif

#if PSLEGACY
        public int LineNumber { get { return _lineNumber; } private set { _lineNumber = value; } }
#else
        public int LineNumber { get; private set; }
#endif

#if PSLEGACY
        public int LinePosition { get { return _linePosition; } private set { _linePosition = value; } }
#else
        public int LinePosition { get; private set; }
#endif

#if PSLEGACY
        public string SourceUri { get { return _sourceUri; } private set { _sourceUri = value; } }
#else
        public string SourceUri { get; private set; }
#endif

        public static XmlResourceLoadError Create(ValidationEventArgs args)
        {
            if (args == null)
                return null;

            return new XmlResourceLoadError(args.Message, args.Exception, args.Severity);
        }

        public XmlResourceLoadError(string message, XmlSchemaException exception, XmlSeverityType severity)
            : base((message == null && exception == null) ? severity.ToString("F") : message, exception)
        {
            Severity = severity;
            if (exception == null)
                return;

            LineNumber = exception.LineNumber;
            LinePosition = exception.LinePosition;
            SourceUri = exception.SourceUri ?? "";
        }

        public XmlResourceLoadError(XmlException exception, XmlSeverityType severity)
            : base((exception == null) ? severity.ToString("F") : null, exception)
        {
            Severity = XmlSeverityType.Error;
            if (exception == null)
                return;
            LineNumber = exception.LineNumber;
            LinePosition = exception.LinePosition;
            SourceUri = exception.SourceUri ?? "";
        }

    }
}

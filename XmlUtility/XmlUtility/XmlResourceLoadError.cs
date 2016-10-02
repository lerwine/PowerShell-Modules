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
        public override bool IsWarning { get { return Severity == XmlSeverityType.Warning; } }
        public XmlSeverityType Severity { get; private set; }
        public int LineNumber { get; private set; }
        public int LinePosition { get; private set; }
        public string SourceUri { get; private set; }

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

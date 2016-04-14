using System;
using System.Xml.Schema;

namespace IOUtilityCLR
{
    public class SchemaValidationError
    {
        private object _sender;
        private Exception _exception;
        private string _message;
        private XmlSeverityType _severity;

        public override string ToString() { return String.Format("{0}: {1}", this._severity.ToString("F"), this.Message); }

        public override int GetHashCode() { return this.ToString().GetHashCode(); }

        public object Sender { get { return this._sender; } protected set { this._sender = value; } }

        public Exception Exception { get { return this._exception; } protected set { this._exception = value; } }

        public string Message { get { return (this._message == "" && this.Exception != null && !String.IsNullOrEmpty(this._exception.Message)) ? this._exception.Message : this._message; } protected set { this._message = value ?? ""; } }

        public XmlSeverityType Severity { get { return this._severity; } protected set { this._severity = value; } }

        public SchemaValidationError(object sender, ValidationEventArgs e)
        {
            this.Sender = sender;
            this.Exception = e.Exception;
            this.Message = e.Message;
            this.Severity = e.Severity;
        }

        public SchemaValidationError(object sender, string message, XmlSeverityType severity, Exception exception)
        {
            this.Sender = sender;
            this.Exception = exception;
            this.Message = message;
            this.Severity = severity;
        }

        public SchemaValidationError(object sender, string message, XmlSeverityType severity)
            : this(sender, message, severity, null)
        { }

        public SchemaValidationError(object sender, string message, Exception exception)
            : this(sender, message, XmlSeverityType.Error, exception)
        { }

        public SchemaValidationError(object sender, string message)
            : this(sender, message, null as Exception)
        { }

        public SchemaValidationError(string message)
            : this(null, message)
        { }
    }
}
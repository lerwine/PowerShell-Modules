using System;
using System.Xml.Schema;

namespace XmlUtilityCLR
{
    /// <summary>
    /// 
    /// </summary>
    public class SchemaValidationError
    {
        private object _sender;
        private Exception _exception;
        private string _message;
        private XmlSeverityType _severity;

        /// <summary>
        /// 
        /// </summary>
        public override string ToString() { return String.Format("{0}: {1}", this._severity.ToString("F"), this.Message); }

        /// <summary>
        /// 
        /// </summary>
        public override int GetHashCode() { return this.ToString().GetHashCode(); }

        /// <summary>
        /// 
        /// </summary>
        public object Sender { get { return this._sender; } protected set { this._sender = value; } }

        /// <summary>
        /// 
        /// </summary>
        public Exception Exception { get { return this._exception; } protected set { this._exception = value; } }

        /// <summary>
        /// 
        /// </summary>
        public string Message { get { return (this._message == "" && this.Exception != null && !String.IsNullOrEmpty(this._exception.Message)) ? this._exception.Message : this._message; } protected set { this._message = value ?? ""; } }

        /// <summary>
        /// 
        /// </summary>
        public XmlSeverityType Severity { get { return this._severity; } protected set { this._severity = value; } }

        /// <summary>
        /// 
        /// </summary>
        public SchemaValidationError(object sender, ValidationEventArgs e)
        {
            this.Sender = sender;
            this.Exception = e.Exception;
            this.Message = e.Message;
            this.Severity = e.Severity;
        }

        /// <summary>
        /// 
        /// </summary>
        public SchemaValidationError(object sender, string message, XmlSeverityType severity, Exception exception)
        {
            this.Sender = sender;
            this.Exception = exception;
            this.Message = message;
            this.Severity = severity;
        }

        /// <summary>
        /// 
        /// </summary>
        public SchemaValidationError(object sender, string message, XmlSeverityType severity)
            : this(sender, message, severity, null)
        { }

        /// <summary>
        /// 
        /// </summary>
        public SchemaValidationError(object sender, string message, Exception exception)
            : this(sender, message, XmlSeverityType.Error, exception)
        { }

        /// <summary>
        /// 
        /// </summary>
        public SchemaValidationError(object sender, string message)
            : this(sender, message, null as Exception)
        { }

        /// <summary>
        /// 
        /// </summary>
        public SchemaValidationError(string message)
            : this(null, message)
        { }
    }
}
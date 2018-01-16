using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Xml;
using System.Xml.Schema;

namespace XmlUtility
{
    /// <summary>
    /// 
    /// </summary>
    public class ResourceLoadError
    {
#if PSLEGACY
        private bool _isWarning = false;
        private string _message = null;
        private string _details = null;
#endif

#if PSLEGACY
        public virtual bool IsWarning { get { return _isWarning; } private set { _isWarning = value; } }
#else
        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsWarning { get; private set; }
#endif

#if PSLEGACY
        public string Message { get { return _message; } private set { _message = value; } }
#else
        /// <summary>
        /// 
        /// </summary>
        public string Message { get; private set; }
#endif

#if PSLEGACY
        public string Details { get { return _details; } private set { _details = value; } }
#else
        /// <summary>
        /// 
        /// </summary>
        public string Details { get; private set; }
#endif
        /// <summary>
        /// 
        /// </summary>

        public static IEnumerable<string> GetMessages(string message, Exception exception)
        {
#if PSLEGACY
            if (message != null && message.Trim().Length > 0)
#else
            if (!String.IsNullOrWhiteSpace(message))
#endif
                yield return message;

            Exception e = exception;
            while (e != null)
            {
#if PSLEGACY
                if (e.Message != null && e.Message.Trim().Length > 0)
#else
                if (!String.IsNullOrWhiteSpace(e.Message))
#endif
                    yield return e.Message;
#if !PSLEGACY
                if (e is AggregateException)
                {
                    AggregateException a = e as AggregateException;
                    foreach (Exception i in a.InnerExceptions)
                    {
                        if (a.InnerException != null && !ReferenceEquals(a.InnerException, i))
                        {
                            foreach (string m in GetMessages(null, i))
                                yield return m;
                        }
                    }
                }
#endif
                e = e.InnerException;
            }
        }

#if PSLEGACY
        public ResourceLoadError(string message) : this(message, false) { }

        public ResourceLoadError(string message, bool isWarning)
#else
        /// <summary>
        /// 
        /// </summary>
        public ResourceLoadError(string message, bool isWarning = false)
#endif
        {
            if (message == null)
                throw new ArgumentNullException("message");

            if (message.Trim() == "")
                throw new ArgumentException("Message cannot be empty.", "message");
            Message = message;
            IsWarning = isWarning;
        }

        /// <summary>
        /// 
        /// </summary>
        public static ResourceLoadError Create(ValidationEventArgs args)
        {
            // TODO: Implement Create method
#warning Not implemented
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public ResourceLoadError(Exception exception) : this((exception == null) ? null : exception.Message, exception) { }

        /// <summary>
        /// 
        /// </summary>
        public ResourceLoadError(string message, Exception exception)
        {
#if PSLEGACY
            if ((message == null || message.Trim().Length == 0) && exception == null)
#else
            if (String.IsNullOrWhiteSpace(message) && exception == null)
#endif
                throw new ArgumentNullException("exception");

            IsWarning = exception != null && exception is WarningException;
            List<string> list = new List<string>(GetMessages(message, exception));
            if (list.Count == 0)
                Message = exception.GetType().Name;
            else
            {
                Message = list[0];
                list.RemoveAt(0);
            }
            for (int i = 0; i < list.Count; i++)
                list[i] = "Inner Error: " + list[i].TrimStart();
            Details = String.Join("\r\n", list.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        public static ResourceLoadError Create(Exception exception)
        {
            if (exception == null)
                return null;

            if (exception is XmlSchemaException)
                return new XmlResourceLoadError(null as string, exception as XmlSchemaException, XmlSeverityType.Error);

            if (exception is XmlException)
                return new XmlResourceLoadError(exception as XmlException, XmlSeverityType.Error);

            if (exception is WebException)
                return new WebResourceLoadError(exception as WebException);
            
            return new ResourceLoadError(exception);
        }
    }
}
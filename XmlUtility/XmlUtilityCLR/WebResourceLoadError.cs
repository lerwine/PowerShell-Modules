using System.Net;

namespace XmlUtilityCLR
{
    /// <summary>
    /// 
    /// </summary>
    public class WebResourceLoadError : ResourceLoadError
    {
        /// <summary>
        /// 
        /// </summary>
        public WebResourceLoadError(WebException exception)
            : base(exception)
        {
            if (exception != null)
                Status = exception.Status;
        }

#if PSLEGACY
        private WebExceptionStatus _status = default(WebExceptionStatus);

#endif
#if PSLEGACY
        public WebExceptionStatus Status { get { return _status; } private set { _status = value; } }
#else
        /// <summary>
        /// 
        /// </summary>
        public WebExceptionStatus Status { get; private set; }
#endif
    }
}
using System.Net;

namespace XmlUtilityCLR
{
    public class WebResourceLoadError : ResourceLoadError
    {
        public WebResourceLoadError(WebException exception)
            : base(exception)
        {
            if (exception != null)
                Status = exception.Status;
        }

        public WebExceptionStatus Status { get; private set; }
    }
}
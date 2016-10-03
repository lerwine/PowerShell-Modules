using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;

namespace XmlUtilityCLR
{
    public abstract class LoadableResource
    {
#if PSLEGACY
        private Uri _sourceUri = null;
        private string _sourcePath = null;
        private bool _isLocal = false;
        private string _fileName = null;
        private ReadOnlyCollection<ResourceLoadError> _errors = null;
        private Collection<ResourceLoadError> _innerErrors = null;
        private bool _success = false;
        private Encoding _encoding = null;
        private string _mediaType = null;

#endif

#if PSLEGACY
        public Uri SourceUri { get { return _sourceUri; } private set { _sourceUri = value; } }
#else
        public Uri SourceUri { get; private set; }
#endif

#if PSLEGACY
        public string SourcePath { get { return _sourcePath; } private set { _sourcePath = value; } }
#else
        public string SourcePath { get; private set; }
#endif

#if PSLEGACY
        public bool IsLocal { get { return _isLocal; } private set { _isLocal = value; } }
#else
        public bool IsLocal { get; private set; }
#endif

        public abstract object RawValue { get; }

#if PSLEGACY
        public string FileName { get { return _fileName; } protected set { _fileName = value; } }
#else
        public string FileName { get; protected set; }
#endif

#if PSLEGACY
        public ReadOnlyCollection<ResourceLoadError> Errors { get { return _errors; } private set { _errors = value; } }
#else
        public ReadOnlyCollection<ResourceLoadError> Errors { get; private set; }
#endif

#if PSLEGACY
        protected Collection<ResourceLoadError> InnerErrors { get { return _innerErrors; } private set { _innerErrors = value; } }
#else
        protected Collection<ResourceLoadError> InnerErrors { get; private set; }
#endif

#if PSLEGACY
        public bool Success { get { return _success; } private set { _success = value; } }
#else
        public bool Success { get; private set; }
#endif

#if PSLEGACY
        public Encoding Encoding { get { return _encoding; } protected set { _encoding = value; } }
#else
        public Encoding Encoding { get; protected set; }
#endif

#if PSLEGACY
        public string MediaType { get { return _mediaType; } protected set { _mediaType = value; } }
#else
        public string MediaType { get; protected set; }
#endif

        protected abstract bool LoadFromWeb();
        protected abstract void LoadFromLocal();
        public static string ParseFileName(Uri uri, out string sourcePath)
        {
            sourcePath = Path.GetFullPath(uri.LocalPath);
            string fileName = Path.GetFileName(sourcePath);
            while (fileName == "")
            {
                sourcePath = Path.GetDirectoryName(sourcePath);
                if (String.IsNullOrEmpty(sourcePath))
                    break;
                fileName = Path.GetFileName(sourcePath);
            }

            if (String.IsNullOrEmpty(sourcePath))
                sourcePath = "";

            return fileName ?? "";
        }

        protected LoadableResource(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");
            if (!uri.IsAbsoluteUri)
                throw new ArgumentException("Relative Uris not supported");

            string sourcePath;
            if ((FileName = ParseFileName(uri, out sourcePath)) == "")
                throw new ArgumentException("Could not parse file name from source URI.", "uri");

            Success = true;
            InnerErrors = new Collection<ResourceLoadError>();
            Errors = new ReadOnlyCollection<ResourceLoadError>(InnerErrors);
            SourceUri = uri;
            SourcePath = sourcePath;
            try
            {
                if (String.Compare(uri.Scheme, Uri.UriSchemeFile, true) == 0)
                {
                    LoadFromLocal();
                    OnSuccess();
                    return;
                }
                if (String.Compare(uri.Scheme, Uri.UriSchemeFtp, true) == 0 || String.Compare(uri.Scheme, Uri.UriSchemeHttp, true) == 0 || String.Compare(uri.Scheme, Uri.UriSchemeHttps, true) == 0)
                {
                    Success = LoadFromWeb();
                    OnSuccess();
                    return;
                }
            }
            catch (Exception exception)
            {
                OnLoadError(exception);
                OnLoadComplete();
                return;
            }

            throw new NotSupportedException(String.Format("The {0} URL scheme is not supported.", uri.Scheme));
        }

        protected virtual void OnLoadError(Exception exception)
        {
            if (exception != null)
                InnerErrors.Add(ResourceLoadError.Create(exception));
        }
        
        protected virtual void OnSuccess()
        {
            OnLoadComplete();
        }

        protected virtual void OnLoadComplete()
        {
            if (String.IsNullOrEmpty(MediaType))
                MediaType = MediaTypeNames.Application.Octet;
        }
    }
    public abstract class LoadableResource<TSource, TTarget> : LoadableResource
    {
#if PSLEGACY
        private TSource _source = default(TSource);
        private TTarget _target = default(TTarget);
#endif

#if PSLEGACY
        public TSource Source { get { return _source; } protected set { _source = value; } }
#else
        public TSource Source { get; private set; }
#endif

#if PSLEGACY
        public TTarget Target { get { return _target; } protected set { _target = value; } }
#else
        public TTarget Target { get; private set; }
#endif

        protected LoadableResource(Uri uri) : base(uri) { }

        public override object RawValue { get { return Source; } }
        
        protected override void LoadFromLocal()
        {
            try
            {
                Source = LoadSourceFromLocal();
            }
            catch
            {
                Source = GetDefaultSourceValue();
                throw;
            }
        }

        protected abstract TSource LoadSourceFromLocal();

        protected abstract TSource GetDefaultSourceValue();

        protected override bool LoadFromWeb()
        {
            try
            {
                WebException exception;
                WebResponse webResponse;
                bool isValid;
                try
                {
                    WebRequest webRequest = WebRequest.Create(SourceUri);
                    OnInitializeWebRequest(webRequest);
                    webResponse = webRequest.GetResponse();
                    isValid = OnValidateWebResponse(webResponse);
                    exception = null;
                }
                catch (WebException e)
                {
                    isValid = false;
                    exception = e;
                    webResponse = e.Response;
                }

                Stream stream;
                if (webResponse == null)
                    stream = null;
                else
                {
                    try { stream = (webResponse == null) ? null : webResponse.GetResponseStream(); } catch { stream = null; }
#if PSLEGACY
                    if (webResponse.ContentType != null && webResponse.ContentType.Trim().Length > 0)
#else
                    if (!String.IsNullOrWhiteSpace(webResponse.ContentType))
#endif
                    {
                        ContentType ct = new ContentType(webResponse.ContentType);
                        MediaType = ct.MediaType;
                        if (!String.IsNullOrEmpty(ct.CharSet))
                        {
                            try { Encoding = Encoding.GetEncoding(ct.CharSet); }
                            catch { }
                        }
                        if (!String.IsNullOrEmpty(ct.Name))
                            FileName = ct.Name;
                    }
                }
                
                if (stream == null)
                {
                    isValid = false;
                    Source = GetDefaultSourceValue();
                }
                else
                    Source = Load(stream, isValid);
                return isValid;
            }
            catch
            {
                Source = GetDefaultSourceValue();
                throw;
            }
        }

        protected virtual void OnInitializeWebRequest(WebRequest webRequest)
        {
            webRequest.Credentials = CredentialCache.DefaultNetworkCredentials;
            webRequest.UseDefaultCredentials = true;
            if (webRequest is HttpWebRequest)
                OnInitializeHttpWebRequest(webRequest as HttpWebRequest);
            else
                OnInitializeFtpWebRequest(webRequest as FtpWebRequest);
        }

        protected virtual void OnInitializeHttpWebRequest(HttpWebRequest webRequest)
        {
            webRequest.AllowAutoRedirect = true;
        }

        protected virtual void OnInitializeFtpWebRequest(FtpWebRequest webRequest) { }

        protected virtual bool OnValidateWebResponse(WebResponse webResponse)
        {
            if (webResponse is HttpWebResponse)
                return OnValidateHttpWebResponse(webResponse as HttpWebResponse);

            return OnValidateFtpWebResponse(webResponse as FtpWebResponse);
        }

        protected virtual bool OnValidateHttpWebResponse(HttpWebResponse webResponse)
        {
            if (webResponse == null)
                return false;

            switch (webResponse.StatusCode)
            {
                case HttpStatusCode.Ambiguous:
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.Conflict:
                case HttpStatusCode.ExpectationFailed:
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.GatewayTimeout:
                case HttpStatusCode.Gone:
                case HttpStatusCode.HttpVersionNotSupported:
                case HttpStatusCode.InternalServerError:
                case HttpStatusCode.LengthRequired:
                case HttpStatusCode.MethodNotAllowed:
                case HttpStatusCode.NotAcceptable:
                case HttpStatusCode.NotFound:
                case HttpStatusCode.NotImplemented:
                case HttpStatusCode.PaymentRequired:
                case HttpStatusCode.PreconditionFailed:
                case HttpStatusCode.ProxyAuthenticationRequired:
                case HttpStatusCode.RequestedRangeNotSatisfiable:
                case HttpStatusCode.RequestEntityTooLarge:
                case HttpStatusCode.RequestTimeout:
                case HttpStatusCode.RequestUriTooLong:
                case HttpStatusCode.ServiceUnavailable:
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.UnsupportedMediaType:
                    return false;
            }

            return true;
        }

        protected virtual bool OnValidateFtpWebResponse(FtpWebResponse webResponse)
        {
            if (webResponse == null)
                return false;

            switch (webResponse.StatusCode)
            {
                case FtpStatusCode.AccountNeeded:
                case FtpStatusCode.ActionAbortedLocalProcessingError:
                case FtpStatusCode.ActionAbortedUnknownPageType:
                case FtpStatusCode.ActionNotTakenFilenameNotAllowed:
                case FtpStatusCode.ActionNotTakenFileUnavailable:
                case FtpStatusCode.ActionNotTakenFileUnavailableOrBusy:
                case FtpStatusCode.ActionNotTakenInsufficientSpace:
                case FtpStatusCode.ArgumentSyntaxError:
                case FtpStatusCode.BadCommandSequence:
                case FtpStatusCode.CantOpenData:
                case FtpStatusCode.CommandExtraneous:
                case FtpStatusCode.CommandNotImplemented:
                case FtpStatusCode.CommandSyntaxError:
                case FtpStatusCode.FileActionAborted:
                case FtpStatusCode.NeedLoginAccount:
                case FtpStatusCode.NotLoggedIn:
                case FtpStatusCode.ServerWantsSecureSession:
                case FtpStatusCode.ServiceNotAvailable:
                case FtpStatusCode.ServiceTemporarilyNotAvailable:
                    return false;
            }

            return true;
        }

        protected abstract TSource Load(Stream stream, bool isValid);

        protected override void OnLoadComplete()
        {
            base.OnLoadComplete();
            Target = CreateTarget();
        }

        protected abstract TTarget CreateTarget();
    }
}

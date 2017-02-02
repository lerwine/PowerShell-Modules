using System;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;

namespace NetworkUtilityCLR
{
    /// <summary>
    /// 
    /// </summary>
    public static class MimeUtility
    {
        /// <summary>
        /// 
        /// </summary>
        public const string MediaType_FormUrlEncoded = "application/x-www-form-urlencoded";

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex UrlEncodedItem = new Regex(@"(^|&)(?<key>[^&=]*)(=(?<value>[^&]*))?", RegexOptions.Compiled);

        /// <summary>
        /// 
        /// </summary>
        public static bool IsUrlFormEncoded(string contentType)
        {
            if (String.IsNullOrEmpty(contentType))
                return false;
                
            ContentType ct = new ContentType(contentType);
            return ct.MediaType == MimeUtility.MediaType_FormUrlEncoded;
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool IsUrlFormEncoded(WebResponse webResponse)
        {
            if (webResponse == null)
                throw new ArgumentNullException("webResponse");
                
            return IsUrlFormEncoded(webResponse.ContentType);
        }

        /// <summary>
        /// 
        /// </summary>
        public static Encoding GetEncoding(ContentType contentType, Encoding defaultEncoding)
        {
            if (contentType == null || String.IsNullOrEmpty(contentType.CharSet))
                return defaultEncoding;
            
            try
            {
                Encoding encoding = Encoding.GetEncoding(contentType.CharSet);
                return encoding ?? defaultEncoding;
            }
            catch
            {
                return defaultEncoding;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static Encoding GetEncoding(string contentType, Encoding defaultEncoding)
        {
            if (String.IsNullOrEmpty(contentType))
                return defaultEncoding;
            
            try
            {
                return GetEncoding(new ContentType(contentType), defaultEncoding);
            }
            catch
            {
                return defaultEncoding;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static Encoding GetEncoding(WebResponse webResponse, Encoding defaultEncoding)
        {
            if (webResponse == null || webResponse.ContentType == null)
                return defaultEncoding;
            return GetEncoding(webResponse.ContentType, defaultEncoding);
        }
    }
}

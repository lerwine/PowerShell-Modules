using System;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;

namespace NetworkUtilityCLR
{
    public static class MimeUtility
    {
        public const string MediaType_FormUrlEncoded = "application/x-www-form-urlencoded";
        public static readonly Regex UrlEncodedItem = new Regex(@"(^|&)(?<key>[^&=]*)(=(?<value>[^&]*))?", RegexOptions.Compiled);
        
        public static bool IsUrlFormEncoded(string contentType)
        {
            if (String.IsNullOrEmpty(contentType))
                return false;
                
            ContentType ct = new ContentType(contentType);
            return ct.MediaType == MimeUtility.MediaType_FormUrlEncoded;
        }
        
        public static bool IsUrlFormEncoded(WebResponse webResponse)
        {
            if (webResponse == null)
                throw new ArgumentNullException("webResponse");
                
            return IsUrlFormEncoded(webResponse.ContentType);
        }
        
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
        
        public static Encoding GetEncoding(WebResponse webResponse, Encoding defaultEncoding)
        {
            if (webResponse == null || webResponse.ContentType == null)
                return defaultEncoding;
            return GetEncoding(webResponse.ContentType, defaultEncoding);
        }
    }
}

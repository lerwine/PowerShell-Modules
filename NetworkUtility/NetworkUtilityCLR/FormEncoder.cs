using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;

namespace NetworkUtilityCLR
{
    /// <summary>
    /// 
    /// </summary>
    public static class FormEncoder
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex UrlEncodedItem = new Regex(@"(^|&)(?<key>[^&=]*)(=(?<value>[^&]*))?", RegexOptions.Compiled);

        /// <summary>
        /// 
        /// </summary>
        public static NameValueCollection Decode(string formUrlEncodedText)
        {
            NameValueCollection nameValueCollection = new NameValueCollection();
            if (String.IsNullOrEmpty(formUrlEncodedText))
                return nameValueCollection;
                
            foreach (Match match in UrlEncodedItem.Matches(formUrlEncodedText))
            {
                string key = Uri.UnescapeDataString(match.Groups["key"].Value);
                if (match.Groups["value"].Success)
                    nameValueCollection.Add(key, match.Groups["value"].Value);
                else
                    nameValueCollection.Add(key, "");
            }
            
            return nameValueCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        public static NameValueCollection Decode(TextReader textReader)
        {
            if (textReader == null)
                throw new ArgumentNullException("textReader");
            
            return Decode(textReader.ReadToEnd());
        }

        /// <summary>
        /// 
        /// </summary>
        public static NameValueCollection Decode(Stream stream, Encoding encoding)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
                
            using (StreamReader streamReader = (encoding == null) ? new StreamReader(stream, true) : new StreamReader(stream, encoding))
                return Decode(streamReader);
        }

        /// <summary>
        /// 
        /// </summary>
        public static NameValueCollection Decode(WebResponse webResponse)
        {
            if (webResponse.ContentLength < 1)
                return new NameValueCollection();

            if (!String.IsNullOrEmpty(webResponse.ContentType) && !IsUrlFormEncoded(webResponse))
                throw new InvalidOperationException("Web response was not URL Form Encoded.");
            
            using (Stream stream = webResponse.GetResponseStream())
                return Decode(stream, MimeUtility.GetEncoding(webResponse, Encoding.UTF8));
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool IsUrlFormEncoded(WebResponse webResponse)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Encode(string key, string value, TextWriter textWriter, bool isNotFirst)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            
            if (textWriter == null)
                throw new ArgumentNullException("textWriter");
            
            if (isNotFirst)
                textWriter.Write("&");
            textWriter.Write(Uri.EscapeDataString(key));
            if (value != null)
                textWriter.Write("={0}", Uri.EscapeDataString(key));
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Encode(NameValueCollection collection, TextWriter textWriter, bool emptyAsNull)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            
            if (textWriter == null)
                throw new ArgumentNullException("textWriter");
            
            string[] allKeys = collection.AllKeys;
            if (allKeys.Length == 0)
                return;
            
            for (int i = 0; i < allKeys.Length; i++)
            {
                string[] values = collection.GetValues(allKeys[i]);
                for (int v = 0; v < values.Length; v++)
                    Encode(allKeys[i], ((values[v] == "" && emptyAsNull) ? null : values[v]), textWriter, (i > 0 && v > 0));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string Encode(NameValueCollection collection, bool emptyAsNull)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            
            using (StringWriter stringWriter = new StringWriter())
            {
                Encode(collection, stringWriter, emptyAsNull);
                return stringWriter.ToString();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Encode(NameValueCollection collection, Stream stream, bool emptyAsNull, Encoding encoding)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            
            if (stream == null)
                throw new ArgumentNullException("stream");
            
            if (encoding == null)
                encoding = Encoding.UTF8;
                
            using (StreamWriter streamWriter = new StreamWriter(stream, encoding))
            {
                Encode(collection, streamWriter, emptyAsNull);
                streamWriter.Flush();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Encode(NameValueCollection collection, WebRequest webRequest, bool emptyAsNull, Encoding encoding)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            
            if (webRequest == null)
                throw new ArgumentNullException("webRequest");
            
            if (encoding == null)
                encoding = Encoding.UTF8;
            
            ContentType contentType = new ContentType(MimeUtility.MediaType_FormUrlEncoded);
            contentType.CharSet = contentType.Name;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Encode(collection, memoryStream, emptyAsNull, encoding);
                
                webRequest.ContentLength = memoryStream.Position;
                if (webRequest is HttpWebRequest)
                    (webRequest as HttpWebRequest).MediaType = MimeUtility.MediaType_FormUrlEncoded;
                webRequest.ContentType = contentType.ToString();
                
                if (memoryStream.Position == 0)
                    return;
                
                memoryStream.Seek(0L, SeekOrigin.Begin);
                
                using (Stream requestStream = webRequest.GetRequestStream())
                {
                    byte[] buffer = new byte[8192];
                    int count = memoryStream.Read(buffer, 0, buffer.Length);
                    while (count > 0)
                    {
                        requestStream.Write(buffer, 0, count);
                        count = memoryStream.Read(buffer, 0, buffer.Length);
                    }
                }
            }
        }
    }
}

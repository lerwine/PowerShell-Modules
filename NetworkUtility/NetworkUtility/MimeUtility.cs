using System;
using System.Net;
using System.Net.Mime;
using System.Runtime.InteropServices;
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
		
        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static System.UInt32 FindMimeFromData(
            System.UInt32 pBC,
            [MarshalAs(UnmanagedType.LPStr)] System.String pwzUrl,
            [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
            System.UInt32 cbSize,
            [MarshalAs(UnmanagedType.LPStr)] System.String pwzMimeProposed,
            System.UInt32 dwMimeFlags,
            out System.UInt32 ppwzMimeOut,
            System.UInt32 dwReserverd
        );

        public static ContentType ContentTypeFromName(string fileName)
        {
            try
            {
                Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(System.IO.Path.GetExtension(fileName).ToLower());
                if (regKey != null && regKey.GetValue("Content Type") != null)
                {
                    string contentType = regKey.GetValue("Content Type").ToString();
                    if (contentType != null && (contentType = contentType.Trim()).Length > 0)
                        return new ContentType(contentType);
                }
            }
            catch { }

            return new ContentType(MediaTypeNames.Application.Octet);
        }

        public static ContentType ContentTypeFromData(byte[] buffer)
        {
            try
            {
                System.UInt32 mimetype;
                FindMimeFromData(0, null, buffer, 256, null, 0, out mimetype, 0);
                System.IntPtr mimeTypePtr = new IntPtr(mimetype);
                string contentType = Marshal.PtrToStringUni(mimeTypePtr);
                Marshal.FreeCoTaskMem(mimeTypePtr);
                if (contentType != null && (contentType = contentType.Trim()).Length > 0)
                    return new ContentType(contentType);
            }
            catch { }

            return new ContentType(MediaTypeNames.Application.Octet);
        }
    }
}

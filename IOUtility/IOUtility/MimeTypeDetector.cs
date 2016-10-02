using Microsoft.Win32;
using System;
using System.Collections.Generic;
#if !PSLEGACY
using System.Linq;
#endif
using System.Net.Mime;
using System.Text;
#if !PSLEGACY
using System.Threading.Tasks;
#endif

namespace IOUtilityCLR
{
    public static class MimeTypeDetector
    {
        private static Dictionary<string, ContentType> _extensionMimeMappings = new Dictionary<string, ContentType>();
        private static Dictionary<string, string> _mimeExtensionMappings = new Dictionary<string, string>();

        public static void InitializeFromRegistry()
        {
            RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey("MIME");
            if (registryKey == null || (registryKey = registryKey.OpenSubKey("Database")) == null || (registryKey = registryKey.OpenSubKey("Content Type")) == null)
                return;
#if PSLEGACY2
            foreach (Tuple<RegistryKey, string> mapping in LinqEmul.Select<string, RegistryKey, Tuple<RegistryKey, string>>(registryKey.GetSubKeyNames(), registryKey, InitializeFromRegistry_Selector))
            {
                if (mapping.Item2 != null && mapping.Item2.Trim().Length > 0)
                    AddMimeExtensionMapping(mapping.Item2, mapping.Item1.Name);
            }
#else
            foreach (var mapping in registryKey.GetSubKeyNames().Select(n =>
            {
                RegistryKey r = registryKey.OpenSubKey(n);
                return new { Key = r, Ext = r.GetValue("Extension") as string };
            }).Where(a => !String.IsNullOrEmpty(a.Ext)))
                AddMimeExtensionMapping(mapping.Ext, mapping.Key.Name);
#endif
        }

        private static Tuple<RegistryKey, string> InitializeFromRegistry_Selector(string n, RegistryKey registryKey)
        {
            RegistryKey r = registryKey.OpenSubKey(n);
            return new Tuple<RegistryKey, string>(r, r.GetValue("Extension") as string);
        }

        public static bool IsExtensionMapped(string extension)
        {
            if (String.IsNullOrEmpty(extension))
                return false;

            if (!extension.StartsWith("."))
                return IsExtensionMapped("." + extension);
            
            lock (_extensionMimeMappings)
                return _extensionMimeMappings.ContainsKey(extension.ToLower());
        }

        public static bool IsMediaTypeMapped(string mediaType)
        {
            if (String.IsNullOrEmpty(mediaType))
                return false;

            return IsContentTypeMapped(new ContentType(mediaType));
        }

        public static bool IsContentTypeMapped(ContentType contentType)
        {
            if (contentType == null || String.IsNullOrEmpty(contentType.MediaType))
                return false;
            
            lock (_mimeExtensionMappings)
                return _extensionMimeMappings.ContainsKey(contentType.MediaType.ToLower());
        }

        public static void AddMimeExtensionMapping(string extension, string mimeType)
        {
            if (extension == null)
                throw new ArgumentNullException("extension");

            if (mimeType == null)
                throw new ArgumentNullException("mimeType");

            if (extension == "")
                throw new ArgumentException("Value cannot be empty.", "extension");

            if (mimeType == "")
                throw new ArgumentException("Value cannot be empty.", "mimeType");

            AddMimeExtensionMapping(extension, new ContentType(mimeType));
        }
        
        public static void AddMimeExtensionMapping(string extension, string mediaType, Encoding encoding)
        {
            if (extension == null)
                throw new ArgumentNullException("extension");

            if (mediaType == null)
                throw new ArgumentNullException("mediaType");

            if (extension == "")
                throw new ArgumentException("Value cannot be empty.", "extension");

            if (mediaType == "")
                throw new ArgumentException("Value cannot be empty.", "mimeType");

            if (encoding == null)
                AddMimeExtensionMapping(extension, new ContentType(mediaType));
            else
                AddMimeExtensionMapping(extension, new ContentType(mediaType) { CharSet = encoding.WebName });
        }
        
        public static void AddMimeExtensionMapping(string extension, ContentType contentType)
        {
            if (extension == null)
                throw new ArgumentNullException("extension");

            if (contentType == null)
                throw new ArgumentNullException("contentType");

            if (extension == "")
                throw new ArgumentException("Value cannot be empty.", "extension");

            if (String.IsNullOrEmpty(contentType.MediaType))
                throw new ArgumentException("MediaType not defined.", "contentType");

            if (!extension.StartsWith("."))
                extension = "." + extension;

            lock (_extensionMimeMappings)
            {
                lock (_mimeExtensionMappings)
                {
                    string lc = extension.ToLower();
                    if (_extensionMimeMappings.ContainsKey(lc))
                        _extensionMimeMappings[lc] = contentType;
                    else
                        _extensionMimeMappings.Add(lc, contentType);

                    lc = contentType.MediaType.ToLower();
                    if (_mimeExtensionMappings.ContainsKey(lc))
                        _mimeExtensionMappings[lc] = extension;
                    else
                        _mimeExtensionMappings.Add(lc, extension);
                }
            }
        }

        private static ContentType _GetDefaultContentType()
        {
            if (_extensionMimeMappings.ContainsKey(""))
                return _extensionMimeMappings[""];

            ContentType contentType = new ContentType(MediaTypeNames.Application.Octet);
            _extensionMimeMappings.Add("", contentType);
            return contentType;
        }

        public static ContentType GetDefaultContentType()
        {
            lock (_extensionMimeMappings)
                return _GetDefaultContentType();
        }

        public static ContentType GetContentTypeFromExtension(string extension)
        {
            if (String.IsNullOrEmpty(extension))
                return GetDefaultContentType();

            if (!extension.StartsWith("."))
                return GetContentTypeFromExtension("." + extension);

            ContentType contentType;

            lock (_extensionMimeMappings)
            {
                if (!_extensionMimeMappings.TryGetValue(extension.ToLower(), out contentType))
                    contentType = _GetDefaultContentType();
            }

            return contentType;
        }
        
        public static string GetExtensionFromMediaType(string mediaType)
        {
            if (mediaType == null)
                throw new ArgumentNullException("mediaType");

            if (mediaType == "")
                throw new ArgumentException("Value cannot be empty.", "mimeType");

            return GetExtensionFromContentType(new ContentType(mediaType));
        }
        
        public static string GetExtensionFromContentType(ContentType contentType)
        {
            if (contentType == null)
                throw new ArgumentNullException("contentType");

            if (String.IsNullOrEmpty(contentType.MediaType))
                throw new ArgumentException("MediaType not defined.", "contentType");

            string extension;

            lock (_mimeExtensionMappings)
            {
                if (!_mimeExtensionMappings.TryGetValue(contentType.MediaType.ToLower(), out extension))
                    extension = null;
            }

            return extension;
        }
    }
}

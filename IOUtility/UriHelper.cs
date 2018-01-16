using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace IOUtility
{
    public class UriHelper
    {
        private object _original;
        private Uri _uri = null;
        private UriBuilder _builder = null;

        public UriHelper(object obj)
        {
            _original = obj;
            if (obj == null)
                obj = new UriBuilder();
            else if (!(obj is UriBuilder))
                obj = AsUri(obj);

            if (obj is Uri)
            {
                Uri uri = (Uri)obj;
                if (uri.IsAbsoluteUri)
                    obj = new UriBuilder(uri);
                else
                    obj = uri.ToString();
            }
            if (obj is UriBuilder)
                _builder = (UriBuilder)obj;
            else
            {
                _builder = new UriBuilder();
                string s = (obj is string) ? (string)obj :  _original.ToString();
                if (!String.IsNullOrEmpty(s))
                {

                }
            }
            
            OnBuilderChanged();
        }
        
        private void OnBuilderChanged()
        {

        }

        public static UriBuilder AsUriBuilder(object obj)
        {
            if (obj == null)
                return new UriBuilder();
            
            object b = (obj is PSObject) ? (obj as PSObject).BaseObject : obj;
            if (b is UriBuilder)
                return (UriBuilder)b;
            Uri uri;
            string r;
            if (b is Uri)
            {
                uri = (Uri)obj;
                if (uri.IsAbsoluteUri)
                    return new UriBuilder(uri);
                r = uri.ToString();
            }
            else
            {
                if (b is string)
                {
                    r = (string)b;
                    if (r.Length == 0)
                        return new UriBuilder();
                }
                else if (String.IsNullOrEmpty(r = b.ToString()))
                    return new UriBuilder();
                if (Uri.TryCreate(r, UriKind.RelativeOrAbsolute, out uri))
                {
                    if (uri.IsAbsoluteUri)
                        return new UriBuilder(uri);
                }
                else
                {
                    Func<string, string> replace = s => EscapeParts.Replace(s, m =>
                    {
                        if (m.Groups["e"].Success)
                        {
                            if (m.Groups["t"].Success)
                                return m.Groups["e"].Value + Uri.EscapeUriString(m.Groups["t"].Value);
                            return m.Groups["e"].Value;
                        }
                        return Uri.EscapeUriString(m.Groups["t"].Value);
                    });

                    int i = r.IndexOfAny(new char[] { '?', '#' });
                    string q = "";
                    string f = "";
                    if (i > 0)
                    {
                        if (r[i] == '#')
                            f = r.Substring(i);
                        else
                        {
                            q = r.Substring(i);
                            int n = r.IndexOf('#');
                            if (n >= 0)
                            {
                                f = q.Substring(n);
                                q = q.Substring(0, n);
                            }
                        }
                        r = r.Substring(0, i);
                    }
                    string r2 = (r.Length == 0) ? r : replace(r);
                    string q2 = (q.Length == 0) ? r : "?" + replace(r.Substring(1).Replace(' ', '+'));
                    string f2 = (f.Length == 0) ? f : "#" + EscapeParts.Replace(f.Substring(1), m =>
                    {
                        if (m.Groups["e"].Success)
                        {
                            if (m.Groups["t"].Success)
                                return m.Groups["e"].Value + Uri.EscapeDataString(m.Groups["t"].Value);
                            return m.Groups["e"].Value;
                        }
                        return Uri.EscapeUriString(m.Groups["t"].Value);
                    });
                    if (r != r2)
                    {
                        if (Uri.TryCreate(r2 + q + f, UriKind.RelativeOrAbsolute, out uri))
                        {
                            if (uri.IsAbsoluteUri)
                                return new UriBuilder(uri);
                        }
                        else if (q != q2)
                        {
                            if (Uri.TryCreate(r + q2 + f, UriKind.RelativeOrAbsolute, out uri) ||
                                Uri.TryCreate(r2 + q2 + f, UriKind.RelativeOrAbsolute, out uri))
                            {
                                if (uri.IsAbsoluteUri)
                                    return new UriBuilder(uri);
                            }
                            else
                            {
                                if (!(Uri.TryCreate(r + q + f2, UriKind.RelativeOrAbsolute, out uri) ||
                                    Uri.TryCreate(r2 + q + f2, UriKind.RelativeOrAbsolute, out uri) ||
                                    Uri.TryCreate(r + q2 + f2, UriKind.RelativeOrAbsolute, out uri)))
                                    uri = new Uri(r2 + q2 + f2, UriKind.RelativeOrAbsolute);
                            }
                        }
                    }
                    else if (q != q2)
                    {
                        if (!(Uri.TryCreate(r + q2 + f, UriKind.RelativeOrAbsolute, out uri) ||
                            Uri.TryCreate(r + q + f2, UriKind.RelativeOrAbsolute, out uri)))
                                uri = new Uri(r + q2 + f2, UriKind.RelativeOrAbsolute);
                    }
                    else if (f != f2 && Uri.TryCreate(r + q + f2, UriKind.RelativeOrAbsolute, out uri))
                        uri = new Uri(r + q2 + f2, UriKind.RelativeOrAbsolute);
                }
            }
            
            if (uri.IsAbsoluteUri)
                return new UriBuilder(uri);
            UriBuilder builder = new UriBuilder();
            r = uri.ToString();
            int index = r.IndexOfAny(new char[] { '?', '#' });
        }
        
        private static readonly Regex Parts = new Regex(@"^([^:\\/\?\#@]*(([^:\\/\?\#@]*(:[^\\/\?\#@]*)@)?[^\\/\?\#]*)?([\?\#]+)?(\?[\#]+)?(#.*)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        /*
        Scheme: [^:\\/\?\#@]*[/\\]{,2}
        Login: ([^:\\/\?\#@]*(:[^\\/\?\#@]*)?@
        Host [^:\\/\?\#]*(:\d+)?
        Path: [^\?\#]*
        Query: \?[^\#]*
        Fragment: \#.*
         */

        private static readonly Regex EscapeParts = new Regex(@"(?(?=%)(?(?=[a-f\d]{2})(?<e>%[a-f\d]{2})(?<t>[^%]+)?|(?<t>%[^%]*))|(?<t>[^%]+))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        public static UriBuilder AsString(object obj)
        {
#warning Not implemented
            throw new NotImplementedException();
        }

    }
}
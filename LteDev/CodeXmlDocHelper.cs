using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LteDev
{
    /// <summary>
    /// Manages Source Code XML Documentation.
    /// </summary>
    public static class CodeXmlDocHelper
    {
        #region Constants

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member

        public const string AttributeName_name = "name";
        public const string AttributeName_cref = "cref";
        public const string AttributeName_type = "type";
        public const string AttributeName_file = "file";
        public const string AttributeName_path = "path";
        public const string ElementName_doc = "doc";
        public const string ElementName_assembly = "assembly";
        public const string ElementName_name = "name";
        public const string ElementName_members = "members";
        public const string ElementName_member = "member";
        public const string ElementName_summary = "summary";
        public const string ElementName_description = "description";
        public const string ElementName_remarks = "remarks";
        public const string ElementName_see = "see";
        public const string ElementName_seealso = "seealso";
        public const string ElementName_c = "c";
        public const string ElementName_exception = "exception";
        public const string ElementName_param = "param";
        public const string ElementName_paramref = "paramref";
        public const string ElementName_para = "para";
        public const string ElementName_code = "code";
        public const string ElementName_list = "list";
        public const string ElementName_term = "term";
        public const string ElementName_item = "item";
        public const string ElementName_example = "example";
        public const string ElementName_permission = "permission";
        public const string ElementName_typeparam = "typeparam";
        public const string ElementName_include = "include";
        public const string ElementName_typeparamref = "typeparamref";
        public const string ElementName_returns = "returns";
        public const string ElementName_value = "value";
        public const string ListType_bullet = "bullet";
        public const string ListType_number = "number";
        public const string ListType_table = "table";
        public const string Prefix_TypeDef = "T:";
        public const string Prefix_Property = "P:";
        public const string Prefix_Const = "F:";
        public const string Prefix_Method = "M:";
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member

        #endregion

        private static IDictionary<XmlDocSourceKey, XDocument> _cache = new Dictionary<XmlDocSourceKey, XDocument>();

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
        public static IEnumerable<XElement> GetCommandSummary(this Type key)
        {
            if (key == null)
                return null;
            XElement summaryElement = GetTypeChildElement(key, "summary");
            if (summaryElement == null)
                return new XElement[0];
            return AsMamlBlocks(summaryElement.Nodes());
        }

        public static IEnumerable<XElement> AsMamlBlocks(IEnumerable<XNode> enumerable)
        {
            throw new NotImplementedException();
        }

        public static void SetMamlContent(XElement parent, IEnumerable<XNode> nodeCollection, uint level = 0)
        {
            if (nodeCollection == null)
                return;

            XName defaultName;

            if ((PSMamlHelper.CanContainText(parent)))
                defaultName = null;
            else
            {
                defaultName = PSMamlHelper.XmlNs_maml.GetName(PSMamlHelper.ElementName_para);
                if (!PSMamlHelper.IsValidChildTree(parent, defaultName))
                {
                    defaultName = PSMamlHelper.XmlNs_maml.GetName(PSMamlHelper.ElementName_row);
                    if (!PSMamlHelper.IsValidChildTree(parent, defaultName))
                    {
                        defaultName = PSMamlHelper.XmlNs_maml.GetName(PSMamlHelper.ElementName_listItem);
                        if (!PSMamlHelper.IsValidChildTree(parent, defaultName))
                            throw new NotSupportedException("Cannot determine default name for " + PSMamlHelper.ToXmlQualifiedName(parent.Name).ToString());
                    }
                }
            }

            XElement currentChildElement = null;
            foreach (XNode node in nodeCollection)
            {
                if (node == null)
                    continue;

                if (node is XText || node is XCData)
                {
                    if (defaultName == null)
                        parent.Add(node);
                    else
                    {
                        if (currentChildElement == null)
                        {
                            currentChildElement = new XElement(defaultName, node);
                            parent.Add(currentChildElement);
                        }
                        else
                            currentChildElement.Add(node);
                    }
                    continue;
                }

                if (!(node is XElement))
                    continue;

                XElement element = node as XElement;
                if (PSMamlHelper.IsValidChildTree(parent, element.Name))
                {
                    if (currentChildElement != null)
                        currentChildElement = null;
                    parent.Add(element);
                    continue;
                }

                if (PSMamlHelper.IsValidChildTree(parent, defaultName, element.Name))
                {
                    if (currentChildElement == null)
                    {
                        currentChildElement = new XElement(defaultName, node);
                        parent.Add(currentChildElement);
                    }
                    else
                        currentChildElement.Add(node);
                    continue;
                }

                if (element.Name.NamespaceName.Length > 0)
                {
                    List<XElement> toSplit = new List<XElement>();
                    toSplit.Add(parent);
                    for (uint n = level; n > 0; n--)
                    {
                        XElement e = toSplit[0].Parent;
                        if (PSMamlHelper.IsValidChildTree(e, element.Name))
                        {
                            // TODO: Insert
                            toSplit = null;
                            break;
                        }
                        toSplit.Insert(0, e);
                    }
                    if (toSplit != null)
                        SetMamlContent(parent, element.Nodes(), level);
                    continue;
                }

                XElement childElement;
                switch (element.Name.LocalName)
                {
                    case ElementName_para:
                        childElement = new XElement(PSMamlHelper.XmlNs_maml.GetName(PSMamlHelper.ElementName_para));
                        break;
                    case ElementName_c:
                    case ElementName_code:
                        childElement = new XElement(PSMamlHelper.XmlNs_maml.GetName(PSMamlHelper.ElementName_codeInline));
                        break;
                    case ElementName_param:
                    case ElementName_paramref:
                        childElement = new XElement(PSMamlHelper.XmlNs_maml.GetName(PSMamlHelper.ElementName_parameterNameInline));
                        break;
                    default:
                        childElement = null;
                        break;
                }

                if (PSMamlHelper.IsValidChildTree(parent, childElement.Name))
                {
                    if (currentChildElement != null)
                        currentChildElement = null;
                    parent.Add(childElement);
                }
                else if (PSMamlHelper.IsValidChildTree(parent, defaultName, childElement.Name))
                {
                    if (currentChildElement == null)
                    {
                        currentChildElement = new XElement(defaultName, childElement);
                        parent.Add(currentChildElement);
                    }
                    else
                        currentChildElement.Add(childElement);
                }
                else
                {
                    List<XElement> toSplit = new List<XElement>();
                    toSplit.Add(parent);
                    for (uint n = level; n > 0; n--)
                    {
                        XElement e = toSplit[0].Parent;
                        if (PSMamlHelper.IsValidChildTree(e, childElement.Name))
                        {
                            // TODO: Insert
                            toSplit = null;
                            break;
                        }
                        toSplit.Insert(0, e);
                    }
                    if (toSplit != null)
                        childElement = null;
                }
                if (childElement == null)
                    SetMamlContent(parent, element.Nodes(), level);
                else
                    SetMamlContent(childElement, element.Nodes(), level + 1);
            }
        }
        
        public static XElement GetXmlDocMemberElement(this Type key)
        {
            if (key == null)
                return null;

            return Get(key).FirstOrDefault();
        }

        public static XElement GetXmlDocMemberElement(this PropertyInfo key)
        {
            if (key == null)
                return null;

            return Get(key).FirstOrDefault();
        }

        public static XDocument GetCodeXmlDoc(this Assembly key)
        {
            Monitor.Enter(_cache);
            try
            {
                XmlDocSourceKey sourceKey = XmlDocSourceKey.Create(key);
                if (!_cache.ContainsKey(sourceKey))
                    return Add(key);
                return _cache[sourceKey];
            }
            finally { Monitor.Exit(_cache); }
        }
        
        #region Get
        
        private static IEnumerable<XDocument> _Get(params Assembly[] assemblies)
        {
            if (assemblies == null)
                yield break;

            foreach (Assembly assembly in assemblies)
                yield return GetCodeXmlDoc(assembly);

            foreach (Assembly assembly in assemblies)
            {
                if (assembly == null)
                    continue;
                string n = assembly.GetName().Name;
                foreach (XmlDocSourceKey key in _cache.Keys.Where(k => k.FullName == null && k.Name == n))
                    yield return _cache[key];
            }
        }

        public static IEnumerable<XElement> Get(Type key)
        {
            if (key == null)
                return new XElement[0];
            string name = Prefix_TypeDef + key.FullName;
            return _Get(key.Assembly).SelectMany(d => d.Elements(ElementName_doc).Elements(ElementName_members).Elements(ElementName_member).Attributes(AttributeName_name)
                .Where(a => a.Value == name).Select(a => a.Parent));
        }

        public static IEnumerable<XElement> Get(PropertyInfo key)
        {
            if (key == null)
                return new XElement[0];
            IEnumerable<XDocument> docs = _Get(key.ReflectedType.Assembly);

            if (key.ReflectedType.Assembly.FullName != key.DeclaringType.Assembly.FullName)
            {
                IEnumerable<XDocument> d = _Get(key.DeclaringType.Assembly);
                docs = docs.Take(1).Concat(d.Take(1)).Concat(docs.Skip(1)).Concat(d.Skip(1));
            }

            string name = Prefix_Property + key.ReflectedType.FullName + "." + key.Name;
            IEnumerable<XElement> results = docs.SelectMany(d => d.Elements(ElementName_doc).Elements(ElementName_members).Elements(ElementName_member).Attributes(AttributeName_name)
                    .Where(a => a.Value == name).Select(a => a.Parent));
            if (key.ReflectedType.FullName == key.DeclaringType.FullName)
                return results;
            string altName = Prefix_Property + key.DeclaringType.FullName + "." + key.Name;
            return results.Concat(docs.SelectMany(d => d.Elements(ElementName_doc).Elements(ElementName_members).Elements(ElementName_member).Attributes(AttributeName_name)
                    .Where(a => a.Value == altName).Select(a => a.Parent)));
        }

        #endregion

        #region Add

        public static XDocument Add(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            Monitor.Enter(_cache);
            try
            {
                if (!String.IsNullOrEmpty(assembly.Location))
                    return _Add(assembly, assembly.Location);

                XmlDocSourceKey key = XmlDocSourceKey.Create(assembly);

                if (_cache.ContainsKey(key))
                    _cache[key] = null;
                else
                    _cache.Add(key, null);
                return null;
            }
            finally { Monitor.Exit(_cache); }
        }

        private static XDocument _Add(Assembly assembly, string path)
        {
            Monitor.Enter(_cache);
            try
            {
                XmlDocSourceKey key = XmlDocSourceKey.Create(assembly);
                XDocument document;
                string xmlPath = Path.Combine(Path.GetDirectoryName(assembly.Location), Path.GetFileNameWithoutExtension(assembly.Location) + ".XML");
                if (File.Exists(xmlPath))
                    document = (File.Exists(xmlPath)) ? XDocument.Load(xmlPath) : null;
                else
                    document = null;
                if (_cache.ContainsKey(key))
                    _cache[key] = document;
                else
                    _cache.Add(key, document);
                return document;
            }
            finally { Monitor.Exit(_cache); }
        }

        public static XDocument Add(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (path.Length == 0)
                throw new ArgumentException("Path cannot be empty.", "path");

            Monitor.Enter(_cache);
            try
            {
                try
                {
                    path = Path.GetFullPath(path);
                    if (!File.Exists(path))
                        throw new FileNotFoundException("Assembly or XML document not found.", "path");
                }
                catch (Exception exc) { throw new ArgumentException(exc.Message, "path", exc); }

                string ext = Path.GetExtension(path);
                if (!String.IsNullOrEmpty(ext))
                {
                    if (XmlDocSourceKey.UriComparer.Equals(ext, ".xml"))
                        return _Add(path, true);

                    if (XmlDocSourceKey.UriComparer.Equals(ext, ".dll"))
                        return _Add(path, false);
                }

                XDocument document;
                Assembly assembly;
                try
                {
                    document = XDocument.Load(path);
                    assembly = null;
                }
                catch
                {
                    assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.Location != null && XmlDocSourceKey.UriComparer.Equals(path, a.Location));
                    if (assembly == null)
                    {
                        try { assembly = Assembly.ReflectionOnlyLoadFrom(path); }
                        catch { assembly = null; }
                        if (assembly == null)
                            throw;
                    }
                    document = null;
                }

                if (assembly != null)
                    return _Add(assembly, path);

                _Add(document, path);
                return document;
            }
            finally { Monitor.Exit(_cache); }
        }

        public static XDocument Add(string path, bool asXml)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (path.Length == 0)
                throw new ArgumentException("Path cannot be empty.", "path");

            return _Add(path, asXml);
        }

        private static XDocument _Add(string path, bool asXml)
        {
            Monitor.Enter(_cache);
            try
            {
                if (asXml)
                {
                    XDocument document = XDocument.Load(path);
                    _Add(document, path);
                    return document;
                }

                Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.Location != null && XmlDocSourceKey.UriComparer.Equals(path, a.Location));
                if (assembly == null)
                    assembly = Assembly.ReflectionOnlyLoadFrom(path);
                return _Add(assembly, path);
            }
            finally { Monitor.Exit(_cache); }

        }

        public static void Add(XDocument document, string sourcePath)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            if (sourcePath == null)
                throw new ArgumentNullException("sourcePath");

            if (sourcePath.Length == 0)
                throw new ArgumentException("Path cannot be empty.", "sourcePath");
            
            _Add(document, sourcePath);
        }

        private static void _Add(XDocument document, string sourcePath)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            string[] names = document.Elements(ElementName_doc).Elements(ElementName_assembly).Elements(ElementName_name)
                .Where(e => !e.IsEmpty).Select(e => e.Value.Trim()).Where(s => s.Length > 0).ToArray();
            if (names.Length == 0)
                throw new ArgumentException("XML document does not contain an assembly name element.", "document");

            if (names.Length > 1)
                throw new ArgumentException("XML document contains multiple assembly name elements.", "document");

            Monitor.Enter(_cache);
            try
            {
                string dllPath = Path.Combine(Path.GetDirectoryName(sourcePath), Path.GetFileNameWithoutExtension(sourcePath) + ".dll");
                Assembly assembly;
                XmlDocSourceKey key;
                
                if (File.Exists(dllPath))
                {
                    assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.Location != null && XmlDocSourceKey.UriComparer.Equals(dllPath, a.Location));
                    if (assembly == null)
                        assembly = Assembly.ReflectionOnlyLoadFrom(dllPath);
                    key = XmlDocSourceKey.Create(assembly);
                    if (key.Name != names[0] && (key.FullName == null || key.Name != key.FullName))
                        key = XmlDocSourceKey.Create(names[0], sourcePath);
                }
                else
                    key = XmlDocSourceKey.Create(names[0], sourcePath);
                if (_cache.ContainsKey(key))
                    _cache[key] = document;
                else
                    _cache.Add(key, document);
            }
            finally { Monitor.Exit(_cache); }
        }

        #endregion

        public static XElement GetPropertyChildElement(PropertyInfo property, string elementName)
        {
            return Get(property).Elements(elementName).FirstOrDefault(e => !e.IsEmpty && (e.HasElements || e.Value.Trim().Length > 0));
        }

        public static XElement GetTypeChildElement(Type implementingType, string elementName)
        {
            if (implementingType == null)
                throw new ArgumentNullException("implementingType");
            
            return Get(implementingType).Elements(elementName).FirstOrDefault(e => !e.IsEmpty && (e.HasElements || e.Value.Trim().Length > 0));
        }

        public static XElement GetPropertyChildElement(Type implementingType, string name, string elementName)
        {
            if (implementingType == null)
                throw new ArgumentNullException("implementingType");

            PropertyInfo[] properties = implementingType.GetProperties();
            PropertyInfo property = properties.FirstOrDefault(p => p.Name == name);
            if (property == null)
            {
                if ((property = properties.FirstOrDefault(p => String.Equals(p.Name, name, StringComparison.InvariantCultureIgnoreCase))) == null)
                    return null;
            }
            return GetPropertyChildElement(property, elementName);
        }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
        
        /// <summary>
        /// Refers to a specific instance of an <seealso cref="XDocument"/> which contains Source Code XML Documentation of an assembly at a specific location.
        /// </summary>
        public struct XmlDocSourceKey : IEquatable<XmlDocSourceKey>, IEquatable<string>
        {
            private string _sourceUri;
            private string _name;
            private string _fullName;

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
            public static readonly StringComparer UriComparer = StringComparer.InvariantCultureIgnoreCase;
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member

            /// <summary>
            /// A URI string which describes the source location of the referenced Source Code XML Documentation.
            /// </summary>
            public string SourceUri { get { return _sourceUri ?? ""; } }

            /// <summary>
            /// Name of assembly which the referenced Source Code XML Documentation refers to.
            /// </summary>
            public string Name { get { return _name ?? ""; } }

            /// <summary>
            /// Full Name of assembly which the referenced Source Code XML Documentation refers to.
            /// </summary>
            /// <remarks>If this is null, then the assembly full name is not known.</remarks>
            public string FullName { get { return _fullName; } }

            private XmlDocSourceKey(string sourceUri, string name, string fullName)
            {
                _sourceUri = sourceUri;
                _name = name;
                _fullName = fullName;
            }

            /// <summary>
            /// Create new <see cref="XmlDocSourceKey"/> from an assembly name.
            /// </summary>
            /// <param name="name"><seealso cref="AssemblyName"/> from which to initialize the new <see cref="XmlDocSourceKey"/> value.</param>
            public XmlDocSourceKey(AssemblyName name)
            {
                if (name == null)
                {
                    _sourceUri = null;
                    _name = null;
                    _fullName = null;
                    return;
                }
                _sourceUri = AsUriString(name.CodeBase);
                _name = name.Name;
                _fullName = name.FullName;
            }

            /// <summary>
            /// Create new <see cref="XmlDocSourceKey"/> from a formatted name and location.
            /// </summary>
            /// <param name="assemblyName">Formatted name of assembly.</param>
            /// <param name="codeBase">Path or URI representing location of an assembly.</param>
            /// <returns>A new <see cref="XmlDocSourceKey"/> object.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="assemblyName"/> or <paramref name="codeBase"/> is null.</exception>
            /// <exception cref="ArgumentException"><paramref name="assemblyName"/> is not a valid assembly name
            /// or <paramref name="codeBase"/> is not a valid path or URI string.</exception>
            public static XmlDocSourceKey Create(string assemblyName, string codeBase)
            {
                if (assemblyName == null)
                    throw new ArgumentNullException("assemblyName");
                if (codeBase == null)
                    throw new ArgumentNullException("codeBase");
                AssemblyName name;
                try { name = new AssemblyName(assemblyName); }
                catch (Exception exc) { throw new ArgumentException(exc.Message, "assemblyName", exc); }
                string uri;
                try { uri = AsUriString(codeBase); }
                catch (Exception exc) { throw new ArgumentException(exc.Message, "codeBase", exc); }

                return new XmlDocSourceKey(uri, name.Name, (name.Name == name.FullName) ? null : name.FullName);
            }

            /// <summary>
            /// Create a new <see cref="XmlDocSourceKey"/> from an <seealso cref="Assembly"/> object and an optional location uri override.
            /// </summary>
            /// <param name="assembly"><seealso cref="Assembly"/> which the resulting <see cref="XmlDocSourceKey"/> is to represent.</param>
            /// <param name="uriOverride">Optional value which overrides the uri to use as the assembly's location.
            /// If this value is not provided, is null or empty, then the assembly's <seealso cref="Assembly.Location"/> value will be used to ascertain the location.</param>
            /// <returns>A new <see cref="XmlDocSourceKey"/> representing the provided <seealso cref="Assembly"/> object.</returns>
            /// <exception cref="ArgumentException"><paramref name="uriOverride"/> is not a valid path or URI string.</exception>
            public static XmlDocSourceKey Create(Assembly assembly, string uriOverride = null)
            {
                if (assembly == null)
                    return new XmlDocSourceKey(null);
                AssemblyName name = assembly.GetName();

                if (!String.IsNullOrEmpty(uriOverride))
                {
                    try { return new XmlDocSourceKey(AsUriString(uriOverride), name.Name, name.FullName); }
                    catch (Exception exc) { throw new ArgumentException(exc.Message, "uriOverride", exc); }
                }

                if (String.IsNullOrEmpty(name.CodeBase))
                {
                    if (String.IsNullOrEmpty(assembly.Location))
                        return new XmlDocSourceKey("", name.Name, name.FullName);

                    try { return new XmlDocSourceKey((new Uri(Path.GetFullPath(assembly.Location), UriKind.Absolute)).ToString(), name.Name, name.FullName); }
                    catch
                    {
                        try { return new XmlDocSourceKey((new Uri(assembly.Location, UriKind.Absolute)).ToString(), name.Name, name.FullName); }
                        catch { return new XmlDocSourceKey(assembly.Location, name.Name, name.FullName); }
                    }
                }
                return new XmlDocSourceKey(name.CodeBase, name.Name, name.FullName);
            }

            /// <summary>
            /// Ensures a string is formatted as a URI.
            /// </summary>
            /// <param name="pathOrUri">A string representing a path or URI.</param>
            /// <returns><paramref name="pathOrUri"/> converted to an absolute URI.</returns>
            /// <remarks>If <paramref name="pathOrUri"/> is null or empty, an empty string is returned.
            /// <para>If <paramref name="pathOrUri"/> is not a valid absolute URI string, then <seealso cref="Path.GetFullPath(string)"/>
            /// is invoked before it is converted to a absolute URI string.</para>
            /// <para>Likewise, if <paramref name="pathOrUri"/> is a URI string with a <c>file:</c> schema,
            /// then <seealso cref="Path.GetFullPath(string)"/> is invoked with the <seealso cref="Uri.LocalPath"/>
            /// value before being re-converted to a URI string.</para></remarks>
            public static string AsUriString(string pathOrUri)
            {
                if (String.IsNullOrEmpty(pathOrUri))
                    return "";

                Uri uri;
                if (Uri.TryCreate(pathOrUri, UriKind.Absolute, out uri))
                {
                    if (uri.Scheme == Uri.UriSchemeFile && !String.IsNullOrEmpty(uri.LocalPath))
                    {
                        pathOrUri = Path.GetFullPath(uri.LocalPath);
                        if (pathOrUri != uri.LocalPath)
                            uri = new Uri(pathOrUri, UriKind.Absolute);
                    }
                }
                else
                    uri = new Uri(Path.GetFullPath(pathOrUri), UriKind.Absolute);

                return uri.ToString();
            }

            /// <summary>
            /// Determines if current <see cref="XmlDocSourceKey"/> object is equal to another.
            /// </summary>
            /// <param name="other">Other <see cref="XmlDocSourceKey"/> to compare.</param>
            /// <returns><c>true</c> if current <see cref="XmlDocSourceKey"/> is equal to <paramref name="other"/>; otherwise, <c>false</c>.</returns>
            /// <remarks>The <see cref="Name"/> and <see cref="FullName"/> value comparisons are case-sensitive, which the
            /// <see cref="SourceUri"/> value comparison is not case-sensitive.</remarks>
            public bool Equals(XmlDocSourceKey other)
            {
                if (_sourceUri == null)
                {
                    if (other._sourceUri != null)
                        return false;
                }
                else if (other._sourceUri == null || !UriComparer.Equals(_sourceUri, other._sourceUri))
                    return false;

                if (_fullName == null)
                {
                    if (other._fullName != null)
                        return false;
                }
                else if (other._fullName == null || _fullName != other._fullName)
                    return false;

                return (_name == null) ? other._name == null : (other._name != null && _name == other._name);
            }

            /// <summary>
            /// Determines if a string value is equal to the current <see cref="Name"/> or <see cref="FullName"/>.
            /// </summary>
            /// <param name="other">Assembly name to compare.</param>
            /// <returns><c>true</c> if see cref="Name"/> or <see cref="FullName"/> is equal to <paramref name="other"/>; otherwise, <c>false</c>.</returns>
            /// <remarks>This is a case-sensitive comparison.</remarks>
            public bool Equals(string other)
            {
                if (other == null)
                    return _name == null;

                if (_name == other)
                    return true;

                return _fullName != null && _fullName == other;
            }

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                if (obj is XmlDocSourceKey)
                    return Equals((XmlDocSourceKey)obj);

                return Equals(obj as string);
            }

            public override int GetHashCode() { return ToString().GetHashCode(); }

            public override string ToString() { return (String.IsNullOrEmpty(_sourceUri)) ? _fullName ?? (_name ?? "") : _sourceUri; }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
        }

        internal static IEnumerable<XElement> GetCommandDescription(Type implementingType)
        {
            throw new NotImplementedException();
        }
    }
}

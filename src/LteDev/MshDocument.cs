using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace LteDev
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    
    public sealed class MshDocument : IList<PSCommandHelp>, IList
    {
        private object _syncRoot;
        private List<PSCommandHelp> _innerList = new List<PSCommandHelp>();
        public int Count { get { return _innerList.Count; } }
        bool ICollection<PSCommandHelp>.IsReadOnly { get { return false; } }
        bool IList.IsReadOnly { get { return false; } }
        bool IList.IsFixedSize { get { return false; } }
        object ICollection.SyncRoot { get { return _syncRoot; } }
        bool ICollection.IsSynchronized { get { return true; } }

        public PSCommandHelp this[int index]
        {
            get { return _innerList[index]; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                Monitor.Enter (_syncRoot);
                try
                {
                    if (_innerList.Any(i => ReferenceEquals(i, value)))
                        throw new InvalidOperationException("Item already exists in the collection");
                    _innerList[index] = value;
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }
        object IList.this[int index] { get { return _innerList[index]; } set { this[index] = (PSCommandHelp)value; } }
        public static class Names
        {
            public const string xmlns = "xmlns";
            public const string helpItems = "helpItems";
            public const string command = "command";
            public const string details = "details";
            public const string name = "name";
            public const string verb = "verb";
            public const string noun = "noun";
            public const string para = "para";
            public const string description = "description";
            public const string dev = "dev";
            public const string maml = "maml";
            internal static readonly string syntax;
            internal static readonly string syntaxItem;
            internal static readonly string parameter;
            internal static readonly string pipelineInput;
            internal static readonly string required;
            internal static readonly string globbing;
            internal static readonly string position;
            internal static readonly string parameterValue;
            internal static readonly string parameters;
        }
        public static class NS
        {
            public const string xmlns = "http://www.w3.org/2000/xmlns/";
            public const string msh = "http://msh";
            public const string maml = "http://schemas.microsoft.com/maml/2004/10";
            public const string command = "http://schemas.microsoft.com/maml/dev/command/2004/10";
            public const string dev = "http://schemas.microsoft.com/maml/dev/2004/10";
        }
        public MshDocument()
        {
            _syncRoot = (((IList)_innerList).IsSynchronized) ? ((IList)_innerList).SyncRoot : null;
            if (_syncRoot == null)
                _syncRoot = new object();
        }
        public static MshDocument Import(XmlElement element)
        {
            // https://msdn.microsoft.com/en-us/library/bb525433(v=vs.85).aspx
            if (element == null || element.LocalName != Names.helpItems || element.NamespaceURI != NS.msh)
                return null;
            MshDocument result = new MshDocument();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(element.OwnerDocument.NameTable);
            nsmgr.AddNamespace("msh", NS.msh);
            nsmgr.AddNamespace(Names.command, NS.command);
            foreach (XmlElement e in element.SelectNodes(Names.command + ":" + Names.command))
                result._innerList.Add(PSCommandHelp.Import(element));
            return result;
        }
        public static MshDocument Import(XmlDocument document) { return Import(document.DocumentElement); }
        public XmlDocument Export()
        {
            // https://msdn.microsoft.com/en-us/library/bb525433(v=vs.85).aspx
            XmlDocument xml = new XmlDocument();
            xml.AppendChild(xml.CreateElement(Names.helpItems, NS.msh));
            xml.DocumentElement.Attributes.Append(xml.CreateAttribute("schema")).Value = Names.maml;
            foreach (PSCommandHelp cmd in _innerList)
                xml.DocumentElement.AppendChild(xml.ImportNode(cmd.Export().DocumentElement, true));
            return xml;
        }
        public int IndexOf(PSCommandHelp item) { return _innerList.IndexOf(item); }
        public void Insert(int index, PSCommandHelp item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            Monitor.Enter (_syncRoot);
            try
            {
                if (_innerList.Any(i => ReferenceEquals(i, item)))
                    throw new InvalidOperationException("Item already exists in the collection");
                if (item.Document == null)
                {
                    _innerList.Insert(index, item);
                    try { item.Document = this; }
                    catch
                    {
                        _innerList.Remove(item);
                        throw;
                    }
                }
                else
                {
                    if (!ReferenceEquals(item.Document, this))
                        throw new InvalidOperationException("Item belongs to another document");
                    _innerList.Insert(index, item);
                }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        public void RemoveAt(int index)
        {
            Monitor.Enter (_syncRoot);
            try
            {
                PSCommandHelp item = _innerList[index];
                _innerList.RemoveAt(index);
                if (item.Document != null && ReferenceEquals(item.Document, this))
                {
                    try { item.Document = null; }
                    catch
                    {
                        if (item.Document != null && ReferenceEquals(item.Document, this))
                        {
                            if (index == _innerList.Count)
                                _innerList.Add(item);
                            else
                                _innerList.Insert(index, item);
                        }
                        throw;
                    }
                }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        public void Add(PSCommandHelp item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            Monitor.Enter (_syncRoot);
            try
            {
                if (_innerList.Any(i => ReferenceEquals(i, item)))
                    throw new InvalidOperationException("Item already exists in the collection");
                if (item.Document == null)
                {
                    _innerList.Add(item);
                    try { item.Document = this; }
                    catch
                    {
                        _innerList.Remove(item);
                        throw;
                    }
                }
                else
                {
                    if (!ReferenceEquals(item.Document, this))
                        throw new InvalidOperationException("Item belongs to another document");
                    _innerList.Add(item);
                }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        public void Clear() { _innerList.Clear(); }
        public bool Contains(PSCommandHelp item) { return _innerList.Contains(item); }
        public void CopyTo(PSCommandHelp[] array, int arrayIndex) { _innerList.CopyTo(array, arrayIndex); }
        public bool Remove(PSCommandHelp item)
        {
            if (item == null)
                return false;
            Monitor.Enter (_syncRoot);
            try
            {
                int index = _innerList.IndexOf(item);
                if (index < 0)
                    return false;
                RemoveAt(index);
            }
            finally { Monitor.Exit(_syncRoot); }
            return true;
        }
        public IEnumerator<PSCommandHelp> GetEnumerator() { return _innerList.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IList)_innerList).GetEnumerator(); }
        int IList.Add(object value)
        {
            Add((PSCommandHelp)value);
            return IndexOf((PSCommandHelp)value);
        }
        bool IList.Contains(object value) { return value != null && value is PSCommandHelp && Contains((PSCommandHelp)value); }
        int IList.IndexOf(object value) { return (value != null && value is PSCommandHelp) ? IndexOf((PSCommandHelp)value) : -1; }
        void IList.Insert(int index, object value) { Insert(index, (PSCommandHelp)value); }
        void IList.Remove(object value)
        {
            if (value != null && value is PSCommandHelp)
                Remove((PSCommandHelp)value);
        }
        void ICollection.CopyTo(Array array, int index) { ((IList)_innerList).CopyTo(array, index); }
    }
    public sealed class PSCommandHelp : IEquatable<PSCommandHelp>, IList<IPSSyntax>, IList
    {
        private object _syncRoot;
        private List<IPSSyntax> _innerList = new List<IPSSyntax>();
        public int Count { get { return _innerList.Count; } }
        bool ICollection<IPSSyntax>.IsReadOnly { get { return false; } }
        bool IList.IsReadOnly { get { return false; } }

        bool IList.IsFixedSize { get { return false; } }

        object ICollection.SyncRoot { get { return _syncRoot; } }

        bool ICollection.IsSynchronized { get { return true; } }

        public IPSSyntax this[int index]
        {
            get { return _innerList[index]; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (!(value is PSSyntaxHelp || value is PSParameterHelp))
                    throw new ArgumentException("Invalid item");
                Monitor.Enter (_syncRoot);
                try
                {
                    if (_innerList.Any(i => ReferenceEquals(i, value)))
                    {
                        if (IndexOf(value) == index)
                            return;
                        throw new InvalidOperationException("Item already exists in the collection");
                    }
                    _innerList[index] = value;
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }
        object IList.this[int index] { get { return _innerList[index]; } set { this[index] = (IPSSyntax)value; } }
        public string Verb { get; set; }
        public string Noun { get; set; }
        public string SynopsisText
        {
            get
            {
                XmlElement synopsisXml = SynopsisXml;
                if (synopsisXml == null || synopsisXml.IsEmpty)
                    return "";
                return synopsisXml.InnerText;
            }
            set
            {
                XmlElement synopsisXml = SynopsisXml;
                if (synopsisXml == null)
                {
                    if (value == null)
                        return;
                    XmlDocument document = new XmlDocument();
                    synopsisXml = (XmlElement)(document.AppendChild(document.CreateElement(MshDocument.Names.maml, MshDocument.Names.para, MshDocument.NS.maml)));
                    synopsisXml.InnerText = value.Trim();
                }
                else if (value == null)
                {
                    SynopsisXml = null;
                    return;   
                }
                else
                {
                    synopsisXml.IsEmpty = true;
                    synopsisXml.InnerText = value;
                }
                SynopsisXml = synopsisXml;
            }
        }
        public XmlElement SynopsisXml { get; set; }
        public string[] DescriptionText
        {
            get
            {
                XmlElement descriptionXml = DescriptionXml;
                if (descriptionXml == null)
                    return new string[0];
                return descriptionXml.SelectNodes("*").Cast<XmlElement>().Where(e => !e.IsEmpty).Select(e => e.InnerText).ToArray();
            }
            set
            {
                XmlElement descriptionXml = DescriptionXml;
                XmlDocument document;
                if (descriptionXml == null)
                {
                    if (value == null)
                        return;
                    document = new XmlDocument();
                    XmlElement descriptionElement = (XmlElement)(document.AppendChild(document.CreateElement(MshDocument.Names.maml, MshDocument.Names.description, MshDocument.NS.maml)));
                }
                else if (value == null)
                {
                    DescriptionXml = null;
                    return;   
                }
                else
                {
                    document = descriptionXml.OwnerDocument;
                    descriptionXml.IsEmpty = true;
                }
                foreach (string s in value)
                {
                    XmlElement element = (XmlElement)(descriptionXml.AppendChild(document.CreateElement(MshDocument.Names.maml, MshDocument.Names.para, MshDocument.NS.maml)));
                    if (s == null)
                        element.IsEmpty = true;
                    else
                        element.InnerText = s;
                }
                DescriptionXml = descriptionXml;
            }
        }
        public XmlElement DescriptionXml { get; set; }
        private MshDocument _document = null;
        public MshDocument Document
        {
            get { return _document; }
            set
            {
                IList document = _document;
                if (value == null)
                {
                    if (document == null)
                        return;
                    Monitor.Enter(document.SyncRoot);
                    try
                    {
                        if (_document.Contains(this))
                            _document.Remove(this);
                        else
                            _document = null;
                    }
                    finally { Monitor.Exit(document.SyncRoot); }
                    return;
                }
                Monitor.Enter(((IList)value).SyncRoot);
                try
                {
                    if (document != null)
                    {
                        if (ReferenceEquals(document, value))
                            return;
                        Monitor.Enter(document.SyncRoot);
                        try
                        {
                            if (_document.Contains(this))
                                _document.Remove(this);
                            else
                                _document = null;
                        }
                        finally { Monitor.Exit(document.SyncRoot); }
                    }
                    if (value.Contains(this))
                        _document = value;
                    else
                        _document.Add(this);
                }
                finally { Monitor.Exit(((IList)value).SyncRoot); }
            }
        }

        public static bool IsValidVerb(string verb)
        {
            if (verb == null || (verb = verb.Trim()).Length == 0)
                return false;
            StringComparer comparer = StringComparer.InvariantCultureIgnoreCase;
            return GetVerbNames().Any(n => comparer.Equals(n, verb));
        }
        public static string GetValidVerb(string verb)
        {
            if (verb == null || (verb = verb.Trim()).Length == 0)
                return null;
            StringComparer comparer = StringComparer.InvariantCultureIgnoreCase;
            return GetVerbNames().FirstOrDefault(n => comparer.Equals(n, verb));
        }
        public static readonly Regex NounRegex = new Regex(@"^\s*((?<n>[A-Z][a-zA-Z\d]*)|(?<c>[a-z])(?<n>[a-zA-Z\d]+)?)\s*$", RegexOptions.Compiled);
        public static bool IsValidNoun(string noun) { return noun != null && NounRegex.IsMatch(noun); }
        public static string GetNormalizedNoun(string noun)
        {
            if (noun == null)
                return null;
            Match m = NounRegex.Match(noun);
            if (m == null)
                return null;
            
            if (m.Groups["c"].Success)
                return (m.Groups["n"].Success) ? m.Groups["c"].Value.ToUpper() + m.Groups["n"].Value : m.Groups["c"].Value.ToUpper();
            return m.Groups["n"].Value;
        }
        public static IEnumerable<string> GetVerbNames()
        {
            string s = (typeof(string)).AssemblyQualifiedName;
            foreach (Type t in new Type[] { typeof(VerbsCommon), typeof(VerbsCommunications), typeof(VerbsData), typeof(VerbsDiagnostic), typeof(VerbsLifecycle), typeof(VerbsOther), typeof(VerbsSecurity) })
            {
                foreach (var n in t.GetProperties().Where(p => p.PropertyType.AssemblyQualifiedName == s).Select(p => new { P = p, G = (p.CanRead) ? p.GetGetMethod() : null })
                    .Where(a => a.G != null && a.G.IsPublic))
                    yield return n.P.Name;
            }
        }
        public PSCommandHelp() { }
        public static PSCommandHelp Import(XmlElement commandElement)
        {
            if (commandElement == null || commandElement.LocalName != MshDocument.Names.command || commandElement.NamespaceURI != MshDocument.NS.command)
                return null;
            PSCommandHelp result = new PSCommandHelp();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(commandElement.OwnerDocument.NameTable);
            nsmgr.AddNamespace(MshDocument.Names.command, MshDocument.NS.command);
            nsmgr.AddNamespace(MshDocument.Names.maml, MshDocument.NS.maml);
            XmlElement detailsElement = (XmlElement)(commandElement.SelectSingleNode(MshDocument.Names.command + ":" + MshDocument.Names.command));
            XmlElement element;
            if (detailsElement != null)
            {
                element = (XmlElement)(detailsElement.SelectSingleNode(MshDocument.Names.command + ":" + MshDocument.Names.verb));
                if (element != null && !element.IsEmpty)
                    result.Verb = GetValidVerb(element.InnerText);
                element = (XmlElement)(detailsElement.SelectSingleNode(MshDocument.Names.command + ":" + MshDocument.Names.noun));
                if (element != null && !element.IsEmpty)
                    result.Noun = GetNormalizedNoun(element.InnerText);
                if (result.Verb == null)
                {
                    element = (XmlElement)(detailsElement.SelectSingleNode(MshDocument.Names.command + ":" + MshDocument.Names.name));
                    if (element != null && !element.IsEmpty)
                    {
                        string[] nv = element.InnerText.Split(new char[] { '-' }, 2);
                        result.Verb = GetValidVerb(nv[0]);
                        if (nv.Length > 1)
                            result.Noun = GetNormalizedNoun(nv[1]);
                        else if (result.Verb == null)
                            result.Noun = GetNormalizedNoun(nv[0]);
                    }
                }
                else if (result.Noun == null)
                {
                    element = (XmlElement)(detailsElement.SelectSingleNode(MshDocument.Names.command + ":" + MshDocument.Names.name));
                    if (element != null && !element.IsEmpty)
                    {
                        string[] nv = element.InnerText.Split(new char[] { '-' }, 2);
                        if (nv.Length > 1)
                            result.Noun = GetNormalizedNoun(nv[1]);
                        else if (GetValidVerb(nv[0]) == null)
                            result.Noun = GetNormalizedNoun(nv[0]);
                    }
                }
            }
            detailsElement = (XmlElement)(detailsElement.SelectSingleNode(MshDocument.Names.maml + ":" + MshDocument.Names.description));
            if (detailsElement != null && !detailsElement.IsEmpty)
            {
                element = (XmlElement)(detailsElement.SelectSingleNode(MshDocument.Names.maml + ":" + MshDocument.Names.para));
                if (element != null)
                {
                    XmlDocument document = new XmlDocument();
                    document.AppendChild(document.ImportNode(element, true));
                    result.SynopsisXml = document.DocumentElement;
                }
                else
                    result.SynopsisText = detailsElement.InnerText.Trim();
            }
            detailsElement = (XmlElement)(commandElement.SelectSingleNode(MshDocument.Names.maml + ":" + MshDocument.Names.description));
            if (detailsElement != null && !detailsElement.IsEmpty)
            {
                element = (XmlElement)(detailsElement.SelectSingleNode(MshDocument.Names.maml + ":" + MshDocument.Names.para));
                if (element != null)
                {
                    XmlDocument document = new XmlDocument();
                    document.AppendChild(document.ImportNode(element, true));
                    result.DescriptionXml = document.DocumentElement;
                }
                else
                    result.DescriptionText = new string[] { detailsElement.InnerText.Trim() };
            }
            
            return result;
        }
        public static PSCommandHelp Import(XmlDocument document) { return Import(document.DocumentElement); }
        public XmlDocument Export()
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml("<" + MshDocument.Names.command + ":" + MshDocument.Names.command + " " + MshDocument.Names.xmlns + ":" + MshDocument.Names.maml + "=\"" + MshDocument.NS.maml +
                "\" " + MshDocument.Names.xmlns + ":" + MshDocument.Names.command + "=\"" + MshDocument.NS.command + "\" " + MshDocument.Names.xmlns + ":" + MshDocument.Names.dev +
                "=\"" + MshDocument.NS.dev + "\"/>");
            XmlElement detailsElement = (XmlElement)(xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.details, MshDocument.NS.command)));
            if (Verb != null)
            {
                ((XmlElement)(detailsElement.AppendChild(xml.CreateElement(MshDocument.Names.verb, MshDocument.NS.command)))).InnerText = Verb;
                if (Noun != null)
                {
                    ((XmlElement)(detailsElement.AppendChild(xml.CreateElement(MshDocument.Names.noun, MshDocument.NS.command)))).InnerText = Noun;
                    ((XmlElement)(detailsElement.AppendChild(xml.CreateElement(MshDocument.Names.name, MshDocument.NS.command)))).InnerText = Verb + "-" + Noun;
                }
                else
                {
                    ((XmlElement)(detailsElement.AppendChild(xml.CreateElement(MshDocument.Names.noun, MshDocument.NS.command)))).AppendChild(xml.CreateComment("Noun goes here"));
                    ((XmlElement)(detailsElement.AppendChild(xml.CreateElement(MshDocument.Names.name, MshDocument.NS.command)))).AppendChild(xml.CreateComment("Verb-Noun goes here"));
                }
            }
            else
            {
                ((XmlElement)(detailsElement.AppendChild(xml.CreateElement(MshDocument.Names.verb, MshDocument.NS.command)))).AppendChild(xml.CreateComment("Verb goes here"));
                if (Noun != null)
                    ((XmlElement)(detailsElement.AppendChild(xml.CreateElement(MshDocument.Names.noun, MshDocument.NS.command)))).InnerText = Noun;
                else
                    ((XmlElement)(detailsElement.AppendChild(xml.CreateElement(MshDocument.Names.noun, MshDocument.NS.command)))).AppendChild(xml.CreateComment("Noun goes here"));
                ((XmlElement)(detailsElement.AppendChild(xml.CreateElement(MshDocument.Names.name, MshDocument.NS.command)))).AppendChild(xml.CreateComment("Verb-Noun goes here"));

            }
            XmlElement element = SynopsisXml;
            if (element == null)
                detailsElement.AppendChild(xml.CreateElement(MshDocument.Names.description, MshDocument.NS.maml))
                    .AppendChild(xml.CreateElement(MshDocument.Names.para, MshDocument.NS.maml)).AppendChild(xml.CreateComment("Synopsis goes here"));
            else if (element.LocalName != MshDocument.Names.para || element.NamespaceURI != MshDocument.NS.maml)
            {
                if (element.IsEmpty)
                    detailsElement.AppendChild(xml.CreateElement(MshDocument.Names.description, MshDocument.NS.maml))
                        .AppendChild(xml.CreateElement(MshDocument.Names.para, MshDocument.NS.maml)).AppendChild(xml.CreateComment("Synopsis goes here"));
                else
                    ((XmlElement)(detailsElement.AppendChild(xml.CreateElement(MshDocument.Names.description, MshDocument.NS.maml))
                        .AppendChild(xml.CreateElement(MshDocument.Names.para, MshDocument.NS.maml)))).InnerText = element.InnerText;
            }
            else
                detailsElement.AppendChild(xml.CreateElement(MshDocument.Names.description, MshDocument.NS.maml)).AppendChild(xml.ImportNode(element, true));
            element = DescriptionXml;
            if (element == null)
                xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.description, MshDocument.NS.maml))
                    .AppendChild(xml.CreateElement(MshDocument.Names.para, MshDocument.NS.maml)).AppendChild(xml.CreateComment("Detailed description goes here"));
            else if (element.NamespaceURI == MshDocument.NS.maml)
            {
                if (element.LocalName != MshDocument.Names.description)
                    xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.description, MshDocument.NS.maml)).AppendChild(xml.ImportNode(element, true));
                else
                    xml.DocumentElement.AppendChild(xml.ImportNode(element, true));
            } 
            else if (element.IsEmpty)
                xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.description, MshDocument.NS.maml))
                    .AppendChild(xml.CreateElement(MshDocument.Names.para, MshDocument.NS.maml)).AppendChild(xml.CreateComment("Detailed description goes here"));
            else
                ((XmlElement)(xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.description, MshDocument.NS.maml))
                    .AppendChild(xml.CreateElement(MshDocument.Names.para, MshDocument.NS.maml)))).InnerText = element.InnerText;
            
            detailsElement = (XmlElement)(xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.syntax, MshDocument.NS.command)));
            foreach (PSSyntaxHelp syntax in _innerList.OfType<PSSyntaxHelp>())
                detailsElement.AppendChild(xml.ImportNode(syntax.Export().DocumentElement, true));
            detailsElement = (XmlElement)(xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.parameters, MshDocument.NS.command)));
            foreach (PSParameterHelp para in _innerList.OfType<PSParameterHelp>())
                detailsElement.AppendChild(xml.ImportNode(para.Export().DocumentElement, true));
            // TODO: inputTypes: https://msdn.microsoft.com/en-us/library/bb736331(v=vs.85).aspx
            // TODO: returnValues: https://msdn.microsoft.com/en-us/library/bb736338(v=vs.85).aspx
            // TODO: alertset: https://msdn.microsoft.com/en-us/library/bb736330(v=vs.85).aspx
            // TODO: examples: https://msdn.microsoft.com/en-us/library/bb736335(v=vs.85).aspx
            // TODO: relatedLinks: https://msdn.microsoft.com/en-us/library/bb736334(v=vs.85).aspx
             return xml;
        }
        public bool Equals(PSCommandHelp other) { return other != null && ReferenceEquals(other, this); }
        public override bool Equals(object obj) { return Equals(obj as PSCommandHelp); }
        public int IndexOf(IPSSyntax item) { return _innerList.IndexOf(item); }
        public void Insert(int index, IPSSyntax item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (!(item is PSSyntaxHelp || item is PSParameterHelp))
                throw new ArgumentException("Invalid item");
            Monitor.Enter (_syncRoot);
            try
            {
                if (_innerList.Any(i => ReferenceEquals(i, item)))
                    throw new InvalidOperationException("Item already exists in the collection");
                if (item.Command == null)
                {
                    _innerList.Insert(index, item);
                    try { item.Command = this; }
                    catch
                    {
                        _innerList.Remove(item);
                        throw;
                    }
                }
                else
                {
                    if (!ReferenceEquals(item.Command, this))
                        throw new InvalidOperationException("Item belongs to another command");
                    _innerList.Insert(index, item);
                }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        public void RemoveAt(int index)
        {
            Monitor.Enter (_syncRoot);
            try
            {
                IPSSyntax item = _innerList[index];
                _innerList.RemoveAt(index);
                if (item.Command != null && ReferenceEquals(item.Command, this))
                {
                    try { item.Command = null; }
                    catch
                    {
                        if (item.Command != null && ReferenceEquals(item.Command, this))
                        {
                            if (index == _innerList.Count)
                                _innerList.Add(item);
                            else
                                _innerList.Insert(index, item);
                        }
                        throw;
                    }
                }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        public void Add(IPSSyntax item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (!(item is PSSyntaxHelp || item is PSParameterHelp))
                throw new ArgumentException("Invalid item");
            Monitor.Enter (_syncRoot);
            try
            {
                if (_innerList.Any(i => ReferenceEquals(i, item)))
                    throw new InvalidOperationException("Item already exists in the collection");
                if (item.Command == null)
                {
                    _innerList.Add(item);
                    try { item.Command = this; }
                    catch
                    {
                        _innerList.Remove(item);
                        throw;
                    }
                }
                else
                {
                    if (!ReferenceEquals(item.Command, this))
                        throw new InvalidOperationException("Item belongs to another command");
                    _innerList.Add(item);
                }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        public void Clear() { _innerList.Clear(); }
        public bool Contains(IPSSyntax item) { return _innerList.Contains(item); }
        public void CopyTo(IPSSyntax[] array, int arrayIndex) { _innerList.CopyTo(array, arrayIndex); }
        public bool Remove(IPSSyntax item)
        {
            if (item == null)
                return false;
            Monitor.Enter (_syncRoot);
            try
            {
                int index = _innerList.IndexOf(item);
                if (index < 0)
                    return false;
                RemoveAt(index);
            }
            finally { Monitor.Exit(_syncRoot); }
            return true;
        }
        public IEnumerator<IPSSyntax> GetEnumerator() { return _innerList.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IList)_innerList).GetEnumerator(); }
        int IList.Add(object value)
        {
            Add((IPSSyntax)value);
            return IndexOf((IPSSyntax)value);
        }
        bool IList.Contains(object value) { return value != null && value is IPSSyntax && Contains((IPSSyntax)value); }
        int IList.IndexOf(object value) { return (value != null && value is IPSSyntax) ? IndexOf((IPSSyntax)value) : -1; }
        void IList.Insert(int index, object value) { Insert(index, (IPSSyntax)value); }
        void IList.Remove(object value)
        {
            if (value != null && value is IPSSyntax)
                Remove((IPSSyntax)value);
        }
        void ICollection.CopyTo(Array array, int index) { ((IList)_innerList).CopyTo(array, index); }
    }
    public interface IPSSyntax
    {
        string Name { get; }
        PSCommandHelp Command { get; set; }
    }
    public sealed class PSSyntaxHelp : IPSSyntax, IEquatable<PSSyntaxHelp>, IList<PSSyntaxParameterHelp>, IList
    {
        string IPSSyntax.Name { get { return CommandName; } }
        public string CommandName
        {
            get
            {
                if (Command == null || Command.Noun == null)
                    return null;
                if (Command.Verb == null)
                    return Command.Noun;
                return Command.Verb + "-" + Command.Noun;
            }
        }
        private PSCommandHelp _command = null;
        public PSCommandHelp Command
        {
            get { return _command; }
            set
            {
                IList document = _command;
                if (value == null)
                {
                    if (document == null)
                        return;
                    Monitor.Enter(document.SyncRoot);
                    try
                    {
                        if (_command.Contains(this))
                            _command.Remove(this);
                        else
                            _command = null;
                    }
                    finally { Monitor.Exit(document.SyncRoot); }
                    return;
                }
                Monitor.Enter(((IList)value).SyncRoot);
                try
                {
                    if (document != null)
                    {
                        if (ReferenceEquals(document, value))
                            return;
                        Monitor.Enter(document.SyncRoot);
                        try
                        {
                            if (_command.Contains(this))
                                _command.Remove(this);
                            else
                                _command = null;
                        }
                        finally { Monitor.Exit(document.SyncRoot); }
                    }
                    if (value.Contains(this))
                        _command = value;
                    else
                        _command.Add(this);
                }
                finally { Monitor.Exit(((IList)value).SyncRoot); }
            }
        }
        private object _syncRoot;
        private List<PSSyntaxParameterHelp> _innerList = new List<PSSyntaxParameterHelp>();
        public int Count { get { return _innerList.Count; } }
        bool ICollection<PSSyntaxParameterHelp>.IsReadOnly { get { return false; } }
        bool IList.IsReadOnly { get { return false; } }

        bool IList.IsFixedSize { get { return false; } }

        object ICollection.SyncRoot { get { return _syncRoot; } }

        bool ICollection.IsSynchronized { get { return true; } }

        public PSSyntaxParameterHelp this[int index]
        {
            get { return _innerList[index]; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                Monitor.Enter (_syncRoot);
                try
                {
                    if (_innerList.Any(i => ReferenceEquals(i, value)))
                    {
                        if (IndexOf(value) == index)
                            return;
                        throw new InvalidOperationException("Item already exists in the collection");
                    }
                    _innerList[index] = value;
                }
                finally { Monitor.Exit(_syncRoot); }
            }
        }
        object IList.this[int index] { get { return _innerList[index]; } set { this[index] = (PSSyntaxParameterHelp)value; } }
        public XmlDocument Export()
        {
            XmlDocument xml = new XmlDocument();
            xml.AppendChild(xml.CreateElement(MshDocument.Names.syntaxItem, MshDocument.NS.command));
            
            string name = CommandName;
            if (name != null)
                ((XmlElement)(xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.name, MshDocument.NS.maml)))).InnerText = name;
            else
                xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.name, MshDocument.NS.maml)).AppendChild(xml.CreateComment("Verb goes here"));
            
            // syntax: https://msdn.microsoft.com/en-us/library/bb525442(v=vs.85).aspx
            foreach (PSSyntaxParameterHelp para in _innerList)
                xml.DocumentElement.AppendChild(xml.ImportNode(para.Export().DocumentElement, true));
             return xml;
        }
        public bool Equals(PSSyntaxHelp other) { return other != null && ReferenceEquals(other, this); }
        public override bool Equals(object obj) { return Equals(obj as PSSyntaxHelp); }
        public int IndexOf(PSSyntaxParameterHelp item) { return _innerList.IndexOf(item); }
        public void Insert(int index, PSSyntaxParameterHelp item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            Monitor.Enter (_syncRoot);
            try
            {
                if (_innerList.Any(i => ReferenceEquals(i, item)))
                    throw new InvalidOperationException("Item already exists in the collection");
                if (item.Syntax == null)
                {
                    _innerList.Insert(index, item);
                    try { item.Syntax = this; }
                    catch
                    {
                        _innerList.Remove(item);
                        throw;
                    }
                }
                else
                {
                    if (!ReferenceEquals(item.Syntax, this))
                        throw new InvalidOperationException("Item belongs to another parameter");
                    _innerList.Insert(index, item);
                }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        public void RemoveAt(int index)
        {
            Monitor.Enter (_syncRoot);
            try
            {
                PSSyntaxParameterHelp item = _innerList[index];
                _innerList.RemoveAt(index);
                if (item.Syntax != null && ReferenceEquals(item.Syntax, this))
                {
                    try { item.Syntax = null; }
                    catch
                    {
                        if (item.Syntax != null && ReferenceEquals(item.Syntax, this))
                        {
                            if (index == _innerList.Count)
                                _innerList.Add(item);
                            else
                                _innerList.Insert(index, item);
                        }
                        throw;
                    }
                }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        public void Add(PSSyntaxParameterHelp item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            Monitor.Enter (_syncRoot);
            try
            {
                if (_innerList.Any(i => ReferenceEquals(i, item)))
                    throw new InvalidOperationException("Item already exists in the collection");
                if (item.Syntax == null)
                {
                    _innerList.Add(item);
                    try { item.Syntax = this; }
                    catch
                    {
                        _innerList.Remove(item);
                        throw;
                    }
                }
                else
                {
                    if (!ReferenceEquals(item.Syntax, this))
                        throw new InvalidOperationException("Item belongs to another parameter");
                    _innerList.Add(item);
                }
            }
            finally { Monitor.Exit(_syncRoot); }
        }
        public void Clear() { _innerList.Clear(); }
        public bool Contains(PSSyntaxParameterHelp item) { return _innerList.Contains(item); }
        public void CopyTo(PSSyntaxParameterHelp[] array, int arrayIndex) { _innerList.CopyTo(array, arrayIndex); }
        public bool Remove(PSSyntaxParameterHelp item)
        {
            if (item == null)
                return false;
            Monitor.Enter (_syncRoot);
            try
            {
                int index = _innerList.IndexOf(item);
                if (index < 0)
                    return false;
                RemoveAt(index);
            }
            finally { Monitor.Exit(_syncRoot); }
            return true;
        }
        public IEnumerator<PSSyntaxParameterHelp> GetEnumerator() { return _innerList.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IList)_innerList).GetEnumerator(); }
        int IList.Add(object value)
        {
            Add((PSSyntaxParameterHelp)value);
            return IndexOf((PSSyntaxParameterHelp)value);
        }
        bool IList.Contains(object value) { return value != null && value is PSParameterHelp && Contains((PSSyntaxParameterHelp)value); }
        int IList.IndexOf(object value) { return (value != null && value is PSParameterHelp) ? IndexOf((PSSyntaxParameterHelp)value) : -1; }
        void IList.Insert(int index, object value) { Insert(index, (PSSyntaxParameterHelp)value); }
        void IList.Remove(object value)
        {
            if (value != null && value is PSSyntaxParameterHelp)
                Remove((PSSyntaxParameterHelp)value);
        }
        void ICollection.CopyTo(Array array, int index) { ((IList)_innerList).CopyTo(array, index); }
    }
    public enum PipelineInputType
    {
        None,
        ByValue,
        ByPropertyName,
    }
    public sealed class PSSyntaxParameterHelp : IEquatable<PSSyntaxParameterHelp>
    {
        public string Name { get; set; }
        public bool Mandatory { get; set; }
        public bool Globbing { get; set; }
        public int? Position { get; set; }
        public PipelineInputType PipelineInput { get; set; }
        public PSTypeName Type { get; set; }
        private PSSyntaxHelp _syntax = null;
        public PSSyntaxHelp Syntax
        {
            get { return _syntax; }
            set
            {
                IList document = _syntax;
                if (value == null)
                {
                    if (document == null)
                        return;
                    Monitor.Enter(document.SyncRoot);
                    try
                    {
                        if (_syntax.Contains(this))
                            _syntax.Remove(this);
                        else
                            _syntax = null;
                    }
                    finally { Monitor.Exit(document.SyncRoot); }
                    return;
                }
                Monitor.Enter(((IList)value).SyncRoot);
                try
                {
                    if (document != null)
                    {
                        if (ReferenceEquals(document, value))
                            return;
                        Monitor.Enter(document.SyncRoot);
                        try
                        {
                            if (_syntax.Contains(this))
                                _syntax.Remove(this);
                            else
                                _syntax = null;
                        }
                        finally { Monitor.Exit(document.SyncRoot); }
                    }
                    if (value.Contains(this))
                        _syntax = value;
                    else
                        _syntax.Add(this);
                }
                finally { Monitor.Exit(((IList)value).SyncRoot); }
            }
        }

        public string DescriptionText
        {
            get
            {
                XmlElement synopsisXml = DescriptionXml;
                if (synopsisXml == null || synopsisXml.IsEmpty)
                    return "";
                return synopsisXml.InnerText;
            }
            set
            {
                XmlElement synopsisXml = DescriptionXml;
                if (synopsisXml == null)
                {
                    if (value == null)
                        return;
                    XmlDocument document = new XmlDocument();
                    synopsisXml = (XmlElement)(document.AppendChild(document.CreateElement(MshDocument.Names.maml, MshDocument.Names.para, MshDocument.NS.maml)));
                    synopsisXml.InnerText = value.Trim();
                }
                else if (value == null)
                {
                    DescriptionXml = null;
                    return;   
                }
                else
                {
                    synopsisXml.IsEmpty = true;
                    synopsisXml.InnerText = value;
                }
                DescriptionXml = synopsisXml;
            }
        }
        public XmlElement DescriptionXml { get; set; }
        public bool Equals(PSSyntaxParameterHelp other) { return other != null && ReferenceEquals(other, this); }
        public override bool Equals(object obj) { return Equals(obj as PSSyntaxHelp); }
        public XmlDocument Export()
        {
            // TODO: parameter: https://msdn.microsoft.com/en-us/library/bb525442(v=vs.85).aspx#ParameterAttributes
            XmlDocument xml = new XmlDocument();
            xml.AppendChild(xml.CreateElement(MshDocument.Names.parameter, MshDocument.NS.command));
            
            xml.DocumentElement.Attributes.Append(xml.CreateAttribute(MshDocument.Names.required)).Value = Mandatory.ToString().ToLower();
            xml.DocumentElement.Attributes.Append(xml.CreateAttribute(MshDocument.Names.globbing)).Value = Globbing.ToString().ToLower();
            switch (PipelineInput)
            { 
                case PipelineInputType.ByValue:
                    xml.DocumentElement.Attributes.Append(xml.CreateAttribute(MshDocument.Names.pipelineInput)).Value = "true (ByValue)";
                    break;
                case PipelineInputType.ByPropertyName:
                    xml.DocumentElement.Attributes.Append(xml.CreateAttribute(MshDocument.Names.pipelineInput)).Value = "true (ByPropertyName)";
                    break;
                default:
                    xml.DocumentElement.Attributes.Append(xml.CreateAttribute(MshDocument.Names.pipelineInput)).Value = "false";
                    break;
            }
            int? position = Position;
            if (position.HasValue)
                xml.DocumentElement.Attributes.Append(xml.CreateAttribute(MshDocument.Names.position)).Value = position.Value.ToString();
            
            string name = Name;
            if (name != null)
                ((XmlElement)(xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.name, MshDocument.NS.maml)))).InnerText = name;
            else
                xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.name, MshDocument.NS.maml)).AppendChild(xml.CreateComment("Parameter name goes here"));
            PSTypeName type = Type;
            XmlElement typeElement = (XmlElement)(xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.parameterValue, MshDocument.NS.command)));
            if (type == null)
                typeElement.AppendChild(xml.CreateComment("Parameter type goes here"));
            else
            {
                string n = (type.Type != null) ? LanguagePrimitives.ConvertTypeNameToPSTypeName(type.Type.FullName) : LanguagePrimitives.ConvertTypeNameToPSTypeName(type.Name);
                typeElement.InnerText = n.Substring(1, n.Length - 2);
            }
            XmlElement element = DescriptionXml;
            if (element == null)
                xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.description, MshDocument.NS.maml))
                    .AppendChild(xml.CreateElement(MshDocument.Names.para, MshDocument.NS.maml)).AppendChild(xml.CreateComment("Description goes here"));
            else if (element.LocalName != MshDocument.Names.para || element.NamespaceURI != MshDocument.NS.maml)
            {
                if (element.IsEmpty)
                    xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.description, MshDocument.NS.maml))
                        .AppendChild(xml.CreateElement(MshDocument.Names.para, MshDocument.NS.maml)).AppendChild(xml.CreateComment("Description goes here"));
                else
                    ((XmlElement)(xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.description, MshDocument.NS.maml))
                        .AppendChild(xml.CreateElement(MshDocument.Names.para, MshDocument.NS.maml)))).InnerText = element.InnerText;
            }
            else
                xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.description, MshDocument.NS.maml)).AppendChild(xml.ImportNode(element, true));
            
            return xml;
        }
    }
    public sealed class PSParameterHelp : IPSSyntax, IEquatable<PSParameterHelp>
    {
        public string Name { get; set; }
        public bool Mandatory { get; set; }
        public bool Globbing { get; set; }
        public int? Position { get; set; }
        public PipelineInputType PipelineInput { get; set; }
        public PSTypeName Type { get; set; }
        private PSCommandHelp _command = null;
        public PSCommandHelp Command
        {
            get { return _command; }
            set
            {
                IList document = _command;
                if (value == null)
                {
                    if (document == null)
                        return;
                    Monitor.Enter(document.SyncRoot);
                    try
                    {
                        if (_command.Contains(this))
                            _command.Remove(this);
                        else
                            _command = null;
                    }
                    finally { Monitor.Exit(document.SyncRoot); }
                    return;
                }
                Monitor.Enter(((IList)value).SyncRoot);
                try
                {
                    if (document != null)
                    {
                        if (ReferenceEquals(document, value))
                            return;
                        Monitor.Enter(document.SyncRoot);
                        try
                        {
                            if (_command.Contains(this))
                                _command.Remove(this);
                            else
                                _command = null;
                        }
                        finally { Monitor.Exit(document.SyncRoot); }
                    }
                    if (value.Contains(this))
                        _command = value;
                    else
                        _command.Add(this);
                }
                finally { Monitor.Exit(((IList)value).SyncRoot); }
            }
        }

        public string DescriptionText
        {
            get
            {
                XmlElement synopsisXml = DescriptionXml;
                if (synopsisXml == null || synopsisXml.IsEmpty)
                    return "";
                return synopsisXml.InnerText;
            }
            set
            {
                XmlElement synopsisXml = DescriptionXml;
                if (synopsisXml == null)
                {
                    if (value == null)
                        return;
                    XmlDocument document = new XmlDocument();
                    synopsisXml = (XmlElement)(document.AppendChild(document.CreateElement(MshDocument.Names.maml, MshDocument.Names.para, MshDocument.NS.maml)));
                    synopsisXml.InnerText = value.Trim();
                }
                else if (value == null)
                {
                    DescriptionXml = null;
                    return;   
                }
                else
                {
                    synopsisXml.IsEmpty = true;
                    synopsisXml.InnerText = value;
                }
                DescriptionXml = synopsisXml;
            }
        }
        public XmlElement DescriptionXml { get; set; }
        public bool Equals(PSParameterHelp other) { return other != null && ReferenceEquals(other, this); }
        public override bool Equals(object obj) { return Equals(obj as PSSyntaxHelp); }
        public XmlDocument Export()
        {
            // TODO: parameters: https://msdn.microsoft.com/en-us/library/bb736339(v=vs.85).aspx
            XmlDocument xml = new XmlDocument();
            xml.AppendChild(xml.CreateElement(MshDocument.Names.parameter, MshDocument.NS.command));
            
            xml.DocumentElement.Attributes.Append(xml.CreateAttribute(MshDocument.Names.required)).Value = Mandatory.ToString().ToLower();
            xml.DocumentElement.Attributes.Append(xml.CreateAttribute(MshDocument.Names.globbing)).Value = Globbing.ToString().ToLower();
            switch (PipelineInput)
            { 
                case PipelineInputType.ByValue:
                    xml.DocumentElement.Attributes.Append(xml.CreateAttribute(MshDocument.Names.pipelineInput)).Value = "true (ByValue)";
                    break;
                case PipelineInputType.ByPropertyName:
                    xml.DocumentElement.Attributes.Append(xml.CreateAttribute(MshDocument.Names.pipelineInput)).Value = "true (ByPropertyName)";
                    break;
                default:
                    xml.DocumentElement.Attributes.Append(xml.CreateAttribute(MshDocument.Names.pipelineInput)).Value = "false";
                    break;
            }
            int? position = Position;
            if (position.HasValue)
                xml.DocumentElement.Attributes.Append(xml.CreateAttribute(MshDocument.Names.position)).Value = position.Value.ToString();
            
            string name = Name;
            if (name != null)
                ((XmlElement)(xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.name, MshDocument.NS.maml)))).InnerText = name;
            else
                xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.name, MshDocument.NS.maml)).AppendChild(xml.CreateComment("Parameter name goes here"));
            PSTypeName type = Type;
            XmlElement typeElement = (XmlElement)(xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.parameterValue, MshDocument.NS.command)));
            if (type == null)
                typeElement.AppendChild(xml.CreateComment("Parameter type goes here"));
            else
            {
                string n = (type.Type != null) ? LanguagePrimitives.ConvertTypeNameToPSTypeName(type.Type.FullName) : LanguagePrimitives.ConvertTypeNameToPSTypeName(type.Name);
                typeElement.InnerText = n.Substring(1, n.Length - 2);
            }
            XmlElement element = DescriptionXml;
            if (element == null)
                xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.description, MshDocument.NS.maml))
                    .AppendChild(xml.CreateElement(MshDocument.Names.para, MshDocument.NS.maml)).AppendChild(xml.CreateComment("Description goes here"));
            else if (element.LocalName != MshDocument.Names.para || element.NamespaceURI != MshDocument.NS.maml)
            {
                if (element.IsEmpty)
                    xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.description, MshDocument.NS.maml))
                        .AppendChild(xml.CreateElement(MshDocument.Names.para, MshDocument.NS.maml)).AppendChild(xml.CreateComment("Description goes here"));
                else
                    ((XmlElement)(xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.description, MshDocument.NS.maml))
                        .AppendChild(xml.CreateElement(MshDocument.Names.para, MshDocument.NS.maml)))).InnerText = element.InnerText;
            }
            else
                xml.DocumentElement.AppendChild(xml.CreateElement(MshDocument.Names.description, MshDocument.NS.maml)).AppendChild(xml.ImportNode(element, true));
            
            return xml;
        }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}
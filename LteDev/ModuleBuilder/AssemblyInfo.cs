using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace LteDev.ModuleBuilder
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public class AssemblyInfo : InformationAggregator, IEquatable<AssemblyInfo>, IEquatable<Assembly>
    {
        public const string ElementName_doc = "doc";
        public const string ElementName_assembly = "assembly";
        public const string ElementName_name = "name";
        public const string ElementName_members = "members";
        public const string ElementName_member = "member";
        public const string ElementName_summary = "summary";
        public const string ElementName_ = "";
        public const string XmlDocPrefix_Namespace = "N";
        public const string XmlDocPrefix_Type = "T";
        public const string XmlDocPrefix_Field = "F";
        public const string XmlDocPrefix_Property = "P";
        public const string XmlDocPrefix_Method = "M";
        public const string XmlDocPrefix_Event = "E";
        public const string XmlDocPrefix_ErrorString = "!";
        public const string AttributeName_name = "name";
        public static readonly Regex CompanyName_Microsoft = new Regex(@"^\s*Microsoft(\s+Corporation)?\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private List<CLRTypeInfo> _typeContexts = new List<CLRTypeInfo>();
        private string _assemblyFolder = null;
        private string _helpDocPath = null;
        private XElement _xmlDoc = null;
        private string _company = null;
        private string _copyright = null;
        private string _description = null;
        private string _product = null;
        private string _title = null;
        private AssemblyName _name = null;
        private Version _version = null;

        public Assembly Assembly { get; private set; }

        public AggregateInfoFactory ContextFactory { get; private set; }

        public AssemblyName Name
        {
            get
            {
                if (_name == null)
                    _name = Assembly.GetName();

                return _name;
            }
        }

        public Version Version
        {
            get
            {
                if (_version == null)
                    _version = Name.Version;
                return _version;
            }
        }

        public string Company
        {
            get
            {
                if (_company == null)
                    _company = Assembly.GetCustomAttributes<AssemblyCompanyAttribute>().Select(a => a.Company).Where(s => !String.IsNullOrWhiteSpace(s)).DefaultIfEmpty("").First();

                return _company;
            }
        }

        public string Copyright
        {
            get
            {
                if (_copyright == null)
                    _copyright = Assembly.GetCustomAttributes<AssemblyCopyrightAttribute>().Select(a => a.Copyright).Where(s => !String.IsNullOrWhiteSpace(s)).DefaultIfEmpty("").First();

                return _copyright;
            }
        }

        public string Description
        {
            get
            {
                if (_description == null)
                    _description = Assembly.GetCustomAttributes<AssemblyDescriptionAttribute>().Select(a => a.Description).Where(s => !String.IsNullOrWhiteSpace(s)).DefaultIfEmpty("").First();

                return _description;
            }
        }

        public string Product
        {
            get
            {
                if (_product == null)
                    _product = Assembly.GetCustomAttributes<AssemblyProductAttribute>().Select(a => a.Product).Where(s => !String.IsNullOrWhiteSpace(s)).DefaultIfEmpty("").First();

                return _product;
            }
        }

        public string Title
        {
            get
            {
                if (_title == null)
                    _title = Assembly.GetCustomAttributes<AssemblyTitleAttribute>().Select(a => a.Title).Where(s => !String.IsNullOrWhiteSpace(s)).DefaultIfEmpty("").First();

                return _title;
            }
        }

        public string AssemblyLocation { get { return Assembly.Location ?? ""; } }

        public string AssemblyFolder
        {
            get
            {
                if (_assemblyFolder == null)
                {
                    string s;
                    _assemblyFolder = ((s = AssemblyLocation).Length > 0) ? Path.GetDirectoryName(s) : s;
                }
                return _assemblyFolder;
            }
        }

        public string HelpDocPath
        {
            get
            {
                if (_helpDocPath == null)
                {
                    string s;
                    _helpDocPath = ((s = AssemblyLocation).Length > 0) ? Path.Combine(Path.GetDirectoryName(s), Path.GetFileNameWithoutExtension(Assembly.Location)) : s;
                }
                return _helpDocPath;
            }
        }

        public XElement DocMembers
        {
            get
            {
                XElement e = XmlDoc.Element(ElementName_members);
                if (e == null)
                {
                    e = new XElement(ElementName_members);
                    XmlDoc.Add(e);
                }
                return e;
            }
        }

        public XElement XmlDoc
        {
            get
            {
                XElement xmlDoc;
                Monitor.Enter(_typeContexts);
                try
                {
                    xmlDoc = _xmlDoc;
                    if (xmlDoc == null)
                    {
                        try
                        {
                            if (HelpDocPath.Length > 0 && File.Exists(_helpDocPath))
                                xmlDoc = XDocument.Load(_helpDocPath).Root;
                        }
                        finally
                        {
                            if (xmlDoc == null || xmlDoc.Name.NamespaceName.Length > 0 || xmlDoc.Name.LocalName != ElementName_doc)
                                xmlDoc = (new XDocument(new XElement("doc",
                                    new XElement("assembly",
                                        new XElement("name", Assembly.FullName)
                                    ),
                                    new XElement("members")
                                ))).Root;
                            _xmlDoc = xmlDoc;
                        }
                    }
                }
                finally { Monitor.Exit(_typeContexts); }
                return xmlDoc;
            }
        }

        public AssemblyInfo(Assembly assembly, AggregateInfoFactory factory)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            if (factory == null)
                throw new ArgumentNullException("factory");

            Assembly = assembly;
            ContextFactory = factory;
        }

        public bool IsMicrosoft() { return Company.Length > 0 && CompanyName_Microsoft.IsMatch(Company); }

        public XElement GetTypeXmlDoc(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (!Equals(type.Assembly))
                throw new ArgumentException("Type belongs to another assembly.");

            string name = XmlDocPrefix_Type + type.FullName;
            XElement xmlDoc;
            Monitor.Enter(_typeContexts);
            try
            {
                XElement members = XmlDoc.Element(ElementName_doc).Element(ElementName_members);
                xmlDoc = members.Elements(ElementName_member).Attributes(AttributeName_name).Where(a => a.Value == name).Select(a => a.Parent).FirstOrDefault();
                if (xmlDoc == null)
                {
                    xmlDoc = new XElement(ElementName_member, new XAttribute(AttributeName_name, name));
                    members.Add(xmlDoc);
                }
            }
            finally { Monitor.Exit(_typeContexts); }
            return xmlDoc;
        }

        public XElement GetTypeXmlDoc(PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            if (!Equals(property.DeclaringType.Assembly))
                return null;

            string name = XmlDocPrefix_Type + property.DeclaringType.FullName;
            XElement xmlDoc;
            Monitor.Enter(_typeContexts);
            try
            {
                XElement members = XmlDoc.Element(ElementName_doc).Element(ElementName_members);
                xmlDoc = members.Elements(ElementName_member).Attributes(AttributeName_name).Where(a => a.Value == name).Select(a => a.Parent).FirstOrDefault();
                if (xmlDoc == null)
                {
                    xmlDoc = new XElement(ElementName_member, new XAttribute(AttributeName_name, name));
                    members.Add(xmlDoc);
                }
            }
            finally { Monitor.Exit(_typeContexts); }
            return xmlDoc;
        }

        public CLRTypeInfo GetTypeContext(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (!Equals(type.Assembly))
                throw new ArgumentException("Type belongs to another assembly.");

            CLRTypeInfo context;
            Monitor.Enter(_typeContexts);
            try
            {
                context = _typeContexts.FirstOrDefault(c => c.Equals(type));
                if (context == null)
                {
                    context = new CLRTypeInfo(type, this);
                    _typeContexts.Add(context);
                }
            }
            finally { Monitor.Exit(_typeContexts); }
            return context;
        }

        public bool Equals(AssemblyInfo other)
        {
            return other != null && ReferenceEquals(this, other);
        }

        public bool Equals(Assembly other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(Assembly, other))
                return true;

            if (!String.IsNullOrEmpty(Assembly.Location) && !String.IsNullOrEmpty(other.Location))
                return String.Equals(Assembly.Location, other.Location, StringComparison.InvariantCultureIgnoreCase);

            return String.Equals(Assembly.FullName, other.FullName, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Assembly)
                return Equals(obj as Assembly);

            return Equals(obj as AssemblyInfo);
        }

        public override int GetHashCode() { return Assembly.FullName.GetHashCode(); }

        public override string ToString() { return Assembly.FullName; }
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LteDev.HelpXml
{
    public class AssemblyContext
    {
        public const string Prefix_Namespace = "N";
        public const string Prefix_Type = "T";
        public const string Prefix_Field = "F";
        public const string Prefix_Property = "P";
        public const string Prefix_Method = "M";
        public const string Prefix_Event = "E";
        public const string Prefix_ErrorString = "!";
        public const string AttributeName_name = "name";

        private AssemblyName _name = null;
        private Version _assemblyVersion = null;
        private XDocument _helpDoc = null;
        private string _copyright = null;
        private string _company = null;
        private string _description = null;
        private string _product = null;
        private string _title = null;

        public string Copyright
        {
            get
            {
                if (_copyright == null)
                {
                    if (Assembly == null)
                        _copyright = "";
                    else
                        _copyright = Assembly.GetCustomAttributes<AssemblyCopyrightAttribute>().Select(a => a.Copyright).FirstOrDefault(s => !String.IsNullOrWhiteSpace(s)) ?? "";
                }

                return _copyright;
            }
        }

        public string Company
        {
            get
            {
                if (_company == null)
                {
                    if (Assembly == null)
                        _company = "";
                    else
                        _company = Assembly.GetCustomAttributes<AssemblyCompanyAttribute>().Select(a => a.Company).FirstOrDefault(s => !String.IsNullOrWhiteSpace(s)) ?? "";
                }

                return _company;
            }
        }

        public string Description
        {
            get
            {
                if (_description == null)
                {
                    if (Assembly == null)
                        _description = "";
                    else
                        _description = Assembly.GetCustomAttributes<AssemblyDescriptionAttribute>().Select(a => a.Description).FirstOrDefault(s => !String.IsNullOrWhiteSpace(s)) ?? "";
                }

                return _description;
            }
        }

        public string Title
        {
            get
            {
                if (_title == null)
                {
                    if (Assembly == null)
                        _title = "";
                    else
                        _title = Assembly.GetCustomAttributes<AssemblyTitleAttribute>().Select(a => a.Title).FirstOrDefault(s => !String.IsNullOrWhiteSpace(s)) ?? "";
                }

                return _title;
            }
        }

        public string Product
        {
            get
            {
                if (_product == null)
                {
                    if (Assembly == null)
                        _product = "";
                    else
                        _product = Assembly.GetCustomAttributes<AssemblyProductAttribute>().Select(a => a.Product).FirstOrDefault(s => !String.IsNullOrWhiteSpace(s)) ?? "";
                }

                return _product;
            }
        }

        public AssemblyName Name
        {
            get
            {
                if (_name == null && Assembly != null)
                    _name = Assembly.GetName();
                return _name;
            }
        }

        public Version AssemblyVersion
        {
            get
            {
                if (_assemblyVersion == null && Assembly != null)
                    _assemblyVersion = Name.Version;
                return _assemblyVersion;
            }
        }

        public XDocument HelpDoc
        {
            get
            {
                if (_helpDoc == null)
                {
                    if (Assembly != null)
                    {
                        string path = Assembly.HelpDocPath();
                        if (File.Exists(path))
                            try { _helpDoc = XDocument.Load(path); } catch { }
                    }
                    if (_helpDoc == null)
                        _helpDoc = new XDocument(new XElement("doc", new XElement("assembly", new XElement("name", Assembly.FullName)), new XElement("members")));
                }

                return _helpDoc;
            }
        }

        public Assembly Assembly { get; private set; }

        public AssemblyContext(Assembly assembly) { Assembly = assembly; }

        internal XElement GetClassElement(Type implementingType)
        {
            string name = Prefix_Type + implementingType.FullName;
            return HelpDoc.Elements("doc").Elements("members").Elements("member").Attributes("name").Where(a => a.Value == name)
                .Select(a => a.Parent).FirstOrDefault();
        }

        internal XElement GetPropertyElement(Type implementingType, string propertyName)
        {
            string name = Prefix_Property + implementingType.FullName + "." + propertyName;
            return HelpDoc.Elements("doc").Elements("members").Elements("member").Attributes("name").Where(a => a.Value == name)
                .Select(a => a.Parent).FirstOrDefault();
        }
    }

}

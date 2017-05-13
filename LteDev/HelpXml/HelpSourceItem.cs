using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Xml;

namespace LteDev.HelpXml
{
    public class HelpSourceItem
    {
        public const string NamespaceURI_xmlns = "http://www.w3.org/2000/xmlns/";
        public const string NamespaceURI_msh = "http://msh";
        public const string ElementName_helpItems = "helpItems";
        public const string ElementName_command = "command";
        public const string ElementName_details = "details";
        public const string ElementName_description = "description";
        public const string ElementName_copyright = "copyright";
        public const string ElementName_verb = "verb";
        public const string ElementName_noun = "noun";
        public const string ElementName_version = "version";
        public const string ElementName_syntax = "syntax";
        public const string ElementName_syntaxItem = "syntaxItem";
        public const string ElementName_parameters = "parameters";
        public const string ElementName_inputTypes = "inputTypes";
        public const string ElementName_inputType = "inputType";
        public const string ElementName_returnValues = "returnValues";
        public const string ElementName_returnValue = "returnValue";
        public const string ElementName_parameterValue = "parameterValue";
        public const string AttributeName_schema = "schema";

        public static XmlDocument CreateHelp(object source)
        {
            if (source != null && source is PSObject)
                source = (source as PSObject).BaseObject;

            if (source == null)
                throw new ArgumentNullException("source");

            IEnumerable enumerable = (source is IEnumerable) ? source as IEnumerable : new object[] { source };

            HelpSourceItem[] items = enumerable.Cast<HelpSourceItem>().Where(o => o != null).ToArray();

            XmlDocument document = new XmlDocument();
            document.AppendChild(document.CreateElement(ElementName_helpItems, NamespaceURI_msh))
                .Attributes.Append(document.CreateAttribute(AttributeName_schema)).Value = "maml";
            foreach (HelpSourceItem item in items)
                item.AddCommands(document);
            return document;
        }

        private void AddCommands(XmlDocument document)
        {
            foreach (SourceCmdletInfo cmdlet in Commands)
                cmdlet.AddCommand(document);
        }

        public class SourceParameterInfo
        {
            public PropertyInfo Property { get; set; }

            public ReadOnlyCollection<ParameterAttribute> Attributes { get; private set; }

            private XmlElement _docElement = null;
            public XmlElement DocElement
            {
                get
                {
                    if (_docElement == null)
                    {
                        XmlElement docElement = null;
                        for (Type t = Property.ReflectedType; t != null && !t.Equals(typeof(Cmdlet)); t = t.BaseType)
                        {
                            if ((docElement = Parent.DocElement.SelectSingleNode("../member[@name='P:" + t.FullName + "." + Property.Name + "']") as XmlElement) != null)
                                break;
                        }
                        if (docElement != null)
                            _docElement = docElement;
                        else
                        {
                            XmlDocument doc = new XmlDocument();
                            _docElement = doc.AppendChild(doc.CreateElement("member")) as XmlElement;
                        }
                    }

                    return _docElement;
                }
            }

            public SourceCmdletInfo Parent { get; private set; }

            private SourceParameterInfo(SourceCmdletInfo parent, PropertyInfo property, ParameterAttribute[] attributes)
            {
                Parent = parent;
                Attributes = new ReadOnlyCollection<ParameterAttribute>(attributes);
                Property = property;
            }

            public static IEnumerable<SourceParameterInfo> Create(SourceCmdletInfo parent)
            {
                foreach (PropertyInfo propertyInfo in parent.ClassType.GetProperties())
                {
                    MethodInfo m;
                    ParameterAttribute[] a;
                    if (!propertyInfo.IsSpecialName && propertyInfo.CanRead && propertyInfo.CanWrite && (m = propertyInfo.GetGetMethod()) != null && m.IsPublic &&
                        (m = propertyInfo.GetSetMethod()) != null && m.IsPublic &&
                        (a = propertyInfo.GetCustomAttributes(typeof(ParameterAttribute), true).OfType<ParameterAttribute>().ToArray()).Length > 0)
                        yield return new SourceParameterInfo(parent, propertyInfo, a);
                }
            }
            public bool IsInParameterSet(string name)
            {
                return Attributes.Any(a => a.ParameterSetName == ParameterAttribute.AllParameterSets || a.ParameterSetName == name);
            }

            public const string ElementName_parameter = "parameter";

            public void AddParameter(XmlElement element, string parameterSetName)
            {
                XmlElement parameterElement = element.AddCommandElement(ElementName_parameter);

                ParameterAttribute attribute;
                if (parameterSetName == null)
                    attribute = Attributes.FirstOrDefault(a => a.ParameterSetName == ParameterAttribute.AllParameterSets);
                else if ((attribute = Attributes.FirstOrDefault(a => a.ParameterSetName == parameterSetName)) == null)
                    attribute = Attributes.FirstOrDefault(a => a.ParameterSetName == ParameterAttribute.AllParameterSets);
                if (attribute == null)
                    attribute = Attributes[0];

                parameterElement.Attributes.Append(element.OwnerDocument.CreateAttribute("globbing")).Value = "false";
                if (attribute.ValueFromPipelineByPropertyName)
                    parameterElement.Attributes.Append(element.OwnerDocument.CreateAttribute("pipelineInput")).Value = "true (ByPropertyName)";
                else if (attribute.ValueFromPipeline)
                    parameterElement.Attributes.Append(element.OwnerDocument.CreateAttribute("pipelineInput")).Value = "true (ByValue)";
                else
                    parameterElement.Attributes.Append(element.OwnerDocument.CreateAttribute("pipelineInput")).Value = "false";
                parameterElement.Attributes.Append(element.OwnerDocument.CreateAttribute("required")).Value = XmlConvert.ToString(attribute.Mandatory);
                parameterElement.Attributes.Append(element.OwnerDocument.CreateAttribute("position")).Value = (attribute.Position < 0) ? "named" : XmlConvert.ToString(attribute.Position);
                parameterElement.Attributes.Append(element.OwnerDocument.CreateAttribute("variableLength")).Value = XmlConvert.ToString(attribute.ValueFromRemainingArguments);
                parameterElement.AddMamlElement(SourceDocExtensions.ElementName_name).InnerText = Property.Name;
                if (String.IsNullOrWhiteSpace(parameterSetName) || String.IsNullOrWhiteSpace(attribute.HelpMessage))
                    parameterElement.AddMamlElement(ElementName_description).AddParagraphElements(DocElement.SelectSingleNode("summary") as XmlElement, "Add parameter description here.");
                else
                    parameterElement.AddMamlElement(ElementName_description).AddParagraphElements(attribute.HelpMessage);
                XmlElement parameterValueElement = parameterElement.AddCommandElement(ElementName_parameterValue);
                parameterValueElement.Attributes.Append(element.OwnerDocument.CreateAttribute("required")).Value = XmlConvert.ToString(attribute.Mandatory);
                parameterValueElement.Attributes.Append(element.OwnerDocument.CreateAttribute("variableLength")).Value = XmlConvert.ToString(attribute.ValueFromRemainingArguments);
                parameterElement.InnerText = Property.PropertyType.FullName;
            }

            public void AddInputType(XmlElement element)
            {
                XmlElement inputTypeElement = element.AddCommandElement(ElementName_inputType);
                inputTypeElement.AddDevType(new PSTypeName(Property.PropertyType));
                inputTypeElement.AddMamlElement(ElementName_description).AddParagraphElements(DocElement.SelectSingleNode("summary") as XmlElement, "Add input type description here.");
            }
        }

        public class SourceParameterSet
        {
            public string Name { get; private set; }

            public ReadOnlyCollection<SourceParameterInfo> Parameters { get; private set; }

            public SourceCmdletInfo Parent { get; private set; }

            private SourceParameterSet(SourceCmdletInfo parent, string name, IEnumerable<SourceParameterInfo> parameters)
            {
                Parent = parent;
                Name = name;
                Parameters = new ReadOnlyCollection<SourceParameterInfo>(parameters.ToArray());
            }

            public static ReadOnlyCollection<SourceParameterSet> Create(SourceCmdletInfo parent, out ReadOnlyCollection<SourceParameterInfo> parameters)
            {
                parameters = new ReadOnlyCollection<SourceParameterInfo>(SourceParameterInfo.Create(parent).ToArray());
                Collection<SourceParameterSet> result = new Collection<SourceParameterSet>();
                if (parameters.Count == 0)
                    result.Add(new SourceParameterSet(parent, ParameterAttribute.AllParameterSets, parameters));
                else
                {
                    string[] allParameterSetNames = parameters.SelectMany(p => p.Attributes.Select(a => a.ParameterSetName).Distinct().Where(s => s != ParameterAttribute.AllParameterSets)).ToArray();
                    if (allParameterSetNames.Length == 0)
                        allParameterSetNames = new string[] { (String.IsNullOrWhiteSpace(parent.Attribute.DefaultParameterSetName)) ? ParameterAttribute.AllParameterSets : parent.Attribute.DefaultParameterSetName };
                    else if (!String.IsNullOrWhiteSpace(parent.Attribute.DefaultParameterSetName) && !allParameterSetNames.Any(s => s == parent.Attribute.DefaultParameterSetName))
                        allParameterSetNames = (new string[] { parent.Attribute.DefaultParameterSetName }).Concat(allParameterSetNames).ToArray();
                    foreach (string n in allParameterSetNames)
                        result.Add(new SourceParameterSet(parent, n, parameters.Where(p => p.IsInParameterSet(n))));
                }
                return new ReadOnlyCollection<SourceParameterSet>(result);
            }

            public void AddSyntaxItem(XmlElement parent)
            {
                XmlElement syntaxItemElement = parent.AddCommandElement(ElementName_syntaxItem);
                syntaxItemElement.AddMamlElement(SourceDocExtensions.ElementName_name).InnerText = Parent.CommandName;
                foreach (SourceParameterInfo parameter in Parameters)
                    parameter.AddParameter(syntaxItemElement, Name);
            }
        }

        public class SourceCmdletInfo
        {
            public HelpSourceItem Parent { get; private set; }

            public CmdletAttribute Attribute { get; private set; }

            private ReadOnlyCollection<OutputTypeAttribute> _outputTypes = null;

            public ReadOnlyCollection<OutputTypeAttribute> OutputTypes
            {
                get
                {
                    if (_outputTypes == null)
                        _outputTypes = new ReadOnlyCollection<OutputTypeAttribute>(ClassType.GetCustomAttributes(typeof(OutputTypeAttribute), true).OfType<OutputTypeAttribute>().ToArray());

                    return _outputTypes;
                }
            }

            public Type ClassType { get; private set; }

            public XmlElement DocElement { get; private set; }

            private string _commandName = null;

            public string CommandName
            {
                get
                {
                    if (_commandName == null)
                        _commandName = Attribute.VerbName + "-" + Attribute.NounName;
                    return _commandName;
                }
            }

            private ReadOnlyCollection<SourceParameterInfo> _parameters = null;
            private ReadOnlyCollection<SourceParameterSet> _parameterSets = null;

            public ReadOnlyCollection<SourceParameterInfo> Parameters
            {
                get
                {
                    if (_parameters == null)
                    {
                        ReadOnlyCollection<SourceParameterInfo> parameters;
                        _parameterSets = SourceParameterSet.Create(this, out parameters);
                        _parameters = parameters;
                    }
                    return _parameters;
                }
            }

            public ReadOnlyCollection<SourceParameterSet> ParameterSets
            {
                get
                {
                    if (_parameterSets == null)
                    {
                        ReadOnlyCollection<SourceParameterInfo> parameters;
                        _parameterSets = SourceParameterSet.Create(this, out parameters);
                        _parameters = parameters;
                    }
                    return _parameterSets;
                }
            }

            private SourceCmdletInfo(HelpSourceItem parent, Type type, CmdletAttribute cmdletAttribute, XmlElement docElement)
            {
                Parent = parent;
                ClassType = type;
                Attribute = cmdletAttribute;
                if (docElement != null)
                    DocElement = docElement;
                else
                {
                    XmlDocument doc = new XmlDocument();
                    DocElement = doc.AppendChild(doc.CreateElement("member")) as XmlElement;
                }
            }

            public static IEnumerable<SourceCmdletInfo> Create(HelpSourceItem parent, XmlDocument xmlDoc)
            {
                if (xmlDoc == null)
                    xmlDoc = new XmlDocument();
                foreach (Type t in parent.ModuleAssembly.GetTypes())
                {
                    CmdletAttribute cmdletAttribute;
                    if (!t.IsPublic || t.IsAbstract || !(typeof(PSCmdlet)).IsAssignableFrom(t) || !t.GetConstructors().Any(c => c.IsPublic && c.GetParameters().Length == 0) ||
                        (cmdletAttribute = t.GetCustomAttributes(typeof(CmdletAttribute), false).OfType<CmdletAttribute>().FirstOrDefault()) == null)
                        continue;
                    yield return new SourceCmdletInfo(parent, t, cmdletAttribute, (xmlDoc == null) ? null : xmlDoc.SelectSingleNode("/doc/members/member[@name=\"T:" + t.FullName + "\"]") as XmlElement);
                }
            }

            public void AddCommand(XmlDocument document)
            {
                XmlElement commandElement;
                commandElement = document.DocumentElement.AddCommandElement(ElementName_command);
                commandElement.Attributes.Append(document.CreateAttribute(SourceDocExtensions.XmlPrefix_xmlns, SourceDocExtensions.XmlPrefix_maml, NamespaceURI_xmlns)).Value = SourceDocExtensions.NamespaceURI_Maml;
                commandElement.Attributes.Append(document.CreateAttribute(SourceDocExtensions.XmlPrefix_xmlns, SourceDocExtensions.XmlPrefix_dev, NamespaceURI_xmlns)).Value = SourceDocExtensions.NamespaceURI_Dev;
                commandElement.Attributes.Append(document.CreateAttribute(SourceDocExtensions.XmlPrefix_xmlns, SourceDocExtensions.XmlPrefix_MSHelp, NamespaceURI_xmlns)).Value = SourceDocExtensions.NamespaceURI_MSHelp;
                XmlElement element = commandElement.AddCommandElement(ElementName_details);
                element.AddCommandElement(SourceDocExtensions.ElementName_name).InnerText = CommandName;
                element.AddMamlElement(ElementName_description).AddParagraphElements(DocElement.SelectSingleNode("summary") as XmlElement, "Add synopsis here");
                element.AddMamlElement(ElementName_copyright).AddParagraphElements(Parent.Copyright);
                element.AddCommandElement(ElementName_verb).InnerText = Attribute.VerbName;
                element.AddCommandElement(ElementName_noun).InnerText = Attribute.NounName;
                element.AddDevElement(ElementName_version).InnerText = Parent.Version.ToString();
                commandElement.AddMamlElement(ElementName_description).AddParagraphElements(DocElement.SelectNodes("para[@type='description']"), "Add detailed description here");
                element = commandElement.AddCommandElement(ElementName_syntax);
                foreach (SourceParameterSet parameterSet in ParameterSets)
                    parameterSet.AddSyntaxItem(element);
                element = commandElement.AddCommandElement(ElementName_parameters);
                foreach (SourceParameterInfo parameter in Parameters)
                    parameter.AddParameter(element, null);
                element = commandElement.AddCommandElement(ElementName_inputTypes);
                foreach (SourceParameterInfo parameter in Parameters.Where(p => p.Attributes.Any(a => a.ValueFromPipeline || a.ValueFromPipelineByPropertyName)))
                    parameter.AddInputType(element);

                element = commandElement.AddCommandElement(ElementName_returnValues);
                foreach (PSTypeName outputType in OutputTypes.SelectMany(o => o.Type).GroupBy(t => t.Name).Select(t => t.First()))
                    element.AddCommandElement(ElementName_returnValue).AddDevType(outputType);
            }
        }

        public string Author { get; private set; }

        public Assembly ModuleAssembly { get; private set; }

        private Version _version = null;

        public Version Version
        {
            get
            {
                if (_version == null)
                {
                    Version version = ModuleAssembly.GetName().Version;
                    if (version.Revision > 0)
                        _version = version;
                    else if (version.Build > 0)
                        _version = new Version(version.Major, version.Minor, version.Build);
                    else
                        _version = new Version(version.Major, version.Minor);
                }
                return _version;
            }
        }
        
        public SourceCmdletInfo[] Commands { get; private set; }

        private string _copyright = null;

        public string Copyright
        {
            get
            {
                if (_copyright == null && (_copyright = ModuleAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false).OfType<AssemblyCopyrightAttribute>().Select(a => a.Copyright).NormalizeWhitespace().FirstOrDefault(s => s.Length > 0)) == null)
                    _copyright = "(c) 2017 " + Author + ". All rights reserved.";

                return _copyright;
            }
        }

        private string _company = null;

        public string Company
        {
            get
            {
                if (_company == null)
                    _company = ModuleAssembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false).OfType<AssemblyCompanyAttribute>().Select(a => a.Company).NormalizeWhitespace().Where(s => s.Length > 0).DefaultIfEmpty("Unknown").First();
                return _company;
            }
        }

        public HelpSourceItem(Assembly moduleAssembly, XmlDocument documentationComments, string author)
        {   
            if (moduleAssembly == null)
                throw new ArgumentNullException("moduleAssembly");

            ModuleAssembly = moduleAssembly;
            Commands = SourceCmdletInfo.Create(this, documentationComments).ToArray();
            if ((Author = author.NormalizeWhitespace()).Length == 0)
                Author = "Unknown";
        }
    }
}
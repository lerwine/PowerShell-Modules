using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LteDev.XsdTypeGen;
using System.Xml.Schema;
using System.Xml;
using System.IO;

namespace UnitTests
{
    /// <summary>
    /// Summary description for XsdTypeGenUnitTest
    /// </summary>
    [TestClass]
    public class XsdTypeGenUnitTest
    {
        public XsdTypeGenUnitTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void CsTypeFactoryTestMethod()
        {
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add("http://schemas.microsoft.com/maml/dev/command/2004/10", @"C:\Windows\System32\WindowsPowerShell\v1.0\Schemas\PSMaml\developerCommand.xsd");
            schemaSet.Compile();
            XmlQualifiedName name = new XmlQualifiedName("command", "http://schemas.microsoft.com/maml/dev/command/2004/10");
            CsTypeFactory factory = new UnitTests.XsdTypeGenUnitTest.CsTypeFactory(schemaSet);
            factory.NameResolver.RootCodeNamespace = "Temp.Unit.Test";
            using (StringWriter writer = new StringWriter())
            {
                factory.WriteCsCode(writer, name); 
            }
        }

        public class TypeGenNameResolver
        {
            private Dictionary<string, Tuple<string, bool>> _uriNamespaceMappings = new Dictionary<string, Tuple<string, bool>>();
            private Dictionary<XmlQualifiedName, string> _qualifiedNameClassMappings = new Dictionary<XmlQualifiedName, string>();
            private string _rootCodeNamespace = "";

            public string RootCodeNamespace
            {
                get { return _rootCodeNamespace; }
                set { _rootCodeNamespace = value ?? ""; }
            }

            public void AddNamsespace(string uri, string codeNamespace, bool isAbsolute = false)
            {
                lock (_uriNamespaceMappings)
                    _uriNamespaceMappings.Add(uri ?? "", new Tuple<string, bool>(codeNamespace ?? "", isAbsolute));
            }

            public void AddClassName(XmlQualifiedName qualifiedName, string className)
            {
                lock (_qualifiedNameClassMappings)
                    _qualifiedNameClassMappings[qualifiedName ?? new XmlQualifiedName()] = className ?? "";
            }

            public static readonly Regex InvalidTypeNameRegex = new Regex(@"^\d|[^a-z\d_]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            public static readonly Regex UrnPartsRegex = new Regex(@"^(<ignore>[^:]+:/*)(?<p>[^#?]*)(\?(?<q>[^#]*))?(#(?<f>.*))?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            public static string ValidateName(string name)
            {
                if (String.IsNullOrEmpty(name))
                    return "";
                return InvalidTypeNameRegex.Replace(name, m => "_0x" + ((int)(m.Value[0])).ToString("x4") + "_");
            }

            public static string ValidateNamespace(string rootNamespace, params string[] subNamespaces)
            {
                IEnumerable<string> ns = new string[] { rootNamespace };
                if (subNamespaces != null)
                    ns = ns.Concat(subNamespaces);
                return String.Join(".", ns.Where(s => !String.IsNullOrEmpty(s)).SelectMany(s => s.Split('.')).Where(s => s.Length > 0).Select(s => InvalidTypeNameRegex.Replace(s, m => "_0x" + ((int)(m.Value[0])).ToString("x4") + "_")));
            }
            
            public static string UrnToNs(string urn)
            {
                if (String.IsNullOrEmpty(urn))
                    return "";

                Match m = UrnPartsRegex.Match(urn);
                IEnumerable<string> names;
                if (m.Success)
                {
                    names = m.Groups["p"].Value.Split('/').SelectMany(s => s.Split('\\'));
                    if (m.Groups["q"].Success)
                        names = names.Concat(m.Groups["q"].Value.Split('&').SelectMany(s => s.Split('=')));
                    if (m.Groups["f"].Success)
                        names = names.Concat(m.Groups["f"].Value.Split('/').SelectMany(s => s.Split('\\')).SelectMany(s => s.Split('&')).SelectMany(s => s.Split('=')));
                }
                else
                    names = urn.Split(':').SelectMany(s => s.Split('/')).SelectMany(s => s.Split('\\')).SelectMany(s => s.Split('&')).SelectMany(s => s.Split('='));

                names = names.Where(s => s.Length > 0).Select(s => Uri.UnescapeDataString(s)).DefaultIfEmpty("");
                return ValidateNamespace(names.First(), names.Skip(1).ToArray());
            }

            public string GetRelativeName(XmlQualifiedName qualifiedName, string currentCodeNamespace)
            {
                string ns = GetNamespace(qualifiedName);
                if (ns.Length == 0 || ns == currentCodeNamespace)
                    return GetTypeName(qualifiedName);

                if (currentCodeNamespace.Length > 0)
                {
                    string[] tn = ns.Split('.');
                    string[] cn = currentCodeNamespace.Split('.');
                    for (int i = 0; i < ns.Length && i < currentCodeNamespace.Length; i++)
                    {
                        if (tn[i] != cn[i])
                        {
                            if (i > 0)
                                ns = String.Join(".", cn.Skip(i));
                            break;
                        }
                    }
                }

                return ns + "." + GetTypeName(qualifiedName);
            }

            public string GetTypeName(XmlQualifiedName qualifiedName)
            {
                string name;
                lock (_qualifiedNameClassMappings)
                {
                    if (qualifiedName == null)
                        qualifiedName = new XmlQualifiedName();
                    if (_qualifiedNameClassMappings.ContainsKey(qualifiedName))
                        name = _qualifiedNameClassMappings[qualifiedName];
                    else
                    {
                        name = (qualifiedName.IsEmpty) ? "" : qualifiedName.Name;
                        _qualifiedNameClassMappings.Add(qualifiedName, name);
                    }
                }

                return ValidateName(name);
            }

            public string GetNamespace(string uri)
            {
                Tuple<string, bool> ns;
                lock (_uriNamespaceMappings)
                {
                    if (_uriNamespaceMappings.ContainsKey(uri))
                        ns = _uriNamespaceMappings[uri];
                    else
                    {
                        ns = new Tuple<string, bool>(UrnToNs(uri), false);
                        _uriNamespaceMappings.Add(uri, ns);
                    }
                }
                if (ns.Item2)
                    return ValidateNamespace(ns.Item1);
                return ValidateNamespace(RootCodeNamespace, ns.Item1);
            }

            public string GetNamespace(XmlQualifiedName qualifiedName)
            {
                return GetNamespace((qualifiedName == null || qualifiedName.IsEmpty) ? "" : qualifiedName.Namespace);
            }
        }

        public class CsTypeFactory
        {
            private TypeGenNameResolver _nameResolver = new TypeGenNameResolver();

            public XmlSchemaSet SchemaSet { get; private set; }

            public TypeGenNameResolver NameResolver { get { return _nameResolver; } set { _nameResolver = value ?? new TypeGenNameResolver(); } }

            public CsTypeFactory(XmlSchemaSet schemaSet)
            {
                if (schemaSet == null)
                    throw new ArgumentNullException("schemaSet");
                if (!schemaSet.IsCompiled)
                    throw new ArgumentException("Schema set is not compiled.", "schemaSet");

                SchemaSet = schemaSet;
            }

            public class QualifiedNameComparer : IComparer<XmlQualifiedName>, IEqualityComparer<XmlQualifiedName>
            {
                private static QualifiedNameComparer _default = null;
                public static QualifiedNameComparer Default
                {
                    get
                    {
                        if (_default == null)
                            _default = new QualifiedNameComparer();
                        return _default;
                    }
                }

                public int Compare(XmlQualifiedName x, XmlQualifiedName y)
                {
                    if (x == null)
                        return (y == null) ? 0 : -1;

                    if (y == null)
                        return 1;

                    if (ReferenceEquals(x, y))
                        return 0;

                    if (x.IsEmpty)
                        return (y.IsEmpty) ? 0 : -1;

                    if (y.IsEmpty)
                        return 1;

                    int c = x.Namespace.CompareTo(y.Namespace);
                    if (c == 0)
                        return x.Name.CompareTo(y.Name);

                    return c;
                }

                public bool Equals(XmlQualifiedName x, XmlQualifiedName y)
                {
                    if (x == null)
                        return y == null;

                    if (y == null)
                        return false;

                    if (ReferenceEquals(x, y))
                        return true;

                    if (x.IsEmpty)
                        return y.IsEmpty;

                    if (y.IsEmpty)
                        return false;

                    return x.Namespace.Equals(y.Namespace) && x.Name.Equals(y.Name);
                }

                public int GetHashCode(XmlQualifiedName obj)
                {
                    return ((obj == null || obj.IsEmpty) ? "" : obj.ToString()).GetHashCode();
                }
            }

            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaComplexContentExtension obj)
            {
                FillGlobalObjectsToRender(globalObjects, obj.Attributes);
                if (obj.Particle != null)
                    FillGlobalObjectsToRender(globalObjects, obj.Particle);
                if (obj.BaseTypeName != null && !obj.BaseTypeName.IsEmpty)
                    FillGlobalObjectsToRender(globalObjects, obj.BaseTypeName);
            }

            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaComplexContentRestriction obj)
            {
                FillGlobalObjectsToRender(globalObjects, obj.Attributes);
                if (obj.Particle != null)
                    FillGlobalObjectsToRender(globalObjects, obj.Particle);
                if (obj.BaseTypeName != null && !obj.BaseTypeName.IsEmpty)
                    FillGlobalObjectsToRender(globalObjects, obj.BaseTypeName);
            }

            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaSimpleContentExtension obj)
            {
                FillGlobalObjectsToRender(globalObjects, obj.Attributes);
                if (obj.BaseTypeName == null || obj.BaseTypeName.IsEmpty)
                    throw new NotImplementedException();

                FillGlobalObjectsToRender(globalObjects, obj.BaseTypeName);
            }

            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaSimpleContentRestriction obj)
            {
                FillGlobalObjectsToRender(globalObjects, obj.Attributes);
                if (obj.BaseTypeName != null && !obj.BaseTypeName.IsEmpty)
                {
                    if (obj.BaseType != null)
                        throw new NotImplementedException();
                    FillGlobalObjectsToRender(globalObjects, obj.BaseTypeName);
                }
                else
                {
                    if (obj.BaseType == null)
                        throw new NotImplementedException();
                    FillGlobalObjectsToRender(globalObjects, obj.BaseType);
                }
            }
            
            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaGroupBase obj)
            {
                FillGlobalObjectsToRender(globalObjects, obj.Items);
            }

            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaGroupRef obj)
            {
                if (obj.RefName == null || obj.RefName.IsEmpty)
                    throw new NotImplementedException();
                FillGlobalObjectsToRender(globalObjects, obj.RefName);
            }

            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaElement obj)
            {
                if (obj.RefName != null && !obj.RefName.IsEmpty)
                {
                    if (obj.SchemaType != null)
                        throw new NotImplementedException();
                    FillGlobalObjectsToRender(globalObjects, obj.RefName);
                }
                else if (obj.SchemaTypeName != null && !obj.SchemaTypeName.IsEmpty)
                {
                    if (obj.SchemaType != null)
                        throw new NotImplementedException();
                    FillGlobalObjectsToRender(globalObjects, obj.SchemaTypeName);
                }
                else if (obj.SchemaType != null)
                    FillGlobalObjectsToRender(globalObjects, obj.SchemaType);
            }

            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaAttribute obj)
            {
                if (obj.RefName != null && !obj.RefName.IsEmpty)
                {
                    if (obj.SchemaType != null)
                        throw new NotImplementedException();
                    FillGlobalObjectsToRender(globalObjects, obj.RefName);
                }
                else if (obj.SchemaTypeName != null && !obj.SchemaTypeName.IsEmpty)
                {
                    if (obj.SchemaType != null)
                        throw new NotImplementedException();
                    FillGlobalObjectsToRender(globalObjects, obj.SchemaTypeName);
                }
                else
                {
                    if (obj.SchemaType == null)
                        throw new NotImplementedException();
                    FillGlobalObjectsToRender(globalObjects, obj.SchemaType);
                }
            }

            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaComplexType obj)
            {
                FillGlobalObjectsToRender(globalObjects, obj.Attributes);
                if (obj.ContentModel != null)
                    FillGlobalObjectsToRender(globalObjects, obj.ContentModel);
                if (obj.Particle != null)
                    FillGlobalObjectsToRender(globalObjects, obj.Particle);
            }

            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaSimpleType obj)
            {
                if (obj.Content != null)
                    FillGlobalObjectsToRender(globalObjects, obj.Content);
            }

            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaAttributeGroup obj)
            {
                if (obj.RedefinedAttributeGroup != null)
                    throw new NotImplementedException();

                FillGlobalObjectsToRender(globalObjects, obj.Attributes);
            }

            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaAttributeGroupRef obj)
            {
                if (obj.RefName == null || obj.RefName.IsEmpty)
                    throw new NotImplementedException();

                FillGlobalObjectsToRender(globalObjects, obj.RefName);
            }

            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaContent obj)
            {
                if (obj is XmlSchemaSimpleContentExtension)
                    FillGlobalObjectsToRender(globalObjects, (XmlSchemaSimpleContentExtension)obj);
                else if (obj is XmlSchemaSimpleContentRestriction)
                    FillGlobalObjectsToRender(globalObjects, (XmlSchemaSimpleContentRestriction)obj);
                else if (obj is XmlSchemaComplexContentExtension)
                    FillGlobalObjectsToRender(globalObjects, (XmlSchemaComplexContentExtension)obj);
                else
                    FillGlobalObjectsToRender(globalObjects, (XmlSchemaComplexContentRestriction)obj);
            }

            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaContentModel obj)
            {
                if (obj.Content == null)
                    throw new NotImplementedException();
                FillGlobalObjectsToRender(globalObjects, obj.Content);
            }

            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaGroup obj)
            {
                if (obj.Particle == null)
                    throw new NotImplementedException();
                FillGlobalObjectsToRender(globalObjects, obj.Particle);
            }
            
            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaParticle obj)
            {
                if (obj is XmlSchemaAny)
                    return;

                if (obj is XmlSchemaGroupBase)
                    FillGlobalObjectsToRender(globalObjects, (XmlSchemaGroupBase)obj);
                else if (obj is XmlSchemaGroupRef)
                    FillGlobalObjectsToRender(globalObjects, (XmlSchemaGroupRef)obj);
                else
                    FillGlobalObjectsToRender(globalObjects, (XmlSchemaElement)obj);
            }

            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaType obj)
            {
                if (obj is XmlSchemaSimpleType)
                    FillGlobalObjectsToRender(globalObjects, (XmlSchemaSimpleType)obj);
                else
                    FillGlobalObjectsToRender(globalObjects, (XmlSchemaComplexType)obj);
            }

            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaSimpleTypeList obj)
            {
                if (obj.ItemTypeName != null && !obj.ItemTypeName.IsEmpty)
                {
                    if (obj.ItemType != null)
                        throw new NotImplementedException();
                    FillGlobalObjectsToRender(globalObjects, obj.ItemTypeName);
                }
                else
                {
                    if (obj.ItemType == null)
                        throw new NotImplementedException();
                    FillGlobalObjectsToRender(globalObjects, obj.ItemType);
                }
            }

            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaSimpleTypeRestriction obj)
            {
                if (obj.BaseTypeName != null && !obj.BaseTypeName.IsEmpty)
                {
                    if (obj.BaseType != null)
                        throw new NotImplementedException();
                    FillGlobalObjectsToRender(globalObjects, obj.BaseTypeName);
                }
                else
                {
                    if (obj.BaseType == null)
                        throw new NotImplementedException();
                    FillGlobalObjectsToRender(globalObjects, obj.BaseType);
                }
            }

            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaSimpleTypeUnion obj)
            {
                foreach (XmlSchemaSimpleType t in obj.BaseMemberTypes)
                    FillGlobalObjectsToRender(globalObjects, t);

                foreach (XmlQualifiedName n in obj.MemberTypes)
                    FillGlobalObjectsToRender(globalObjects, n);
            }

            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaSimpleTypeContent obj)
            {
                if (obj is XmlSchemaSimpleTypeList)
                    FillGlobalObjectsToRender(globalObjects, (XmlSchemaSimpleTypeList)obj);
                else if (obj is XmlSchemaSimpleTypeRestriction)
                    FillGlobalObjectsToRender(globalObjects, (XmlSchemaSimpleTypeRestriction)obj);
                else
                    FillGlobalObjectsToRender(globalObjects, (XmlSchemaSimpleTypeUnion)obj);
            }

            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaObject obj)
            {
                if (obj is XmlSchemaAttribute)
                    FillGlobalObjectsToRender(globalObjects, obj as XmlSchemaAttribute);
                else if (obj is XmlSchemaAttributeGroup)
                    FillGlobalObjectsToRender(globalObjects, obj as XmlSchemaAttributeGroup);
                else if (obj is XmlSchemaAttributeGroupRef)
                    FillGlobalObjectsToRender(globalObjects, obj as XmlSchemaAttributeGroupRef);
                else if (obj is XmlSchemaContent)
                    FillGlobalObjectsToRender(globalObjects, obj as XmlSchemaContent);
                else if (obj is XmlSchemaContentModel)
                    FillGlobalObjectsToRender(globalObjects, obj as XmlSchemaContentModel);
                else if (obj is XmlSchemaGroup)
                    FillGlobalObjectsToRender(globalObjects, obj as XmlSchemaGroup);
                else if (obj is XmlSchemaParticle)
                    FillGlobalObjectsToRender(globalObjects, obj as XmlSchemaParticle);
                else if (obj is XmlSchemaType)
                    FillGlobalObjectsToRender(globalObjects, obj as XmlSchemaType);
                else if (obj is XmlSchemaSimpleTypeContent)
                    FillGlobalObjectsToRender(globalObjects, obj as XmlSchemaSimpleTypeContent);
                else
                    throw new NotImplementedException("Handling of type " + obj.GetType().FullName + " not implemented");
            }

            private void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlSchemaObjectCollection objects)
            {
                if (objects == null)
                    return;

                foreach (XmlSchemaObject obj in objects)
                    FillGlobalObjectsToRender(globalObjects, obj);
            }

            public XmlSchemaObject Find(XmlQualifiedName name)
            {
                if (SchemaSet.GlobalTypes.Contains(name))
                    return SchemaSet.GlobalTypes[name];

                if (SchemaSet.GlobalElements.Contains(name))
                    return SchemaSet.GlobalElements[name];

                if (SchemaSet.GlobalAttributes.Contains(name))
                    return SchemaSet.GlobalAttributes[name];

                foreach (XmlSchema schema in SchemaSet.Schemas())
                {
                    if (schema.AttributeGroups.Contains(name))
                        return schema.AttributeGroups[name];
                    if (schema.Groups.Contains(name))
                        return schema.Groups[name];
                }

                return null;
            }

            public const string NamespaceURI_XMLSchema = "http://www.w3.org/2001/XMLSchema";

            public void FillGlobalObjectsToRender(Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects, XmlQualifiedName name)
            {
                if (name.Namespace == NamespaceURI_XMLSchema || globalObjects.ContainsKey(name))
                    return;

                XmlSchemaObject obj = Find(name);
                if (obj == null)
                    throw new KeyNotFoundException("Lookup failed for " + name.ToString());

                globalObjects.Add(name, obj);
                FillGlobalObjectsToRender(globalObjects, obj);
            }

            public void WriteCsCode(StringWriter writer, XmlQualifiedName name)
            {
                Dictionary<XmlQualifiedName, XmlSchemaObject> globalObjects = new Dictionary<XmlQualifiedName, XmlSchemaObject>(new QualifiedNameComparer());
                FillGlobalObjectsToRender(globalObjects, name);

                foreach (var nsGroup in globalObjects.Keys.Select(k => new
                {
                    Name = k,
                    Obj = globalObjects[k]
                }).GroupBy(k => k.Name.Namespace))
                {
                    string codeNamespace = NameResolver.GetNamespace(nsGroup.Key);
                    string indent;
                    if (codeNamespace.Length == 0)
                        indent = "";
                    else
                    {
                        indent = "\t";
                        writer.WriteLine("namespace " + codeNamespace);
                        writer.WriteLine("{");
                    }

                    foreach (XmlSchemaType type in nsGroup.OfType<XmlSchemaType>())
                        WriteCsCode(writer, type, indent);

                    foreach (XmlSchemaElement element in nsGroup.OfType<XmlSchemaElement>())
                        WriteCsCode(writer, element, codeNamespace, indent);

                    if (codeNamespace.Length > 0)
                        writer.WriteLine("}");
                }
            }

            private void WriteCsCode(StringWriter writer, XmlSchemaElement element, string codeNamespace, string indent)
            {
                writer.WriteLine("");
                if (element.RefName != null && !element.RefName.IsEmpty)
                {
                    writer.WriteLine(indent + "public class " + NameResolver.GetTypeName(element.QualifiedName) + " : " + NameResolver.GetRelativeName(element.RefName, codeNamespace) + " { }");
                    return;
                }
                writer.WriteLine(indent + "public class " + NameResolver.GetTypeName(element.QualifiedName));
                writer.WriteLine(indent + "{");
                indent += "\t";
                writer.WriteLine(indent + "}");
                throw new NotImplementedException();
            }

            private void WriteCsCode(StringWriter writer, XmlSchemaType type, string indent)
            {
                throw new NotImplementedException();
            }
        }
    }
}

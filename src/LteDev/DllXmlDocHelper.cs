using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LteDev
{
    ///// <summary>
    ///// Helper class for extracting information from a DLL Help file.
    ///// </summary>
    //public static class DllXmlDocHelper
    //{
    //    public const string AttributeName_name = "name";
    //    public const string AttributeName_cref = "cref";
    //    public const string AttributeName_type = "type";
    //    public const string AttributeName_file = "file";
    //    public const string AttributeName_path = "path";
    //    public const string ElementName_doc = "doc";
    //    public const string ElementName_members = "members";
    //    public const string ElementName_member = "member";
    //    public const string ElementName_summary = "summary";
    //    public const string ElementName_description = "description";
    //    public const string ElementName_remarks = "remarks";
    //    public const string ElementName_see = "see";
    //    public const string ElementName_seeaso = "seealso";
    //    public const string ElementName_c = "c";
    //    public const string ElementName_exception = "exception";
    //    public const string ElementName_param = "param";
    //    public const string ElementName_paramref = "paramref";
    //    public const string ElementName_para = "para";
    //    public const string ElementName_code = "code";
    //    public const string ElementName_list = "list";
    //    public const string ElementName_term = "term";
    //    public const string ElementName_item = "item";
    //    public const string ElementName_example = "example";
    //    public const string ElementName_permission = "permission";
    //    public const string ElementName_typeparam = "typeparam";
    //    public const string ElementName_include = "include";
    //    public const string ElementName_typeparamref = "typeparamref";
    //    public const string ElementName_returns = "returns";
    //    public const string ElementName_value = "value";
    //    public const string ListType_bullet = "bullet";
    //    public const string ListType_number = "number";
    //    public const string ListType_table = "table";
    //    public const string Prefix_TypeDef = "T:";
    //    public const string Prefix_Property = "P:";
    //    public const string Prefix_Const = "F:";
    //    public const string Prefix_Method = "M:";

    //    /// <summary>
    //    /// Loads Source Code XML Documentation from assembly location.
    //    /// </summary>
    //    /// <param name="assembly">Assembly whose location may also contain a Source Code XML Documentation file.</param>
    //    /// <returns>An <seealso cref="XDocument"/> containing the Source Code XML Documentation if found at the same location as the <paramref name="assembly"/>; otherwise returns a <c>null</c> value.</returns>
    //    public static XDocument GetHelpDoc(this Assembly assembly)
    //    {
    //        try
    //        {
    //            string path;
    //            if (assembly == null || String.IsNullOrEmpty(path = assembly.Location))
    //                return null;

    //            path = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + ".XML");
    //            return (File.Exists(path)) ? null : XDocument.Load(path);
    //        }
    //        catch { return null; }
    //    }

    //    /// <summary>
    //    /// Gets Source Code XML Documentation for a custom type, typically a <seealso cref="System.Management.Automation.PSCmdlet"/> type.
    //    /// </summary>
    //    /// <param name="helpDoc"></param>
    //    /// <param name="type"></param>
    //    /// <returns></returns>
    //    public static XElement GetCommandElement(this XDocument helpDoc, Type type)
    //    {
    //        if (helpDoc == null || type == null)
    //            return null;

    //        string name = Prefix_TypeDef + type.FullName;
    //        XAttribute attribute = helpDoc.Elements(ElementName_doc).Elements(ElementName_members).Elements(ElementName_member)
    //            .Attributes(AttributeName_name).FirstOrDefault(a => a.Value == name);
    //        if (attribute == null)
    //            return null;
    //        return attribute.Parent;
    //    }

    //    public static IEnumerable<XElement> GetPropertyElements(this XDocument helpDoc, Type type)
    //    {
    //        if (helpDoc == null || type == null)
    //            return null;

    //        string name = Prefix_Property + type.FullName + ".";
    //        return helpDoc.Elements(ElementName_doc).Elements(ElementName_members).Elements(ElementName_member)
    //            .Attributes(AttributeName_name).Where(a => a.Value.StartsWith(name)).Select(a => a.Parent);
    //    }

    //    public static XElement GetPropertyElement(this XDocument helpDoc, Type type, string name)
    //    {
    //        if (helpDoc == null || type == null || String.IsNullOrEmpty(name))
    //            return null;

    //        name = Prefix_Property + type.FullName + "." + name;
    //        XAttribute attribute = helpDoc.Elements(ElementName_doc).Elements(ElementName_members).Elements(ElementName_member)
    //            .Attributes(AttributeName_name).FirstOrDefault(a => a.Value == name);

    //        return attribute.Parent;
    //    }

    //    public static XElement GetPropertyElement(this XDocument helpDoc, PropertyInfo property)
    //    {
    //        if (helpDoc == null || property == null)
    //            return null;

    //        string name = Prefix_Property + property.ReflectedType.FullName + "." + property.Name;
    //        XAttribute attribute = helpDoc.Elements(ElementName_doc).Elements(ElementName_members).Elements(ElementName_member)
    //            .Attributes(AttributeName_name).FirstOrDefault(a => a.Value == name);
    //        if (attribute == null)
    //        {
    //            name = Prefix_Property + property.DeclaringType.FullName + "." + property.Name;
    //            attribute = helpDoc.Elements(ElementName_doc).Elements(ElementName_members).Elements(ElementName_member)
    //                .Attributes(AttributeName_name).FirstOrDefault(a => a.Value == name);
    //            if (attribute == null)
    //                return null;
    //        }

    //        return attribute.Parent;
    //    }

    //    /// <summary>
    //    /// Gets command summary converted to PSMaml.
    //    /// </summary>
    //    /// <param name="implementingType">Implementing type for Cmdlet.</param>
    //    /// <returns>Elements which represent the command help summary.</returns>
    //    public static IEnumerable<XElement> GetCommandSummary(this XDocument helpDoc, Type implementingType)
    //    {
    //        XElement element = helpDoc.GetCommandElement(implementingType);
    //        if (element == null)
    //            yield break;
    //        foreach (XElement summaryElement in element.Elements(ElementName_summary))
    //        {
    //            List<XNode> nonBlockNodes = new List<XNode>();
    //            foreach (XNode node in summaryElement.Nodes())
    //            {
    //                if (node.IsHelpDocBlockNode())
    //                {
    //                    if (nonBlockNodes.Count > 0)
    //                    {
    //                        XElement para = new XElement(PSMamlHelper.XmlNs_maml.GetName(PSMamlHelper.ElementName_para));
    //                        foreach (XNode n in nonBlockNodes)
    //                            para.Add(n.HelpDocToMamlBlockContent());
    //                        yield return para;
    //                        nonBlockNodes.Clear();
    //                    }
    //                    foreach (XElement n in node.HelpDocToMamlBlockNodes())
    //                        yield return n;
    //                }
    //                else if (node.IsHelpDocContentNode())
    //                    nonBlockNodes.Add(node);
    //            }
    //            if (nonBlockNodes.Count > 0)
    //            {
    //                XElement para = new XElement(PSMamlHelper.XmlNs_maml.GetName(PSMamlHelper.ElementName_para));
    //                foreach (XNode n in nonBlockNodes)
    //                    para.Add(n.HelpDocToMamlBlockContent());
    //                yield return para;
    //            }
    //        }
    //    }

    //    public static bool IsHelpDocBlockNode(this XNode node)
    //    {
    //        if (node == null || !(node is XElement))
    //            return false;

    //        XElement element = node as XElement;
    //        if (element.Name.NamespaceName.Length == 0)
    //        {
    //            switch (element.Name.LocalName)
    //            {
    //                case ElementName_description:
    //                case ElementName_summary:
    //                case ElementName_exception:
    //                case ElementName_para:
    //                case ElementName_list:
    //                case ElementName_remarks:
    //                case ElementName_example:
    //                case ElementName_returns:
    //                case ElementName_value:
    //                    return true;
    //            }

    //            return false;
    //        }
    //        throw new NotImplementedException();
    //    }

    //    public static bool IsHelpDocContentNode(this XNode node)
    //    {
    //        if (node == null)
    //            return false;

    //        if (node is XText || node is XCData)
    //            return true;

    //        if (!(node is XElement))
    //            return false;

    //        XElement element = node as XElement;
    //        if (element.Name.NamespaceName.Length == 0)
    //        {
    //            switch (element.Name.LocalName)
    //            {
    //                case ElementName_description:
    //                case ElementName_summary:
    //                case ElementName_exception:
    //                case ElementName_para:
    //                case ElementName_list:
    //                case ElementName_remarks:
    //                case ElementName_example:
    //                case ElementName_returns:
    //                case ElementName_value:
    //                    return false;
    //            }

    //            return true;
    //        }

    //        throw new NotImplementedException();
    //    }

    //    public static XNode HelpDocToMamlBlockContent(this XNode node)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public static IEnumerable<XElement> HelpDocToMamlBlockNodes(this XNode node)
    //    {
    //        if (node == null)
    //            yield break;

    //        if (!node.IsHelpDocBlockNode())
    //        {
    //            if (node.IsHelpDocContentNode())
    //                yield return new XElement(PSMamlHelper.XmlNs_maml.GetName(PSMamlHelper.ElementName_para), node.HelpDocToMamlBlockContent());
    //            yield break;
    //        }

    //        XElement element = node as XElement;

    //        if (element.IsEmpty)
    //            yield break;

    //        XName name = element.GetPsMamlName();
    //        List<XNode> nonBlockNodes = new List<XNode>();
    //        foreach (XNode childNode in element.Nodes())
    //        {
    //            if (childNode.IsHelpDocBlockNode())
    //            {
    //                if (nonBlockNodes.Count > 0)
    //                {
    //                    XElement para = new XElement(name);
    //                    foreach (XNode n in nonBlockNodes)
    //                        para.Add(n.HelpDocToMamlBlockContent());
    //                    yield return para;
    //                    nonBlockNodes.Clear();
    //                }
    //                foreach (XElement e in childNode.HelpDocToMamlBlockNodes())
    //                    yield return e;
    //            }
    //            else if (childNode.IsHelpDocContentNode())
    //                nonBlockNodes.Add(childNode);
    //        }
    //        if (nonBlockNodes.Count > 0)
    //        {
    //            XElement para = new XElement(name);
    //            foreach (XNode n in nonBlockNodes)
    //                para.Add(n.HelpDocToMamlBlockContent());
    //            yield return para;
    //        }
    //    }

    //    public static XName GetPsMamlName(this XElement element)
    //    {
    //        if (element.Name.NamespaceName.Length == 0)
    //        {
    //            switch (element.Name.LocalName)
    //            {
    //                case ElementName_description:
    //                case ElementName_summary:
    //                    return PSMamlHelper.XmlNs_maml.GetName(PSMamlHelper.ElementName_description);
    //                case ElementName_remarks:
    //                case ElementName_see:
    //                case ElementName_seeaso:
    //                case ElementName_c:
    //                case ElementName_exception:
    //                    return PSMamlHelper.XmlNs_command.GetName(PSMamlHelper.ElementName_nonTerminatingErrors);
    //                case ElementName_param:
    //                case ElementName_paramref:
    //                case ElementName_para:
    //                    return PSMamlHelper.XmlNs_maml.GetName(PSMamlHelper.ElementName_para);
    //                case ElementName_code:
    //                case ElementName_list:
    //                case ElementName_term:
    //                case ElementName_item:
    //                case ElementName_example:
    //                    return PSMamlHelper.XmlNs_command.GetName(PSMamlHelper.ElementName_example);
    //                case ElementName_permission:
    //                case ElementName_typeparam:
    //                case ElementName_include:
    //                case ElementName_typeparamref:
    //                case ElementName_returns:
    //                case ElementName_value:
    //                    break;
    //                default:
    //                    break;
    //            }
    //        }

    //        throw new NotImplementedException();
    //    }

    //    /// <summary>
    //    /// Gets command description converted to PSMaml.
    //    /// </summary>
    //    /// <param name="implementingType">Implementing type for Cmdlet.</param>
    //    /// <returns>Elements which represent the command help details.</returns>
    //    public static IEnumerable<XElement> GetCommandDescription(this XDocument helpDoc, Type implementingType)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}

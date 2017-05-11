using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UnitTests.ModuleConformance
{
    public static class ExtensionMethods
    {
        public const string Xmlns_msh = "http://msh";
        public const string Xmlns_maml = "http://schemas.microsoft.com/maml/2004/10";
        public const string Xmlns_command = "http://schemas.microsoft.com/maml/dev/command/2004/10";
        public const string Xmlns_dev = "http://schemas.microsoft.com/maml/dev/2004/10";
        public const string Xmlns_MSHelp = "http://msdn.microsoft.com/mshelp";
        public const string ElementName_helpItems = "helpItems";
        public const string ElementName_command = "command";
        public const string ElementName_details = "details";
        public const string ElementName_name = "name";
        public const string ElementName_description = "description";
        public const string ElementName_copyright = "copyright";
        public const string ElementName_para = "para";
        public const string ElementName_verb = "verb";
        public const string ElementName_noun = "noun";
        public const string ElementName_version = "version";
        public const string ElementName_ = "";

        public static IEnumerable<XmlElement> GetElements(this XmlElement parent, string localName, string ns)
        {
            if (parent == null || !parent.HasChildNodes)
                return null;

            return parent.ChildNodes.OfType<XmlElement>().Where(e => e.LocalName == localName && e.NamespaceURI == ns);
        }

        public static IEnumerable<XmlElement> GetElements(this XmlElement parent, string localName1, string ns1, string localName2, string ns2)
        {
            if (parent == null || !parent.HasChildNodes)
                return new XmlElement[0];

            return parent.GetElements(localName1, ns1).SelectMany(e => e.GetElements(localName2, ns2));
        }

        public static XmlElement GetSingleElement(this XmlElement parent, string localName1, string ns1, string localName2, string ns2)
        {
            return parent.GetElements(localName1, ns1, localName2, ns2).FirstOrDefault();
        }

        public static XmlElement GetSingleElement(this XmlElement parent, string localName, string ns)
        {
            return parent.GetElements(localName, ns).FirstOrDefault();
        }

        public static IEnumerable<string> GetText(this XmlElement parent, string localName1, string ns1, string localName2, string ns2)
        {
            return parent.GetElements(localName1, ns1, localName2, ns2).Where(e => !e.IsEmpty).Select(e => e.InnerText);
        }

        public static IEnumerable<string> GetText(this XmlElement parent, string localName, string ns)
        {
            return parent.GetElements(localName, ns).Where(e => !e.IsEmpty).Select(e => e.InnerText);
        }

        public static IEnumerable<string> GetNonEmptyText(this XmlElement parent, string localName1, string ns1, string localName2, string ns2)
        {
            return parent.GetText(localName1, ns1, localName2, ns2).Where(s => s.Trim().Length > 0);
        }

        public static IEnumerable<string> GetNonEmptyText(this XmlElement parent, string localName, string ns)
        {
            return parent.GetText(localName, ns).Where(s => s.Trim().Length > 0);
        }

        public static XmlElement GetHelpItemsElement(this XmlDocument xmlHelp)
        {
            XmlElement result;
            if (xmlHelp == null || (result = xmlHelp.DocumentElement) == null || result.LocalName != ElementName_helpItems || result.NamespaceURI != Xmlns_msh)
                return null;

            return result;
        }

        public static IEnumerable<CommandNode> GetCommandNodes(this XmlDocument xmlHelp)
        {
            return CommandNode.GetCommandNodes(xmlHelp);
        }

        public static CommandNode GetCommandNode(XmlDocument xmlHelp, string commandName)
        {
            return xmlHelp.GetCommandNodes().FirstOrDefault(n => n.Name == commandName);
        }
    }
}

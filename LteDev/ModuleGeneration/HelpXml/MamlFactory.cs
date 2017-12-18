using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LteDev.HelpXml
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    public class MamlFactory : PSHelpFactoryBase
    {
        public const string XmlNsUri_maml = "http://schemas.microsoft.com/maml/2004/10";
        public const string alertSet = "alertSet";
        public const string description = "description";
        public const string relatedLinks = "relatedLinks";
        public const string copyright = "copyright";
        public const string para = "para";
        public const string name = "name";

        public static readonly XNamespace XmlNs_maml = XNamespace.Get(XmlNsUri_maml);

        internal MamlFactory(PSHelpFactory psHelpFactory) : base(psHelpFactory) { }

        public static void EnsureParagraphContent(XElement textBlockElement, Func<XNode> getDefaultContent)
        {
            XElement element = textBlockElement.FirstChildElement();
            if (element == null)
            {
                element = new XElement(XmlNs_maml.GetName(para));
                textBlockElement.Add(element);
            }
            else if (element.Name.NamespaceName != XmlNsUri_maml || element.Name.LocalName != para || element.NextSiblingElement() != null || !element.IsEmpty)
                return;

            EnsureContent(element, getDefaultContent);
        }

        internal XElement CreateParagraph(params object[] content)
        {
            return new XElement(XmlNs_maml.GetName(para), content);
        }

        internal object CreateName(params object[] content)
        {
            return new XElement(XmlNs_maml.GetName(name), content);
        }

        internal IEnumerable<XElement> ConvertAssemblyHelp(XElement element, bool inlinesOnly = false)
        {
            throw new NotImplementedException();
        }

#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
    }
}

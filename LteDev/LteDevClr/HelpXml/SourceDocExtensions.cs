using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace LteDevClr.HelpXml
{
    public static class SourceDocExtensions
    {
        public const string NamespaceURI_Command = "http://schemas.microsoft.com/maml/dev/command/2004/10";
        public const string NamespaceURI_Maml = "http://schemas.microsoft.com/maml/2004/10";
        public const string NamespaceURI_Dev = "http://schemas.microsoft.com/maml/dev/2004/10";
        public const string NamespaceURI_MSHelp = "http://msdn.microsoft.com/mshelp";
        public const string XmlPrefix_xmlns = "xmlns";
        public const string XmlPrefix_command = "command";
        public const string XmlPrefix_maml = "maml";
        public const string XmlPrefix_dev = "dev";
        public const string XmlPrefix_MSHelp = "MSHelp";
        public const string ElementName_para = "para";
        public const string ElementName_name = "name";
        public const string ElementName_type = "type";
        public const string ElementName_uri = "uri";

        private static readonly byte[][] _tokens = new byte[][]
        {
            new byte[] {0xb7, 0x7a, 0x5c, 0x56, 0x19, 0x34, 0xe0, 0x89},
            new byte[] {0x31, 0xbf, 0x38, 0x56, 0xad, 0x36, 0x4e, 0x35},
            new byte[] {0xb0, 0x3f, 0x5f, 0x7f, 0x11, 0xd5, 0x0a, 0x3a}
        };

        public static bool IsFrameworkType(this Type type)
        {
            if (type == null)
                return false;

            byte[] publicKeyToken = type.Assembly.GetName().GetPublicKeyToken();

            if (publicKeyToken == null || publicKeyToken.Length != 8)
                return false;

            return _tokens.Any(t =>
            {
                for (int i=0; i< t.Length; i++)
                {
                    if (t[i] != publicKeyToken[i])
                        return false;
                }
                return true;
            });
        }

        public static IEnumerable<string> GetText(this XmlElement source)
        {
            if (source == null)
                return new string[0];

            if (source.IsEmpty)
            {
                IEnumerable<XmlAttribute> attributes = source.SelectNodes("@*").OfType<XmlAttribute>().Where(a => a.Value.Trim().Length > 0).ToArray();

                XmlAttribute m = attributes.FirstOrDefault(a => a.Name == "name");
                if (m != null)
                    return new string[] { m.Value };
                m = attributes.FirstOrDefault(a => a.Name == "cref");
                if (m != null)
                    return new string[] { m.Value };
                return new string[] { attributes.Select(a => a.Value).DefaultIfEmpty(source.LocalName).First() };
            }

            source.Normalize();

            return source.ChildNodes.OfType<XmlNode>().SelectMany(n => (n is XmlText || n is XmlCDataSection) ? new string[] { n.Value } : ((n is XmlElement) ? GetText(n as XmlElement) : new string[0]));
        }

        public static IEnumerable<string> GetText(this IEnumerable<XmlElement> source)
        {
            if (source == null)
                return new string[0];

            return source.SelectMany(s => s.GetText());
        }

        public static XmlElement AddCommandElement(this XmlElement parent, string localName)
        {
            return parent.AppendChild(parent.OwnerDocument.CreateElement(XmlPrefix_command, localName, NamespaceURI_Command)) as XmlElement;
        }

        public static XmlElement AddMamlElement(this XmlElement parent, string localName)
        {
            return parent.AppendChild(parent.OwnerDocument.CreateElement(XmlPrefix_maml, localName, NamespaceURI_Maml)) as XmlElement;
        }

        public static XmlElement AddDevElement(this XmlElement parent, string localName)
        {
            return parent.AppendChild(parent.OwnerDocument.CreateElement(XmlPrefix_dev, localName, NamespaceURI_Dev)) as XmlElement;
        }

        public static XmlElement AddMSHelpElement(this XmlElement parent, string localName)
        {
            return parent.AppendChild(parent.OwnerDocument.CreateElement(XmlPrefix_MSHelp, localName, NamespaceURI_MSHelp)) as XmlElement;
        }

        public static IEnumerable<XmlElement> AddParagraphElements(this XmlElement parent, IEnumerable<string> text)
        {
            if (text == null)
                yield break;

            foreach (string s in text)
            {
                XmlElement element = parent.AddMamlElement(ElementName_para);
                if (s == null)
                    element.IsEmpty = true;
                else
                    element.InnerText = s.NormalizeWhitespace();
                yield return element;
            }
        }

        public static IEnumerable<XmlElement> AddParagraphElements(this XmlElement parent, params string[] text)
        {
            return parent.AddParagraphElements(text as IEnumerable<string>);
        }

        public static IEnumerable<XmlElement> AddParagraphElements(this XmlElement parent, XmlElement sourceElement, string placeholderComment)
        {
            IEnumerable<string> paragraphs = sourceElement.GetText();
            if (paragraphs.Any())
                return AddParagraphElements(parent, paragraphs);

            if (placeholderComment == null)
                return new XmlElement[0];

            return parent.AddParagraphElements(null as string).Select(e =>
            {
                e.AppendChild(parent.OwnerDocument.CreateComment(placeholderComment));
                return e;
            });
        }

        public static IEnumerable<XmlElement> AddParagraphElements(this XmlElement parent, XmlNodeList sourceNodes, string placeholderComment)
        {
            if (sourceNodes == null || sourceNodes.Count == 0)
                return parent.AddParagraphElements(null as XmlElement, placeholderComment);

            return parent.AddParagraphElements(sourceNodes.OfType<XmlNode>().SelectMany(n => (n is XmlAttribute || n is XmlCDataSection || n is XmlText) ? new string[] { n.Value } : ((n is XmlElement) ? GetText(n as XmlElement) : new string[] { n.InnerText })));
        }

        public static XmlElement AddDevType(this XmlElement parent, PSTypeName type)
        {
            XmlElement d = parent.AddDevElement(ElementName_type);
            d.AddMamlElement(ElementName_name).InnerXml = type.Name;
            if (type.Type.IsFrameworkType())
                d.AddMamlElement(ElementName_uri).InnerXml = "https://msdn.microsoft.com/en-us/library/" + type.Type.FullName.ToLower() + ".aspx";
            return d;
        }
        public static readonly Regex MultiWhitespaceRegex = new Regex(@"[\p{C}\s][\p{C}\s]+|\p{C}+", RegexOptions.Compiled);

        public static IEnumerable<string> GetParagraphs(this XmlElement parent, XmlNamespaceManager nsmgr)
        {
            return parent.SelectNodes(XmlPrefix_maml + ":" + ElementName_para, nsmgr).OfType<XmlElement>().Where(e => !e.IsEmpty).Select(e => e.InnerText.Trim())
                .Where(s => s.Length > 0);
        }
        public static void SetParagraph(this XmlElement parent, XmlNamespaceManager nsmgr, string paragraph, string defaultComment = null)
        {
            parent.SetParagraphs(nsmgr, (paragraph == null) ? new string[0] : new string[] { paragraph }, defaultComment);
        }

        public static void SetParagraphs(this XmlElement parent, XmlNamespaceManager nsmgr, IEnumerable<string> description, string defaultComment = null)
        {
            if (parent.GetParagraphs(nsmgr).Any())
                return;

            IEnumerable<XmlComment> comments = parent.SelectNodes(XmlPrefix_maml + ":" + ElementName_para + "/comment()", nsmgr).OfType<XmlComment>();

            description = description.NormalizeWhitespace();
            if (description.Any())
                parent.AddParagraphElements(description);
            else if (!String.IsNullOrWhiteSpace(defaultComment) && !comments.Any())
                parent.AddParagraphElements(null as string).First().AppendChild(parent.OwnerDocument.CreateComment(defaultComment));
        }

        public static string NormalizeWhitespace(this string source)
        {
            string s;
            if ((s = source) == null || s.Trim().Length == 0)
                return "";
            return MultiWhitespaceRegex.Replace(s, " ");
        }
        public static IEnumerable<string> NormalizeWhitespace(this IEnumerable<string> source)
        {
            if (source != null)
            {
                foreach (string s in source)
                    yield return s.NormalizeWhitespace();
            }
        }
    }
}
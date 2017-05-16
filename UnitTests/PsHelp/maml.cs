using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UnitTests.PsHelp
{
    public static class maml
    {
        public const string Xmlns_Prefix = "maml";
        public const string Xmlns_URI = "http://schemas.microsoft.com/maml/2004/10";
        public const string ElementName_description = "description";
        public const string ElementName_copyright = "copyright";
        public const string ElementName_para = "para";
        public const string ElementName_alertSet = "alertSet";
        public const string ElementName_relatedLinks = "relatedLinks";

        public static XmlElement AddMamlElement(this XmlNode node, string name, XmlNode insertBefore = null)
        {
            return node.AddElement(Xmlns_Prefix, name, Xmlns_URI, insertBefore);
        }

        public static IEnumerable<XmlElement> GetMamlElements(this XmlNode node)
        {
            return node.GetCurrentElement().GetChildElements().Where(e => e.NamespaceURI == Xmlns_URI);
        }

        internal static XmlElement GetCopyrightElement(XmlNode node)
        {
            throw new NotImplementedException();
        }

        internal static XmlElement GetDescriptionElement(XmlNode node)
        {
            throw new NotImplementedException();
        }

        internal static void SetEmptyMaml(XmlElement descriptionElement, string comment)
        {
            XmlElement para;
            if (!descriptionElement.IsEmpty)
            {
                IEnumerable<XmlNode> toRemove = descriptionElement.ChildNodes.OfType<XmlNode>();
                para = toRemove.OfType<XmlElement>().FirstOrDefault(n => n.LocalName == ElementName_para && n.NamespaceURI == Xmlns_URI);
                if (para != null)
                {
                    foreach (XmlNode node in toRemove.TakeWhile(n => !(n is XmlElement) || n.LocalName != ElementName_para || n.NamespaceURI != Xmlns_URI)
                        .Concat(toRemove.SkipWhile(n => !(n is XmlElement) || n.LocalName != ElementName_para || n.NamespaceURI != Xmlns_URI).Skip(1)).ToArray())
                        descriptionElement.RemoveChild(node);
                    if (!para.IsEmpty)
                    {
                        toRemove = para.ChildNodes.OfType<XmlNode>();
                        XmlComment cn = toRemove.OfType<XmlComment>().FirstOrDefault();
                        if (cn != null)
                        {
                            toRemove = toRemove.TakeWhile(n => !(n is XmlComment)).Concat(toRemove.SkipWhile(n => !(n is XmlComment)).Skip(1));
                            cn.InnerText = comment;
                        }

                        foreach (XmlNode n in toRemove.ToArray())
                            para.RemoveChild(n);

                        if (cn != null)
                            return;
                    }
                }
                else
                {
                    foreach (XmlNode n in toRemove.ToArray())
                        descriptionElement.RemoveChild(n);
                    para = descriptionElement.AddMamlElement(ElementName_para);
                }
            }
            else
                para = descriptionElement.AddMamlElement(ElementName_para);

            para.AppendChild(para.OwnerDocument.CreateComment(comment));
        }

        //public static void WriteMamlParagraphs(this XmlWriter writer, params string[] text)
        //{
        //    if (text == null || (text = text.Where(s => s != null).Select(s => s.Trim()).Where(s => s.Length > 0).ToArray()).Length == 0)
        //        return;

        //    string prefix = writer.LookupPrefix(ModuleConformance.ModuleValidator.XmlnsURI_maml);
        //    foreach (string s in text.Normalized())
        //        writer.WriteElementString(prefix, "para", ModuleConformance.ModuleValidator.XmlnsURI_maml, s);
        //}
    }
}

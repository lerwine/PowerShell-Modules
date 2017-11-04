using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace UnitTests
{
    public static class ExtensionMethods
    {
        public static readonly Regex WhiteSpaceRegex = new Regex(@"[\s\p{C}]", RegexOptions.Compiled);
        public static readonly Regex LineBreakRegex = new Regex(@"\r\n?\n", RegexOptions.Compiled);

        //        public static XmlWriter WriteElement(this XmlWriter writer, string prefix, string localName, string ns, Action action)
        //        {
        //            if (prefix == null)
        //                writer.WriteStartElement(localName, ns);
        //            else
        //                writer.WriteStartElement(prefix, localName, ns);
        //#if DEBUG
        //            if (System.Diagnostics.Debugger.IsAttached)
        //            {
        //                action();
        //                writer.WriteEndElement();
        //                return writer;
        //            }
        //#endif
        //            try
        //            {
        //                action();
        //                writer.WriteEndElement();
        //            }
        //            catch (Exception exception)
        //            {
        //                try { writer.WriteEndElement(); } catch { }
        //                if (exception is AssertFailedException)
        //                    throw;
        //                throw new AssertFailedException(String.Format("Unexpected exception while writing XML element: {0}", exception.Message), exception);
        //            }
        //            return writer;
        //        }

        //        public static XmlWriter WriteElement<TArg>(this XmlWriter writer, string prefix, string localName, string ns, TArg arg, Action<TArg> action)
        //        {
        //            return writer.WriteElement(prefix, localName, ns, () =>
        //            {
        //                action(arg);
        //            });
        //        }

        //        public static XmlWriter WriteElement<TArg1, TArg2>(this XmlWriter writer, string prefix, string localName, string ns, TArg1 arg1, TArg2 arg2, Action<TArg1, TArg2> action)
        //        {
        //            return writer.WriteElement(prefix, localName, ns, () =>
        //            {
        //                action(arg1, arg2);
        //            });
        //        }

        //        public static XmlWriter WriteElement<TArg1, TArg2, TArg3>(this XmlWriter writer, string prefix, string localName, string ns, TArg1 arg1, TArg2 arg2, TArg3 arg3, Action<TArg1, TArg2, TArg3> action)
        //        {
        //            return writer.WriteElement(prefix, localName, ns, () =>
        //            {
        //                action(arg1, arg2, arg3);
        //            });
        //        }

        //        public static XmlWriter WriteElement<TArg1, TArg2, TArg3, TArg4>(this XmlWriter writer, string prefix, string localName, string ns, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, Action<TArg1, TArg2, TArg3, TArg4> action)
        //        {
        //            return writer.WriteElement(prefix, localName, ns, () =>
        //            {
        //                action(arg1, arg2, arg3, arg4);
        //            });
        //        }

        public const string XmlnsPrefix_xmlns = "xmlns";
        public const string XmlnsURI_xmlns = "http://www.w3.org/XML/1998/namespace";

        public static void AddXmlnsAttribute(this XmlElement element, string prefix, string namespaceURI)
        {
            XmlAttribute attribute = element.GetAttributeNode(prefix, XmlnsURI_xmlns);
            if (attribute != null)
                element.Attributes.Append(element.OwnerDocument.CreateAttribute(XmlnsPrefix_xmlns, prefix, XmlnsURI_xmlns)).Value = namespaceURI;
        }

        public static XmlElement AddElement(this XmlNode node, string prefix, string localName, string namepsaceURI, XmlNode insertBefore = null)
        {
            XmlNode parent = node;
            while (parent != null && !(parent is XmlElement || parent is XmlDocument))
                parent = parent.ParentNode;

            XmlDocument ownerDocument;
            if (parent != null && parent is XmlDocument)
            {
                ownerDocument = parent as XmlDocument;
                if (ownerDocument.DocumentElement != null)
                    parent = ownerDocument.DocumentElement;
            }
            else
                ownerDocument = parent.OwnerDocument;

            if (insertBefore == null)
                return parent.AppendChild(ownerDocument.CreateElement(prefix, localName, namepsaceURI)) as XmlElement;

            return parent.InsertBefore(ownerDocument.CreateElement(prefix, localName, namepsaceURI), insertBefore) as XmlElement;
        }
        
        public static XmlAttribute GetNodeAttribute(this XmlNode node, string localName, string namespaceURI = "")
        {
            XmlElement element = node.GetCurrentElement();
            if (element == null)
                return null;
            return element.Attributes.OfType<XmlAttribute>().FirstOrDefault(a => a.LocalName == localName && a.NamespaceURI == namespaceURI);
        }

        public static void SetComment(this XmlNode node, string data)
        {
            XmlElement element = node.GetCurrentElement();
            XmlComment comment = element.ChildNodes.OfType<XmlComment>().FirstOrDefault();
            if (comment != null)
            {
                comment.InnerText = data;
                return;
            }

            if (element.IsEmpty)
                element.AppendChild(element.OwnerDocument.CreateComment(data));
            else
                element.InsertBefore(element.OwnerDocument.CreateComment(data), element.FirstChild);
        }

        public static void SetAttributeValue(this XmlNode node, string localName, string value)
        {
            node.SetAttributeValue(localName, "", value);
        }

        public static void SetAttributeValue(this XmlNode node, string localName, string namespaceURI, string value)
        {
            XmlElement element = node.GetCurrentElement();
            XmlAttribute attribute = element.GetNodeAttribute(localName, namespaceURI);
            if (attribute == null)
            {
                if (value != null)
                    element.Attributes.Append(element.OwnerDocument.CreateAttribute(localName, namespaceURI)).Value = value;
            }
            else if (value == null)
                element.Attributes.Remove(attribute);
            else
                attribute.Value = value;
        }

        public static void SetAttributeValue(this XmlNode node, string prefix, string localName, string namespaceURI, string value)
        {
            XmlElement element = node.GetCurrentElement();
            XmlAttribute attribute = element.GetNodeAttribute(localName, namespaceURI);
            if (attribute == null)
            {
                if (value != null)
                    element.Attributes.Append(element.OwnerDocument.CreateAttribute(prefix, localName, namespaceURI)).Value = value;
            }
            else if (value == null)
                element.Attributes.Remove(attribute);
            else
                attribute.Value = value;
        }

        public static XmlElement GetCurrentElement(this XmlNode node)
        {
            while (node != null && !(node is XmlElement || node is XmlDocument))
                node = node.ParentNode;

            if (node != null && node is XmlDocument)
                return (node as XmlDocument).DocumentElement;

            return node as XmlElement;
        }

        public static IEnumerable<XmlElement> GetChildElements(this XmlNode parent)
        {
            while (parent != null && !(parent is XmlElement || parent is XmlDocument))
                parent = parent.ParentNode;

            if (parent == null)
                return new XmlElement[0];


            XmlElement element;
            if (parent is XmlDocument)
            {
                element = (parent as XmlDocument).DocumentElement;
                if (element != null)
                    return new XmlElement[] { element };
                return new XmlElement[0];
            }

            element = parent as XmlElement;
            if (element.IsEmpty)
                return new XmlElement[0];

            return element.ChildNodes.OfType<XmlElement>();
        }

        public static IEnumerable<XmlElement> GetChildElements(this XmlNode parent, string localName, string namespaceURI = null)
        {
            IEnumerable<XmlElement> elements = parent.GetChildElements();

            if (namespaceURI == null)
            {
                if (String.IsNullOrEmpty(localName))
                    return elements;
                return elements.Where(e => e.LocalName == localName);
            }

            if (String.IsNullOrEmpty(localName))
                return elements.Where(e => e.NamespaceURI == namespaceURI);
            return elements.Where(e => e.LocalName == localName && e.NamespaceURI == namespaceURI);
        }

        public static IEnumerable<XmlElement> GetChildElements(this IEnumerable<XmlNode> elements)
        {
            if (elements == null)
                return new XmlElement[0];

            return elements.SelectMany(e => e.GetChildElements());
        }

        public static IEnumerable<XmlElement> GetChildElements(this IEnumerable<XmlNode> elements, string localName, string namespaceURI = null)
        {
            if (elements == null)
                return new XmlElement[0];

            return elements.SelectMany(e => e.GetChildElements(localName, namespaceURI));
        }

        public static IEnumerable<string> GetText(this XmlNode node, bool recursive = false)
        {
            if (node == null)
                return new string[0];

            if (node is XmlAttribute)
                return new string[] { node.Value };

            if (node is XmlCharacterData)
                return new string[] { node.InnerText };

            XmlElement element = node.GetCurrentElement();

            if (element == null || element.IsEmpty)
                return new string[0];

            if (!recursive)
                return (element.ChildNodes.OfType<XmlText>().Select(s => s.InnerText));

            return element.ChildNodes.OfType<XmlNode>().SelectMany(n => n.GetText(true));
        }

        public static bool HasText(this XmlNode node, bool recursive = false)
        {
            return node.GetText(recursive).Any(s => !String.IsNullOrWhiteSpace(s));
        }

        public static IEnumerable<string> Normalized(this IEnumerable<string> source)
        {
            if (source == null)
                yield break;

            foreach (string s in source)
            {
                string t = s.Normalized();
                if (t.Length > 0)
                    yield return t;
            }
        }

        public static string Normalized(this string source)
        {
            string t;
            if ((t = source ?? "").Length > 0 && (t = t.Trim()).Length > 0)
                return WhiteSpaceRegex.Replace(t, " ");
            return t;
        }

        public static IEnumerable<string> SplitLines(this string source)
        {
            if (source == null)
                yield break;

            if (source.Length == 0)
                yield return source;
            else
            {
                foreach (string l in LineBreakRegex.Split(source))
                    yield return l;
            }
        }

        public static IEnumerable<string> SplitLines(this IEnumerable<string> source)
        {
            if (source == null)
                return new string[0];

            return source.SelectMany(s => s.SplitLines());
        }

        public static IEnumerable<string> GetPropertyText(this PSObject obj, string name)
        {
            PSPropertyInfo propertyInfo = obj.GetMemberProperty(name);

            if (propertyInfo == null || !propertyInfo.IsGettable)
                return new string[0];

            return propertyInfo.AsStrings();
        }

        public static IEnumerable<PSObject> AsPsObjectEnumerable(this PSPropertyInfo property)
        {
            if (property == null || property.Value == null)
                return new PSObject[0];

            object value = property.Value;

            if (value is PSObject)
            {
                    PSObject obj = (PSObject)(value);
                    if (obj.BaseObject is IEnumerable<PSObject>)
                        return (IEnumerable<PSObject>)(obj.BaseObject);
                if (!(obj.BaseObject is IEnumerable))
                    return new PSObject[] { obj };
                value = obj.BaseObject;
            }

            if (value is IEnumerable<PSObject>)
                return (IEnumerable<PSObject>)(property.Value);

            if (value is IEnumerable && !(value is string))
                return (value as IEnumerable).Cast<object>().Select(o => (o == null || o is PSObject) ? o as PSObject : PSObject.AsPSObject(o));

            return new PSObject[] { PSObject.AsPSObject(value) };
        }

        public static IEnumerable<string> GetPropertyText(this PSPropertyInfo property, string name)
        {
            PSPropertyInfo propertyInfo = property.GetMemberProperty(name);

            if (propertyInfo == null || !propertyInfo.IsGettable)
                return new string[0];

            return propertyInfo.AsStrings();
        }

        public static IEnumerable<string> AsStrings(this PSPropertyInfo propertyInfo)
        {
            object obj;
            if (propertyInfo == null || (obj = propertyInfo.Value) == null)
                yield break;

            if (obj is PSObject)
            {
                object o = (obj as PSObject).BaseObject;
                if (o == null)
                    yield break;
                if (!(o is PSCustomObject))
                    obj = o;
            }
            else if (obj is PSCustomObject)
                obj = PSObject.AsPSObject(obj);

            if (obj is PSObject)
            {
                foreach (string s in (obj as PSObject).Properties.SelectMany(p => p.AsStrings()))
                    yield return s;
                yield break;
            }

            if (obj is string)
            {
                yield return obj as string;
                yield break;
            }

            if (obj is IEnumerable<string>)
            {
                foreach (string s in (obj as IEnumerable<string>))
                    yield return s;
                yield break;
            }

            if (obj is IEnumerable<char>)
            {
                yield return new string((obj as IEnumerable<char>).ToArray());
                yield break;
            }

            if (obj is IEnumerable)
            {
                foreach (object o in (obj as IEnumerable))
                {
                    if (o == null)
                        continue;

                    if (o is string)
                        yield return o as string;
                    else if (o is IEnumerable<string>)
                    {
                        foreach (string s in (o as IEnumerable<string>))
                            yield return s;
                    }
                    else if (o is IEnumerable<char>)
                        yield return new string((o as IEnumerable<char>).ToArray());
                    else
                        yield return o.ToString();
                }
            }
            else
                yield return obj.ToString();
        }

        public static PSPropertyInfo GetMemberProperty(this PSPropertyInfo property, string name)
        {
            object obj;
            if (property == null || (obj = property.Value) == null)
                return null;
            
            try { return ((obj is PSObject) ? obj as PSObject : PSObject.AsPSObject(obj)).Properties[name]; } catch { return null; }
        }

        public static PSPropertyInfo GetMemberProperty(this PSObject obj, string name)
        {
            if (obj == null)
                return null;
            try { return obj.Properties[name]; } catch { return null; }
        }

        public static IEnumerable<AliasInfo> GetAliases(this PSModuleInfo moduleInfo, string commandName)
        {
            return moduleInfo.ExportedAliases.Select(kvp => kvp.Value).Where(a => String.Equals(a.ReferencedCommand.Name, commandName, StringComparison.InvariantCultureIgnoreCase));
        }

        //public static void WriteFromElement(this XmlWriter writer, XmlElement element)
        //{
        //    writer.WriteElement(writer.LookupPrefix(element.NamespaceURI), element.LocalName, element.NamespaceURI, () =>
        //    {
        //        if (element.Attributes.Count > 0)
        //        {
        //            foreach (XmlAttribute attribute in element.Attributes)
        //            {
        //                if (attribute.NamespaceURI.Length == 0)
        //                    writer.WriteAttributeString(attribute.LocalName, attribute.Value);
        //                else
        //                {
        //                    string p = writer.LookupPrefix(attribute.NamespaceURI);
        //                    if (p == null)
        //                        writer.WriteAttributeString(attribute.LocalName, attribute.NamespaceURI, attribute.Value);
        //                    else
        //                        writer.WriteAttributeString(p, attribute.LocalName, attribute.NamespaceURI, attribute.Value);
        //                }
        //            }
        //        }
        //        if (!element.IsEmpty)
        //        {
        //            foreach (XmlNode node in element.ChildNodes)
        //            {
        //                if (node is XmlElement)
        //                    writer.WriteFromElement(node as XmlElement);
        //                else
        //                    node.WriteTo(writer);
        //            }
        //        }
        //    });
        //}
        


        public static IEnumerable<ParameterSetMetadata> GetInputParameterSets(this ParameterMetadata parameter)
        {
            return parameter.ParameterSets.Values.Where(s => s.ValueFromPipeline || s.ValueFromPipelineByPropertyName);
        }

        //public static void WriteTypeHelp(this Type type, PSObject helpInfo, XmlElement element, XmlWriter writer)
        //{
        //    string mamlPrefix = writer.LookupPrefix(ModuleConformance.ModuleValidator.XmlnsURI_maml);
        //    writer.WriteElement(writer.LookupPrefix(ModuleConformance.ModuleValidator.XmlnsURI_dev), "type", ModuleConformance.ModuleValidator.XmlnsURI_dev, () =>
        //    {
        //        writer.WriteElementString(mamlPrefix, "name", ModuleConformance.ModuleValidator.XmlnsURI_maml, type.FullName);
        //        string u = element.GetElements("uri", ModuleConformance.ModuleValidator.XmlnsURI_maml).Where(e => !e.IsEmpty).Select(e => e.InnerText.Trim()).FirstOrDefault(s => s.Length > 0);
        //        if (u == null)
        //            u = String.Format("https://msdn.microsoft.com/en-us/library/{0}.aspx", Uri.EscapeDataString(type.FullName.ToLower()));
        //        writer.WriteElementString(mamlPrefix, "uri", ModuleConformance.ModuleValidator.XmlnsURI_maml, u);
        //    });
        //}

        //public static void WriteTypeHelp(this PSTypeName typeName, PSObject helpInfo, XmlElement element, XmlWriter writer)
        //{
        //    writer.WriteElement(writer.LookupPrefix(ModuleConformance.ModuleValidator.XmlnsURI_dev), "type", ModuleConformance.ModuleValidator.XmlnsURI_dev, () =>
        //    {
        //        string mamlPrefix = writer.LookupPrefix(ModuleConformance.ModuleValidator.XmlnsURI_maml);
        //        string u = element.GetElements("uri", ModuleConformance.ModuleValidator.XmlnsURI_maml).Where(e => !e.IsEmpty).Select(e => e.InnerText.Trim()).FirstOrDefault(s => s.Length > 0);
        //        if (typeName.Type == null)
        //            writer.WriteElementString(mamlPrefix, "name", ModuleConformance.ModuleValidator.XmlnsURI_maml, typeName.Name);
        //        else
        //        {
        //            writer.WriteElementString(mamlPrefix, "name", ModuleConformance.ModuleValidator.XmlnsURI_maml, typeName.Type.FullName);
        //            if (u == null)
        //                u = String.Format("https://msdn.microsoft.com/en-us/library/{0}.aspx", Uri.EscapeDataString(typeName.Type.FullName.ToLower()));
        //        }
        //        if (u != null)
        //            writer.WriteElementString(mamlPrefix, "uri", ModuleConformance.ModuleValidator.XmlnsURI_maml, u);
        //    });
        //}
        
        public static string DefaultIfNullOrWhiteSpace(this string source, string defaultText)
        {
            if (String.IsNullOrWhiteSpace(source))
                return defaultText;

            return source.Trim();
        }

    }
}

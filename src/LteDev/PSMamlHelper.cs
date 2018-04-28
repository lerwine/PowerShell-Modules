using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace LteDev
{
    /// <summary>
    /// Helper class for generating PSMaml Help
    /// </summary>
    public static class PSMamlHelper
    {
        #region Constants

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member

        public const string HelpXml_FileName_Append = "-Help.ps1xml";
        public const string HelpInfo_FileName_Append = "_HelpInfo.xml";
        public const string XmlNsUri_msh = "http://msh";
        public const string XmlNsUri_command = "http://schemas.microsoft.com/maml/dev/command/2004/10";
        public const string XmlNsUri_maml = "http://schemas.microsoft.com/maml/2004/10";
        public const string XmlNsUri_dev = "http://schemas.microsoft.com/maml/dev/2004/10";
        public const string XmlNsUri_managed = "http://schemas.microsoft.com/maml/dev/managed/2004/10";
        public const string XmlNsUri_MSHelp = "http://msdn.microsoft.com/mshelp";
        public const string XmlNsUri_helpInfo = "http://schemas.microsoft.com/powershell/help/2010/05";
        public static readonly XNamespace XmlNs_msh = XNamespace.Get(XmlNsUri_msh);
        public static readonly XNamespace XmlNs_command = XNamespace.Get(XmlNsUri_command);
        public static readonly XNamespace XmlNs_maml = XNamespace.Get(XmlNsUri_maml);
        public static readonly XNamespace XmlNs_dev = XNamespace.Get(XmlNsUri_dev);
        public static readonly XNamespace XmlNs_managed = XNamespace.Get(XmlNsUri_managed);
        public static readonly XNamespace XmlNs_MSHelp = XNamespace.Get(XmlNsUri_MSHelp);
        public static readonly XNamespace XmlNs_helpInfo = XNamespace.Get(XmlNsUri_helpInfo);

        public const string ParameterSetName_PipelineableModule = "InputModule";
        public const string ParameterSetName_PositionedModule = "ModuleInfo";
        public const string ParameterSetName_Command = "CommandInfo";

        public const string VariableName_HelpItems = "HelpItems";

        public const string ElementName_helpItems = "helpItems";
        public const string ElementName_command = "command";
        public const string ElementName_details = "details";
        public const string ElementName_syntax = "syntax";
        public const string ElementName_parameters = "parameters";
        public const string ElementName_inputTypes = "inputTypes";
        public const string ElementName_inputType = "inputType";
        public const string ElementName_returnValues = "returnValues";
        public const string ElementName_returnValue = "returnValue";
        public const string ElementName_terminatingErrors = "terminatingErrors";
        public const string ElementName_terminatingError = "terminatingError";
        public const string ElementName_nonTerminatingErrors = "nonTerminatingErrors";
        public const string ElementName_nonTerminatingError = "nonTerminatingError";
        public const string ElementName_examples = "examples";
        public const string ElementName_example = "example";
        public const string ElementName_name = "name";
        public const string ElementName_synonyms = "synonyms";
        public const string ElementName_synonym = "synonym";
        public const string ElementName_verb = "verb";
        public const string ElementName_noun = "noun";
        public const string ElementName_vendor = "vendor";
        public const string ElementName_syntaxItem = "syntaxItem";
        public const string ElementName_version = "version";
        public const string ElementName_alertSet = "alertSet";
        public const string ElementName_description = "description";
        public const string ElementName_relatedLinks = "relatedLinks";
        public const string ElementName_copyright = "copyright";
        public const string ElementName_para = "para";
        public const string ElementName_list = "list";
        public const string ElementName_title = "title";
        public const string ElementName_alert = "alert";
        public const string ElementName_definitionList = "definitionList";
        public const string ElementName_definitionListItem = "definitionListItem";
        public const string ElementName_term = "term";
        public const string ElementName_definition = "definition";
        public const string ElementName_uri = "uri";
        public const string ElementName_table = "table";
        public const string ElementName_summary = "summary";
        public const string ElementName_introduction = "introduction";
        public const string ElementName_tableHeader = "tableHeader";
        public const string ElementName_row = "row";
        public const string ElementName_headerEntry = "headerEntry";
        public const string ElementName_entry = "entry";
        public const string ElementName_tableFooter = "tableFooter";
        public const string ElementName_footerEntry = "footerEntry";
        public const string ElementName_leadInPhrase = "leadInPhrase";
        public const string ElementName_errorInline = "errorInline";
        public const string ElementName_buildInstructions = "buildInstructions";
        public const string ElementName_robustProgramming = "robustProgramming";
        public const string ElementName_security = "security";
        public const string ElementName_results = "results";
        public const string ElementName_remarks = "remarks";
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member

        #endregion

        #region URI validation

        /// <summary>
        /// Ensures string values represent a valid absolute URI string.
        /// </summary>
        /// <param name="baseUriOrPath">The base URI or path.</param>
        /// <param name="relativePathOrUri">The relative path or URI to add to the base URI.</param>
        /// <param name="doNotResolveLocalPaths">If <c>true</c>, <seealso cref="Path.GetFullPath(string)"/> is not invoked for strings which represent local paths.</param>
        /// <returns>If <paramref name="baseUriOrPath"/> and <paramref name="relativePathOrUri"/> are both null or empty, an empty string is returned.
        /// Otherwise, <paramref name="baseUriOrPath"/>, with <paramref name="relativePathOrUri"/> appended, is returned as a valid absolute URI string.</returns>
        /// <exception cref="ArgumentException"><paramref name="baseUriOrPath"/> or <paramref name="relativePathOrUri"/> is not an absolute URI string and is not a format that can be converted to an absolute URI string.</exception>
        public static string ValidateUriString(string baseUriOrPath, string relativePathOrUri, bool doNotResolveLocalPaths = false)
        {
            Uri uri = AsAbsoluteUri(baseUriOrPath, relativePathOrUri, doNotResolveLocalPaths);
            return (uri == null) ? "" : uri.ToString();
        }

        /// <summary>
        /// Ensures that a string is a valid absolute URI string.
        /// </summary>
        /// <param name="uriOrPath">A URI or path string.</param>
        /// <param name="doNotResolveLocalPaths">If <c>true</c>, <seealso cref="Path.GetFullPath(string)"/> is not invoked for strings which represent local paths.</param>
        /// <returns>If <paramref name="uriOrPath"/> is null or empty, and empty string is returned.
        /// Otherwise, if <paramref name="uriOrPath"/> is not a valid absolute URI string, it is converted to an absolute URI string.</returns>
        /// <exception cref="ArgumentException"><paramref name="uriOrPath"/> is not an absolute URI string and is not a format that can be converted to an absolute URI string.</exception>
        public static string ValidateUriString(string uriOrPath, bool doNotResolveLocalPaths = false)
        {
            Uri uri = AsAbsoluteUri(uriOrPath, doNotResolveLocalPaths);
            return (uri == null) ? "" : uri.ToString();
        }

        /// <summary>
        /// Converts string values to an absolute URI.
        /// </summary>
        /// <param name="baseUriOrPath">The base URI or path.</param>
        /// <param name="relativePathOrUri">The relative path or URI to add to the base URI.</param>
        /// <param name="doNotResolveLocalPaths">If <c>true</c>, <seealso cref="Path.GetFullPath(string)"/> is not invoked for strings which represent local paths.</param>
        /// <returns>If <paramref name="baseUriOrPath"/> and <paramref name="relativePathOrUri"/> are both null or empty, an empty string is returned.
        /// Otherwise, <paramref name="baseUriOrPath"/>, with <paramref name="relativePathOrUri"/> appended, is returned as a valid absolute URI.</returns>
        /// <exception cref="ArgumentException"><paramref name="baseUriOrPath"/> or <paramref name="relativePathOrUri"/> is not an absolute URI string and is not a format that can be converted to an absolute URI.</exception>
        public static Uri AsAbsoluteUri(string baseUriOrPath, string relativePathOrUri, bool doNotResolveLocalPaths = false)
        {
            if (String.IsNullOrEmpty(baseUriOrPath))
            {
                if (String.IsNullOrEmpty(relativePathOrUri))
                    return null;
                baseUriOrPath = relativePathOrUri;
                relativePathOrUri = null;
            }

            Uri uri;
            try
            {
                if (doNotResolveLocalPaths)
                    uri = new Uri(baseUriOrPath, UriKind.Absolute);
                else if (!Uri.TryCreate(baseUriOrPath, UriKind.Absolute, out uri))
                {
                    if (String.IsNullOrEmpty(relativePathOrUri) || Uri.IsWellFormedUriString(relativePathOrUri, UriKind.Absolute))
                        uri = new Uri(Path.GetFullPath(baseUriOrPath), UriKind.Absolute);
                    else
                    {
                        uri = new Uri(Path.GetFullPath(Path.Combine(baseUriOrPath, relativePathOrUri)), UriKind.Absolute);
                        relativePathOrUri = null;
                    }
                }
            }
            catch (Exception exc) { throw new ArgumentException(exc.Message, "baseUriOrPath", exc); }

            if (uri.Scheme == Uri.UriSchemeFile && !doNotResolveLocalPaths && !String.IsNullOrEmpty(uri.LocalPath) && uri.LocalPath != baseUriOrPath)
            {
                try
                {
                    if (String.IsNullOrEmpty(relativePathOrUri) || Uri.IsWellFormedUriString(relativePathOrUri, UriKind.Absolute))
                        baseUriOrPath = Path.GetFullPath(uri.LocalPath);
                    else
                    {
                        baseUriOrPath = Path.GetFullPath(Path.Combine(uri.LocalPath, relativePathOrUri));
                        relativePathOrUri = null;
                    }
                    uri = new Uri(baseUriOrPath, UriKind.Absolute);
                }
                catch (Exception exc) { throw new ArgumentException(exc.Message, "relativePathOrUri", exc); }
            }

            if (!String.IsNullOrEmpty(relativePathOrUri))
            {
                try { uri = new Uri(uri, relativePathOrUri); }
                catch (Exception exc) { throw new ArgumentException(exc.Message, "relativePathOrUri", exc); }
            }
            return uri;
        }

        /// <summary>
        /// Converts a string to an absolute URI.
        /// </summary>
        /// <param name="uriOrPath">A URI or path string.</param>
        /// <param name="doNotResolveLocalPaths">If <c>true</c>, <seealso cref="Path.GetFullPath(string)"/> is not invoked for strings which represent local paths.</param>
        /// <returns>If <paramref name="uriOrPath"/> is null or empty, an empty string is returned.
        /// Otherwise, if <paramref name="uriOrPath"/> is not a valid absolute URI string, it is converted to an absolute URI string.</returns>
        /// <exception cref="ArgumentException"><paramref name="uriOrPath"/> is not an absolute URI string and is not a format that can be converted to an absolute URI.</exception>
        public static Uri AsAbsoluteUri(string uriOrPath, bool doNotResolveLocalPaths = false)
        {
            return AsAbsoluteUri(uriOrPath, null, doNotResolveLocalPaths);
        }

        #endregion

        #region General XNode extensions

        /// <summary>
        /// Determines if element contains non-whitespace text or at least one element.
        /// </summary>
        /// <param name="element">Element to check.</param>
        /// <returns><c>true</c> if <paramref name="element"/> contains at least one non-whitespace text node or child element;
        /// otherwise, <c>false</c> to indicate that <paramref name="element"/> has no elements or non-whitespace text nodes..</returns>
        public static bool HasNonWsTextOrElementNode(this XElement element)
        {
            if (element == null || element.IsEmpty)
                return false;

            XNode node = element.FirstNode;
            do
            {
                if (node is XElement)
                    return true;

                if (node is XText)
                {
                    if ((node as XText).Value.Trim().Length > 0)
                        return true;
                }
                else if (node is XCData && (node as XCData).Value.Trim().Length > 0)
                    return true;
            } while ((node = node.NextNode) != null);
            return false;
        }

        /// <summary>
        /// Determines if element contains at least one non-whitespace text node.
        /// </summary>
        /// <param name="element">Element to check.</param>
        /// <param name="recursive">If <c>true</c>, child elements will be checked as well; otherwise, false to just check current element.</param>
        /// <returns><c>true</c> if <paramref name="element"/> contains at least one non-whitespace text node;
        /// otherwise, <c>false</c> to indicate that <paramref name="element"/> has no non-whitespace text nodes..</returns>
        public static bool HasNonWhitespaceTextNode(this XElement element, bool recursive = false)
        {
            if (element == null || element.IsEmpty)
                return false;

            XNode node = element.FirstNode;
            do
            {
                if (node is XText)
                {
                    if ((node as XText).Value.Trim().Length > 0)
                        return true;
                }
                else if ((node is XCData && (node as XCData).Value.Trim().Length > 0) || (node is XElement && recursive && (node as XElement).HasNonWhitespaceTextNode(true)))
                    return true;
            } while ((node = node.NextNode) != null);
            return false;
        }

        /// <summary>
        /// Determins if an element contains only text, which matches the given text.
        /// </summary>
        /// <param name="element">Element to check.</param>
        /// <param name="text">Text to match.</param>
        /// <returns><c>true</c> if <paramref name="element"/> contains no elements and only one non-whitespace text node that matches <paramref name="text"/>;
        /// otherwise, false to indicate that either <paramref name="element"/> was null, it had at least one element,
        /// or no text nodes are non-whitespace which match <paramref name="text"/>.</returns>
        /// <remarks>If this <paramref name="text"/> is <c>null</c>, this this will test whether <paramref name="element"/> is null.</remarks>
        public static bool IsTextOnlyMatching(this XElement element, string text)
        {
            if (element == null || element.IsEmpty)
                return false;

            XNode node = element.FirstNode;
            while (node != null)
            {
                if (node is XText)
                {
                    string s = (node as XText).Value.Trim();
                    if (s == text)
                        break;
                    if (s.Length > 0)
                        return false;
                }
                else if (node is XCData)
                {
                    string s = (node as XCData).Value.Trim();
                    if (s == text)
                        break;
                    if (s.Length > 0)
                        return false;
                }
                node = node.NextNode;
            }

            if (node == null)
                return false;

            while ((node = node.NextNode) != null)
            {
                if (node is XText)
                {
                    if ((node as XText).Value.Trim().Length > 0)
                        return false;
                }
                else if (node is XCData)
                {
                    if ((node as XCData).Value.Trim().Length > 0)
                        return false;
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines if any element name matches a specific <seealso cref="XName"/>.
        /// </summary>
        /// <param name="element">Element to check.</param>
        /// <param name="name">Name to match.</param>
        /// <returns><c>true</c> if the name of <paramref name="element"/> matches <paramref name="name"/>; otherwise, false.</returns>
        public static bool NameEquals(this XElement element, XName name)
        {
            if (name == null)
                return element == null;

            if (element == null)
                return false;

            return element.Name.NamespaceName == name.NamespaceName && element.Name.LocalName == name.LocalName;
        }

        #endregion

        #region InsertElement

        /// <summary>
        /// Inserts new <seealso cref="XElement"/> element next to another <seealso cref="XElement"/>.
        /// </summary>
        /// <param name="name">Name of <seealso cref="XElement"/> to be added.</param>
        /// <param name="refNode">Reference <seealso cref="XNode"/> which determines where any new <seealso cref="XElement"/> will be inserted.</param>
        /// <param name="before">If <c>true</c>, new <seealso cref="XElement"/> will be inserted before <paramref name="refNode"/>;
        /// otherwise, new <seealso cref="XElement"/> will be inserted after <paramref name="refNode"/>.</param>
        /// <param name="content">Optional content to be added to new <seealso cref="XElement"/>.</param>
        /// <returns>Newly created <seealso cref="XElement"/> inserted into parent element of <paramref name="refNode"/>.</returns>
        public static XElement InsertElement(this XName name, bool before, XElement refNode, params object[] content)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (refNode == null)
                throw new ArgumentNullException("refNode");

            XElement element = new XElement(name);
            if (before)
                refNode.AddBeforeSelf(element);
            else
                refNode.AddAfterSelf(element);

            if (content != null)
            {
                foreach (object obj in content)
                {
                    if (obj != null)
                        element.Add(obj);
                }
            }

            return element;
        }

        /// <summary>
        /// Inserts new <seealso cref="XElement"/> after another <seealso cref="XElement"/>.
        /// </summary>
        /// <param name="name">Name of <seealso cref="XElement"/> to be added.</param>
        /// <param name="refNode">Reference <seealso cref="XNode"/> which determines where any new <seealso cref="XElement"/> will be inserted.</param>
        /// <param name="content">Optional content to be added to new <seealso cref="XElement"/>.</param>
        /// <returns>Newly created <seealso cref="XElement"/> inserted into parent element of <paramref name="refNode"/>.</returns>
        public static XElement InsertElement(this XName name, XElement refNode, params object[] content)
        {
            return name.InsertElement(false, refNode, content);
        }

        #endregion

        #region Add*Element

        /// <summary>
        /// Adds a new <seealso cref="XElement"/> to another <seealso cref="XElement"/>.
        /// </summary>
        /// <param name="parent">Parent <seealso cref="XElement"/> to add new child <seealso cref="XElement"/> to.</param>
        /// <param name="addFirst">If <c>true</c>, new element is inserted as the first child of <paramref name="parent"/>;
        /// otherwise, it is added as the last element of <paramref name="parent"/>.</param>
        /// <param name="name">Name of <seealso cref="XElement"/> to be added.</param>
        /// <param name="content"></param>
        /// <returns>Newly created <seealso cref="XElement"/> added to <paramref name="parent"/>.</returns>
        public static XElement AddElement(this XElement parent, bool addFirst, XName name, params object[] content)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (parent == null)
                throw new ArgumentNullException("parent");

            XElement element = new XElement(name);
            if (addFirst)
                parent.AddFirst(element);
            else
                parent.Add(element);

            if (content != null)
            {
                foreach (object obj in content)
                {
                    if (obj != null)
                        element.Add(obj);
                }
            }

            return element;
        }

        /// <summary>
        /// Appends a new <seealso cref="XElement"/> to another <seealso cref="XElement"/>.
        /// </summary>
        /// <param name="parent">Parent <seealso cref="XElement"/> to add new child <seealso cref="XElement"/> to.</param>
        /// <param name="name">Name of <seealso cref="XElement"/> to be added.</param>
        /// <param name="content">Content to be added to new <seealso cref="XElement"/>.</param>
        /// <returns>Newly created <seealso cref="XElement"/> appended to <paramref name="parent"/>.</returns>
        public static XElement AddElement(this XElement parent, XName name, params object[] content)
        {
            return parent.AddElement(name, false, content);
        }

        /// <summary>
        /// Adds a command element to another <seealso cref="XElement"/>.
        /// </summary>
        /// <param name="parent">Parent <seealso cref="XElement"/> to add new child command <seealso cref="XElement"/> to.</param>
        /// <param name="addFirst">If <c>true</c>, new command element is inserted as the first child node of <paramref name="parent"/>;
        /// otherwise, new command element is appended as the last child node of <paramref name="parent"/>.</param>
        /// <param name="localName">Local name of command element.</param>
        /// <param name="content">Nodes to be appended to the new command element.</param>
        /// <returns>Newly created command <seealso cref="XElement"/> added to <paramref name="parent"/>.</returns>
        public static XElement AddCommandElement(this XElement parent, bool addFirst, string localName, params object[] content)
        {
            return parent.AddElement(addFirst, XmlNs_command.GetName(localName), content);
        }

        /// <summary>
        /// Adds a maml element to another <seealso cref="XElement"/>.
        /// </summary>
        /// <param name="parent">Parent <seealso cref="XElement"/> to add new child maml <seealso cref="XElement"/> to.</param>
        /// <param name="addFirst">If <c>true</c>, new maml element is inserted as the first child node of <paramref name="parent"/>;
        /// otherwise, new maml element is appended as the last child node of <paramref name="parent"/>.</param>
        /// <param name="localName">Local name of maml element.</param>
        /// <param name="content">Nodes to be appended to the new maml element.</param>
        /// <returns>Newly created maml <seealso cref="XElement"/> added to <paramref name="parent"/>.</returns>
        public static XElement AddMamlElement(this XElement parent, bool addFirst, string localName, params object[] content)
        {
            return parent.AddElement(addFirst, XmlNs_maml.GetName(localName), content);
        }

        /// <summary>
        /// Adds a dev element to another <seealso cref="XElement"/>.
        /// </summary>
        /// <param name="parent">Parent <seealso cref="XElement"/> to add new child dev <seealso cref="XElement"/> to.</param>
        /// <param name="addFirst">If <c>true</c>, new dev element is inserted as the first child node of <paramref name="parent"/>;
        /// otherwise, new dev element is appended as the last child node of <paramref name="parent"/>.</param>
        /// <param name="localName">Local name of dev element.</param>
        /// <param name="content">Nodes to be appended to the new dev element.</param>
        /// <returns>Newly created dev <seealso cref="XElement"/> added to <paramref name="parent"/>.</returns>
        public static XElement AddDevElement(this XElement parent, bool addFirst, string localName, params object[] content)
        {
            return parent.AddElement(addFirst, XmlNs_dev.GetName(localName), content);
        }

        /// <summary>
        /// Appends a command element to another <seealso cref="XElement"/>.
        /// </summary>
        /// <param name="parent">Parent <seealso cref="XElement"/> to add new child command <seealso cref="XElement"/> to.</param>
        /// <param name="localName">Local name of command element.</param>
        /// <param name="content">Nodes to be appended to the new command element.</param>
        /// <returns>Newly created command <seealso cref="XElement"/> appended to <paramref name="parent"/>.</returns>
        public static XElement AddCommandElement(this XElement parent, string localName, params object[] content)
        {
            return parent.AddElement(XmlNs_command.GetName(localName), content);
        }

        /// <summary>
        /// Appends a maml element to another <seealso cref="XElement"/>.
        /// </summary>
        /// <param name="parent">Parent <seealso cref="XElement"/> to add new child maml <seealso cref="XElement"/> to.</param>
        /// <param name="localName">Local name of maml element.</param>
        /// <param name="content">Nodes to be appended to the new maml element.</param>
        /// <returns>Newly created maml <seealso cref="XElement"/> appended to <paramref name="parent"/>.</returns>
        public static XElement AddMamlElement(this XElement parent, string localName, params object[] content)
        {
            return parent.AddElement(XmlNs_maml.GetName(localName), content);
        }

        /// <summary>
        /// Appends a dev element to another <seealso cref="XElement"/>.
        /// </summary>
        /// <param name="parent">Parent <seealso cref="XElement"/> to add new child dev <seealso cref="XElement"/> to.</param>
        /// <param name="localName">Local name of dev element.</param>
        /// <param name="content">Nodes to be appended to the new dev element.</param>
        /// <returns>Newly created dev <seealso cref="XElement"/> appended to <paramref name="parent"/>.</returns>
        public static XElement AddDevElement(this XElement parent, string localName, params object[] content)
        {
            return parent.AddElement(XmlNs_dev.GetName(localName), content);
        }

        #endregion

        #region EnsureElement

        /// <summary>
        /// Gets first matching element or inserts a new one if none is found.
        /// </summary>
        /// <param name="ns">Namespace of element to search for.</param>
        /// <param name="localName">Local name of lement to search for</param>
        /// <param name="refNode">Reference <seealso cref="XNode"/> which determines where any new <seealso cref="XElement"/> will be inserted.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="addBefore">If <c>true</c>, and a new element is created, it is inserted before <paramref name="refNode"/>;
        /// otherwise, any new element is inserted after <paramref name="refNode"/>.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        /// This can be <c>null</c>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="ns"/>, <paramref name="localName"/>, <paramref name="refNode"/>, or <paramref name="predicate"/> was null.</exception>
        /// <exception cref="ArgumentException"><paramref name="localName"/> was empty.</exception>
        public static XElement EnsureElement(this XNamespace ns, string localName, XNode refNode, Func<XElement, bool> predicate, bool addBefore, Action<XElement> onNew,
            Action<XElement> onExists, params XName[] predicateNames)
        {
            if (ns == null)
                throw new ArgumentNullException("ns");

            if (localName == null)
                throw new ArgumentNullException("localName");

            if (localName.Trim().Length == 0)
                throw new ArgumentException("Local name cannot be empty", "localName");

            return ns.GetName(localName).EnsureElement(refNode, predicate, addBefore, onNew, onExists, predicateNames);
        }

        /// <summary>
        /// Gets first matching element or inserts a new one if none is found.
        /// </summary>
        /// <param name="name">Name of element to search for.</param>
        /// <param name="refNode">Reference <seealso cref="XNode"/> which determines where any new <seealso cref="XElement"/> will be inserted.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="addBefore">If <c>true</c>, and a new element is created, it is inserted before <paramref name="refNode"/>;
        /// otherwise, any new element is inserted after <paramref name="refNode"/>.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        /// This can be <c>null</c>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/>, <paramref name="refNode"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureElement(this XName name, XNode refNode, Func<XElement, bool> predicate, bool addBefore, Action<XElement> onNew,
            Action<XElement> onExists, params XName[] predicateNames)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (refNode == null)
                throw new ArgumentNullException("refNode");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            IEnumerable<XElement> elements = refNode.Parent.Elements(name);
            if (refNode is XElement && (refNode as XElement).NameEquals(name))
                elements = elements.Where(e => !ReferenceEquals(e, refNode));
            int level = 0;
            if (predicateNames != null)
            {
                foreach (XName n in predicateNames)
                {
                    if (n != null)
                    {
                        level++;
                        elements = elements.Elements(n);
                    }
                }
            }

            XElement result = elements.FirstOrDefault(predicate);
            if (result == null)
            {
                result = new XElement(name);
                if (addBefore)
                    refNode.AddBeforeSelf(result);
                else
                    refNode.AddAfterSelf(result);
                if (onNew != null)
                    onNew(result);
                return result;
            }
            for (int i = 0; i < level; i++)
                result = result.Parent;
            if (onExists != null)
                onExists(result);
            return result;
        }

        /// <summary>
        /// Gets first matching element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="addFirst">If <c>true</c>, and a new element is created, it is inserted as the first child of <paramref name="parent"/>;
        /// otherwise, any new element is added as the last child of <paramref name="parent"/>.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        /// This can be <c>null</c>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureElement(this XElement parent, XName name, Func<XElement, bool> predicate, bool addFirst, Action<XElement> onNew,
            Action<XElement> onExists, params XName[] predicateNames)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            if (name == null)
                throw new ArgumentNullException("name");

            if (predicate == null)
                throw new ArgumentNullException("predicate");

            IEnumerable<XElement> elements = parent.Elements(name);
            int level = 0;
            if (predicateNames != null)
            {
                foreach (XName n in predicateNames)
                {
                    if (n != null)
                    {
                        level++;
                        elements = elements.Elements(n);
                    }
                }
            }

            XElement result = elements.FirstOrDefault(predicate);
            if (result == null)
            {
                result = new XElement(name);
                if (addFirst)
                    parent.AddFirst(result);
                else
                    parent.Add(result);
                if (onNew != null)
                    onNew(result);
                return result;
            }
            for (int i = 0; i < level; i++)
                result = result.Parent;
            if (onExists != null)
                onExists(result);
            return result;
        }


        /// <summary>
        /// Gets first matching element or inserts a new one if none is found.
        /// </summary>
        /// <param name="ns">Namespace of element to search for.</param>
        /// <param name="localName">Local name of lement to search for</param>
        /// <param name="refNode">Reference <seealso cref="XNode"/> which determines where any new <seealso cref="XElement"/> will be inserted.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="addBefore">If <c>true</c>, and a new element is created, it is inserted before <paramref name="refNode"/>;
        /// otherwise, any new element is inserted after <paramref name="refNode"/>.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="ns"/>, <paramref name="localName"/>, <paramref name="refNode"/>, or <paramref name="predicate"/> was null.</exception>
        /// <exception cref="ArgumentException"><paramref name="localName"/> was empty.</exception>
        public static XElement EnsureElement(this XNamespace ns, string localName, XNode refNode, Func<XElement, bool> predicate, bool addBefore, Action<XElement> onNew,
            params XName[] predicateNames)
        { return ns.EnsureElement(localName, refNode, predicate, addBefore, onNew, null, predicateNames); }


        /// <summary>
        /// Gets first matching element or inserts a new one if none is found.
        /// </summary>
        /// <param name="name">Name of element to search for.</param>
        /// <param name="refNode">Reference <seealso cref="XNode"/> which determines where any new <seealso cref="XElement"/> will be inserted.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="addBefore">If <c>true</c>, and a new element is created, it is inserted before <paramref name="refNode"/>;
        /// otherwise, any new element is inserted after <paramref name="refNode"/>.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/>, <paramref name="refNode"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureElement(this XName name, XNode refNode, Func<XElement, bool> predicate, bool addBefore, Action<XElement> onNew,
            params XName[] predicateNames)
        { return name.EnsureElement(refNode, predicate, addBefore, onNew, null, predicateNames); }

        /// <summary>
        /// Gets first matching element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="addFirst">If <c>true</c>, and a new element is created, it is inserted as the first child of <paramref name="parent"/>;
        /// otherwise, any new element is added as the last child of <paramref name="parent"/>.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureElement(this XElement parent, XName name, Func<XElement, bool> predicate, bool addFirst, Action<XElement> onNew,
            params XName[] predicateNames)
        { return parent.EnsureElement(name, predicate, addFirst, onNew, null, predicateNames); }

        /// <summary>
        /// Gets first matching element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        /// This can be <c>null</c>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureElement(this XElement parent, XName name, Func<XElement, bool> predicate, Action<XElement> onNew, Action<XElement> onExists,
            params XName[] predicateNames)
        { return parent.EnsureElement(name, predicate, false, onNew, onExists, predicateNames); }


        /// <summary>
        /// Gets first matching element or inserts a new one if none is found.
        /// </summary>
        /// <param name="ns">Namespace of element to search for.</param>
        /// <param name="localName">Local name of lement to search for</param>
        /// <param name="refNode">Reference <seealso cref="XNode"/> which determines where any new <seealso cref="XElement"/> will be inserted.</param>
        /// <param name="addBefore">If <c>true</c>, and a new element is created, it is inserted before <paramref name="refNode"/>;
        /// otherwise, any new element is inserted after <paramref name="refNode"/>.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        /// This can be <c>null</c>.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="ns"/>, <paramref name="localName"/>, or <paramref name="refNode"/> was null.</exception>
        /// <exception cref="ArgumentException"><paramref name="localName"/> was empty.</exception>
        public static XElement EnsureElement(this XNamespace ns, string localName, XNode refNode, bool addBefore, Action<XElement> onNew = null, Action<XElement> onExists = null)
        {
            if (ns == null)
                throw new ArgumentNullException("ns");

            if (localName == null)
                throw new ArgumentNullException("localName");

            if (localName.Trim().Length == 0)
                throw new ArgumentException("Local name cannot be empty", "localName");

            return ns.GetName(localName).EnsureElement(refNode, addBefore, onNew, onExists);
        }

        /// <summary>
        /// Gets first matching element or inserts a new one if none is found.
        /// </summary>
        /// <param name="name">Name of element to search for.</param>
        /// <param name="refNode">Reference <seealso cref="XNode"/> which determines where any new <seealso cref="XElement"/> will be inserted.</param>
        /// <param name="addBefore">If <c>true</c>, and a new element is created, it is inserted before <paramref name="refNode"/>;
        /// otherwise, any new element is inserted after <paramref name="refNode"/>.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        /// This can be <c>null</c>.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="refNode"/> was null.</exception>
        public static XElement EnsureElement(this XName name, XNode refNode, bool addBefore, Action<XElement> onNew = null, Action<XElement> onExists = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (refNode == null)
                throw new ArgumentNullException("refNode");

            XElement result;
            if (refNode is XElement && (refNode as XElement).NameEquals(name))
                result = refNode.Parent.Element(name);
            else
                result = refNode.Parent.Elements(name).FirstOrDefault(e => !ReferenceEquals(e, refNode));
            
            if (result != null)
            {
                if (onExists != null)
                    onExists(result);
                return result;
            }

            result = new XElement(name);
            if (addBefore)
                refNode.AddBeforeSelf(result);
            else
                refNode.AddAfterSelf(result);
            if (onNew != null)
                onNew(result);
            return result;
        }

        /// <summary>
        /// Gets first matching element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of element to search for.</param>
        /// <param name="addFirst">If <c>true</c>, and a new element is created, it is inserted as the first child of <paramref name="parent"/>;
        /// otherwise, any new element is added as the last child of <paramref name="parent"/>. The default value is <c>false</c>.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        /// This can be <c>null</c>.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/> or <paramref name="name"/> was null.</exception>
        public static XElement EnsureElement(this XElement parent, XName name, bool addFirst = false, Action<XElement> onNew = null, Action<XElement> onExists = null)

        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            if (name == null)
                throw new ArgumentNullException("name");
            
            XElement result = parent.Element(name);
            if (result != null)
            {
                if (onExists != null)
                    onExists(result);
                return result;
            }

            result = new XElement(name);
            if (addFirst)
                parent.AddFirst(result);
            else
                parent.Add(result);
            if (onNew != null)
                onNew(result);
            return result;
        }


        /// <summary>
        /// Gets first matching element or inserts a new one if none is found.
        /// </summary>
        /// <param name="ns">Namespace of element to search for.</param>
        /// <param name="localName">Local name of lement to search for</param>
        /// <param name="refNode">Reference <seealso cref="XNode"/> which determines where any new <seealso cref="XElement"/> will be inserted.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="addBefore">If <c>true</c>, and a new element is created, it is inserted before <paramref name="refNode"/>;
        /// otherwise, any new element is inserted after <paramref name="refNode"/>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="ns"/>, <paramref name="localName"/>, <paramref name="refNode"/>, or <paramref name="predicate"/> was null.</exception>
        /// <exception cref="ArgumentException"><paramref name="localName"/> was empty.</exception>
        public static XElement EnsureElement(this XNamespace ns, string localName, XNode refNode, Func<XElement, bool> predicate, bool addBefore, params XName[] predicateNames)
        { return ns.EnsureElement(localName, refNode, predicate, addBefore, null, null, predicateNames); }

        /// <summary>
        /// Gets first matching element or inserts a new one if none is found.
        /// </summary>
        /// <param name="name">Name of element to search for.</param>
        /// <param name="refNode">Reference <seealso cref="XNode"/> which determines where any new <seealso cref="XElement"/> will be inserted.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="addBefore">If <c>true</c>, and a new element is created, it is inserted before <paramref name="refNode"/>;
        /// otherwise, any new element is inserted after <paramref name="refNode"/>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="name"/>, <paramref name="refNode"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureElement(this XName name, XNode refNode, Func<XElement, bool> predicate, bool addBefore, params XName[] predicateNames)
        { return name.EnsureElement(refNode, predicate, addBefore, null, null, predicateNames); }

        /// <summary>
        /// Gets first matching element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="addFirst">If <c>true</c>, and a new element is created, it is inserted as the first child of <paramref name="parent"/>;
        /// otherwise, any new element is added as the last child of <paramref name="parent"/>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureElement(this XElement parent, XName name, Func<XElement, bool> predicate, bool addFirst, params XName[] predicateNames)
        { return parent.EnsureElement(name, predicate, addFirst, null, null, predicateNames); }

        /// <summary>
        /// Gets first matching element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureElement(this XElement parent, XName name, Func<XElement, bool> predicate, Action<XElement> onNew, params XName[] predicateNames)
        { return parent.EnsureElement(name, predicate, onNew, null, predicateNames); }

        /// <summary>
        /// Gets first matching element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureElement(this XElement parent, XName name, Func<XElement, bool> predicate, params XName[] predicateNames)
        { return parent.EnsureElement(name, predicate, null, null, predicateNames); }

        #endregion

        #region EnsureCommandElement

        ///// <summary>
        ///// Gets first matching command element or inserts a new one if none is found.
        ///// </summary>
        ///// <param name="name">Name of command element to search for.</param>
        ///// <param name="refNode">Reference <seealso cref="XNode"/> which determines where any new <seealso cref="XElement"/> will be inserted.</param>
        ///// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        ///// <param name="addBefore">If <c>true</c>, and a new element is created, it is inserted before <paramref name="refNode"/>;
        ///// otherwise, any new element is inserted after <paramref name="refNode"/>.</param>
        ///// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        ///// This can be <c>null</c>.</param>
        ///// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        ///// This can be <c>null</c>.</param>
        ///// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        ///// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        ///// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        ///// <exception cref="ArgumentNullException"><paramref name="name"/>, <paramref name="refNode"/>, or <paramref name="predicate"/> was null.</exception>
        //public static XElement EnsureCommandElement(this string name, XNode refNode, Func<XElement, bool> predicate, bool addBefore, Action<XElement> onNew,
        //    Action<XElement> onExists, params XName[] predicateNames)
        //{
        //    if (name == null)
        //        throw new ArgumentNullException("name");

        //    if (name.Trim().Length == 0)
        //        throw new ArgumentException("Name cannot be empty.");

        //    return XmlNs_command.GetName(name).EnsureElement(refNode, predicate, addBefore, onNew, onExists, predicateNames);
        //}

        /// <summary>
        /// Gets first matching command element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of command element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="addFirst">If <c>true</c>, and a new element is created, it is inserted as the first child of <paramref name="parent"/>;
        /// otherwise, any new element is added as the last child of <paramref name="parent"/>.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        /// This can be <c>null</c>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureCommandElement(this XElement parent, string name, Func<XElement, bool> predicate, bool addFirst, Action<XElement> onNew,
            Action<XElement> onExists, params XName[] predicateNames)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Trim().Length == 0)
                throw new ArgumentException("Name cannot be empty.");

            return parent.EnsureElement(XmlNs_command.GetName(name), predicate, addFirst, onNew, onExists, predicateNames);
        }

        ///// <summary>
        ///// Gets first matching command element or inserts a new one if none is found.
        ///// </summary>
        ///// <param name="name">Name of command element to search for.</param>
        ///// <param name="refNode">Reference <seealso cref="XNode"/> which determines where any new <seealso cref="XElement"/> will be inserted.</param>
        ///// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        ///// <param name="addBefore">If <c>true</c>, and a new element is created, it is inserted before <paramref name="refNode"/>;
        ///// otherwise, any new element is inserted after <paramref name="refNode"/>.</param>
        ///// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        ///// This can be <c>null</c>.</param>
        ///// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        ///// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        ///// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        ///// <exception cref="ArgumentNullException"><paramref name="name"/>, <paramref name="refNode"/>, or <paramref name="predicate"/> was null.</exception>
        //public static XElement EnsureCommandElement(this string name, XNode refNode, Func<XElement, bool> predicate, bool addBefore, Action<XElement> onNew,
        //    params XName[] predicateNames)
        //{ return name.EnsureCommandElement(refNode, predicate, addBefore, onNew, null, predicateNames); }

        /// <summary>
        /// Gets first matching command element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of command element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="addFirst">If <c>true</c>, and a new element is created, it is inserted as the first child of <paramref name="parent"/>;
        /// otherwise, any new element is added as the last child of <paramref name="parent"/>.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureCommandElement(this XElement parent, string name, Func<XElement, bool> predicate, bool addFirst, Action<XElement> onNew,
            params XName[] predicateNames)
        { return parent.EnsureCommandElement(name, predicate, addFirst, onNew, null, predicateNames); }

        /// <summary>
        /// Gets first matching command element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of command element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        /// This can be <c>null</c>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureCommandElement(this XElement parent, string name, Func<XElement, bool> predicate, Action<XElement> onNew, Action<XElement> onExists,
            params XName[] predicateNames)
        { return parent.EnsureCommandElement(name, predicate, false, onNew, onExists, predicateNames); }

        ///// <summary>
        ///// Gets first matching command element or inserts a new one if none is found.
        ///// </summary>
        ///// <param name="name">Name of command element to search for.</param>
        ///// <param name="refNode">Reference <seealso cref="XNode"/> which determines where any new <seealso cref="XElement"/> will be inserted.</param>
        ///// <param name="addBefore">If <c>true</c>, and a new element is created, it is inserted before <paramref name="refNode"/>;
        ///// otherwise, any new element is inserted after <paramref name="refNode"/>.</param>
        ///// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        ///// This can be <c>null</c>.</param>
        ///// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        ///// This can be <c>null</c>.</param>
        ///// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        ///// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="refNode"/> was null.</exception>
        //public static XElement EnsureCommandElement(this string name, XNode refNode, bool addBefore, Action<XElement> onNew = null, Action<XElement> onExists = null)
        //{
        //    if (name == null)
        //        throw new ArgumentNullException("name");

        //    if (name.Trim().Length == 0)
        //        throw new ArgumentException("Name cannot be empty.");

        //    return XmlNs_command.GetName(name).EnsureElement(refNode, addBefore, onNew, onExists);
        //}

        /// <summary>
        /// Gets first matching command element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of command element to search for.</param>
        /// <param name="addFirst">If <c>true</c>, and a new element is created, it is inserted as the first child of <paramref name="parent"/>;
        /// otherwise, any new element is added as the last child of <paramref name="parent"/>. The default value is <c>false</c>.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        /// This can be <c>null</c>.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/> or <paramref name="name"/> was null.</exception>
        public static XElement EnsureCommandElement(this XElement parent, string name, bool addFirst = false, Action<XElement> onNew = null, Action<XElement> onExists = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Trim().Length == 0)
                throw new ArgumentException("Name cannot be empty.");

            return parent.EnsureElement(XmlNs_command.GetName(name), addFirst, onNew, onExists);
        }

        /// <summary>
        /// Gets first matching command element or appends a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of command element to search for.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        /// This can be <c>null</c>.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/> or <paramref name="name"/> was null.</exception>
        public static XElement EnsureCommandElement(this XElement parent, string name, Action<XElement> onNew, Action<XElement> onExists = null)
        {
            return parent.EnsureCommandElement(name, false, onNew, onExists);
        }

        ///// <summary>
        ///// Gets first matching command element or inserts a new one if none is found.
        ///// </summary>
        ///// <param name="name">Name of command element to search for.</param>
        ///// <param name="refNode">Reference <seealso cref="XNode"/> which determines where any new <seealso cref="XElement"/> will be inserted.</param>
        ///// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        ///// <param name="addBefore">If <c>true</c>, and a new element is created, it is inserted before <paramref name="refNode"/>;
        ///// otherwise, any new element is inserted after <paramref name="refNode"/>.</param>
        ///// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        ///// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        ///// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        ///// <exception cref="ArgumentNullException"><paramref name="name"/>, <paramref name="refNode"/>, or <paramref name="predicate"/> was null.</exception>
        //public static XElement EnsureCommandElement(this string name, XNode refNode, Func<XElement, bool> predicate, bool addBefore, params XName[] predicateNames)
        //{ return name.EnsureCommandElement(refNode, predicate, addBefore, null, null, predicateNames); }

        /// <summary>
        /// Gets first matching command element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of command element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="addFirst">If <c>true</c>, and a new element is created, it is inserted as the first child of <paramref name="parent"/>;
        /// otherwise, any new element is added as the last child of <paramref name="parent"/>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureCommandElement(this XElement parent, string name, Func<XElement, bool> predicate, bool addFirst, params XName[] predicateNames)
        { return parent.EnsureCommandElement(name, predicate, addFirst, null, null, predicateNames); }

        /// <summary>
        /// Gets first matching command element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of command element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureCommandElement(this XElement parent, string name, Func<XElement, bool> predicate, Action<XElement> onNew, params XName[] predicateNames)
        { return parent.EnsureCommandElement(name, predicate, onNew, null, predicateNames); }

        /// <summary>
        /// Gets first matching command element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of command element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureCommandElement(this XElement parent, string name, Func<XElement, bool> predicate, params XName[] predicateNames)
        { return parent.EnsureCommandElement(name, predicate, null, null, predicateNames); }

        #endregion

        #region EnsureMamlElement

        ///// <summary>
        ///// Gets first matching maml element or inserts a new one if none is found.
        ///// </summary>
        ///// <param name="name">Name of maml element to search for.</param>
        ///// <param name="refNode">Reference <seealso cref="XNode"/> which determines where any new <seealso cref="XElement"/> will be inserted.</param>
        ///// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        ///// <param name="addBefore">If <c>true</c>, and a new element is created, it is inserted before <paramref name="refNode"/>;
        ///// otherwise, any new element is inserted after <paramref name="refNode"/>.</param>
        ///// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        ///// This can be <c>null</c>.</param>
        ///// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        ///// This can be <c>null</c>.</param>
        ///// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        ///// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        ///// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        ///// <exception cref="ArgumentNullException"><paramref name="name"/>, <paramref name="refNode"/>, or <paramref name="predicate"/> was null.</exception>
        //public static XElement EnsureMamlElement(this string name, XNode refNode, Func<XElement, bool> predicate, bool addBefore, Action<XElement> onNew,
        //    Action<XElement> onExists, params XName[] predicateNames)
        //{
        //    if (name == null)
        //        throw new ArgumentNullException("name");

        //    if (name.Trim().Length == 0)
        //        throw new ArgumentException("Name cannot be empty.");

        //    return XmlNs_maml.GetName(name).EnsureElement(refNode, predicate, addBefore, onNew, onExists, predicateNames);
        //}

        /// <summary>
        /// Gets first matching maml element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of maml element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="addFirst">If <c>true</c>, and a new element is created, it is inserted as the first child of <paramref name="parent"/>;
        /// otherwise, any new element is added as the last child of <paramref name="parent"/>.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        /// This can be <c>null</c>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureMamlElement(this XElement parent, string name, Func<XElement, bool> predicate, bool addFirst, Action<XElement> onNew,
            Action<XElement> onExists, params XName[] predicateNames)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Trim().Length == 0)
                throw new ArgumentException("Name cannot be empty.");

            return parent.EnsureElement(XmlNs_maml.GetName(name), predicate, addFirst, onNew, onExists, predicateNames);
        }

        ///// <summary>
        ///// Gets first matching maml element or inserts a new one if none is found.
        ///// </summary>
        ///// <param name="name">Name of maml element to search for.</param>
        ///// <param name="refNode">Reference <seealso cref="XNode"/> which determines where any new <seealso cref="XElement"/> will be inserted.</param>
        ///// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        ///// <param name="addBefore">If <c>true</c>, and a new element is created, it is inserted before <paramref name="refNode"/>;
        ///// otherwise, any new element is inserted after <paramref name="refNode"/>.</param>
        ///// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        ///// This can be <c>null</c>.</param>
        ///// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        ///// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        ///// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        ///// <exception cref="ArgumentNullException"><paramref name="name"/>, <paramref name="refNode"/>, or <paramref name="predicate"/> was null.</exception>
        //public static XElement EnsureMamlElement(this string name, XNode refNode, Func<XElement, bool> predicate, bool addBefore, Action<XElement> onNew,
        //    params XName[] predicateNames)
        //{ return name.EnsureMamlElement(refNode, predicate, addBefore, onNew, null, predicateNames); }

        /// <summary>
        /// Gets first matching maml element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of maml element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="addFirst">If <c>true</c>, and a new element is created, it is inserted as the first child of <paramref name="parent"/>;
        /// otherwise, any new element is added as the last child of <paramref name="parent"/>.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureMamlElement(this XElement parent, string name, Func<XElement, bool> predicate, bool addFirst, Action<XElement> onNew,
            params XName[] predicateNames)
        { return parent.EnsureMamlElement(name, predicate, addFirst, onNew, null, predicateNames); }

        /// <summary>
        /// Gets first matching maml element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of maml element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        /// This can be <c>null</c>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureMamlElement(this XElement parent, string name, Func<XElement, bool> predicate, Action<XElement> onNew, Action<XElement> onExists,
            params XName[] predicateNames)
        { return parent.EnsureMamlElement(name, predicate, false, onNew, onExists, predicateNames); }

        ///// <summary>
        ///// Gets first matching maml element or inserts a new one if none is found.
        ///// </summary>
        ///// <param name="name">Name of maml element to search for.</param>
        ///// <param name="refNode">Reference <seealso cref="XNode"/> which determines where any new <seealso cref="XElement"/> will be inserted.</param>
        ///// <param name="addBefore">If <c>true</c>, and a new element is created, it is inserted before <paramref name="refNode"/>;
        ///// otherwise, any new element is inserted after <paramref name="refNode"/>.</param>
        ///// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        ///// This can be <c>null</c>.</param>
        ///// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        ///// This can be <c>null</c>.</param>
        ///// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        ///// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="refNode"/> was null.</exception>
        //public static XElement EnsureMamlElement(this string name, XNode refNode, bool addBefore, Action<XElement> onNew = null, Action<XElement> onExists = null)
        //{
        //    if (name == null)
        //        throw new ArgumentNullException("name");

        //    if (name.Trim().Length == 0)
        //        throw new ArgumentException("Name cannot be empty.");

        //    return XmlNs_maml.GetName(name).EnsureElement(refNode, addBefore, onNew, onExists);
        //}

        /// <summary>
        /// Gets first matching maml element or appends a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of maml element to search for.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        /// This can be <c>null</c>.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/> or <paramref name="name"/> was null.</exception>
        public static XElement EnsureMamlElement(this XElement parent, string name, Action<XElement> onNew, Action<XElement> onExists = null)
        {
            return parent.EnsureMamlElement(name, false, onNew, onExists);
        }

        /// <summary>
        /// Gets first matching maml element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of maml element to search for.</param>
        /// <param name="addFirst">If <c>true</c>, and a new element is created, it is inserted as the first child of <paramref name="parent"/>;
        /// otherwise, any new element is added as the last child of <paramref name="parent"/>. The default value is <c>false</c>.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        /// This can be <c>null</c>.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/> or <paramref name="name"/> was null.</exception>
        public static XElement EnsureMamlElement(this XElement parent, string name, bool addFirst = false, Action<XElement> onNew = null, Action<XElement> onExists = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Trim().Length == 0)
                throw new ArgumentException("Name cannot be empty.");

            return parent.EnsureElement(XmlNs_maml.GetName(name), addFirst, onNew, onExists);
        }

        ///// <summary>
        ///// Gets first matching maml element or inserts a new one if none is found.
        ///// </summary>
        ///// <param name="name">Name of maml element to search for.</param>
        ///// <param name="refNode">Reference <seealso cref="XNode"/> which determines where any new <seealso cref="XElement"/> will be inserted.</param>
        ///// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        ///// <param name="addBefore">If <c>true</c>, and a new element is created, it is inserted before <paramref name="refNode"/>;
        ///// otherwise, any new element is inserted after <paramref name="refNode"/>.</param>
        ///// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        ///// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        ///// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        ///// <exception cref="ArgumentNullException"><paramref name="name"/>, <paramref name="refNode"/>, or <paramref name="predicate"/> was null.</exception>
        //public static XElement EnsureMamlElement(this string name, XNode refNode, Func<XElement, bool> predicate, bool addBefore, params XName[] predicateNames)
        //{ return name.EnsureMamlElement(refNode, predicate, addBefore, null, null, predicateNames); }

        /// <summary>
        /// Gets first matching maml element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of maml element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="addFirst">If <c>true</c>, and a new element is created, it is inserted as the first child of <paramref name="parent"/>;
        /// otherwise, any new element is added as the last child of <paramref name="parent"/>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureMamlElement(this XElement parent, string name, Func<XElement, bool> predicate, bool addFirst, params XName[] predicateNames)
        { return parent.EnsureMamlElement(name, predicate, addFirst, null, null, predicateNames); }

        /// <summary>
        /// Gets first matching maml element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of maml element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureMamlElement(this XElement parent, string name, Func<XElement, bool> predicate, Action<XElement> onNew, params XName[] predicateNames)
        { return parent.EnsureMamlElement(name, predicate, onNew, null, predicateNames); }

        /// <summary>
        /// Gets first matching maml element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of maml element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureMamlElement(this XElement parent, string name, Func<XElement, bool> predicate, params XName[] predicateNames)
        { return parent.EnsureMamlElement(name, predicate, null, null, predicateNames); }

        #endregion

        #region EnsureDevElement
        
        /// <summary>
        /// Gets first matching dev element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of dev element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="addFirst">If <c>true</c>, and a new element is created, it is inserted as the first child of <paramref name="parent"/>;
        /// otherwise, any new element is added as the last child of <paramref name="parent"/>.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        /// This can be <c>null</c>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureDevElement(this XElement parent, string name, Func<XElement, bool> predicate, bool addFirst, Action<XElement> onNew,
            Action<XElement> onExists, params XName[] predicateNames)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Trim().Length == 0)
                throw new ArgumentException("Name cannot be empty.");

            return parent.EnsureElement(XmlNs_dev.GetName(name), predicate, addFirst, onNew, onExists, predicateNames);
        }

        ///// <summary>
        ///// Gets first matching dev element or inserts a new one if none is found.
        ///// </summary>
        ///// <param name="name">Name of dev element to search for.</param>
        ///// <param name="refNode">Reference <seealso cref="XNode"/> which determines where any new <seealso cref="XElement"/> will be inserted.</param>
        ///// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        ///// <param name="addBefore">If <c>true</c>, and a new element is created, it is inserted before <paramref name="refNode"/>;
        ///// otherwise, any new element is inserted after <paramref name="refNode"/>.</param>
        ///// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        ///// This can be <c>null</c>.</param>
        ///// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        ///// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        ///// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        ///// <exception cref="ArgumentNullException"><paramref name="name"/>, <paramref name="refNode"/>, or <paramref name="predicate"/> was null.</exception>
        //public static XElement EnsureDevElement(this string name, XNode refNode, Func<XElement, bool> predicate, bool addBefore, Action<XElement> onNew,
        //    params XName[] predicateNames)
        //{ return name.EnsureDevElement(refNode, predicate, addBefore, onNew, null, predicateNames); }

        /// <summary>
        /// Gets first matching dev element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of dev element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="addFirst">If <c>true</c>, and a new element is created, it is inserted as the first child of <paramref name="parent"/>;
        /// otherwise, any new element is added as the last child of <paramref name="parent"/>.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureDevElement(this XElement parent, string name, Func<XElement, bool> predicate, bool addFirst, Action<XElement> onNew,
            params XName[] predicateNames)
        { return parent.EnsureDevElement(name, predicate, addFirst, onNew, null, predicateNames); }

        /// <summary>
        /// Gets first matching dev element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of dev element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        /// This can be <c>null</c>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureDevElement(this XElement parent, string name, Func<XElement, bool> predicate, Action<XElement> onNew, Action<XElement> onExists,
            params XName[] predicateNames)
        { return parent.EnsureDevElement(name, predicate, false, onNew, onExists, predicateNames); }

        ///// <summary>
        ///// Gets first matching dev element or inserts a new one if none is found.
        ///// </summary>
        ///// <param name="name">Name of dev element to search for.</param>
        ///// <param name="refNode">Reference <seealso cref="XNode"/> which determines where any new <seealso cref="XElement"/> will be inserted.</param>
        ///// <param name="addBefore">If <c>true</c>, and a new element is created, it is inserted before <paramref name="refNode"/>;
        ///// otherwise, any new element is inserted after <paramref name="refNode"/>.</param>
        ///// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        ///// This can be <c>null</c>.</param>
        ///// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        ///// This can be <c>null</c>.</param>
        ///// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        ///// <exception cref="ArgumentNullException"><paramref name="name"/> or <paramref name="refNode"/> was null.</exception>
        //public static XElement EnsureDevElement(this string name, XNode refNode, bool addBefore, Action<XElement> onNew = null, Action<XElement> onExists = null)
        //{
        //    if (name == null)
        //        throw new ArgumentNullException("name");

        //    if (name.Trim().Length == 0)
        //        throw new ArgumentException("Name cannot be empty.");

        //    return XmlNs_dev.GetName(name).EnsureElement(refNode, addBefore, onNew, onExists);
        //}

        /// <summary>
        /// Gets first matching dev element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of dev element to search for.</param>
        /// <param name="addFirst">If <c>true</c>, and a new element is created, it is inserted as the first child of <paramref name="parent"/>;
        /// otherwise, any new element is added as the last child of <paramref name="parent"/>. The default value is <c>false</c>.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        /// This can be <c>null</c>.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/> or <paramref name="name"/> was null.</exception>
        public static XElement EnsureDevElement(this XElement parent, string name, bool addFirst = false, Action<XElement> onNew = null, Action<XElement> onExists = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Trim().Length == 0)
                throw new ArgumentException("Name cannot be empty.");

            return parent.EnsureElement(XmlNs_dev.GetName(name), addFirst, onNew, onExists);
        }

        /// <summary>
        /// Gets first matching dev element or appends a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of command element to search for.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="onExists"><seealso cref="Action{T}"/> which gets invoked when a matching <seealso cref="XElement"/> is about to be returned.
        /// This can be <c>null</c>.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/> or <paramref name="name"/> was null.</exception>
        public static XElement EnsureDevElement(this XElement parent, string name, Action<XElement> onNew, Action<XElement> onExists = null)
        {
            return parent.EnsureDevElement(name, false, onNew, onExists);
        }

        ///// <summary>
        ///// Gets first matching dev element or inserts a new one if none is found.
        ///// </summary>
        ///// <param name="name">Name of dev element to search for.</param>
        ///// <param name="refNode">Reference <seealso cref="XNode"/> which determines where any new <seealso cref="XElement"/> will be inserted.</param>
        ///// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        ///// <param name="addBefore">If <c>true</c>, and a new element is created, it is inserted before <paramref name="refNode"/>;
        ///// otherwise, any new element is inserted after <paramref name="refNode"/>.</param>
        ///// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        ///// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        ///// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        ///// <exception cref="ArgumentNullException"><paramref name="name"/>, <paramref name="refNode"/>, or <paramref name="predicate"/> was null.</exception>
        //public static XElement EnsureDevElement(this string name, XNode refNode, Func<XElement, bool> predicate, bool addBefore, params XName[] predicateNames)
        //{ return name.EnsureDevElement(refNode, predicate, addBefore, null, null, predicateNames); }

        /// <summary>
        /// Gets first matching dev element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of dev element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="addFirst">If <c>true</c>, and a new element is created, it is inserted as the first child of <paramref name="parent"/>;
        /// otherwise, any new element is added as the last child of <paramref name="parent"/>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureDevElement(this XElement parent, string name, Func<XElement, bool> predicate, bool addFirst, params XName[] predicateNames)
        { return parent.EnsureDevElement(name, predicate, addFirst, null, null, predicateNames); }

        /// <summary>
        /// Gets first matching dev element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of dev element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="onNew"><seealso cref="Action{T}"/> which gets invoked when a new <seealso cref="XElement"/> is inserted.
        /// This can be <c>null</c>.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureDevElement(this XElement parent, string name, Func<XElement, bool> predicate, Action<XElement> onNew, params XName[] predicateNames)
        { return parent.EnsureDevElement(name, predicate, onNew, null, predicateNames); }

        /// <summary>
        /// Gets first matching dev element or adds a new one if none is found.
        /// </summary>
        /// <param name="parent">Parent element.</param>
        /// <param name="name">Name of dev element to search for.</param>
        /// <param name="predicate">Predicate which gets invoked to determine if element is a match.</param>
        /// <param name="predicateNames">Additional nested child element names to search when looking for matches. The nested child element matching the last
        /// <seealso cref="XName"/> is passed to <paramref name="predicate"/> to determine if it is a match.</param>
        /// <returns>The first matching <seealso cref="XElement"/> if a match is found; otherwise, the newly inserted <seealso cref="XElement"/> is returned.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="parent"/>, <paramref name="name"/>, or <paramref name="predicate"/> was null.</exception>
        public static XElement EnsureDevElement(this XElement parent, string name, Func<XElement, bool> predicate, params XName[] predicateNames)
        { return parent.EnsureDevElement(name, predicate, null, null, predicateNames); }

        #endregion

        public static XmlQualifiedName ToXmlQualifiedName(XName name)
        {
            if (name == null)
                return null;

            return new XmlQualifiedName(name.LocalName, name.NamespaceName);
        }

        public static IEnumerable<XmlQualifiedName> ToXmlQualifiedName(IEnumerable<XName> name)
        {
            if (name == null)
                yield break;

            foreach (XName n in name)
            {
                if (n != null)
                    yield return ToXmlQualifiedName(n);
            }
        }

        public static bool CanContainText(XName rootName, params XName[] childNames)
        {
            return CanContainText(ToXmlQualifiedName(rootName), ToXmlQualifiedName(childNames));
        }

        public static XmlSchemaObject GetSchema(XName rootName, params XName[] childNames)
        {
            return GetSchema(ToXmlQualifiedName(rootName), ToXmlQualifiedName(childNames));
        }

        private static XmlSchemaSet _mamlSchemaSet = new XmlSchemaSet();

        public static XmlSchemaObject GetSchema(XmlQualifiedName rootName, params XmlQualifiedName[] childNames)
        {
            return GetSchema(rootName, childNames as IEnumerable<XmlQualifiedName>);
        }
        public static XmlSchemaObject GetSchema(XmlQualifiedName rootName, IEnumerable<XmlQualifiedName> names)
        {
            if (!_mamlSchemaSet.GlobalElements.Contains(rootName))
                return null;

            XmlSchemaObject result = _mamlSchemaSet.GlobalElements[rootName];
            foreach (XmlQualifiedName n in names)
            {
                if ((result = GetSchema(result, n)) == null)
                    break;
            }

            return result;
        }

        private static XmlSchemaObject GetSchema(XmlSchemaObject obj, XmlQualifiedName name)
        {
            if (obj == null)
                return null;

            if (obj is XmlSchemaElement)
            {
                XmlSchemaElement e = obj as XmlSchemaElement;
                if (e.ElementSchemaType == null)
                    throw new NotSupportedException("ElementSchemaType is null");
                obj = e.ElementSchemaType;
            }
            if (obj is XmlSchemaComplexType)
            {
                XmlSchemaComplexType ct = obj as XmlSchemaComplexType;
                if (ct.ContentTypeParticle == null)
                    throw new NotSupportedException("ContentTypeParticle is null");
                obj = ct.ContentTypeParticle;
            }
            if (obj is XmlSchemaSequence)
                return GetSchema((obj as XmlSchemaSequence).Items, name);
            if (obj is XmlSchemaChoice)
                return GetSchema((obj as XmlSchemaChoice).Items, name);
            if (obj is XmlSchemaAll)
                return GetSchema((obj as XmlSchemaAll).Items, name);
            throw new NotSupportedException("Unexpected type: " + obj.GetType().FullName);
        }

        private static XmlSchemaObject GetSchema(XmlSchemaObjectCollection itemCollection, XmlQualifiedName name)
        {
            foreach (XmlSchemaObject item in itemCollection)
            {
                if (item is XmlSchemaElement)
                {
                    XmlSchemaElement e = item as XmlSchemaElement;
                    if (e.QualifiedName.Name == name.Name && e.QualifiedName.Namespace == name.Namespace)
                        return e;
                }
                else
                {
                    XmlSchemaObject o = GetSchema(item, name);
                    if (o != null)
                        return o;
                }
            }

            return null;
        }

        public static List<XmlQualifiedName> GetParentNames(XElement element)
        {
            List<XmlQualifiedName> result = new List<XmlQualifiedName>();
            if (element == null)
                return result;
            result.Add(ToXmlQualifiedName(element.Name));
            while ((element = element.Parent) != null)
                result.Insert(0, ToXmlQualifiedName(element.Name));
            return result;
        }

        public static bool CanContainText(XElement element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            List<XmlQualifiedName> names = GetParentNames(element);
            XmlQualifiedName rootName = names[0];
            names.RemoveAt(0);
            return CanContainText(rootName, names);
        }

        public static bool CanContainText(XmlQualifiedName rootName, params XmlQualifiedName[] childNames)
        {
            return CanContainText(rootName, childNames as IEnumerable<XmlQualifiedName>);
        }
        public static bool CanContainText(XmlQualifiedName rootName, IEnumerable<XmlQualifiedName> names)
        {
            XmlSchemaObject obj = GetSchema(rootName, names);
            if (obj == null)
                throw new ArgumentException("Unknown item", "rootName");

            if (obj is XmlSchemaElement)
            {
                XmlSchemaElement element = obj as XmlSchemaElement;
                obj = element.ElementSchemaType;
            }

            if (obj is XmlSchemaSimpleType || obj is XmlSchemaAttribute)
                return true;

            return obj is XmlSchemaComplexType && (obj as XmlSchemaComplexType).IsMixed;
        }

        public static bool IsValidChildTree(XElement element, params XName[] childNames)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            List<XmlQualifiedName> names = GetParentNames(element);
            if (childNames != null)
            {
                foreach (XName n  in childNames)
                {
                    if (n != null)
                        names.Add(ToXmlQualifiedName(n));
                }
            }
            XmlQualifiedName rootName = names[0];
            names.RemoveAt(0);
            return IsValidChildTree(rootName, names);
        }

        public static bool IsValidChildTree(XName rootName, params XName[] childNames) { return IsValidChildTree(ToXmlQualifiedName(rootName), ToXmlQualifiedName(childNames)); }

        public static bool IsValidChildTree(XName rootName, IEnumerable<XName> childNames) { return IsValidChildTree(ToXmlQualifiedName(rootName), ToXmlQualifiedName(childNames)); }

        public static bool IsValidChildTree(XmlQualifiedName rootName, params XmlQualifiedName[] childNames) { return IsValidChildTree(rootName, childNames as IEnumerable<XmlQualifiedName>); }

        public static bool IsValidChildTree(XmlQualifiedName rootName, IEnumerable<XmlQualifiedName> childNames)
        {
            if (!_mamlSchemaSet.GlobalElements.Contains(rootName))
                throw new ArgumentException("Unknown item", "rootName");

            XmlSchemaObject item = _mamlSchemaSet.GlobalElements[rootName];
            foreach (XmlQualifiedName n in childNames)
            {
                if ((item = GetSchema(item, n)) == null)
                    return false;
            }

            return true;
        }
        
        /// <summary>
        /// Imports help information for command.
        /// </summary>
        /// <param name="helpItems">Parent element containing command elements.</param>
        /// <param name="commandInfo">Command to import.</param>
        /// <param name="overWrite"><c>true</c> to overwrite existing information; otherwise, false to only import missing information.</param>
        /// <returns><seealso cref="XElement"/> representing PSMaml Help for command.</returns>
        public static XElement ImportCommandElement(this XElement helpItems, CommandInfo commandInfo, bool overWrite = false)
        {
            if (helpItems == null)
                throw new ArgumentNullException("helpItems");

            if (commandInfo == null)
                throw new ArgumentNullException("commandInfo");
            
            XElement commandElement = helpItems.EnsureCommandElement(ElementName_command, e => helpItems.IsTextOnlyMatching(commandInfo.Name),
                XmlNs_command.GetName(ElementName_details), XmlNs_command.GetName(ElementName_name));
            
            #region command:command/command:details

            XElement detailsElement = commandElement.EnsureCommandElement(ElementName_details, true);
            XElement lastDetailsElement = commandElement.EnsureElement(ElementName_name, true, e => e.Add(new XText(commandInfo.Name)), e =>
            {
                if (e.HasNonWhitespaceTextNode())
                    return;
                if (!e.IsEmpty)
                    e.RemoveNodes();
                e.Add(commandInfo.Name);
            });

            #region command:command/command:details/maml:description

            lastDetailsElement = XmlNs_maml.EnsureElement(ElementName_description, lastDetailsElement, false, e => {
                XElement p = e.AddMamlElement(ElementName_para);

                IEnumerable<XElement> summaryElements = (commandInfo is CmdletInfo) ? (commandInfo as CmdletInfo).ImplementingType.GetCommandSummary() : null;
                if (summaryElements != null)
                {
                    foreach (XElement s in summaryElements)
                        p.Add(s);
                }
                if (p.IsEmpty)
                    p.Add(new XComment("Command summary goes here."));
            }, e =>
            {
                XElement[] summaryElements;
                if (overWrite)
                {
                    summaryElements = (commandInfo is CmdletInfo) ? CodeXmlDocHelper.GetCommandSummary((commandInfo as CmdletInfo).ImplementingType).ToArray() : null;
                    if (summaryElements == null || summaryElements.Length > 0 && !e.IsEmpty)
                        e.RemoveNodes();
                }
                else
                {
                    if (e.HasElements)
                        summaryElements = null;
                    else
                    {
                        summaryElements = (commandInfo is CmdletInfo) ? CodeXmlDocHelper.GetCommandSummary((commandInfo as CmdletInfo).ImplementingType).ToArray() : null;
                        if (summaryElements != null && summaryElements.Length > 0 && !e.IsEmpty)
                            e.RemoveNodes();
                    }
                }

                XElement p;
                if (summaryElements != null && summaryElements.Length > 0)
                {
                    if (!e.IsEmpty)
                        e.RemoveNodes();

                    p = new XElement(XmlNs_maml.GetName(ElementName_para));
                    e.Add(p);
                    foreach (XElement s in summaryElements)
                        p.Add(s);
                    return;
                }

                if (e.HasElements)
                {
                    if (e.Elements().Count() > 1 || e.HasNonWhitespaceTextNode() || (p = e.Element(XmlNs_maml.GetName(ElementName_para))) == null)
                        return;

                    if (!p.IsEmpty)
                    {
                        if (p.HasElements || p.HasNonWhitespaceTextNode())
                            return;
                        p.RemoveNodes();
                    }
                }
                else
                {
                    if (!e.IsEmpty)
                        e.RemoveNodes();
                    p = e.AddMamlElement(ElementName_para);
                }

                p.Add(new XComment("Command summary goes here."));
            });

            #endregion

            #region command:command/command:details/maml:copyright

            lastDetailsElement = XmlNs_maml.EnsureElement(ElementName_copyright, lastDetailsElement, false, e =>
            {   
                string s = commandInfo.GetCopyright();
                if (!String.IsNullOrEmpty(s))
                    e.AddMamlElement(ElementName_para, new XText(s));
            }, e =>
            {
                string s;
                if (overWrite)
                {
                    s = commandInfo.GetCopyright();
                    if (String.IsNullOrEmpty(s))
                        return;
                    if (!e.IsEmpty)
                        e.RemoveNodes();
                    e.AddMamlElement(ElementName_para, new XText(s));
                    return;
                }

                if (e.HasNonWhitespaceTextNode())
                    return;

                if (!e.HasElements)
                {
                    s = commandInfo.GetCopyright();
                    if (String.IsNullOrEmpty(s))
                        return;
                    if (!e.IsEmpty)
                        e.RemoveNodes();
                    e.AddMamlElement(ElementName_para, new XText(s));
                    return;
                }

                XElement p;
                if (e.Elements().Count() > 1 || (p = e.Element(XmlNs_maml.GetName(ElementName_para))) == null || p.HasElements ||
                        p.HasNonWhitespaceTextNode())
                    return;

                s = commandInfo.GetCopyright();
                if (String.IsNullOrEmpty(s))
                    return;
                if (!p.IsEmpty)
                    p.RemoveNodes();

                p.Add(new XText(s));
            });

            #endregion

            if (commandInfo is CmdletInfo)
            {
                lastDetailsElement = XmlNs_command.EnsureElement(ElementName_verb, lastDetailsElement, false, e => e.Add(new XText((commandInfo as CmdletInfo).Verb)), e =>
                {
                    string s = (commandInfo as CmdletInfo).Verb;
                    if (e.IsTextOnlyMatching(s))
                        return;
                    if (!e.IsEmpty)
                        e.RemoveNodes();
                    e.Add(new XText(s));
                });
                lastDetailsElement = XmlNs_command.EnsureElement(ElementName_noun, lastDetailsElement, false, e => e.Add(new XText((commandInfo as CmdletInfo).Noun)), e =>
                {
                    string s = (commandInfo as CmdletInfo).Noun;
                    if (e.IsTextOnlyMatching(s))
                        return;
                    if (!e.IsEmpty)
                        e.RemoveNodes();
                    e.Add(new XText(s));
                });
            }
            else if (commandInfo is FunctionInfo)
            {
                lastDetailsElement = XmlNs_command.EnsureElement(ElementName_verb, lastDetailsElement, false, e => e.Add(new XText((commandInfo as FunctionInfo).Verb)), e =>
                {
                    string s = (commandInfo as FunctionInfo).Verb;
                    if (e.IsTextOnlyMatching(s))
                        return;
                    if (!e.IsEmpty)
                        e.RemoveNodes();
                    e.Add(new XText(s));
                });
                lastDetailsElement = XmlNs_command.EnsureElement(ElementName_noun, lastDetailsElement, false, e => e.Add(new XText((commandInfo as FunctionInfo).Noun)), e =>
                {
                    string s = (commandInfo as FunctionInfo).Noun;
                    if (e.IsTextOnlyMatching(s))
                        return;
                    if (!e.IsEmpty)
                        e.RemoveNodes();
                    e.Add(new XText(s));
                });
            }

            lastDetailsElement = XmlNs_dev.EnsureElement(ElementName_version, lastDetailsElement, false, e =>
            {
                Version version = commandInfo.GetVersion();
                if (version == null)
                    return;
                
                if (version.Revision < 1)
                    e.Add(new XText(version.ToString((version.Build < 1) ? 2 : 3)));
                else
                    e.Add(new XText(version.ToString(4)));
            }, e =>
            {
                Version version = commandInfo.GetVersion();
                if (version == null)
                    return;

                if ((e.HasNonWhitespaceTextNode() || e.HasElements) && !overWrite)
                    return;

                if (!e.IsEmpty)
                    e.RemoveNodes();
                if (version.Revision < 1)
                    e.Add(new XText(version.ToString((version.Build < 1) ? 2 : 3)));
                else
                    e.Add(new XText(version.ToString(4)));
            });

            #endregion

            #region command:command/command:description

            XElement descriptionElement = XmlNs_maml.EnsureElement(ElementName_description, detailsElement, false, e => {
                XElement p = e.AddMamlElement(ElementName_para);

                IEnumerable<XElement> summaryElements = (commandInfo is CmdletInfo) ? CodeXmlDocHelper.GetCommandDescription((commandInfo as CmdletInfo).ImplementingType) : null;
                if (summaryElements != null)
                {
                    foreach (XElement s in summaryElements)
                        p.Add(s);
                }
                if (p.IsEmpty)
                    p.Add(new XComment("Command description goes here."));
            }, e =>
            {
                XElement[] summaryElements;
                if (overWrite)
                {
                    summaryElements = (commandInfo is CmdletInfo) ? CodeXmlDocHelper.GetCommandDescription((commandInfo as CmdletInfo).ImplementingType).ToArray() : null;
                    if (summaryElements == null || summaryElements.Length > 0 && !e.IsEmpty)
                        e.RemoveNodes();
                }
                else
                {
                    if (e.HasElements)
                        summaryElements = null;
                    else
                    {
                        summaryElements = (commandInfo is CmdletInfo) ? CodeXmlDocHelper.GetCommandSummary((commandInfo as CmdletInfo).ImplementingType).ToArray() : null;
                        if (summaryElements != null && summaryElements.Length > 0 && !e.IsEmpty)
                            e.RemoveNodes();
                    }
                }

                XElement p;
                if (summaryElements != null && summaryElements.Length > 0)
                {
                    if (!e.IsEmpty)
                        e.RemoveNodes();

                    p = new XElement(XmlNs_maml.GetName(ElementName_para));
                    e.Add(p);
                    foreach (XElement s in summaryElements)
                        p.Add(s);
                    return;
                }

                if (e.HasElements)
                {
                    if (e.Elements().Count() > 1 || e.HasNonWhitespaceTextNode() || (p = e.Element(XmlNs_maml.GetName(ElementName_para))) == null)
                        return;

                    if (!p.IsEmpty)
                    {
                        if (p.HasElements || p.HasNonWhitespaceTextNode())
                            return;
                        p.RemoveNodes();
                    }
                }
                else
                {
                    if (!e.IsEmpty)
                        e.RemoveNodes();
                    p = e.AddMamlElement(ElementName_para);
                }

                p.Add(new XComment("Command description goes here."));
            });

            #endregion

            #region command:command/command:syntax

            XElement syntaxElement = XmlNs_command.EnsureElement(ElementName_syntax, descriptionElement, false);

            // TODO: Import command:command/command:syntax/command:syntaxItem elements

            #endregion

            #region command:command/command:parameters

            XElement parametersElement = XmlNs_command.EnsureElement(ElementName_parameters, syntaxElement, false);

            // TODO: Import command:command/command:parameters/command:parameter elements

            #endregion

            #region command:command/command:inputTypes

            XElement inputTypesElement = XmlNs_command.EnsureElement(ElementName_inputTypes, parametersElement, false);

            // TODO: Import command:command/command:inputTypes/command:inputType elements

            #endregion

            #region command:command/command:returnValues

            XElement returnValuesElement = XmlNs_command.EnsureElement(ElementName_returnValues, inputTypesElement, false);

            // TODO: Import command:command/command:returnValues/command:returnValue elements

            #endregion

            #region command:command/command:terminatingErrors

            XElement terminatingErrorsElement = XmlNs_command.EnsureElement(ElementName_terminatingErrors, returnValuesElement, false);

            // TODO: Import command:command/command:terminatingErrors/command:terminatingError elements

            #endregion

            #region command:command/command:nonTerminatingErrors

            XElement nonTerminatingErrorsElement = XmlNs_command.EnsureElement(ElementName_nonTerminatingErrors, terminatingErrorsElement, false);

            // TODO: Import command:command/command:nonTerminatingErrors/command:nonTerminatingError elements

            #endregion

            #region command:command/command:examples

            XElement examplesElement = XmlNs_command.EnsureElement(ElementName_examples, nonTerminatingErrorsElement, false);

            // TODO: Import command:command/command:examples/command:example elements

            #endregion

            #region command:command/command:relatedLinks

            XElement relatedLinksElement = XmlNs_command.EnsureElement(ElementName_relatedLinks, examplesElement, false);

            // TODO: Import command:command/command:relatedLinks/command:navigationLink elements

            #endregion

            return commandElement;
        }

        /// <summary>
        /// Gets version for command.
        /// </summary>
        /// <param name="commandInfo">Command from which to get version.</param>
        /// <returns>Version for command.</returns>
        public static Version GetVersion(this CommandInfo commandInfo)
        {
            if (commandInfo == null)
                return null;

            Version v;
            if (commandInfo is CmdletInfo && (v = (commandInfo as CmdletInfo).ImplementingType.Assembly.GetVersion()) != null)
                return v;

            return commandInfo.Module.Version;
        }
        
        /// <summary>
        /// Get assembly version.
        /// </summary>
        /// <param name="assembly">Assemly from which to get version.</param>
        /// <returns>Version of assembly.</returns>
        public static Version GetVersion(this Assembly assembly)
        {
            if (assembly == null)
                return null;

            return assembly.GetCustomAttributes(typeof(AssemblyVersionAttribute), false).OfType<AssemblyVersionAttribute>().Select(a => a.Version)
                .Where(s => !String.IsNullOrEmpty(s)).Select(s =>
                {
                    Version v;
                    if (Version.TryParse(s, out v))
                        return v;
                    return null;
                }).FirstOrDefault(v => v != null);
        }
        
        /// <summary>
        /// Get copyright for command.
        /// </summary>
        /// <param name="commandInfo">Command from which to get copyright.</param>
        /// <returns>Copyright associated with command.</returns>
        public static string GetCopyright(this CommandInfo commandInfo)
        {
            if (commandInfo == null)
                return null;

            string c;
            if (commandInfo is CmdletInfo && (c = (commandInfo as CmdletInfo).ImplementingType.Assembly.GetCopyright()) != null)
                return c;

            c = commandInfo.Module.Copyright;
            if (c == null || c.Trim().Length > 0)
                return c;

            return null;
        }

        /// <summary>
        /// Get assembly copyright.
        /// </summary>
        /// <param name="assembly">Assemly from which to get copyright.</param>
        /// <returns>Copyright of assembly.</returns>
        public static string GetCopyright(this Assembly assembly)
        {
            if (assembly == null)
                return null;

            return assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false).OfType<AssemblyCopyrightAttribute>().Select(a => a.Copyright)
                .FirstOrDefault(s => s != null && s.Trim().Length > 0);
        }

        /// <summary>
        /// Gets PSMaml element associated with command.
        /// </summary>
        /// <param name="helpItems">Parent element of command elements.</param>
        /// <param name="name">Name of command.</param>
        /// <returns><seealso cref="XElement"/> representing PSMaml Help for command. Returns <c>null</c> if matching element not found.</returns>
        public static XElement GetCommandElement(this XElement helpItems, string name)
        {
            if (helpItems == null)
                throw new ArgumentNullException("helpItems");

            if (name == null || name.Trim().Length == 0)
                return null;

            XName commandXName = XmlNs_command.GetName(ElementName_command);
            XName detailsXName = XmlNs_command.GetName(ElementName_details);
            XName nameXName = XmlNs_command.GetName(ElementName_name);
            XElement nameElement = helpItems.Elements(commandXName).Elements(detailsXName).Elements(nameXName).FirstOrDefault(e => IsTextOnlyMatching(e, name));
            if (nameElement == null)
                return null;
            return nameElement.Parent.Parent;
        }

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
        public const string ElementName_quote = "quote";
        public const string ElementName_code = "code";
        public const string ElementName_codeReference = "codeReference";

        public static bool CanAddNode(XElement element, XNode node)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            if (node == null)
                throw new ArgumentNullException("node");

            if (node is XComment)
                return true;

            if (node is XText || node is XCData)
            {
                switch (element.Name.NamespaceName)
                {
                    case XmlNsUri_maml:
                        switch (element.Name.LocalName)
                        {
                            case ElementName_para:
                            case ElementName_quote:
                            case ElementName_title:
                            case ElementName_computerOutput:
                            case ElementName_subTitle:
                            case ElementName_uri:
                            case ElementName_term:
                            case ElementName_rubyText:
                            case ElementName_abbreviation:
                            case ElementName_acronym:
                            case ElementName_alertInline:
                            case ElementName_application:
                            case ElementName_codeInline:
                            case ElementName_commandInline:
                            case ElementName_computerOutputInline:
                            case ElementName_conditionalInline:
                            case ElementName_corporation:
                            case ElementName_country:
                            case ElementName_database:
                            case ElementName_date:
                            case ElementName_entityInline:
                            case ElementName_environmentVariable:
                            case ElementName_footnote:
                            case ElementName_foreignPhrase:
                            case ElementName_hardware:
                            case ElementName_internetUri:
                            case ElementName_languageKeyword:
                            case ElementName_literal:
                            case ElementName_localUri:
                            case ElementName_markup:
                            case ElementName_math:
                            case ElementName_newTerm:
                            case ElementName_notLocalizable:
                            case ElementName_phrase:
                            case ElementName_prompt:
                            case ElementName_quoteInline:
                            case ElementName_rangeInline:
                            case ElementName_replaceable:
                            case ElementName_subscript:
                            case ElementName_superscript:
                            case ElementName_token:
                            case ElementName_ui:
                            case ElementName_userInput:
                            case ElementName_trademark:
                            case ElementName_year:
                            case ElementName_holder:
                            case ElementName_objectParameters:
                            case ElementName_caption:
                            case ElementName_summary:
                            case ElementName_key:
                            case ElementName_button:
                            case ElementName_symbolicName:
                            case ElementName_icon:
                            case ElementName_label:
                            case ElementName_menuItem:
                            case ElementName_name:
                            case ElementName_value:
                                return true;
                        }
                        break;
                    case XmlNsUri_command:
                        switch (element.Name.LocalName)
                        {
                            case ElementName_parameterNameInline:
                            case ElementName_parameterValueInline:
                                return true;
                        }
                        break;
                    case XmlNsUri_dev:
                        switch (element.Name.LocalName)
                        {
                            case ElementName_code:
                            case ElementName_codeReference:
                                return true;
                        }
                        break;
                }
                return false;
            }

            if (!(node is XElement))
                return false;

            XName name = (node as XElement).Name;
            switch (element.Name.NamespaceName)
            {
                case XmlNsUri_command:
                    break;
                case XmlNsUri_dev:
                    break;
                case XmlNsUri_maml:
                    switch (element.Name.LocalName)
                    {
                        case ElementName_description:
                            // Allow maml:alertSet, maml:definitionList, maml:example, maml:list, maml:para, maml:quote, maml:table
                            if (name.NamespaceName != XmlNsUri_maml)
                                return false;
                            switch (name.LocalName)
                            {
                                case ElementName_alertSet:
                                case ElementName_definitionList:
                                case ElementName_example:
                                case ElementName_alert:
                                case ElementName_list:
                                case ElementName_para:
                                case ElementName_quote:
                                case ElementName_table:
                                    return true;
                            }
                            break;
                        case ElementName_copyright:
                            return name.NamespaceName == XmlNsUri_maml && name.LocalName == ElementName_para;
                        case ElementName_alertSet:
                            if (name.NamespaceName != XmlNsUri_maml)
                                return false;
                            switch (name.LocalName)
                            {
                                case ElementName_title:
                                    return !element.HasElements;
                                case ElementName_alert:
                                    return true;
                            }
                            break;
                        case ElementName_computerOutput:
                        case ElementName_para:
                        case ElementName_quote:
                        case ElementName_parameterizedBlock:
                        case ElementName_notLocalizable:
                        case ElementName_alertInline:
                        case ElementName_conditionalInline:
                        case ElementName_phrase:
                            // Allow command:parameterNameInline, command:parameterValueInline, maml:abbreviation, maml:acronym, maml:alertInline, maml:application,
                            // maml:codeInline, maml:commandInline, maml:computerOutputInline, maml:conditionalInline, maml:copyrightInline, maml:corporation, maml:country,
                            // maml:database, maml:date, maml:embedObject, maml:entityInline, maml:environmentVariable, maml:footnote, maml:foreignPhrase, maml:glossaryEntryLink,
                            // maml:hardware, maml:internetUri, maml:keyCombinationInline, maml:languageKeyword, maml:lineBreak, maml:literal, maml:localUri, maml:markup, maml:math,
                            // maml:menuSelection, maml:navigationLink, maml:newTerm, maml:notLocalizable, maml:phrase, maml:prompt, maml:quoteInline, maml:rangeInline,
                            // maml:registryEntryInline, maml:replaceable, maml:shellExecuteLink, maml:shortcut, maml:subscript, maml:superscript, maml:token, maml:ui, maml:userInput
                            #region inlines
                            switch (name.NamespaceName)
                            {
                                case XmlNsUri_command:
                                    switch (name.LocalName)
                                    {
                                        case ElementName_parameterNameInline:
                                        case ElementName_parameterValueInline:
                                            return true;
                                    }
                                    break;
                                case XmlNsUri_maml:
                                    switch (name.LocalName)
                                    {
                                        case ElementName_abbreviation:
                                        case ElementName_acronym:
                                        case ElementName_alertInline:
                                        case ElementName_application:
                                        case ElementName_codeInline:
                                        case ElementName_commandInline:
                                        case ElementName_computerOutputInline:
                                        case ElementName_conditionalInline:
                                        case ElementName_copyrightInline:
                                        case ElementName_corporation:
                                        case ElementName_country:
                                        case ElementName_database:
                                        case ElementName_date:
                                        case ElementName_embedObject:
                                        case ElementName_entityInline:
                                        case ElementName_environmentVariable:
                                        case ElementName_footnote:
                                        case ElementName_foreignPhrase:
                                        case ElementName_glossaryEntryLink:
                                        case ElementName_hardware:
                                        case ElementName_internetUri:
                                        case ElementName_keyCombinationInline:
                                        case ElementName_languageKeyword:
                                        case ElementName_lineBreak:
                                        case ElementName_literal:
                                        case ElementName_localUri:
                                        case ElementName_markup:
                                        case ElementName_math:
                                        case ElementName_menuSelection:
                                        case ElementName_navigationLink:
                                        case ElementName_newTerm:
                                        case ElementName_notLocalizable:
                                        case ElementName_phrase:
                                        case ElementName_prompt:
                                        case ElementName_quoteInline:
                                        case ElementName_rangeInline:
                                        case ElementName_registryEntryInline:
                                        case ElementName_replaceable:
                                        case ElementName_shellExecuteLink:
                                        case ElementName_shortcut:
                                        case ElementName_subscript:
                                        case ElementName_superscript:
                                        case ElementName_token:
                                        case ElementName_ui:
                                        case ElementName_userInput:
                                            return true;
                                    }
                                    break;
                            }
                            #endregion
                            break;
                        case ElementName_definitionList:
                            return name.NamespaceName == XmlNsUri_maml && name.LocalName == ElementName_definitionListItem;
                        case ElementName_definitionListItem:
                            if (name.NamespaceName != XmlNsUri_maml)
                                return false;
                            switch (name.LocalName)
                            {
                                case ElementName_term:
                                    return !element.HasElements;
                                case ElementName_definition:
                                    return element.HasElements && element.Elements().Count() == 1;
                                case ElementName_uri:
                                    return element.HasElements && element.Elements().Count() == 2;
                            }
                            break;
                        case ElementName_alert:
                        case ElementName_example:
                        case ElementName_listItem:
                        case ElementName_introduction:
                        case ElementName_step:
                            // Allow dev:code, dev:codeReference, maml:alertSet, maml:computerOutput, maml:definitionList, maml:example, maml:list, maml:para,
                            // maml:parameterizedBlock, maml:procedure, maml:quote, maml:sections, maml:table
                            #region item content
                            switch (name.NamespaceName)
                            {
                                case XmlNsUri_dev:
                                    switch (name.LocalName)
                                    {
                                        case ElementName_code:
                                        case ElementName_codeReference:
                                            return true;
                                    }
                                    break;
                                case XmlNsUri_maml:
                                    switch (name.LocalName)
                                    {
                                        case ElementName_alertSet:
                                        case ElementName_computerOutput:
                                        case ElementName_definitionList:
                                        case ElementName_example:
                                        case ElementName_list:
                                        case ElementName_para:
                                        case ElementName_parameterizedBlock:
                                        case ElementName_procedure:
                                        case ElementName_quote:
                                        case ElementName_sections:
                                        case ElementName_table:
                                            return true;
                                    }
                                    break;
                                case XmlNsUri_command:
                                    break;
                            }
                            #endregion
                            break;
                        case ElementName_list:
                            return name.NamespaceName == XmlNsUri_maml && name.LocalName == ElementName_listItem;
                        case ElementName_procedure:
                            if (name.NamespaceName != XmlNsUri_maml)
                                return false;
                            switch (name.LocalName)
                            {
                                case ElementName_title:
                                    return !element.HasElements;
                                case ElementName_introduction:
                                    if (!element.HasElements)
                                        return true;
                                    return (element.Elements().Count() == 1 && element.Elements(XmlNs_maml.GetName(ElementName_title)).Count() == 1);
                                case ElementName_step:
                                    return element.Elements(XmlNs_maml.GetName(ElementName_uri)).Count() == 0;
                                case ElementName_uri:
                                    return element.HasElements && element.Elements(XmlNs_maml.GetName(ElementName_step)).Count() > 0 && element.Elements(XmlNs_maml.GetName(ElementName_uri)).Count() == 0;
                            }
                            break;
                        case ElementName_sections:
                            if (name.NamespaceName != XmlNsUri_maml)
                                return false;
                            switch (name.LocalName)
                            {
                                case ElementName_pullQuote:
                                case ElementName_section:
                                case ElementName_sidebar:
                                    return true;
                            }
                            break;
                        case ElementName_table:
                            #region Table content
                            if (name.NamespaceName != XmlNsUri_maml)
                                return false;
                            switch (name.LocalName)
                            {
                                case ElementName_title:
                                    return !element.HasElements;
                                case ElementName_summary:
                                    return !element.HasElements || (element.Elements().Count() == 1 && element.Elements(XmlNs_maml.GetName(ElementName_title)).Count() == 1);
                                case ElementName_introduction:
                                    if (!element.HasElements)
                                        return true;
                                    if (element.Elements().Count() == 1)
                                        return element.Elements(XmlNs_maml.GetName(ElementName_title)).Count() == 1 || 
                                            element.Elements(XmlNs_maml.GetName(ElementName_summary)).Count() == 1;
                                    return element.Elements().Count() == 2 && element.Elements(XmlNs_maml.GetName(ElementName_title)).Count() == 1 && 
                                        element.Elements(XmlNs_maml.GetName(ElementName_summary)).Count() == 1;
                                case ElementName_tableHeader:
                                    return (element.Elements().Count() < 4 && element.Elements(XmlNs_maml.GetName(ElementName_tableHeader)).Count() == 0 &&
                                        element.Elements(XmlNs_maml.GetName(ElementName_row)).Count() == 0);
                                case ElementName_row:
                                    return element.Elements(XmlNs_maml.GetName(ElementName_tableFooter)).Count() == 0 && 
                                        element.Elements(XmlNs_maml.GetName(ElementName_alertSet)).Count() == 0;
                                case ElementName_tableFooter:
                                    return element.Elements(XmlNs_maml.GetName(ElementName_row)).Count() > 0 && element.Elements(XmlNs_maml.GetName(ElementName_tableFooter)).Count() == 0 &&
                                        element.Elements(XmlNs_maml.GetName(ElementName_alertSet)).Count() == 0;
                                case ElementName_alertSet:
                                    return element.Elements(XmlNs_maml.GetName(ElementName_row)).Count() > 0;
                            }
                            #endregion
                            break;
                        case ElementName_tableHeader:
                        case ElementName_row:
                        case ElementName_tableFooter:
                        case ElementName_pullQuote:
                        case ElementName_sidebar:
                        case ElementName_section:
                            switch (name.NamespaceName)
                            {
                                case XmlNsUri_dev:
                                    switch (name.LocalName)
                                    {
                                        case ElementName_code:
                                        case ElementName_codeReference:
                                            return true;
                                    }
                                    break;
                                case XmlNsUri_maml:
                                    switch (name.LocalName)
                                    {
                                        case ElementName_title:
                                            return !element.HasElements;
                                        case ElementName_subTitle:
                                            return !element.HasElements || (element.Elements().Count() == 1 && element.Elements(XmlNs_maml.GetName(ElementName_title)).Count() == 1);
                                        case ElementName_introduction:
                                            if (!element.HasElements)
                                                return false;
                                            if (element.Elements().Count() == 1)
                                                return element.Elements(XmlNs_maml.GetName(ElementName_title)).Count() == 1;
                                            return element.Elements().Count() == 2 && element.Elements(XmlNs_maml.GetName(ElementName_title)).Count() == 1 &&
                                                element.Elements(XmlNs_maml.GetName(ElementName_subTitle)).Count() == 1;
                                        case ElementName_alertSet:
                                        case ElementName_computerOutput:
                                        case ElementName_definitionList:
                                        case ElementName_example:
                                        case ElementName_list:
                                        case ElementName_para:
                                        case ElementName_parameterizedBlock:
                                        case ElementName_procedure:
                                        case ElementName_quote:
                                        case ElementName_sections:
                                        case ElementName_table:
                                            return true;
                                    }
                                    break;
                                case XmlNsUri_command:
                                    break;
                            }
                            break;
                        case ElementName_term:
                            return name.NamespaceName == XmlNsUri_maml && name.LocalName == ElementName_rubyText && element.Elements(XmlNs_maml.GetName(ElementName_rubyText)).Count() == 0;
                        case ElementName_copyrightInline:
                            if (name.NamespaceName != XmlNsUri_maml)
                                return false;
                            switch (name.LocalName)
                            {
                                case ElementName_trademark:
                                    return !element.HasElements;
                                case ElementName_year:
                                    return element.HasElements && element.Elements().Count() == 1 && element.Elements(XmlNs_maml.GetName(ElementName_trademark)).Count() == 1;
                                case ElementName_holder:
                                    return element.HasElements && element.Elements(XmlNs_maml.GetName(ElementName_trademark)).Count() == 1;
                            }
                            break;
                        case ElementName_embedObject:
                            if (name.NamespaceName != XmlNsUri_maml)
                                return false;
                            switch (name.LocalName)
                            {
                                case ElementName_caption:
                                    return !element.HasElements;
                                case ElementName_objectUri:
                                    return !element.HasElements || (element.Elements().Count() == 1 && element.Elements(XmlNs_maml.GetName(ElementName_caption)).Count() == 1 &&
                                        element.Elements(XmlNs_maml.GetName(ElementName_summary)).Count() == 0);
                                case ElementName_summary:
                                    return element.HasElements && element.Elements(XmlNs_maml.GetName(ElementName_objectUri)).Count() > 1 &&
                                        element.Elements(XmlNs_maml.GetName(ElementName_summary)).Count() == 0;
                            }
                            break;
                        case ElementName_objectParameters:
                            return name.NamespaceName == XmlNsUri_maml && name.LocalName == ElementName_objectParameter;
                        case ElementName_glossaryEntryLink:
                            if (name.NamespaceName != XmlNsUri_maml)
                                return false;
                            switch (name.LocalName)
                            {
                                case ElementName_linkText:
                                    return !element.HasElements;
                                case ElementName_uri:
                                    return !element.HasElements || (element.Elements().Count() == 1 && element.Elements(XmlNs_maml.GetName(ElementName_linkText)).Count() == 1);
                            }
                            break;
                        case ElementName_keyCombinationInline:
                        case ElementName_shortcut:
                            if (name.NamespaceName != XmlNsUri_maml)
                                return false;
                            switch (name.LocalName)
                            {
                                case ElementName_key:
                                case ElementName_button:
                                case ElementName_symbolicName:
                                    return true;
                            }
                            break;
                        case ElementName_localUri:
                            return name.NamespaceName == XmlNsUri_maml && name.LocalName == ElementName_replaceable;
                        case ElementName_menu:
                            return name.NamespaceName == XmlNsUri_maml && name.LocalName == ElementName_menuItem;
                        case ElementName_menuSelection:
                            if (name.NamespaceName != XmlNsUri_maml)
                                return false;
                            switch (name.LocalName)
                            {
                                case ElementName_key:
                                case ElementName_button:
                                case ElementName_symbolicName:
                                case ElementName_icon:
                                case ElementName_label:
                                case ElementName_menu:
                                    return true;
                            }
                            break;
                        case ElementName_navigationLink:
                            if (name.NamespaceName != XmlNsUri_maml)
                                return false;
                            switch (name.LocalName)
                            {
                                case ElementName_linkText:
                                case ElementName_uri:
                                    return true;
                            }
                            break;
                        case ElementName_linkText:
                            if (name.NamespaceName != XmlNsUri_maml)
                                return false;
                            switch (name.LocalName)
                            {
                                case ElementName_embedObject:
                                case ElementName_notLocalizable:
                                    return true;
                            }
                            break;
                        case ElementName_registryEntryInline:
                            if (name.NamespaceName != XmlNsUri_maml)
                                return false;
                            switch (name.LocalName)
                            {
                                case ElementName_name:
                                    return !element.HasElements;
                                case ElementName_key:
                                    return !element.HasElements || (element.Elements().Count() == 1 && element.Elements(XmlNs_maml.GetName(ElementName_name)).Count() == 1);
                                case ElementName_value:
                                    return !element.HasElements || (element.Elements().Count() > 1 && element.Elements(XmlNs_maml.GetName(ElementName_name)).Count() == 1 &&
                                        element.Elements(XmlNs_maml.GetName(ElementName_key)).Count() == 1);
                            }
                            break;
                        case ElementName_shellExecuteLink:
                            if (name.NamespaceName != XmlNsUri_maml)
                                return false;
                            switch (name.LocalName)
                            {
                                case ElementName_linkText:
                                    return !element.HasElements;
                                case ElementName_command:
                                    return element.HasElements && element.Elements().Count() == 1 && element.Elements(XmlNs_maml.GetName(ElementName_linkText)).Count() == 1;
                                case ElementName_summary:
                                    return element.HasElements && element.Elements().Count() == 2 && element.Elements(XmlNs_maml.GetName(ElementName_linkText)).Count() == 1 &&
                                        element.Elements(XmlNs_maml.GetName(ElementName_command)).Count() == 1;
                            }
                            break;
                        case ElementName_userInput:
                            if (name.NamespaceName != XmlNsUri_maml)
                                return false;
                            switch (name.LocalName)
                            {
                                case ElementName_commandInline:
                                    return !element.HasElements;
                                case ElementName_replaceable:
                                    return element.HasElements && element.Elements().Count() == 1 && element.Elements(XmlNs_maml.GetName(ElementName_linkText)).Count() == 1;
                            }
                            break;
                        case ElementName_abbreviation:
                        case ElementName_acronym:
                        case ElementName_application:
                        case ElementName_codeInline:
                        case ElementName_commandInline:
                        case ElementName_computerOutputInline:
                        case ElementName_corporation:
                        case ElementName_country:
                        case ElementName_database:
                        case ElementName_caption:
                        case ElementName_summary:
                        case ElementName_environmentVariable:
                        case ElementName_foreignPhrase:
                        case ElementName_hardware:
                        case ElementName_internetUri:
                        case ElementName_languageKeyword:
                        case ElementName_literal:
                        case ElementName_markup:
                        case ElementName_math:
                        case ElementName_newTerm:
                        case ElementName_quoteInline:
                        case ElementName_rangeInline:
                        case ElementName_prompt:
                        case ElementName_name:
                        case ElementName_replaceable:
                        case ElementName_token:
                        case ElementName_ui:
                            return name.NamespaceName == XmlNsUri_maml && name.LocalName == ElementName_notLocalizable;
                    }
                    break;
            }

            return false;
        }

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
        public const string ElementName_value = "value";
        public const string ElementName_icon = "icon";
        public const string ElementName_label = "label";
        public const string ElementName_menu = "menu";
        public const string ElementName_menuItem = "menuItem";
        public const string ElementName_key = "key";
        public const string ElementName_button = "button";
        public const string ElementName_symbolicName = "symbolicName";
        public const string ElementName_linkText = "linkText";
        public const string ElementName_objectUri = "objectUri";
        public const string ElementName_objectParameters = "objectParameters";
        public const string ElementName_objectParameter = "objectParameter";
        public const string ElementName_caption = "caption";
        public const string ElementName_trademark = "trademark";
        public const string ElementName_year = "year";
        public const string ElementName_holder = "holder";
        public const string ElementName_rubyText = "rubyText";
        public const string ElementName_subTitle = "subTitle";
        public const string ElementName_pullQuote = "pullQuote";
        public const string ElementName_sidebar = "sidebar";
        public const string ElementName_step = "step";
        public const string ElementName_computerOutput = "computerOutput";
        public const string ElementName_parameterizedBlock = "parameterizedBlock";
        public const string ElementName_procedure = "procedure";
        public const string ElementName_sections = "sections";
        public const string ElementName_section = "section";
        public const string ElementName_parameterNameInline = "parameterNameInline";
        public const string ElementName_parameterValueInline = "parameterValueInline";
        public const string ElementName_abbreviation = "";
        public const string ElementName_acronym = "acronym";
        public const string ElementName_alertInline = "alertInline";
        public const string ElementName_application = "application";
        public const string ElementName_codeInline = "codeInline";
        public const string ElementName_commandInline = "commandInline";
        public const string ElementName_computerOutputInline = "computerOutputInline";
        public const string ElementName_conditionalInline = "conditionalInline";
        public const string ElementName_copyrightInline = "copyrightInline";
        public const string ElementName_corporation = "corporation";
        public const string ElementName_country = "country";
        public const string ElementName_database = "database";
        public const string ElementName_date = "date";
        public const string ElementName_embedObject = "embedObject";
        public const string ElementName_entityInline = "entityInline";
        public const string ElementName_environmentVariable = "environmentVariable";
        public const string ElementName_footnote = "footnote";
        public const string ElementName_foreignPhrase = "foreignPhrase";
        public const string ElementName_glossaryEntryLink = "glossaryEntryLink";
        public const string ElementName_hardware = "hardware";
        public const string ElementName_internetUri = "internetUri";
        public const string ElementName_keyCombinationInline = "keyCombinationInline";
        public const string ElementName_languageKeyword = "languageKeyword";
        public const string ElementName_lineBreak = "lineBreak";
        public const string ElementName_literal = "literal";
        public const string ElementName_localUri = "localUri";
        public const string ElementName_markup = "markup";
        public const string ElementName_math = "math";
        public const string ElementName_menuSelection = "menuSelection";
        public const string ElementName_navigationLink = "navigationLink";
        public const string ElementName_newTerm = "newTerm";
        public const string ElementName_notLocalizable = "notLocalizable";
        public const string ElementName_phrase = "phrase";
        public const string ElementName_prompt = "prompt";
        public const string ElementName_quoteInline = "quoteInline";
        public const string ElementName_rangeInline = "rangeInline";
        public const string ElementName_registryEntryInline = "registryEntryInline";
        public const string ElementName_replaceable = "replaceable";
        public const string ElementName_shellExecuteLink = "shellExecuteLink";
        public const string ElementName_shortcut = "shortcut";
        public const string ElementName_subscript = "subscript";
        public const string ElementName_superscript = "superscript";
        public const string ElementName_token = "token";
        public const string ElementName_ui = "ui";
        public const string ElementName_userInput = "userInput";
        public const string ElementName_listItem = "listItem";

        public static XElement ToPSMaml(XElement xmlDocElement)
        {
            if (xmlDocElement == null)
                throw new ArgumentNullException("xmlDocElement");

            XElement element;
            XAttribute attribute;
            switch (xmlDocElement.Name.LocalName)
            {
                case CodeXmlDocHelper.ElementName_see:
                case CodeXmlDocHelper.ElementName_seealso:
                    attribute = xmlDocElement.Attribute("cref");
                    if (attribute == null)
                        return null;
                    return new XElement(XmlNs_maml.GetName(ElementName_token), attribute.Value);
                case CodeXmlDocHelper.ElementName_c:
                case CodeXmlDocHelper.ElementName_code:
                    element = new XElement(XmlNs_maml.GetName(ElementName_codeInline));
                    break;
                case CodeXmlDocHelper.ElementName_exception:
                case CodeXmlDocHelper.ElementName_param:
                case CodeXmlDocHelper.ElementName_paramref:
                    attribute = xmlDocElement.Attribute("namea");
                    if (attribute == null)
                        return null;
                    return new XElement(XmlNs_maml.GetName(ElementName_parameterNameInline), attribute.Value);
                case CodeXmlDocHelper.ElementName_list:
                case CodeXmlDocHelper.ElementName_term:
                case CodeXmlDocHelper.ElementName_item:
                case CodeXmlDocHelper.ElementName_permission:
                case CodeXmlDocHelper.ElementName_typeparam:
                case CodeXmlDocHelper.ElementName_include:
                case CodeXmlDocHelper.ElementName_typeparamref:
                case CodeXmlDocHelper.ElementName_returns:
                case CodeXmlDocHelper.ElementName_value:
                case CodeXmlDocHelper.ElementName_name:
                case CodeXmlDocHelper.ElementName_description:
                case CodeXmlDocHelper.ElementName_example:
                case CodeXmlDocHelper.ElementName_para:
                    element = new XElement(XmlNs_maml.GetName(ElementName_para));
                    break;
                default:
#warning Not implemented
                    throw new NotImplementedException();
            }
            if (!xmlDocElement.IsEmpty)
            {
                foreach (XNode node in xmlDocElement.Nodes())
                {
                    if (node is XCData)
                        element.Add(new XText((node as XText).Value));
                    else if (node is XCData)
                        element.Add(new XText((node as XCData).Value));
                    else if (node is XElement)
                    {
                        XElement e = ToPSMaml(node as XElement);
                        if (e != null)
                            element.Add(e);
                    }
                }
            }

            return element;
        }
        public static void ImportPSMaml(XElement psMamlParent, XElement xmlDocElement)
        {
            XElement result;
            if (xmlDocElement.Name.NamespaceName.Length > 0)
                result = new XElement(xmlDocElement.Name);
            else
            {
                switch (xmlDocElement.Name.LocalName)
                {
                    case CodeXmlDocHelper.ElementName_see:
                    case CodeXmlDocHelper.ElementName_seealso:
                    case CodeXmlDocHelper.ElementName_c:
                        break;
                    case CodeXmlDocHelper.ElementName_exception:
                    case CodeXmlDocHelper.ElementName_param:
                    case CodeXmlDocHelper.ElementName_paramref:
                    case CodeXmlDocHelper.ElementName_code:
                    case CodeXmlDocHelper.ElementName_list:
                    case CodeXmlDocHelper.ElementName_term:
                    case CodeXmlDocHelper.ElementName_item:
                    case CodeXmlDocHelper.ElementName_permission:
                    case CodeXmlDocHelper.ElementName_typeparam:
                    case CodeXmlDocHelper.ElementName_include:
                    case CodeXmlDocHelper.ElementName_typeparamref:
                    case CodeXmlDocHelper.ElementName_returns:
                    case CodeXmlDocHelper.ElementName_value:
                    case CodeXmlDocHelper.ElementName_name:
                        break;
                    case CodeXmlDocHelper.ElementName_description:
                    case CodeXmlDocHelper.ElementName_para:
                        result = new XElement(XmlNs_maml.GetName(xmlDocElement.Name.LocalName));
                        break;
                    case CodeXmlDocHelper.ElementName_example:
                        result = new XElement(XmlNs_command.GetName(xmlDocElement.Name.LocalName));
                        break;

                }
            }
        }

        private static void ImportPSMamlBlocks(XElement psMamlParent, XElement xmlDocElement)
        {
            // Allow list, example, quote, alertSet, definitionList, table, para
#warning Not implemented
            throw new NotImplementedException();
        }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
    }
}

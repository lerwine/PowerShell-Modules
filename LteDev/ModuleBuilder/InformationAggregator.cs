using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LteDev.ModuleBuilder
{
    /// <summary>
    /// Base class for objects which aggregate module information.
    /// </summary>
    public abstract class InformationAggregator
    {
        /// <summary>
        /// Namespace for the PSMaml Command schema
        /// </summary>
        public static readonly XNamespace NS_command = XNamespace.Get("http://schemas.microsoft.com/maml/dev/command/2004/10");

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
        public static class maml
        {
            /// <summary>
            /// Namespace for the PSMaml Maml schema
            /// </summary>
            public static readonly XNamespace NS = XNamespace.Get("http://schemas.microsoft.com/maml/2004/10");

            private static XName _para = null;
            public static XName para
            {
                get
                {
                    if (_para == null)
                        _para = NS.GetName("para");
                    return _para;
                }
            }

            private static XName _name = null;
            public static XName name
            {
                get
                {
                    if (_name == null)
                        _name = NS.GetName("name");
                    return _name;
                }
            }

            private static XName _uri = null;
            public static XName uri
            {
                get
                {
                    if (_uri == null)
                        _uri = NS.GetName("uri");
                    return _uri;
                }
            }

            private static XName _description = null;
            public static XName description
            {
                get
                {
                    if (_description == null)
                        _description = NS.GetName("description");
                    return _description;
                }
            }

            private static XName _copyright = null;
            public static XName copyright
            {
                get
                {
                    if (_copyright == null)
                        _copyright = NS.GetName("copyright");
                    return _copyright;
                }
            }

            private static XName _alertSet= null;
            public static XName alertSet
            {
                get
                {
                    if (_alertSet == null)
                        _alertSet = NS.GetName("alertSet");
                    return _alertSet;
                }
            }

            private static XName _relatedLinks = null;
            public static XName relatedLinks
            {
                get
                {
                    if (_relatedLinks == null)
                        _relatedLinks = NS.GetName("relatedLinks");
                    return _relatedLinks;
                }
            }

            private static XName _navigationLink = null;
            public static XName navigationLink
            {
                get
                {
                    if (_navigationLink == null)
                        _navigationLink = NS.GetName("navigationLink");
                    return _navigationLink;
                }
            }

            private static XName _title = null;
            public static XName title
            {
                get
                {
                    if (_title == null)
                        _title = NS.GetName("title");
                    return _title;
                }
            }

            private static XName _introduction = null;
            public static XName introduction
            {
                get
                {
                    if (_introduction == null)
                        _introduction = NS.GetName("introduction");
                    return _introduction;
                }
            }

            public static IEnumerable<XElement> FromMamlParaTextItems(IEnumerable<PSObject> values)
            {
                return values.Where(o => !o.IsNullOrEmpty()).Select(o =>
                {
                    if (!o.TypeNames.Contains("MamlParaTextItem"))
                        throw new NotImplementedException("Help object type not supported: " + String.Join(", ", o.TypeNames.ToArray()));
                    return o.PSObjectValues("Text").SelectMany(v => v.AsStringValues()).Where(s => s != null).Select(s => s.TrimEnd()).Where(s => s.Length > 0);
                }).Where(s => s.Any()).Select(s => new XElement(para, String.Join(Environment.NewLine, s.ToArray())));
            }
        }

        public static class dev
        {
            /// <summary>
            /// Namespace for the PSMaml Dev schema
            /// </summary>
            public static readonly XNamespace NS = XNamespace.Get("http://schemas.microsoft.com/maml/dev/2004/10");

            private static XName _version = null;
            public static XName version
            {
                get
                {
                    if (_version == null)
                        _version = NS.GetName("version");
                    return _version;
                }
            }

            private static XName _type = null;
            public static XName type
            {
                get
                {
                    if (_type == null)
                        _type = NS.GetName("type");
                    return _type;
                }
            }

            private static XName _code = null;
            public static XName code
            {
                get
                {
                    if (_code == null)
                        _code = NS.GetName("code");
                    return _code;
                }
            }

            private static XName _codeReference = null;
            public static XName codeReference
            {
                get
                {
                    if (_codeReference == null)
                        _codeReference = NS.GetName("codeReference");
                    return _codeReference;
                }
            }

            private static XName _security = null;
            public static XName security
            {
                get
                {
                    if (_security == null)
                        _security = NS.GetName("security");
                    return _security;
                }
            }

            private static XName _remarks = null;
            public static XName remarks
            {
                get
                {
                    if (_remarks == null)
                        _remarks = NS.GetName("remarks");
                    return _remarks;
                }
            }
        }

        /// <summary>
        /// Namespace for the PSMaml PSHelp schema
        /// </summary>
        public static readonly XNamespace NS_MSHelp = XNamespace.Get("http://msdn.microsoft.com/mshelp");
        /// <summary>
        /// Namespace for the PSMaml root schema
        /// </summary>
        public static readonly XNamespace NS_msh = XNamespace.Get("http://msh");

        /// <summary>
        /// Extracts text for a given property from the full help object.
        /// </summary>
        /// <param name="psObject">FullHelp object</param>
        /// <param name="propertyName">
        /// Name of the property for which text needs to be extracted.
        /// </param>
        /// <returns></returns>
        private string ExtractTextForHelpProperty(PSObject psObject, string propertyName)
        {
            if (psObject == null)
                return string.Empty;

            if (psObject.Properties[propertyName] == null ||
                psObject.Properties[propertyName].Value == null)
            {
               return string.Empty;
            }
           
            return ExtractText(PSObject.AsPSObject(psObject.Properties[propertyName].Value));
        }

        /// <summary>
        /// Given a PSObject, this method will traverse through the objects properties,
        /// extracts content from properties that are of type System.String, appends them
        /// together and returns.
        /// </summary>
        /// <param name="psObject"></param>
        /// <returns></returns>
        private string ExtractText(PSObject psObject)
        {
            if (null == psObject)
            {
                return string.Empty;
            }

            // I think every cmdlet description should atleast have 400 characters...
            // so starting with this assumption..I did an average of all the cmdlet
            // help content available at the time of writing this code and came up
            // with this number.
            StringBuilder result = new StringBuilder(400);

            foreach (PSPropertyInfo propertyInfo in psObject.Properties)
            {
                string typeNameOfValue = propertyInfo.TypeNameOfValue;
                switch (typeNameOfValue.ToLowerInvariant())
                {
                    case "system.boolean":
                    case "system.int32":
                    case "system.object":
                    case "system.object[]":
                        continue;
                    case "system.string":
                        result.Append((string)LanguagePrimitives.ConvertTo(propertyInfo.Value,
                            typeof(string), CultureInfo.InvariantCulture));
                        break;
                    case "system.management.automation.psobject[]":
                        PSObject[] items = (PSObject[])LanguagePrimitives.ConvertTo(
                                propertyInfo.Value,
                                typeof(PSObject[]),
                                CultureInfo.InvariantCulture);
                        foreach (PSObject item in items)
                        {
                            result.Append(ExtractText(item));
                        }
                        break;
                    case "system.management.automation.psobject":
                        result.Append(ExtractText(PSObject.AsPSObject(propertyInfo.Value)));
                        break;
                    default:
                        result.Append(ExtractText(PSObject.AsPSObject(propertyInfo.Value)));
                        break;
                }
            }

            return result.ToString();
        }
   }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}

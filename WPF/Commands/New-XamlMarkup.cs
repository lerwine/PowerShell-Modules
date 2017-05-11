using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Xml;

namespace Erwine.Leonard.T.WPF.Commands
{
    /// <summary>
    /// Create empty markup for new window.
    /// </summary>
    /// <remarks>Creates template XAML markup for creating a new window.</remarks>
    [Cmdlet(VerbsCommon.New, "XamlMarkup", RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(XmlDocument))]
    public class New_XamlMarkup : XamlMarkupCmdlet
    {
        public const string ParameterSetName_ImplicitNs = "ImplicitNs";
        public const string ParameterSetName_ExplicitNs = "ExplicitNs";
        public const string DefaultValue_ElementName = "Window";
        [Parameter(ParameterSetName = ParameterSetName_ImplicitNs)]
        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_ExplicitNs)]
        [PSDefaultValue(Value = DefaultValue_ElementName)]
        [ValidateNotNullOrEmpty()]
        public string ElementName { get; set; }

        [Parameter()]
        public string XName { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_ExplicitNs)]
        [ValidateNotNull()]
        [AllowEmptyString()]
        public string NamespaceURI { get; set; }

        protected override void ProcessRecord()
        {
            object targetObject = (String.IsNullOrWhiteSpace(ElementName)) ? DefaultValue_ElementName : ElementName;
            try
            {
                if (ParameterSetName == ParameterSetName_ExplicitNs || NamespaceURI != null)
                {
                    targetObject = new XmlQualifiedName(targetObject as string, NamespaceURI);
                    WriteObject(XamlUtility.CreateXamlMarkup(targetObject as XmlQualifiedName, XName));
                }
                else
                    WriteObject(XamlUtility.CreateXamlMarkup(targetObject as string, XName));
            }
            catch (Exception exception)
            {
                WriteError(new ErrorRecord(exception, "Erwine.Leonard.T.WPF.Commands.New_XamlMarkup", ErrorCategory.InvalidArgument, targetObject));
            }
        }
    }
}

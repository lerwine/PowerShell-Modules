using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Erwine.Leonard.T.WPF.Commands
{
    /// <summary>
    /// Asserts validity of XAML markup.
    /// </summary>
    /// <remarks>Writes an error if the XML Markup is not valid.</remarks>
    [Cmdlet(VerbsLifecycle.Assert, "ValidXamlMarkup", RemotingCapability = RemotingCapability.None, DefaultParameterSetName = ParameterSetName_BooleanOutput)]
    [OutputType(typeof(bool), ParameterSetName = new string[] { ParameterSetName_BooleanOutput })]
    [OutputType(typeof(XamlLoadResult), ParameterSetName = new string[] { ParameterSetName_PassThru })]
    public class Assert_ValidXamlMarkup : XamlMarkupCmdlet
    {
        public const string ParameterSetName_BooleanOutput = "BooleanOutput";
        public const string ParameterSetName_PassThru = "PassThru";

        [Parameter(Mandatory = true, HelpMessage = "XAML markup.", Position = 0, ValueFromPipeline = true, ParameterSetName = ParameterSetName_BooleanOutput)]
        [Parameter(Mandatory = true, HelpMessage = "XAML markup.", Position = 0, ValueFromPipeline = true, ParameterSetName = ParameterSetName_PassThru)]
        [ValidateNotNullOrEmpty()]
        [ValidateType("System.Xml.XmlDocument", "System.Xml.XmlElement", "System.Xml.Linq.XDocument", "System.Xml.Linq.XElement", "System.String")]
        public object[] Xaml { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "Return the XAML parse results instead of a boolean value.", ParameterSetName = ParameterSetName_PassThru)]
        public SwitchParameter ParseResult { get; set; }

        [Parameter(HelpMessage = "Do not write any errors.")]
        public SwitchParameter NoError { get; set; }

        protected override void ProcessRecord()
        {
            if (Xaml == null)
                return;

            XamlLoadResult loadResult;
            
            foreach (object xaml in Xaml)
            {
                bool success;
                if (xaml is string)
                    success = XamlUtility.TryParseXaml(xaml as string, out loadResult);
                else if (xaml is XmlDocument)
                    success = XamlUtility.TryLoadXaml(xaml as XmlDocument, out loadResult);
                else if (xaml is XDocument)
                    success = XamlUtility.TryLoadXaml(xaml as XDocument, out loadResult);
                else if (xaml is XmlElement)
                    success = XamlUtility.TryLoadXaml(xaml as XmlElement, out loadResult);
                else if (xaml is XElement)
                    success = XamlUtility.TryLoadXaml(xaml as XElement, out loadResult);
                else
                    continue;

                if (ParseResult.IsPresent)
                    WriteObject(loadResult);
                else
                    WriteObject(success);
                if (!(success || NoError.IsPresent))
                    WriteError(new ErrorRecord(loadResult.Error, GetType().FullName, ErrorCategory.ParserError, xaml));
            }
        }
    }
}

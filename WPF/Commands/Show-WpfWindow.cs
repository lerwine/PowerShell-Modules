using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Xml;

namespace Erwine.Leonard.T.WPF.Commands
{
    /// <summary>
    /// Show a WPF Window.
    /// </summary>
    /// <remarks>Shows a WPF Window from a proxy object.</remarks>
    [Cmdlet(VerbsCommon.Show, "WpfWindow", RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(XmlDocument))]
    public class Show_WpfWindow : XamlMarkupCmdlet
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public const string ParameterSetName_Existing = "Existing";
        public const string ParameterSetName_ = "";

        [Parameter(Mandatory = true)]
        public WpfWindow Window { get; set; }

        [Parameter()]
        public SwitchParameter Modal { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Xml;

namespace Erwine.Leonard.T.WPF.Commands
{
    /// <summary>
    /// Test-XamlMarkup
    /// </summary>
    [Cmdlet(VerbsDiagnostic.Test, "XamlMarkup", RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(XmlDocument))]
    public class Test_XamlMarkup
    {
    }
}

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LteDev.Commands
{
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
    [Cmdlet(VerbsCommon.Add, "UsingNamespace")]
    [OutputType(typeof(CodeNamespaceImport), typeof(CodeNamespace))]
    public class Add_UsingNamespace : PSCmdlet
    {
        #region Properties

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public CodeNamespace[] Namespace { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty()]
        [ValidateCsNamespace()]
        public string[] Using { get; set; }

        public SwitchParameter ReturnTarget { get; set; }

        #endregion

        #region Overrides

        protected override void ProcessRecord()
        {
        }

        #endregion
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}

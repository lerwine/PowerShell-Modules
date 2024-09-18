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
    [Cmdlet(VerbsCommon.New, "CodeNamespace")]
    [OutputType(typeof(CodeNamespace))]
    public class New_CodeNamespace : PSCmdlet
    {
        #region Properties

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty]
        [ValidateCsNamespace]
        public string[] Name { get; set; } = null!;

        [Parameter()]
        [ValidateNotNullOrEmpty]
        [ValidateCsNamespace]
        public string[]? Using { get; set; }

        #endregion

        #region Overrides

        protected override void ProcessRecord()
        {
        }

        #endregion
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}

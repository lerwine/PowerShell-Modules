using System;
using System.CodeDom.Compiler;
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
    [Cmdlet(VerbsCommon.New, "CodeDomProvider")]
    [OutputType(typeof(CodeDomProvider))]
    public class New_CodeDomProvider : PSCmdlet
    {
        public const string ParameterSetName_CSharp = "CSharp";
        public const string ParameterSetName_VisualBasic = "VisualBasic";
        public const string ParameterSetName_JScript = "JScript";

        #region Properties

        [Parameter(ParameterSetName = ParameterSetName_CSharp)]
        public SwitchParameter CSharp { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_VisualBasic)]
        public SwitchParameter VisualBasic { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = ParameterSetName_JScript)]
        public SwitchParameter JScript { get; set; }

        #endregion

        #region Overrides

        protected override void ProcessRecord()
        {
            if (JScript.IsPresent)
                WriteObject(CodeDomProvider.CreateProvider(ParameterSetName_JScript));
            else if (VisualBasic.IsPresent)
                WriteObject(CodeDomProvider.CreateProvider(ParameterSetName_VisualBasic));
            else
                WriteObject(CodeDomProvider.CreateProvider(ParameterSetName_CSharp));
        }

        #endregion
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}

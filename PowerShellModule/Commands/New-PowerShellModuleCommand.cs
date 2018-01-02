using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace PowerShellModule.Commands
{
    [Cmdlet(VerbsCommon.New, "PowerShellModuleCommand")]
    public class New_PowerShellModuleCommand : PSCmdlet
    {
        [Parameter(ValueFromPipeline = true)]
        [ValidateNotNullOrEmpty()]
        public string[]  InputText { get; set; }

        protected override void ProcessRecord()
        {
            
        }
    }
}

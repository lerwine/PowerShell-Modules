using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace MsExcelUtil.Commands
{

    [Cmdlet(VerbsCommon.Exit, "ExcelAppDependency", RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(ExcelAppDependency))]
    public class Exit_ExcelAppDependency : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [ValidateNotNull()]
        public ExcelAppDependency Dependency { get; set; }

        protected override void ProcessRecord()
        {
            try { Dependency.Dispose(); }
            catch (Exception exception) { WriteError(new ErrorRecord(exception, Strings.ERROR_ID_APP_DEPENDENCY_EXIT_FAIL, ErrorCategory.ResourceBusy, Dependency)); }
        }
    }
}

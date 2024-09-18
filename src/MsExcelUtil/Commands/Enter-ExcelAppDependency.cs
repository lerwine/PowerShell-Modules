using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace MsExcelUtil.Commands
{
    [Cmdlet(VerbsCommon.Enter, "ExcelAppDependency", RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(ExcelAppDependency))]
    public class Enter_ExcelAppDependency : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            try { WriteObject(new ExcelAppDependency()); }
            catch (Exception exception) { WriteError(new ErrorRecord(exception, Strings.ERROR_ID_APP_DEPENDENCY_ENTER_FAIL, ErrorCategory.ResourceBusy, null)); }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace MsExcelUtil.Commands
{

    [Cmdlet(VerbsCommon.Close, "ExcelWorkbook", RemotingCapability = RemotingCapability.None)]
    public class Close_ExcelWorkbook : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [ValidateNotNull()]
        public PSExcelWorkbook[] Workbook { get; set; }

        [Parameter()]
        public SwitchParameter SaveChanges { get; set; }

        protected override void ProcessRecord()
        {
            foreach (PSExcelWorkbook workbook in Workbook)
            {
                try { workbook.DependencyInstance.Exit(SaveChanges.IsPresent); }
                catch (WorkbookDependencyException exception) { WriteError(exception.ErrorRecord); }
                catch (ExcelAppDependencyException exception) { WriteError(exception.ErrorRecord); }
                catch (Exception exception) { WriteError(new ErrorRecord(exception, Strings.ERROR_ID_WORKBOOK_SAVE_FAIL, ErrorCategory.NotSpecified, workbook)); }
            }
        }
    }
}

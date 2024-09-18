using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace MsExcelUtil.Commands
{
    [Cmdlet(VerbsCommon.New, "ExcelWorkbook", RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(PSExcelWorkbook))]
    public class New_ExcelWorkbook : PSCmdlet
    {
        [Parameter(Position = 0, ValueFromPipeline = true, HelpMessage = "Specifies an explicit path to for the new excel workbook.")]
        [ValidateNotNullOrEmpty()]
        [Alias("Path", "FileName", "FullName")]
        public string[] LiteralPath { get; set; }

        protected override void ProcessRecord()
        {
            WorkbookDependency.Instance instance = null;
            try { instance = WorkbookDependency.Enter(null); }
            catch (ExcelAppDependencyException exception) { WriteError(exception.ErrorRecord); }
            catch (Exception exception) { WriteError((exception is IContainsErrorRecord containsError && containsError.ErrorRecord != null) ? containsError.ErrorRecord : new ErrorRecord(exception, Strings.ERROR_ID_WORKBOOK_ADD_FAIL, ErrorCategory.NotSpecified, null)); }
            if (instance != null)
                WriteObject(instance);

        }
    }
}

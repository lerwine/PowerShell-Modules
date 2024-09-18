using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace MsExcelUtil.Commands
{

    [Cmdlet(VerbsData.Save, "ExcelWorkbook", RemotingCapability = RemotingCapability.None, DefaultParameterSetName = ParameterSetName_CurrentPath)]
    [OutputType(typeof(PSExcelWorkbook))]
    public class Save_ExcelWorkbook : PSCmdlet
    {
        public const string ParameterSetName_SaveAs = "SaveAs";
        public const string ParameterSetName_CurrentPath = "CurrentPath";

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = ParameterSetName_SaveAs)]
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = ParameterSetName_CurrentPath)]
        [ValidateNotNull()]
        public PSExcelWorkbook Workbook { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = ParameterSetName_SaveAs)]
        [ValidateNotNullOrEmpty()]
        public string SaveAs { get; set; }

        [Parameter(ParameterSetName = ParameterSetName_CurrentPath)]
        public SwitchParameter Force { get; set; }

        [Parameter()]
        public SwitchParameter Close { get; set; }

        protected override void ProcessRecord()
        {
            if (ParameterSetName == ParameterSetName_CurrentPath)
            {
                try
                {
                    if (Close.IsPresent)
                        Workbook.DependencyInstance.Exit(true, Force.IsPresent);
                    else
                        Workbook.DependencyInstance.SaveChanges(Force.IsPresent);
                }
                catch (WorkbookDependencyException exception) { WriteError(exception.ErrorRecord); }
                catch (ExcelAppDependencyException exception) { WriteError(exception.ErrorRecord); }
                catch (Exception exception) { WriteError(new ErrorRecord(exception, Strings.ERROR_ID_WORKBOOK_SAVE_FAIL, ErrorCategory.NotSpecified, Workbook)); }
            }
            else
            {
                try
                {
                    FileInfo fileInfo = null;
                    try { fileInfo = new FileInfo(SaveAs); }
                    catch (System.Security.SecurityException exception) { WriteError(new ErrorRecord(exception, Strings.ERROR_ID_WORKBOOK_OPEN_UNAUTHORIZED, ErrorCategory.PermissionDenied, SaveAs)); }
                    catch (UnauthorizedAccessException exception) { WriteError(new ErrorRecord(exception, Strings.ERROR_ID_WORKBOOK_OPEN_UNAUTHORIZED, ErrorCategory.PermissionDenied, SaveAs)); }
                    catch (PathTooLongException exception) { WriteError(new ErrorRecord(exception, Strings.ERROR_ID_WORKBOOK_OPEN_INVALIDPATH, ErrorCategory.InvalidArgument, SaveAs)); }
                    catch (NotSupportedException exception) { WriteError(new ErrorRecord(exception, Strings.ERROR_ID_WORKBOOK_OPEN_INVALIDPATH, ErrorCategory.InvalidArgument, SaveAs)); }
                    catch (ArgumentException exception) { WriteError(new ErrorRecord(exception, Strings.ERROR_ID_WORKBOOK_OPEN_INVALIDPATH, ErrorCategory.InvalidArgument, SaveAs)); }
                    catch (Exception exception) { WriteError(new ErrorRecord(exception, Strings.ERROR_ID_WORKBOOK_OPEN_INVALIDPATH, ErrorCategory.NotSpecified, SaveAs)); }
                    if (fileInfo != null)
                    {
                        Workbook.DependencyInstance.SaveAs(fileInfo);
                        if (Close.IsPresent)
                            Workbook.DependencyInstance.Exit();
                    }
                }
                catch (WorkbookDependencyException exception) { WriteError(exception.ErrorRecord); }
                catch (ExcelAppDependencyException exception) { WriteError(exception.ErrorRecord); }
                catch (Exception exception) { WriteError(new ErrorRecord(exception, Strings.ERROR_ID_WORKBOOK_SAVE_FAIL, ErrorCategory.NotSpecified, Workbook)); }
            }
        }
    }
}

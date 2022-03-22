using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace MsExcelUtil.Commands
{
    [Cmdlet(VerbsCommon.Open, "ExcelWorkbook", RemotingCapability = RemotingCapability.None)]
    [OutputType(typeof(PSExcelWorkbook))]
    public class Open_ExcelWorkbook : PSCmdlet
    {
        public const string ParameterSetName_Path = "Path";
        public const string ParameterSetName_LiteralPath = "LiteralPath";

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = ParameterSetName_Path, HelpMessage = "Specifies the path to one or more existing excel workbooks.")]
        [ValidateNotNullOrEmpty()]
        public string[] Path { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = ParameterSetName_LiteralPath, HelpMessage = "Specifies an explicit path to an existing excel workbook or for the path of a new workbook.")]
        [ValidateNotNullOrEmpty()]
        public string[] LiteralPath { get; set; }

        protected override void ProcessRecord()
        {
            if (ParameterSetName == ParameterSetName_LiteralPath)
                foreach (string path in LiteralPath)
                {
                    FileInfo fileInfo = null;
                    try { fileInfo = new FileInfo(path); }
                    catch (System.Security.SecurityException exception) { WriteError(new ErrorRecord(exception, Strings.ERROR_ID_WORKBOOK_OPEN_UNAUTHORIZED, ErrorCategory.PermissionDenied, path)); }
                    catch (UnauthorizedAccessException exception) { WriteError(new ErrorRecord(exception, Strings.ERROR_ID_WORKBOOK_OPEN_UNAUTHORIZED, ErrorCategory.PermissionDenied, path)); }
                    catch (PathTooLongException exception) { WriteError(new ErrorRecord(exception, Strings.ERROR_ID_WORKBOOK_OPEN_INVALIDPATH, ErrorCategory.InvalidArgument, path)); }
                    catch (NotSupportedException exception) { WriteError(new ErrorRecord(exception, Strings.ERROR_ID_WORKBOOK_OPEN_INVALIDPATH, ErrorCategory.InvalidArgument, path)); }
                    catch (ArgumentException exception) { WriteError(new ErrorRecord(exception, Strings.ERROR_ID_WORKBOOK_OPEN_INVALIDPATH, ErrorCategory.InvalidArgument, path)); }
                    catch (Exception exception) { WriteError(new ErrorRecord(exception, Strings.ERROR_ID_WORKBOOK_OPEN_INVALIDPATH, ErrorCategory.NotSpecified, path)); }
                    if (fileInfo != null)
                        ProcessResolvedPath(fileInfo);
                }
            else
                foreach (string wcPath in Path)
                {
                    Collection<string> resolvedPaths = null;
                    try { resolvedPaths = GetResolvedProviderPathFromPSPath(wcPath, out _); }
                    catch (Exception exception)
                    {
                        WriteError((exception is IContainsErrorRecord containsError && containsError.ErrorRecord != null) ? containsError.ErrorRecord : new ErrorRecord(exception, Strings.ERROR_ID_WORKBOOK_OPEN_INVALIDPATH, ErrorCategory.InvalidArgument, wcPath));
                    }
                    if (resolvedPaths != null)
                        foreach (string path in resolvedPaths)
                        {
                            FileInfo fileInfo = null;
                            try { fileInfo = new FileInfo(path); }
                            catch (System.Security.SecurityException exception) { WriteError(new ErrorRecord(exception, Strings.ERROR_ID_WORKBOOK_OPEN_UNAUTHORIZED, ErrorCategory.PermissionDenied, path)); }
                            catch (UnauthorizedAccessException exception) { WriteError(new ErrorRecord(exception, Strings.ERROR_ID_WORKBOOK_OPEN_UNAUTHORIZED, ErrorCategory.PermissionDenied, path)); }
                            catch (Exception exception) { WriteError(new ErrorRecord(exception, Strings.ERROR_ID_WORKBOOK_OPEN_NOTSUPPORTED, ErrorCategory.NotImplemented, path)); }
                            if (fileInfo != null)
                                ProcessResolvedPath(fileInfo);
                        }
                }
        }

        private void ProcessResolvedPath(FileInfo fileInfo)
        {
            WorkbookDependency.Instance instance = null;
            try { instance = WorkbookDependency.Enter(fileInfo); }
            catch (WorkbookDependencyException exception) { WriteError(exception.ErrorRecord); }
            catch (ExcelAppDependencyException exception) { WriteError(exception.ErrorRecord); }
            catch (Exception exception) { WriteError(new ErrorRecord(exception, Strings.ERROR_ID_WORKBOOK_OPEN_READERROR, ErrorCategory.NotSpecified, fileInfo)); }
            if (instance != null)
                WriteObject(instance);
        }
    }
}
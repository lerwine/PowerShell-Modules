using Excel = Microsoft.Office.Interop.Excel;
using System;
using System.IO;
using System.Threading;
using System.Management.Automation;

namespace MsExcelUtil
{
    public sealed partial class WorkbookDependency
    {
        private static readonly object _SYNCROOT = new object();
        private static readonly StringComparer _pathComparer = StringComparer.InvariantCultureIgnoreCase;
        private static ExcelAppDependency _excelAppDependency;
        private readonly object _syncRoot = new object();
        private static WorkbookDependency _first;
        private static WorkbookDependency _last;
        internal Excel.Workbook Workbook { get; private set; }
        private FileInfo _fileInfo;
        private Instance _latest;
        private WorkbookDependency _previous;
        private WorkbookDependency _next;

        private WorkbookDependency(FileInfo fileInfo, Excel.Workbook workbook)
        {
            _fileInfo = fileInfo;
            Workbook = workbook;
            workbook.AfterSave += Workbook_AfterSave;
            if ((_previous = _last) is null)
                _first = _last = this;
            else
                _previous._next = _last = this;
        }

        private void Workbook_AfterSave(bool Success)
        {
            if (Success && _fileInfo.FullName != Workbook.Path)
                _fileInfo = new FileInfo(Workbook.Path);
        }

        public static Instance Enter(FileInfo fileInfo)
        {
            Monitor.Enter(_SYNCROOT);
            try
            {
                WorkbookDependency dependencyManager = _first;
                if (dependencyManager is null)
                {
                    if (fileInfo is null)
                    {
                        _excelAppDependency = new ExcelAppDependency();
                        try { return new Instance(new WorkbookDependency(fileInfo, _excelAppDependency.Application.Workbooks.Add())); }
                        catch (ExcelAppDependencyException) { throw; }
                        catch (Exception exception)
                        {
                            _excelAppDependency.Dispose();
                            throw new WorkbookDependencyException("Error adding new workbook", Strings.ERROR_ID_WORKBOOK_ADD_FAIL, ErrorCategory.ReadError, null, exception);
                        }
                    }
                    if (fileInfo.Exists)
                    {
                        _excelAppDependency = new ExcelAppDependency();
                        try { return new Instance(new WorkbookDependency(fileInfo, _excelAppDependency.Application.Workbooks.Open(fileInfo.FullName))); }
                        catch (ExcelAppDependencyException)
                        {
                            _excelAppDependency.Dispose();
                            throw;
                        }
                        catch (IOException exception)
                        {
                            _excelAppDependency.Dispose();
                            throw new WorkbookDependencyException("Error reading file", Strings.ERROR_ID_WORKBOOK_OPEN_READERROR, ErrorCategory.ReadError, fileInfo, exception);
                        }
                        catch (UnauthorizedAccessException exception)
                        {
                            _excelAppDependency.Dispose();
                            throw new WorkbookDependencyException("Access not authorized", Strings.ERROR_ID_WORKBOOK_OPEN_UNAUTHORIZED, ErrorCategory.ReadError, fileInfo, exception);
                        }
                        catch (Exception exception)
                        {
                            _excelAppDependency.Dispose();
                            throw new WorkbookDependencyException("Error reading file", Strings.ERROR_ID_WORKBOOK_OPEN_READERROR, ErrorCategory.ReadError, fileInfo, exception);
                        }
                    }
                    else
                        throw new WorkbookDependencyException($"File {fileInfo.FullName} does not exist", Strings.ERROR_ID_WORKBOOK_OPEN_NOTFOUND, ErrorCategory.ObjectNotFound, fileInfo);
                }
                if (fileInfo is null)
                    try { return new Instance(new WorkbookDependency(fileInfo, _excelAppDependency.Application.Workbooks.Add())); }
                    catch (Exception exception) { throw new WorkbookDependencyException("Error adding new workbook", Strings.ERROR_ID_WORKBOOK_ADD_FAIL, ErrorCategory.ReadError, null, exception); }
                if (fileInfo.Exists)
                {
                    for (; dependencyManager != null; dependencyManager = dependencyManager._next)
                        if (dependencyManager._fileInfo != null && _pathComparer.Equals(dependencyManager._fileInfo.FullName, fileInfo.FullName))
                            return new Instance(dependencyManager);
                    try { return new Instance(new WorkbookDependency(fileInfo, _excelAppDependency.Application.Workbooks.Open(fileInfo.FullName))); }
                    catch (IOException exception) { throw new WorkbookDependencyException("Error reading file", Strings.ERROR_ID_WORKBOOK_OPEN_READERROR, ErrorCategory.ReadError, fileInfo, exception); }
                    catch (UnauthorizedAccessException exception) { throw new WorkbookDependencyException("Access not authorized", Strings.ERROR_ID_WORKBOOK_OPEN_UNAUTHORIZED, ErrorCategory.ReadError, fileInfo, exception); }
                    catch (Exception exception) { throw new WorkbookDependencyException("Error reading file", Strings.ERROR_ID_WORKBOOK_OPEN_READERROR, ErrorCategory.ReadError, fileInfo, exception); }
                }
                throw new WorkbookDependencyException($"File {fileInfo.FullName} does not exist", Strings.ERROR_ID_WORKBOOK_OPEN_NOTFOUND, ErrorCategory.ObjectNotFound, fileInfo);
            }
            finally { Monitor.Exit(_SYNCROOT); }
        }
    }
}

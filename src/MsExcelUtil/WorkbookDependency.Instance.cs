using Excel = Microsoft.Office.Interop.Excel;
using System;
using System.IO;
using System.Threading;
using System.Management.Automation;

namespace MsExcelUtil
{
    public sealed partial class WorkbookDependency
    {
        public sealed class Instance : IDisposable
        {
            private readonly WorkbookDependency _dependencyManager;
            private bool _isDisposed;
            private Instance _previous;
            private Instance _next;

            public Excel.Workbook Workbook
            {
                get
                {
                    if (_isDisposed)
                        throw new ObjectDisposedException(nameof(ExcelAppDependency));
                    return _dependencyManager.Workbook;
                }
            }

            public FileInfo FileInfo
            {
                get
                {
                    if (_isDisposed)
                        throw new ObjectDisposedException(nameof(ExcelAppDependency));
                    return _dependencyManager._fileInfo;
                }
            }

            public Instance(WorkbookDependency dependencyManager)
            {
                Monitor.Enter(dependencyManager._syncRoot);
                try
                {
                    if ((_previous = (_dependencyManager = dependencyManager)._latest) != null)
                        _previous._next = this;
                    dependencyManager._latest = this;
                }
                finally { Monitor.Exit(dependencyManager._syncRoot); }
            }

            public void SaveChanges(bool force = false)
            {
                Monitor.Enter(_dependencyManager._syncRoot);
                try
                {
                    if (_isDisposed)
                        throw new ObjectDisposedException(nameof(ExcelAppDependency));
                    if (_dependencyManager._fileInfo is null)
                        throw new WorkbookDependencyException("Workbook has never been saved", Strings.ERROR_ID_WORKBOOK_SAVE_FAIL, ErrorCategory.WriteError, _dependencyManager.Workbook);
                    if (force || !_dependencyManager.Workbook.Saved)
                        try { _dependencyManager.Workbook.Save(); }
                        catch (UnauthorizedAccessException exception) { throw new WorkbookDependencyException("Access not authorized", Strings.ERROR_ID_WORKBOOK_SAVE_UNAUTHORIZED, ErrorCategory.ReadError, _dependencyManager.Workbook, exception); }
                        catch (Exception exception) { throw new WorkbookDependencyException("Error saving file", Strings.ERROR_ID_WORKBOOK_SAVE_FAIL, ErrorCategory.WriteError, _dependencyManager.Workbook, exception); }
                }
                finally { Monitor.Exit(_dependencyManager._syncRoot); }
            }

            public void SaveAs(FileInfo fileInfo)
            {
                if (fileInfo is null) throw new ArgumentNullException(nameof(fileInfo));
                if (!(fileInfo.Exists || fileInfo.Directory.Exists))
                    throw new WorkbookDependencyException($"Director {fileInfo.Directory.FullName} does not exist", Strings.ERROR_ID_WORKBOOK_SAVE_NOTFOUND, ErrorCategory.ObjectNotFound, fileInfo);
                Monitor.Enter(_dependencyManager._syncRoot);
                try
                {
                    if (_isDisposed)
                        throw new ObjectDisposedException(nameof(ExcelAppDependency));
                    try { _dependencyManager.Workbook.SaveAs(fileInfo.FullName); }
                    catch (UnauthorizedAccessException exception) { throw new WorkbookDependencyException("Access not authorized", Strings.ERROR_ID_WORKBOOK_SAVE_UNAUTHORIZED, ErrorCategory.ReadError, _dependencyManager.Workbook, exception); }
                    catch (Exception exception) { throw new WorkbookDependencyException("Error saving file", Strings.ERROR_ID_WORKBOOK_SAVE_FAIL, ErrorCategory.WriteError, _dependencyManager.Workbook, exception); }
                }
                finally { Monitor.Exit(_dependencyManager._syncRoot); }
            }

            public void Exit(bool saveChanges = false, bool force = false)
            {
                Monitor.Enter(_dependencyManager._syncRoot);
                try
                {
                    if (_isDisposed)
                        throw new ObjectDisposedException(nameof(ExcelAppDependency));
                    if (saveChanges)
                        SaveChanges(force);
                    _isDisposed = true;
                    if (_next is null)
                    {
                        if ((_dependencyManager._latest = _previous) is null)
                        {
                            try { _dependencyManager.Workbook.Close(); }
                            catch
                            {
                                _dependencyManager._latest = this;
                                _isDisposed = false;
                                throw;
                            }
                            if (_dependencyManager._next is null)
                            {
                                if ((_last = _dependencyManager._previous) is null)
                                {
                                    _first = null;
                                    _excelAppDependency.Dispose();
                                }
                                else
                                    _dependencyManager._previous._next = _dependencyManager._next;
                            }
                            else if ((_dependencyManager._next._previous = _dependencyManager._previous) is null)
                                _first = _dependencyManager._next;
                            else
                                _dependencyManager._previous._next = _dependencyManager._next;
                            return;
                        }
                        _previous._next = null;
                    }
                    else if ((_next._previous = _previous) != null)
                        _previous._next = _next;
                    _dependencyManager.Workbook.AfterSave -= _dependencyManager.Workbook_AfterSave;
                }
                finally { Monitor.Exit(_dependencyManager._syncRoot); }
            }

            void IDisposable.Dispose()
            {
                if (!_isDisposed)
                    Exit();
            }
        }
    }
}

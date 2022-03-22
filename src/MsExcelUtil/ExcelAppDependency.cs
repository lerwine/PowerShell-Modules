using Excel = Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MsExcelUtil
{
    public sealed class ExcelAppDependency : IDisposable
    {
        private static readonly object _syncRoot = new object();
        private static ExcelAppDependency _latest;
        private static Excel.Application _application;
        private bool _isDisposed;
        private ExcelAppDependency _previous;
        private ExcelAppDependency _next;

        public Excel.Application Application
        {
            get
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(nameof(ExcelAppDependency));
                return _application;
            }
        }

        public ExcelAppDependency()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if ((_previous = _latest) is null)
                {
                    try { _application = new Excel.Application(); }
                    catch (Exception exception) { throw new ExcelAppDependencyException(exception); }
                    _latest = this;
                }
                else
                    _latest = _previous._next = this;
            }
            finally { Monitor.Exit(_syncRoot); }
        }

        public void Dispose()
        {
            Monitor.Enter(_syncRoot);
            try
            {
                if (_isDisposed)
                    return;
                _isDisposed = true;
                if (_next is null)
                {
                    if ((_latest = _previous) is null)
                        try { _application.Quit(); }
                        catch
                        {
                            _latest = this;
                            _isDisposed = false;
                            throw;
                        }
                    else
                        _previous._next = null;
                }
                else if ((_next._previous = _previous) != null)
                    _previous._next = _next;
            }
            finally { Monitor.Exit(_syncRoot); }
        }
    }
}

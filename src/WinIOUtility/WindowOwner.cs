using System;
using System.Windows.Forms;

namespace WinIOUtility
{
    public class WindowOwner : IWin32Window
    {
        private IntPtr _handle;
        public WindowOwner() : this(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle) { }
        public WindowOwner(IntPtr handle) { _handle = handle; }
        public IntPtr Handle { get { return _handle; } }
    }
}
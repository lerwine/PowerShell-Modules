using System;
using System.Windows.Forms;

namespace IOUtility
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class WindowOwner : IWin32Window
    {
        private IntPtr _handle;
        public WindowOwner() : this(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle) { }
        public WindowOwner(IntPtr handle) { _handle = handle; }
        public IntPtr Handle { get { return _handle; } }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
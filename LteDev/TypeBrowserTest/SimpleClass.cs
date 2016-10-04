using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Erwine.Leonard.T.TypeBrowserTest
{
    public class SimpleClass
    {
        public const string ExampleConstString = "This is a constant string.";

        public static readonly Regex ReadOnlyStaticRegex = new Regex(@"^(?<n>[^`]*(`\d+[^`]+)*)(`(?<g>\d+))?$");

        public static int StaticField = 12;

        private static double _privateField = 0.0;

        public static double PublicReadOnlyProperty
        {
            get { return SimpleClass._privateField; }
            private set { SimpleClass._privateField = value; }
        }
        
        public static event EventHandler<UnhandledExceptionEventArgs> ExampleStaticEventHandler;

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr h, string m, string c, int type);
        
        public SimpleClass(params string[] values)
        {

        }

        public void SimpleMethod()
        {
        }
    }
}

using IOUtility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;

namespace UnitTests
{
    /// <summary>
    /// Summary description for IOUtilityUnitTest
    /// </summary>
    [TestClass]
    public class IOUtilityUnitTest
    {
        public const string ModuleName = "Erwine.Leonard.T.IOUtility";
        public const string RelativeModulePath = @"Deployment\IOUtility";

        public IOUtilityUnitTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        [TestCategory("IOUtility")]
        [Description("Tests loading the IOUtility PowerShell module.")]
        public void ImportIOUtilityTestMethod()
        {
            PSModuleInfo module = PowerShellHelper.LoadPSModuleFromDeploymentDir(TestContext, "IOUtility\\Erwine.Leonard.T.IOUtility.psd1");
            ModuleConformance.ModuleValidator.AssertPSModule(TestContext, module);
        }

        [TestMethod]
        [TestCategory("IOUtility")]
        public void GetSpecialFolderNamesTestMethod()
        {
            PowerShellHelper.PSInvocationResult result = PowerShellHelper.InvokeScript(TestContext, "Get-SpecialFolderNames", null, "IOUtility\\Erwine.Leonard.T.IOUtility.psd1");
            Assert.AreEqual(0, result.Errors.Count);
            string[] expected = Enum.GetNames(typeof(Environment.SpecialFolder));
            Assert.AreEqual(expected.Length, result.Output.Count, "Incorrect number of results");
            foreach (PSObject obj in result.Output)
            {
                Assert.IsNotNull(obj);
                object o = obj.BaseObject;
                Assert.IsNotNull(o);
                Assert.IsInstanceOfType(o, typeof(string));
                string s = o as string;
                Assert.AreNotEqual(1, s.Length);
                string t = s.Trim();
                Assert.AreEqual(s, t, "\"" + t + "\" Value as extraneous whitespace");
                Assert.IsTrue(expected.Any(n => String.Equals(s, n, StringComparison.CurrentCulture)), "\"" + s + "\" value was not expected.");
            }
        }

        [TestMethod]
        [TestCategory("IOUtility")]
        public void GetSpecialFolderTestMethod()
        {
            Environment.SpecialFolder[] values = Enum.GetValues(typeof(Environment.SpecialFolder)).OfType<Environment.SpecialFolder>().ToArray();

            PowerShellHelper.PSInvocationResult result = PowerShellHelper.InvokeScript(TestContext, @"$args | Get-SpecialFolder", values.Select(v => v.ToString("F")).ToArray(), "IOUtility\\Erwine.Leonard.T.IOUtility.psd1");
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(values.Length, result.Output.Count, "Incorrect number of results");
            for (int i=0; i< values.Length; i++)
            {
                object o = (result.Output[i] == null) ? null : result.Output[i].BaseObject;
                string expected = Environment.GetFolderPath(values[i]);
                if (expected == null)
                {
                    Assert.IsNull(o, String.Format("{0} did not return a null value.", values[i]));
                    continue;
                }
                Assert.IsNotNull(o);
                Assert.IsInstanceOfType(o, typeof(string));
                string actual = o as string;
                Assert.AreEqual(expected, actual, String.Format("{0} did not return the expected value.", values[i]));
            }

            result = PowerShellHelper.InvokeScript(TestContext, @"$args | Get-SpecialFolder", values, "IOUtility\\Erwine.Leonard.T.IOUtility.psd1");
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(values.Length, result.Output.Count, "Incorrect number of results");
            for (int i = 0; i < values.Length; i++)
            {
                object o = (result.Output[i] == null) ? null : result.Output[i].BaseObject;
                string expected = Environment.GetFolderPath(values[i]);
                if (expected == null)
                {
                    Assert.IsNull(o, String.Format("{0} did not return a null value.", values[i]));
                    continue;
                }
                Assert.IsNotNull(o);
                Assert.IsInstanceOfType(o, typeof(string));
                string actual = o as string;
                Assert.AreEqual(expected, actual, String.Format("{0} did not return the expected value.", values[i]));
            }
        }

        [TestMethod]
        [TestCategory("IOUtility")]
        public void WriteLongIntegerToStreamTestMethod()
        {
            PowerShellHelper.PSInvocationResult result = PowerShellHelper.InvokeScript(TestContext, @"$MemoryStream = New-MemoryStream;
$args | Write-LongIntegerToStream -Stream $MemoryStream;
$MemoryStream.ToArray() | Write-Output;
$MemoryStream.Dispose();", new object[] { 0, 1024L }, "IOUtility\\Erwine.Leonard.T.IOUtility.psd1");
            byte[] expected = BitConverter.GetBytes(1024L);
            Assert.AreEqual(0, result.Errors.Count);
            Assert.AreEqual(expected.Length * 2, result.Output.Count, "Incorrect number of results");
            for (int i=0; i< expected.Length; i++)
            {
                Assert.IsNotNull(result.Output[i]);
                object obj = result.Output[i].BaseObject;
                Assert.IsNotNull(obj);
                Assert.IsInstanceOfType(obj, typeof(byte));
                Assert.AreEqual((byte)0, (byte)obj);

                Assert.IsNotNull(result.Output[i + expected.Length]);
                obj = result.Output[i + expected.Length].BaseObject;
                Assert.IsNotNull(obj);
                Assert.IsInstanceOfType(obj, typeof(byte));
                Assert.AreEqual(expected[i], (byte)obj);
            }
        }
    }
}

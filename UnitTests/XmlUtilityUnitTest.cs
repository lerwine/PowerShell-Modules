using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.IO;
using System.Collections.ObjectModel;

namespace UnitTests
{
    /// <summary>
    /// Summary description for XmlUtilityUnitTest
    /// </summary>
    [TestClass]
    public class XmlUtilityUnitTest
    {
        public const string ModuleName = "Erwine.Leonard.T.XmlUtility";
        public const string RelativeModulePath = @"XmlUtility";

        public XmlUtilityUnitTest()
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
        public void ImportXmlUtilityTestMethod()
        {
            InitialSessionState iss = InitialSessionState.CreateDefault();
            // this.TestContext;
            using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
            {
                runspace.Open();
                using (PowerShell powershell = PowerShell.Create(RunspaceMode.NewRunspace))
                {
                    string modulePath = Path.GetFullPath(Path.Combine(@"..\..\..", RelativeModulePath, ModuleName + ".psd1"));
                    this.TestContext.WriteLine("Path: {0}", modulePath);
                    powershell.Runspace = runspace;
                    PowerShell ps = powershell.AddCommand("Import-Module");
                    ps.AddParameter("Name", modulePath);
                    ps.AddParameter("PassThru");
                    Collection<PSObject> result = powershell.Invoke();
                    if (powershell.HadErrors)
                    {
                        foreach (ErrorRecord errorRecord in powershell.Streams.Error)
                            this.TestContext.WriteLine("Error: {0}", errorRecord);
                        Assert.IsFalse(powershell.HadErrors, "Multiple errors encountered: See test output for details.");
                    }
                    Assert.AreEqual(1, result.Count);
                    Assert.IsNotNull(result[0]);
                    Assert.IsNotNull(result[0].BaseObject);
                    Assert.IsInstanceOfType(result[0].BaseObject, typeof(PSModuleInfo));
                    PSModuleInfo psModuleInfo = result[0].BaseObject as PSModuleInfo;
                    Assert.AreEqual(ModuleName, psModuleInfo.Name);
                    Assert.AreEqual(ModuleName + ".psm1", psModuleInfo.RootModule);
                    Assert.IsNotNull(psModuleInfo.PowerShellVersion);
                    Assert.AreEqual(psModuleInfo.PowerShellVersion, new Version(4, 0));
                    Assert.IsNotNull(psModuleInfo.ClrVersion);
                    Assert.AreEqual(psModuleInfo.ClrVersion, new Version(4, 0));
                    Assert.IsNotNull(psModuleInfo.DotNetFrameworkVersion);
                    Assert.AreEqual(psModuleInfo.DotNetFrameworkVersion, new Version(4, 0));
                }
            }
        }
    }
}

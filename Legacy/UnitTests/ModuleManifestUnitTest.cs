using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSModuleInstallUtil;
using System.IO;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.Collections.ObjectModel;

namespace UnitTests
{
    [TestClass]
    public class ModuleManifestUnitTest
    {
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
        [TestCategory("PSModuleInstallUtil")]
        [Description("Tests ModuleManifest class in context of PowerShell invocation.")]
        public void ModuleManifestFromPsdTestMethod()
        {
            ModuleManifest target;
            InitialSessionState iss = InitialSessionState.CreateDefault();
            // this.TestContext;
            using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
            {
                runspace.Open();
                runspace.SessionStateProxy.SetVariable("DeploymentDirectory", TestContext.DeploymentDirectory);
                using (PowerShell powershell = PowerShell.Create(RunspaceMode.NewRunspace))
                {
                    powershell.Runspace = runspace;
                    PowerShell ps = powershell.AddScript("Add-Type -Path ($DeploymentDirectory | Join-Path -ChildPath 'PSModuleInstallUtil.dll'); New-Object -TypeName 'PSModuleInstallUtil.ModuleManifest' -ArgumentList ($DeploymentDirectory | Join-Path -ChildPath 'ExampleManifest1.psd1')");
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
                    Assert.IsInstanceOfType(result[0].BaseObject, typeof(ModuleManifest));
                    target = result[0].BaseObject as ModuleManifest;
                }
            }
            Assert.AreEqual("Erwine.Leonard.T.PsNuGet.psm1", target.RootModule);
            Assert.AreEqual("Erwine.Leonard.T.PsNuGet", target.Name);
        }
    }
}

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSModuleInstallUtil.Module;
using Microsoft.PowerShell.Commands;
using System.IO;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.Collections.ObjectModel;

namespace UnitTests
{
    /// <summary>
    /// Summary description for ScriptDocumentTest
    /// </summary>
    [TestClass]
    public class ScriptDocumentTest
    {
        public ScriptDocumentTest()
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
        [TestCategory("Installation")]
        [Description("Tests loading module manifest")]
        public void ScriptDocumentParsingTestMethod()
        {
            TestModuleManifestCommand cmd = new TestModuleManifestCommand();
            cmd.Path = Path.Combine(TestContext.DeploymentDirectory, "PSModuleInstallUtil.psd1");
            InitialSessionState iss = InitialSessionState.CreateDefault();
            // this.TestContext;
            using (Runspace runspace = RunspaceFactory.CreateRunspace(iss))
            {
                runspace.Open();
                runspace.SessionStateProxy.SetVariable("DeploymentDirectory", TestContext.DeploymentDirectory);
                using (PowerShell powershell = PowerShell.Create(RunspaceMode.NewRunspace))
                {
                    powershell.Runspace = runspace;
                    PowerShell ps = powershell.AddScript("Test-ModuleManifest -Path ($DeploymentDirectory | Join-Path -ChildPath 'PSModuleInstallUtil.psd1');");
                    Collection<PSObject> result = powershell.Invoke();
                    if (powershell.HadErrors)
                    {
                        foreach (ErrorRecord errorRecord in powershell.Streams.Error)
                            this.TestContext.WriteLine("Error: {0}", errorRecord);
                        Assert.IsFalse(powershell.HadErrors, "Multiple errors encountered: See test output for details.");
                    }
                }
            }
            //ScriptDocument target = new ScriptDocument(InstallerTestResources.ScriptParseTest);
            //Assert.IsNotNull(target.FirstChild);
            //Assert.AreEqual(ScriptNodeType.Variable, target.FirstChild.NodeType);
        }
    }
}

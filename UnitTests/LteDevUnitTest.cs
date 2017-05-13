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
    /// Summary description for LteDevUnitTest
    /// </summary>
    [TestClass]
    public class LteDevUnitTest
    {
        public const string ModuleName = "Erwine.Leonard.T.LteDev";
        public const string RelativeModulePath = @"LteDev";

        public LteDevUnitTest()
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
        [TestCategory("LteDev")]
        [Description("Tests loading the LteDev PowerShell module.")]
        public void ImportLteDevTestMethod()
        {
            PowerShellHelper.TestLoadModule(this.TestContext, ModuleName, RelativeModulePath, ".psm1", Path.GetFullPath(@"..\..\..\IOUtility\IOUtility\Erwine.Leonard.T.IOUtility.psd1"));
        }
    }
}

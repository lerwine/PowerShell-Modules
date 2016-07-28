using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.IO;
using System.Collections.ObjectModel;
using IOUtilityCLR;
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
        public const string RelativeModulePath = @"IOUtility\IOUtility";

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
        public void ImportIOUtilityTestMethod()
        {
            PowerShellHelper.TestLoadModule(this.TestContext, ModuleName, RelativeModulePath, ".psm1");
        }

        [TestMethod]
        public void PSInvocationBuilderTestMethod()
        {
            PSInvocationBuilder target = new PSInvocationBuilder();
            Assert.AreEqual<ApartmentState>(ApartmentState.STA, target.ApartmentState);
            Assert.AreEqual<PSThreadOptions>(PSThreadOptions.ReuseThread, target.ThreadOptions);
            Assert.IsNull(target.InitialSessionState);
            Assert.IsNull(target.Input);
            Assert.IsNull(target.Settings);
            target.AddScript("$MyVar = 5");
            target.Variables.Add("MyVar", 23);
            PSInvocationResult result = target.GetResult();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Variables.ContainsKey("MyVar"));
            Assert.IsNotNull(result.Variables["MyVar"]);
            object obj = (result.Variables["MyVar"] is PSObject) ? (result.Variables["MyVar"] as PSObject).BaseObject : result.Variables["MyVar"];
            Assert.IsNotNull(obj);
            Assert.IsTrue(obj is int);
            Assert.AreEqual<int>(5, (int)obj);
        }
    }
}

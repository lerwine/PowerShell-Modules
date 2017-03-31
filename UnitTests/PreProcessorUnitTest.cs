using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSModuleInstallUtil;
using System.Collections.ObjectModel;

namespace UnitTests
{
    /// <summary>
    /// Summary description for PreProcessorUnitTest
    /// </summary>
    [TestClass]
    public class PreProcessorUnitTest
    {
        public PreProcessorUnitTest()
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

//        [TestMethod]
//        public void PreProcessorTestMethod1()
//        {
//            PreProcessor target = new PreProcessor(@"<#:if PSV2 :#><#:=
//$Text = ""#V2`n"" + $ScriptBlock.ToString();:#>
//<#:else:#>
//$Text = $ScriptBlock.ToString();
//<#:endif:#>", new string[] { "PSV2" });
//            Collection<PreProcessor.PreProcessorData> result = target.GetPreProcessorData();
//            Assert.IsNotNull(result);
//            Assert.AreEqual(5, result.Count);
//            Assert.IsNotNull(result[0]);
//            Assert.IsNotNull(result[0].Code);
//            Assert.AreEqual(@"if ($PSV2) {", result[0].Code);
//            Assert.IsNotNull(result[0].Content);
//            Assert.AreEqual(0, result[0].Content.Length);
//            Assert.IsNotNull(result[1]);
//            Assert.IsNotNull(result[1].Code);
//            Assert.AreEqual(0, result[1].Code.Length);
//            Assert.IsNotNull(result[1].Content);
//            Assert.AreEqual(@"
//$Text = ""#V2`n"" + $ScriptBlock.ToString();
//", result[1].Content);
//            Assert.IsNotNull(result[2]);
//            Assert.IsNotNull(result[2].Code);
//            Assert.AreEqual(@"} else {", result[2].Code);
//            Assert.IsNotNull(result[2].Content);
//            Assert.AreEqual(0, result[2].Content.Length);
//            Assert.IsNotNull(result[3]);
//            Assert.IsNotNull(result[3].Code);
//            Assert.AreEqual(0, result[3].Code.Length);
//            Assert.IsNotNull(result[3].Content);
//            Assert.AreEqual(@"
//$Text = $ScriptBlock.ToString();
//", result[3].Content);
//            Assert.IsNotNull(result[4]);
//            Assert.IsNotNull(result[4].Code);
//            Assert.AreEqual(@"}", result[4].Code);
//            Assert.IsNotNull(result[4].Content);
//            Assert.AreEqual(0, result[4].Content.Length);
//        }
    }
}

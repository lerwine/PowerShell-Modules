using System;
using System.Linq;
using System.Windows;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CredentialStorageLibrary;

namespace UnitTests
{
    /// <summary>
    /// Summary description for CredentialStorageTest
    /// </summary>
    [TestClass]
    public class CredentialStorageTest
    {
        public CredentialStorageTest()
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
        public void MainWindowVMConstructorTestMethod()
        {
            MainWindowVM vm = new MainWindowVM();
            Assert.IsNotNull(vm.BrowserConfigCollection);
            Assert.AreEqual(0, vm.BrowserConfigCollection.Count);
            Assert.IsNotNull(vm.CloseWindowCommand);
            Assert.IsNull(vm.DefaultBrowserConfig);
            Assert.AreEqual(-1, vm.DefaultBrowserConfigIndex);
            Assert.AreEqual(Visibility.Collapsed, vm.DeleteConfirmVisibility);
            Assert.IsNotNull(vm.DeleteItemCommand);
            Assert.IsNotNull(vm.DeleteNoCommand);
            Assert.IsNotNull(vm.DeleteYesCommand);
            Assert.IsNotNull(vm.EditCancelCommand);
            Assert.AreEqual(Visibility.Visible, vm.EditControlsVisibility);
            Assert.IsNotNull(vm.EditingItem);
            Assert.AreEqual(Guid.Empty, vm.EditingItem.Id);
            Assert.IsNotNull(vm.EditItemCommand);
            Assert.IsNotNull(vm.EditSaveCommand);
            Assert.IsNotNull(vm.Items);
            Assert.AreEqual(0, vm.Items.Count);
            Assert.IsNotNull(vm.MainWindow);
            Assert.IsNotNull(vm.NewItemCommand);
            Assert.IsNotNull(vm.SelectedBrowserConfig);
            Assert.AreEqual(-1, vm.SelectedBrowserConfigIndex);
            Assert.AreEqual(-1, vm.SelectedIndex);
            Assert.IsNotNull(vm.SelectedItem);
            Assert.AreEqual(0, vm.SelectedTabIndex);
        }
    }
}

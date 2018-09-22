using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Management.Automation;
using System.Collections;

namespace UnitTests
{
    /// <summary>
    /// Summary description for PowerShellAssumptionsUnitTest
    /// </summary>
    [TestClass]
    public class PowerShellAssumptionsUnitTest
    {
        public PowerShellAssumptionsUnitTest()
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
        public void PSObjectTestMethod()
        {
            PSObject target = new PSObject();
            Assert.IsNotNull(target.BaseObject);
            Assert.IsInstanceOfType(target.BaseObject, typeof(PSCustomObject));
            Assert.AreEqual(0, target.Properties.Count());

            try { target = PSObject.AsPSObject(null); } catch (ArgumentNullException) { }

            PowerShellHelper.PSInvocationResult result = PowerShellHelper.InvokeScript(TestContext, @"'' | Write-Output;
' NonEmpty ' | Write-Output;
5 | Write-Output;
@{ First = 1; Second = 'Two' } | Write-Output;
Write-Output -InputObject $null;
(New-Object -TypeName 'System.Management.Automation.PSObject' -Property @{
    Third = 3.0;
    Fourth = [System.BitConverter]::GetBytes(4);
    h = Get-Help -Name 'Get-Item' -Full;
}) | Write-Output;", null);
            if (result.Errors.Count > 0)
            {
                foreach (ErrorRecord errorRecord in result.Errors)
                    PowerShellHelper.WriteErrorRecord(TestContext, errorRecord);
            }
            Assert.AreEqual(0, result.Errors.Count, "Multiple errors encountered: See test output for details.");
            Assert.AreEqual(6, result.Output.Count);

            target = result.Output[0];
            Assert.IsNotNull(target);
            Assert.IsNotNull(target.BaseObject);
            Assert.IsInstanceOfType(target.BaseObject, typeof(string));
            Assert.AreEqual(0, ((string)(target.BaseObject)).Length);

            target = result.Output[1];
            Assert.IsNotNull(target);
            Assert.IsNotNull(target.BaseObject);
            Assert.IsInstanceOfType(target.BaseObject, typeof(string));
            Assert.AreEqual(" NonEmpty ", target.BaseObject);

            target = result.Output[2];
            Assert.IsNotNull(target);
            Assert.IsNotNull(target.BaseObject);
            Assert.IsInstanceOfType(target.BaseObject, typeof(int));
            Assert.AreEqual(5, target.BaseObject);

            target = result.Output[3];
            Assert.IsNotNull(target);
            Assert.IsNotNull(target.BaseObject);
            Assert.IsInstanceOfType(target.BaseObject, typeof(Hashtable));
            Hashtable hashTable = (Hashtable)(target.BaseObject);
            Assert.AreEqual(2, hashTable.Count);
            Assert.IsNotNull(hashTable["First"]);
            Assert.IsInstanceOfType(hashTable["First"], typeof(int));
            Assert.AreEqual(1, hashTable["First"]);
            Assert.IsNotNull(hashTable["Second"]);
            Assert.IsInstanceOfType(hashTable["Second"], typeof(string));
            Assert.AreEqual("Two", hashTable["Second"]);

            target = result.Output[4];
            Assert.IsNull(target);

            target = result.Output[5];
            Assert.IsNotNull(target);
            Assert.IsNotNull(target.BaseObject);
            Assert.IsInstanceOfType(target.BaseObject, typeof(PSCustomObject));
            PSPropertyInfo propertyInfo = target.Properties["Third"];
            Assert.IsNotNull(propertyInfo);
            Assert.AreEqual(propertyInfo.MemberType, PSMemberTypes.NoteProperty);
            Assert.IsNotNull(propertyInfo.Value);
            Assert.IsInstanceOfType(propertyInfo.Value, typeof(double));
            Assert.AreEqual(3.0, (double)(propertyInfo.Value));
            propertyInfo = target.Properties["Fourth"];
            Assert.IsNotNull(propertyInfo);
            Assert.AreEqual(propertyInfo.MemberType, PSMemberTypes.NoteProperty);
            Assert.IsNotNull(propertyInfo.Value);
            Assert.IsInstanceOfType(propertyInfo.Value, typeof(byte[]));
            byte[] expectedByteArray = BitConverter.GetBytes(4);
            byte[] actualByteArray = (byte[])(propertyInfo.Value);
            Assert.AreEqual(expectedByteArray.Length, actualByteArray.Length);
            Assert.AreEqual(BitConverter.ToInt32(expectedByteArray, 0), BitConverter.ToInt32(actualByteArray, 0));
            propertyInfo = target.Properties["h"];
            Assert.IsNotNull(propertyInfo);
            Assert.AreEqual(propertyInfo.MemberType, PSMemberTypes.NoteProperty);
            Assert.IsNotNull(propertyInfo.Value);
            Assert.IsInstanceOfType(propertyInfo.Value, typeof(PSObject));
            target = (PSObject)(propertyInfo.Value);
            Assert.IsNotNull(target.BaseObject);
            Assert.IsInstanceOfType(target.BaseObject, typeof(PSCustomObject));
            propertyInfo = target.Properties["Name"];
            Assert.IsNotNull(propertyInfo);
            Assert.AreEqual(propertyInfo.MemberType, PSMemberTypes.NoteProperty);
            Assert.IsNotNull(propertyInfo.Value);
            Assert.IsInstanceOfType(propertyInfo.Value, typeof(string));
            Assert.AreEqual("Get-Item", (string)(propertyInfo.Value));
            
            result = PowerShellHelper.InvokeScript(TestContext, "(Get-Module -ListAvailable) | Write-Output;", null);
            if (result.Errors.Count > 0)
            {
                foreach (ErrorRecord errorRecord in result.Errors)
                    PowerShellHelper.WriteErrorRecord(TestContext, errorRecord);
            }
            Assert.AreEqual(0, result.Errors.Count, "Multiple errors encountered: See test output for details.");
            Assert.AreNotEqual(0, result.Output.Count);
            foreach (PSObject o in result.Output)
            {
                Assert.IsNotNull(o);
                Assert.IsNotNull(o.BaseObject);
                Assert.IsInstanceOfType(o.BaseObject, typeof(PSModuleInfo));
            }
            
            result = PowerShellHelper.InvokeScript(TestContext, "($env:PSModulePath.Split([System.IO.Path]::PathSeparator) | ForEach-Object { if ($_ | Test-Path) { Get-Item -LiteralPath $_ } else { New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList $_ } }) | Write-Output;", null);
            if (result.Errors.Count > 0)
            {
                foreach (ErrorRecord errorRecord in result.Errors)
                    PowerShellHelper.WriteErrorRecord(TestContext, errorRecord);
            }
            Assert.AreEqual(0, result.Errors.Count, "Multiple errors encountered: See test output for details.");
            Assert.AreNotEqual(0, result.Output.Count);
            foreach (PSObject o in result.Output)
            {
                Assert.IsNotNull(o);
                Assert.IsNotNull(o.BaseObject);
                Assert.IsInstanceOfType(o.BaseObject, typeof(System.IO.FileSystemInfo));
            }
        }
    }
}

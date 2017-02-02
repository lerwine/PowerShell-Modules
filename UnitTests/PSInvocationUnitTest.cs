using System;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Management.Automation.Language;
using WpfCLR.PSInvocation;

namespace UnitTests
{
    [TestClass]
    public class PSInvocationUnitTest
    {
        public class TestThisObj
        {
            private long _value = 0L;
            public long Value
            {
                get { return _value; }
                set { _value = value; }
            }
        }

        [TestMethod]
        public void PSInvocationContextTestMethod()
        {
            Context context = new Context();
            TestThisObj thisObj = new TestThisObj();
            context.This = PSObject.AsPSObject(thisObj);
            context.Variables.Add("TestVar", 5);
            InvocationResult result = context.GetResult(PSInvocationResources.PsInvocaion1);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.HadErrors);
            Assert.AreEqual(1, result.Output.Count);
            Assert.IsNotNull(result.Output[0]);
            Assert.IsNotNull(result.Output[0].BaseObject);
            Assert.IsInstanceOfType(result.Output[0].BaseObject, typeof(string));
            Assert.AreEqual("Test", result.Output[0].BaseObject);
            Assert.IsTrue(context.SynchronizedData.ContainsKey("C"));
            Assert.IsNotNull(context.SynchronizedData["C"]);
            Assert.IsInstanceOfType(context.SynchronizedData["C"], typeof(int));
            Assert.AreEqual(0, context.SynchronizedData["C"]);
            Assert.IsTrue(context.Variables.ContainsKey("TestVar"));
            Assert.IsTrue(result.Variables.ContainsKey("TestVar"));
            Assert.IsNotNull(context.Variables["TestVar"]);
            Assert.IsNotNull(result.Variables["TestVar"]);
            Assert.IsInstanceOfType(context.Variables["TestVar"], typeof(int));
            Assert.IsInstanceOfType(result.Variables["TestVar"], typeof(int));
            Assert.AreEqual(5, context.Variables["TestVar"]);
            Assert.AreEqual(7, result.Variables["TestVar"]);
            Assert.AreEqual(12L, thisObj.Value);

        }
    }
}

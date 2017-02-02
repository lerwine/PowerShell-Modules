using System;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Management.Automation.Language;
using IOUtilityCLR;
using System.ComponentModel;

namespace UnitTests
{
    [TestClass]
    public class PSInvocationUnitTest
    {
        public class TestThisObj : INotifyPropertyChanged
        {
            private long _value = 0L;
            public long Value
            {
                get { return _value; }
                set
                {
                    if (value == _value)
                        return;
                    _value = value;
                    PropertyChangedEventHandler propertyChanged = PropertyChanged;
                    if (propertyChanged != null)
                        propertyChanged(this, new PropertyChangedEventArgs("Value"));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        [TestMethod]
        public void PSInvocationContextTestMethod()
        {
            PSInvocationContext context = new PSInvocationContext();
            TestThisObj thisObj = new TestThisObj();
            context.This = PSObject.AsPSObject(thisObj);
            context.Variables.Add("TestVar", 5);
            PSInvocationResult result = context.GetResult(PSInvocationResources.PsInvocation1);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.HadErrors);
            Assert.IsTrue(result.RanToCompletion);
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

        [TestMethod]
        public void PSEventScriptHandlerTestMethod()
        {
            PSInvocationContext context = new PSInvocationContext();
            TestThisObj thisObj = new TestThisObj();
            TestThisObj anotherObj = new TestThisObj();
            context.This = PSObject.AsPSObject(thisObj);
            context.Variables.Add("TestVar", 5);
            PSEventScriptHandler<PropertyChangedEventArgs> target = new PSEventScriptHandler<PropertyChangedEventArgs>("ThisChanged", 
                ScriptBlock.Create(PSInvocationResources.PSInvocationEventHandler), context);
            target.This = PSObject.AsPSObject(anotherObj);
            target.Variables.Add("TestVar2", 5);
            thisObj.PropertyChanged += target.EventHandler;
            context.AddEventHandler(target);
            PSInvocationResult result = context.GetResult(PSInvocationResources.PsInvocation1);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.HadErrors);
            Assert.IsTrue(result.RanToCompletion);
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
            Assert.AreEqual(1, context.EventHandlerResults.Count);
            Assert.IsNotNull(context.EventHandlerResults[0]);
            Assert.IsNotNull(context.EventHandlerResults[0].Name);
            Assert.AreEqual("ThisChanged", context.EventHandlerResults[0].Name);
            Assert.IsNotNull(context.EventHandlerResults[0].Args);
            Assert.IsNotNull(context.EventHandlerResults[0].Args.EventArgs);
            Assert.IsInstanceOfType(context.EventHandlerResults[0].Args.EventArgs, typeof(PropertyChangedEventArgs));
            PropertyChangedEventArgs args = context.EventHandlerResults[0].Args.EventArgs as PropertyChangedEventArgs;
            Assert.IsNotNull(args.PropertyName);
            Assert.AreEqual("Value", args.PropertyName);
            Assert.IsFalse(context.EventHandlerResults[0].Args.HadErrors);
            Assert.IsTrue(context.EventHandlerResults[0].Args.RanToCompletion);
            Assert.AreEqual(1, context.EventHandlerResults[0].Args.Output.Count);
            Assert.IsNotNull(context.EventHandlerResults[0].Args.Output[0]);
            Assert.IsNotNull(context.EventHandlerResults[0].Args.Output[0].BaseObject);
            Assert.IsInstanceOfType(context.EventHandlerResults[0].Args.Output[0].BaseObject, typeof(string));
            Assert.AreEqual("Again", context.EventHandlerResults[0].Args.Output[0].BaseObject);
            Assert.IsTrue(context.SynchronizedData.ContainsKey("xyz"));
            Assert.IsNotNull(context.SynchronizedData["xyz"]);
            Assert.IsInstanceOfType(context.SynchronizedData["xyz"], typeof(int));
            Assert.AreEqual(0, context.SynchronizedData["xyz"]);
            Assert.IsFalse(context.EventHandlerResults[0].Args.Variables.ContainsKey("TestVar"));
            Assert.IsTrue(context.EventHandlerResults[0].Args.Variables.ContainsKey("TestVar2"));
            Assert.IsNotNull(context.EventHandlerResults[0].Args.Variables["TestVar2"]);
            Assert.IsInstanceOfType(context.EventHandlerResults[0].Args.Variables["TestVar2"], typeof(PropertyChangedEventArgs));
            args = context.EventHandlerResults[0].Args.Variables["TestVar2"] as PropertyChangedEventArgs;
            Assert.IsNotNull(args.PropertyName);
            Assert.AreEqual("Value", args.PropertyName);
            Assert.AreEqual(40L, anotherObj.Value);
        }
    }
}

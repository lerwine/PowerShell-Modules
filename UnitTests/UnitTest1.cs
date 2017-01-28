using System;
using System.Management.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Management.Automation.Language;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            PSToken t;
            System.Management.Automation.PSParser.Tokenize()
            Token[] tokens;
            ParseError[] errors;
            ScriptBlockAst scriptBlock = Parser.ParseInput("@{ Test = @('one', 'two', 'three') }", out tokens, out errors);
        }
    }
}

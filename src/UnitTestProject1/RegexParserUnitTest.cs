using LteDev.RegexParsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UnitTestProject1
{
    /// <summary>
    /// Summary description for RegexParserUnitTest
    /// </summary>
    [TestClass]
    public class RegexParserUnitTest
    {
        public RegexParserUnitTest()
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

        public class ExpectedCaptureResult
        {
            public int Index { get; }
            public int Length { get; }
            public bool Success { get; }
            public string Value { get; }
            protected ExpectedCaptureResult()
            {
                Index = Length = 0;
                Success = false;
                Value = "";
            }
            protected ExpectedCaptureResult(int index, string value)
            {
                Index = index;
                Length = (Value = value).Length;
                Success = true;
            }
        }

        public class ExpectedMatchGroupResult : ExpectedCaptureResult
        {
            public string Name { get; }
            public ExpectedMatchGroupResult(string name) : base() { Name = name; }
            public ExpectedMatchGroupResult(string name, int index, string value) : base(index, value) { Name = name; }
        }

        public class ExpectedMatchResult : ExpectedCaptureResult
        {
            public ReadOnlyCollection<ExpectedMatchGroupResult> Groups { get; }

            private ExpectedMatchResult() : base() { Groups = new ReadOnlyCollection<ExpectedMatchGroupResult>(new Collection<ExpectedMatchGroupResult>()); }

            internal ExpectedMatchResult(int index, string value, IList<ExpectedMatchGroupResult> groups) : base(index, value)
            {
                Groups = new ReadOnlyCollection<ExpectedMatchGroupResult>((groups == null) ? new Collection<ExpectedMatchGroupResult>() : groups);
            }

            internal static readonly ExpectedMatchResult Failed = new ExpectedMatchResult();

            internal static object[] CreateTestData(string input, int startAt, ExpectedMatchResult expected) => new object[] { input, startAt, expected };

            internal static object[] CreateTestData(string input, int startAt, Action<ExpectedRegexMatchBuilder> builderCallback)
            {
                ExpectedRegexMatchBuilder builder = new ExpectedRegexMatchBuilder(startAt);
                builderCallback(builder);
                return new object[] { input, startAt, builder.Build() };
            }

            internal static object[] CreateTestData(string input, int startAt, string initialUngroupedMatch, Action<ExpectedRegexMatchBuilder> builderCallback)
            {
                ExpectedRegexMatchBuilder builder = new ExpectedRegexMatchBuilder(startAt, initialUngroupedMatch);
                builderCallback(builder);
                return new object[] { input, startAt, builder.Build() };
            }
        }

        public class ExpectedRegexMatchBuilder
        {
            private readonly int _startIndex;
            private int _nextIndex;
            private readonly StringBuilder _value = new StringBuilder();
            private readonly Collection<ExpectedMatchGroupResult> _groups;
            private readonly ExpectedRegexMatchBuilder _outer;

            private ExpectedRegexMatchBuilder(ExpectedRegexMatchBuilder outer, string initialUngroupedMatch)
            {
                _groups = outer._groups;
                _startIndex = outer._nextIndex;
                if (string.IsNullOrEmpty(initialUngroupedMatch))
                    _nextIndex = _startIndex;
                else
                    _nextIndex = _startIndex + (_value.Append(initialUngroupedMatch)).Length;
            }

            public ExpectedRegexMatchBuilder(int index = 0, string initialUngroupedMatch = null)
            {
                _groups = new Collection<ExpectedMatchGroupResult>();
                _startIndex = index;
                if (string.IsNullOrEmpty(initialUngroupedMatch))
                    _nextIndex = index;
                else
                    _nextIndex = index + (_value.Append(initialUngroupedMatch)).Length;
            }

            public ExpectedRegexMatchBuilder(string initialUngroupedMatch) : this(0, initialUngroupedMatch) { }

            public ExpectedRegexMatchBuilder Ungrouped(string match)
            {
                _nextIndex += match.Length;
                _value.Append(match);
                return this;
            }

            public ExpectedRegexMatchBuilder Grouped(string name, string match)
            {
                ExpectedMatchGroupResult group = new ExpectedMatchGroupResult(name, _nextIndex, match);
                _nextIndex += group.Length;
                _value.Append(group.Value);
                _groups.Add(group);
                return this;
            }

            public ExpectedRegexMatchBuilder FailedGroup(params string[] name)
            {
                foreach (string n in name)
                    _groups.Add(new ExpectedMatchGroupResult(n));
                return this;
            }

            public ExpectedRegexMatchBuilder Nested(string outerGroupName, Action<ExpectedRegexMatchBuilder> innerBuilderCallback)
            {
                ExpectedRegexMatchBuilder builder = new ExpectedRegexMatchBuilder(this, null);
                innerBuilderCallback(builder);
                ExpectedMatchGroupResult group = builder.Build(outerGroupName);
                _nextIndex += group.Length;
                _value.Append(group.Value);
                _groups.Add(group);
                return this;
            }

            public ExpectedRegexMatchBuilder Nested(string outerGroupName, string initialGroupedMatch, Action<ExpectedRegexMatchBuilder> innerBuilderCallback)
            {
                ExpectedRegexMatchBuilder builder = new ExpectedRegexMatchBuilder(this, initialGroupedMatch);
                innerBuilderCallback(builder);
                ExpectedMatchGroupResult group = builder.Build(outerGroupName);
                _nextIndex += group.Length;
                _value.Append(group.Value);
                _groups.Add(group);
                return this;
            }

            public ExpectedMatchResult Build() => new ExpectedMatchResult(_startIndex, _value.ToString(), _groups);

            private ExpectedMatchGroupResult Build(string groupName) => new ExpectedMatchGroupResult(groupName, _startIndex, _value.ToString());
        }

        public static IEnumerable<object[]> GetCharacterClassFirstTokenRegexTestData()
        {
            yield return ExpectedMatchResult.CreateTestData(@"^", 0, builder => builder.Grouped("neg", @"^").FailedGroup("lit", "em", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"-", 0, builder => builder.Grouped("lit", @"-").FailedGroup("neg", "em", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\^", 0, builder => builder.Grouped("em", @"\^").FailedGroup("neg", "lit", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\-", 0, builder => builder.Grouped("esc", @"\-").FailedGroup("neg", "lit", "em", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\-\x\u\p\\", 0, builder => builder.Grouped("esc", @"\-\x\u\p").FailedGroup("neg", "lit", "em", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"--", 0, builder => builder.Grouped("lit", @"--").FailedGroup("neg", "em", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"--?", 0, builder => builder.Grouped("lit", @"-").Grouped("r", @"-").FailedGroup("neg", "em", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"-[^$.|?*+(){}\", 0, builder => builder.Grouped("lit", @"-[^$.|?*+(){}").FailedGroup("neg", "em", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"-[^$.|?*+(){}\", 0, builder => builder.Grouped("lit", @"-[^$.|?*+(){}").FailedGroup("neg", "em", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"-[^$.|?*+(){}-", 0, builder => builder.Grouped("lit", @"-[^$.|?*+(){}-").FailedGroup("neg", "em", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"-[^$.|?*+(){}-\", 0, builder => builder.Grouped("lit", @"-[^$.|?*+(){}").Grouped("r", @"-").FailedGroup("neg", "em", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"\^\\\-\b", 0, builder => builder.Grouped("em", @"\^\\\-").FailedGroup("neg", "lit", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"^-", 0, builder => builder.Grouped("neg", @"^").Grouped("lit", @"-").FailedGroup("em", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"^\-", 0, builder => builder.Grouped("neg", @"^").Grouped("esc", @"\-").FailedGroup("lit", "em", "r"));

            yield return ExpectedMatchResult.CreateTestData(@"^\-\x\u\p\\", 0, builder => builder.Grouped("neg", @"^").Grouped("esc", @"\-\x\u\p").FailedGroup("lit", "em", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"^--", 0, builder => builder.Grouped("neg", @"^").Grouped("lit", @"--").FailedGroup("em", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"^--?", 0, builder => builder.Grouped("neg", @"^").Grouped("lit", @"-").Grouped("r", @"-").FailedGroup("em", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"\^-", 0, builder => builder.Grouped("em", @"\^").FailedGroup("neg", "lit", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\^-?", 0, builder => builder.Grouped("em", @"\^").Grouped("r", @"-").FailedGroup("neg", "lit", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"^-[^$.|?*+(){}\", 0, builder => builder.Grouped("neg", @"^").Grouped("lit", @"-[^$.|?*+(){}").FailedGroup("em", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"^-[^$.|?*+(){}-", 0, builder => builder.Grouped("neg", @"^").Grouped("lit", @"-[^$.|?*+(){}-").FailedGroup("em", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"^-[^$.|?*+(){}-\", 0, builder => builder.Grouped("neg", @"^").Grouped("lit", @"-[^$.|?*+(){}").Grouped("r", @"-").FailedGroup("em", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"\a", 0, ExpectedMatchResult.Failed);
            yield return ExpectedMatchResult.CreateTestData(@"]", 0, ExpectedMatchResult.Failed);
            yield return ExpectedMatchResult.CreateTestData(@" ", 0, ExpectedMatchResult.Failed);
            yield return ExpectedMatchResult.CreateTestData(@" ^", 0, ExpectedMatchResult.Failed);
            yield return ExpectedMatchResult.CreateTestData(@" -", 0, ExpectedMatchResult.Failed);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetCharacterClassFirstTokenRegexTestData), DynamicDataSourceType.Method)]
        public void CharacterClassFirstTokenRegexTestMethod(string input, int startAt, ExpectedMatchResult expectedMatch)
        {
            Match actualMatch = RegexParser.CharacterClassFirstTokenRegex.Match(input, startAt);
            Assert.AreEqual(expectedMatch.Success, actualMatch.Success);
            if (actualMatch.Success)
            {
                string expectedMatchedNames = string.Join("\n", expectedMatch.Groups.Where(g => g.Success).OrderBy(g => g.Name).Select(g => g.Name).Distinct());
                string actualMatchedNames = string.Join("\n", expectedMatch.Groups.Where(g => actualMatch.Groups[g.Name].Success).OrderBy(g => g.Name).Select(g => g.Name).Distinct());
                Assert.AreEqual(expectedMatchedNames, actualMatchedNames);
                Assert.AreEqual(expectedMatch.Value, actualMatch.Value);
                Assert.AreEqual(expectedMatch.Index, actualMatch.Index);
                Assert.AreEqual(expectedMatch.Length, actualMatch.Length);
                foreach (ExpectedMatchGroupResult expectedGroup in expectedMatch.Groups)
                {
                    Group actualGroup = actualMatch.Groups[expectedGroup.Name];
                    Assert.AreEqual(expectedGroup.Success, actualGroup.Success, $"Group = {expectedGroup.Name}");
                    if (actualGroup.Success)
                    {
                        Assert.AreEqual(expectedGroup.Value, actualGroup.Value, $"Group = {expectedGroup.Name}");
                        Assert.AreEqual(expectedGroup.Index, actualGroup.Index, $"Group = {expectedGroup.Name}");
                        Assert.AreEqual(expectedGroup.Length, actualGroup.Length, $"Group = {expectedGroup.Name}");
                    }
                }
            }
        }

        public static IEnumerable<object[]> GetCharacterClassNextTokenRegexTestData()
        {
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\b\e\f\v", 0, builder => builder.Grouped("ce", @"\r\n\t\a\b\e\f\v").FailedGroup("lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\b\e\f\v-", 0, builder => builder.Grouped("ce", @"\r\n\t\a\b\e\f\v").FailedGroup("lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\b\e\f\v-\z", 0, builder => builder.Grouped("ce", @"\r\n\t\a\b\e\f\v").Grouped("r", "-").FailedGroup("lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"\\\]\-", 0, builder => builder.Grouped("em", @"\\\]\-").FailedGroup("lit", "ce", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\\\^\-", 0, builder => builder.Grouped("em", @"\\").FailedGroup("lit", "ce", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\\\]\--", 0, builder => builder.Grouped("em", @"\\\]\-").FailedGroup("lit", "ce", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\\\]\--\\", 0, builder => builder.Grouped("em", @"\\\]\-").Grouped("r", "-").FailedGroup("lit", "ce", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc"));

            yield return ExpectedMatchResult.CreateTestData(@"\1", 0, builder => builder.Grouped("oct", @"\1").FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\7", 0, builder => builder.Grouped("oct", @"\7").FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\10", 0, builder => builder.Grouped("oct", @"\10").FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\77", 0, builder => builder.Grouped("oct", @"\77").FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\100", 0, builder => builder.Grouped("oct", @"\100").FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\177", 0, builder => builder.Grouped("oct", @"\177").FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\200", 0, builder => builder.Grouped("oct", @"\200").FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\377", 0, builder => builder.Grouped("oct", @"\377").FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\400", 0, builder => builder.Grouped("oct", @"\40").FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\700", 0, builder => builder.Grouped("oct", @"\70").FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\01", 0, builder => builder.Grouped("oct", @"\01").FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\07", 0, builder => builder.Grouped("oct", @"\07").FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\010", 0, builder => builder.Grouped("oct", @"\010").FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\077", 0, builder => builder.Grouped("oct", @"\077").FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\8", 0, builder => builder.Grouped("esc", @"\8").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\18", 0, builder => builder.Grouped("oct", @"\1").FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\78", 0, builder => builder.Grouped("oct", @"\7").FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\08", 0, builder => builder.Grouped("nul", @"\0").FailedGroup("lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\018", 0, builder => builder.Grouped("oct", @"\01").FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\078", 0, builder => builder.Grouped("oct", @"\07").FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\40\200\177\377\100\01\77\07\10\010\7\077\1", 0, builder => builder.Grouped("oct", @"\40\200\177\377\100\01\77\07\10\010\7\077\1")
                .FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\40\200\177\377\100\01\77\0\10\010\7\077\1", 0, builder => builder.Grouped("oct", @"\40\200\177\377\100\01\77")
                .FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\40\200\177\377\100\01\77\07\10\010\7\077\1-", 0, builder => builder.Grouped("oct", @"\40\200\177\377\100\01\77\07\10\010\7\077\1")
                .FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\40\200\177\377\100\01\77\07\10\010\7\077-\1", 0, builder => builder.Grouped("oct", @"\40\200\177\377\100\01\77\07\10\010\7\077").Grouped("r", "-")
                .FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc"));

            yield return ExpectedMatchResult.CreateTestData(@"\0", 0, builder => builder.Grouped("nul", @"\0").FailedGroup("lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\00", 0, builder => builder.Grouped("nul", @"\0").FailedGroup("lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\08", 0, builder => builder.Grouped("nul", @"\0\0\0\0").FailedGroup("lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\07", 0, builder => builder.Grouped("nul", @"\0\0\0").FailedGroup("lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0-\077", 0, builder => builder.Grouped("nul", @"\0\0\0").Grouped("r", "-").FailedGroup("lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "esc"));

            yield return ExpectedMatchResult.CreateTestData(@"\cA", 0, builder => builder.Grouped("ctl", @"\cA").FailedGroup("lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ", 0, builder => builder.Grouped("ctl", @"\cZ").FailedGroup("lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\c", 0, builder => builder.Grouped("esc", @"\c").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\ca", 0, builder => builder.Grouped("esc", @"\c").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\cz", 0, builder => builder.Grouped("esc", @"\c").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL\cM\cN\cO\cP\cQ\cR\cS\cT\cU\cV\cW\cX\cY", 0, builder => 
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL\cM\cN\cO\cP\cQ\cR\cS\cT\cU\cV\cW\cX\cY").FailedGroup("lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL\cm\cN", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL-", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL-\cm\cN", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Grouped("r", "-")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"\c\c\cZ", 0, builder => builder.Grouped("esc", @"\c\c").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "r"));

            yield return ExpectedMatchResult.CreateTestData(@"\xFF", 0, builder => builder.Grouped("hex", @"\xFF").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\xAA", 0, builder => builder.Grouped("hex", @"\xAA").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\xff", 0, builder => builder.Grouped("hex", @"\xff").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa", 0, builder => builder.Grouped("hex", @"\xaa").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\x00", 0, builder => builder.Grouped("hex", @"\x00").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\x99", 0, builder => builder.Grouped("hex", @"\x99").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xffff", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff-", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff-\uffff", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Grouped("r", "-")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"\xFFFF", 0, builder => builder.Grouped("hex", @"\xFF").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\xF", 0, builder => builder.Grouped("esc", @"\x").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\xFG", 0, builder => builder.Grouped("esc", @"\x").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\x0", 0, builder => builder.Grouped("esc", @"\x").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\x\x\x99", 0, builder => builder.Grouped("esc", @"\x\x").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "r"));

            yield return ExpectedMatchResult.CreateTestData(@"\uFFFF", 0, builder => builder.Grouped("uni", @"\uFFFF").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\uAAAA", 0, builder => builder.Grouped("uni", @"\uAAAA").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\uffff", 0, builder => builder.Grouped("uni", @"\uffff").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa", 0, builder => builder.Grouped("uni", @"\uaaaa").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\u9999", 0, builder => builder.Grouped("uni", @"\u9999").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\u0000", 0, builder => builder.Grouped("uni", @"\u0000").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000-", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000-\uFFFF", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000").Grouped("r", "-")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"\uFFF", 0, builder => builder.Grouped("esc", @"\u").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\uFF", 0, builder => builder.Grouped("esc", @"\u").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\uF", 0, builder => builder.Grouped("esc", @"\u").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\u999", 0, builder => builder.Grouped("esc", @"\u").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaa", 0, builder => builder.Grouped("esc", @"\u").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\u0", 0, builder => builder.Grouped("esc", @"\u").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\u\u\u0000", 0, builder => builder.Grouped("esc", @"\u\u").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "r"));

            yield return ExpectedMatchResult.CreateTestData(@"\p{C}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"C").Ungrouped(@"}"))
                .FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{C}\p{C}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"C").Ungrouped(@"}"))
                .FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}"))
                .FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}-", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}"))
                .FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}-\", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Grouped("r", "-")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"\p\p\p{C}", 0, builder => builder.Grouped("esc", @"\p\p").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc", 0, builder => builder.Grouped("esc", @"\p").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc }", 0, builder => builder.Grouped("esc", @"\p").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "r"));

            yield return ExpectedMatchResult.CreateTestData(@"[", 0, builder => builder.Grouped("lit", @"[").FailedGroup("ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"}", 0, builder => builder.Grouped("lit", @"}").FailedGroup("ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"-[^$.|?*+(){}", 0, builder => builder.Grouped("lit", @"-[^$.|?*+(){}").FailedGroup("ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"[^$.|?*+(){}-", 0, builder => builder.Grouped("lit", @"[^$.|?*+(){}-").FailedGroup("ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"-[^$.|?*+(){}-", 0, builder => builder.Grouped("lit", @"-[^$.|?*+(){}-").FailedGroup("ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"[^$.|?*+(){}-.", 0, builder => builder.Grouped("lit", @"[^$.|?*+(){}").Grouped("r", "-").FailedGroup("ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"-[^$.|?*+(){}-.", 0, builder => builder.Grouped("lit", @"-[^$.|?*+(){}").Grouped("r", "-").FailedGroup("ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc"));

            yield return ExpectedMatchResult.CreateTestData(@"^\r\n\t\a\b\e\f\v-", 1, builder => builder.Grouped("ce", @"\r\n\t\a\b\e\f\v").FailedGroup("lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"-\r\n\t\a\b\e\f\v-\z", 1, builder => builder.Grouped("ce", @"\r\n\t\a\b\e\f\v").Grouped("r", "-")
                .FailedGroup("lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"^\\\]\--", 1, builder => builder.Grouped("em", @"\\\]\-").FailedGroup("lit", "ce", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"-\\\]\--\\", 1, builder => builder.Grouped("em", @"\\\]\-").Grouped("r", "-").FailedGroup("lit", "ce", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"^\40\200\177\377\100\01\77\07\10\010\7\077\1", 1, builder => builder.Grouped("oct", @"\40\200\177\377\100\01\77\07\10\010\7\077\1")
                .FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"-\40\200\177\377\100\01\77\07\10\010\7\077-\1", 1, builder => builder.Grouped("oct", @"\40\200\177\377\100\01\77\07\10\010\7\077").Grouped("r", "-")
                .FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"^\0\0\0-\077", 1, builder => builder.Grouped("nul", @"\0\0\0").Grouped("r", "-").FailedGroup("lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"-\0\0\0\08", 1, builder => builder.Grouped("nul", @"\0\0\0\0").FailedGroup("lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"^\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL-", 1, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"-\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL-\cm\cN", 1, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Grouped("r", "-")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"^\xaa\x99\xFF\xAA\x00\xff", 1, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"-\xaa\x99\xFF\xAA\x00\xff-\uffff", 1, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Grouped("r", "-")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"^\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF", 1, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"-\uaaaa\uffff\u9999\uAAAA\u0000-\uFFFF", 1, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000").Grouped("r", "-")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"^\p{C}", 1, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"C").Ungrouped(@"}")).FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"-\p{Cc}-\", 1, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Grouped("r", "-")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"^-[^$.|?*+(){}", 1, builder => builder.Grouped("lit", @"-[^$.|?*+(){}").FailedGroup("ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));

            yield return ExpectedMatchResult.CreateTestData(@"\^\r\n\t\a\b\e\f\v-", 2, builder => builder.Grouped("ce", @"\r\n\t\a\b\e\f\v").FailedGroup("lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\-\r\n\t\a\b\e\f\v-\z", 2, builder => builder.Grouped("ce", @"\r\n\t\a\b\e\f\v").Grouped("r", "-")
                .FailedGroup("lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"--\\\]\--", 2, builder => builder.Grouped("em", @"\\\]\-").FailedGroup("lit", "ce", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"^-\\\]\--\\", 2, builder => builder.Grouped("em", @"\\\]\-").Grouped("r", "-").FailedGroup("lit", "ce", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"\^\40\200\177\377\100\01\77\07\10\010\7\077\1", 2, builder => builder.Grouped("oct", @"\40\200\177\377\100\01\77\07\10\010\7\077\1")
                .FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\-\40\200\177\377\100\01\77\07\10\010\7\077-\1", 2, builder => builder.Grouped("oct", @"\40\200\177\377\100\01\77\07\10\010\7\077").Grouped("r", "-")
                .FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"--\0\0\0-\077", 2, builder => builder.Grouped("nul", @"\0\0\0").Grouped("r", "-").FailedGroup("lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"^-\0\0\0\08", 2, builder => builder.Grouped("nul", @"\0\0\0\0").FailedGroup("lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\^\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL-", 2, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\-\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL-\cm\cN", 2, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Grouped("r", "-")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"--\xaa\x99\xFF\xAA\x00\xff", 2, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"^-\xaa\x99\xFF\xAA\x00\xff-\uffff", 2, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Grouped("r", "-")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"\^\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF", 2, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\-\uaaaa\uffff\u9999\uAAAA\u0000-\uFFFF", 2, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000").Grouped("r", "-")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"--\p{C}", 2, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"C").Ungrouped(@"}")).FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"^-\p{Cc}-\", 2, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Grouped("r", "-")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"\^-[^$.|?*+(){}", 2, builder => builder.Grouped("lit", @"-[^$.|?*+(){}").FailedGroup("ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\--[^$.|?*+(){}-.", 2, builder => builder.Grouped("lit", @"-[^$.|?*+(){}").Grouped("r", "-").FailedGroup("ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc"));

            yield return ExpectedMatchResult.CreateTestData(@"^\-\r\n\t\a\b\e\f\v-", 3, builder => builder.Grouped("ce", @"\r\n\t\a\b\e\f\v").FailedGroup("lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"^--\r\n\t\a\b\e\f\v-\z", 3, builder => builder.Grouped("ce", @"\r\n\t\a\b\e\f\v").Grouped("r", "-")
                .FailedGroup("lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"\^-\\\]\--", 3, builder => builder.Grouped("em", @"\\\]\-").FailedGroup("lit", "ce", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"^\-\\\]\--\\", 3, builder => builder.Grouped("em", @"\\\]\-").Grouped("r", "-").FailedGroup("lit", "ce", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"^--\40\200\177\377\100\01\77\07\10\010\7\077\1", 3, builder => builder.Grouped("oct", @"\40\200\177\377\100\01\77\07\10\010\7\077\1")
                .FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\^-\40\200\177\377\100\01\77\07\10\010\7\077-\1", 3, builder => builder.Grouped("oct", @"\40\200\177\377\100\01\77\07\10\010\7\077").Grouped("r", "-")
                .FailedGroup("lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"^\-\0\0\0-\077", 3, builder => builder.Grouped("nul", @"\0\0\0").Grouped("r", "-").FailedGroup("lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"^--\0\0\0\08", 3, builder => builder.Grouped("nul", @"\0\0\0\0").FailedGroup("lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\^-\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL-", 3, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"^\-\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL-\cm\cN", 3, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Grouped("r", "-")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"^--\xaa\x99\xFF\xAA\x00\xff", 3, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\^-\xaa\x99\xFF\xAA\x00\xff-\uffff", 3, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Grouped("r", "-")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"^\-\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF", 3, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"^--\uaaaa\uffff\u9999\uAAAA\u0000-\uFFFF", 3, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000").Grouped("r", "-")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"\^-\p{C}", 3, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"C").Ungrouped(@"}")).FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"^\-\p{Cc}-\", 3, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Grouped("r", "-")
                .FailedGroup("lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "esc"));
            yield return ExpectedMatchResult.CreateTestData(@"^---[^$.|?*+(){}", 3, builder => builder.Grouped("lit", @"-[^$.|?*+(){}").FailedGroup("ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc", "r"));
            yield return ExpectedMatchResult.CreateTestData(@"\^--[^$.|?*+(){}-.", 3, builder => builder.Grouped("lit", @"-[^$.|?*+(){}").Grouped("r", "-").FailedGroup("ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "esc"));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetCharacterClassNextTokenRegexTestData), DynamicDataSourceType.Method)]
        public void CharacterClassNextTokenRegexTestMethod(string input, int startAt, ExpectedMatchResult expectedMatch)
        {
            Match actualMatch = RegexParser.CharacterClassNextTokenRegex.Match(input, startAt);
            Assert.AreEqual(expectedMatch.Success, actualMatch.Success);
            if (actualMatch.Success)
            {
                string expectedMatchedNames = string.Join("\n", expectedMatch.Groups.Where(g => g.Success).OrderBy(g => g.Name).Select(g => g.Name).Distinct());
                string actualMatchedNames = string.Join("\n", expectedMatch.Groups.Where(g => actualMatch.Groups[g.Name].Success).OrderBy(g => g.Name).Select(g => g.Name).Distinct());
                Assert.AreEqual(expectedMatchedNames, actualMatchedNames);
                Assert.AreEqual(expectedMatch.Value, actualMatch.Value);
                Assert.AreEqual(expectedMatch.Index, actualMatch.Index);
                Assert.AreEqual(expectedMatch.Length, actualMatch.Length);
                foreach (ExpectedMatchGroupResult expectedGroup in expectedMatch.Groups)
                {
                    Group actualGroup = actualMatch.Groups[expectedGroup.Name];
                    Assert.AreEqual(expectedGroup.Success, actualGroup.Success, $"Group = {expectedGroup.Name}");
                    if (actualGroup.Success)
                    {
                        Assert.AreEqual(expectedGroup.Value, actualGroup.Value, $"Group = {expectedGroup.Name}");
                        Assert.AreEqual(expectedGroup.Index, actualGroup.Index, $"Group = {expectedGroup.Name}");
                        Assert.AreEqual(expectedGroup.Length, actualGroup.Length, $"Group = {expectedGroup.Name}");
                    }
                }
            }
        }

        public static IEnumerable<object[]> GetSingleLinePatternTokenRegexTestData()
        {
            yield return ExpectedMatchResult.CreateTestData(@"^", 0, builder => builder.Grouped("anc", @"^")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"$", 0, builder => builder.Grouped("anc", @"$")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\G", 0, builder => builder.Grouped("anc", @"\G")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\A", 0, builder => builder.Grouped("anc", @"\A")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\Z", 0, builder => builder.Grouped("anc", @"\Z")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\z", 0, builder => builder.Grouped("anc", @"\z")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return ExpectedMatchResult.CreateTestData(@"|", 0, builder => builder.Grouped("alt", @"|")
                .FailedGroup("anc", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return ExpectedMatchResult.CreateTestData(@".", 0, builder => builder.Grouped("any", @".")
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\e\f\v", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return ExpectedMatchResult.CreateTestData(@"\[\\\^\$\.\|\?\*\+\(\)\{\}", 0, builder => builder.Grouped("em", @"\[\\\^\$\.\|\?\*\+\(\)\{\}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy"));

            yield return ExpectedMatchResult.CreateTestData(@"\100", 0, builder => builder.Grouped("oct", @"\100")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\177", 0, builder => builder.Grouped("oct", @"\177")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\200", 0, builder => builder.Grouped("oct", @"\200")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\377", 0, builder => builder.Grouped("oct", @"\377")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\400", 0, builder => builder.Grouped("dbr", @"\40")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\700", 0, builder => builder.Grouped("dbr", @"\70")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\01", 0, builder => builder.Grouped("oct", @"\01")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\07", 0, builder => builder.Grouped("oct", @"\07")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\010", 0, builder => builder.Grouped("oct", @"\010")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\077", 0, builder => builder.Grouped("oct", @"\077")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077\1", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077-", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return ExpectedMatchResult.CreateTestData(@"\0", 0, builder => builder.Grouped("nul", @"\0")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\00", 0, builder => builder.Grouped("nul", @"\0")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\08", 0, builder => builder.Grouped("nul", @"\0\0\0\0")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\07", 0, builder => builder.Grouped("nul", @"\0\0\0")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return ExpectedMatchResult.CreateTestData(@"\cA", 0, builder => builder.Grouped("ctl", @"\cA")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ", 0, builder => builder.Grouped("ctl", @"\cZ")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return ExpectedMatchResult.CreateTestData(@"\c", 0, builder => builder.Grouped("esc", @"\c")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\ca", 0, builder => builder.Grouped("esc", @"\c")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\cz", 0, builder => builder.Grouped("esc", @"\c")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL\cM\cN\cO\cP\cQ\cR\cS\cT\cU\cV\cW\cX\cY", 0, 
                builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL\cM\cN\cO\cP\cQ\cR\cS\cT\cU\cV\cW\cX\cY")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL\cm\cN", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL-", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return ExpectedMatchResult.CreateTestData(@"\c\c\cZ", 0, builder => builder.Grouped("esc", @"\c\c")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
           
            yield return ExpectedMatchResult.CreateTestData(@"\xFF", 0, builder => builder.Grouped("hex", @"\xFF")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\xAA", 0, builder => builder.Grouped("hex", @"\xAA")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\xff", 0, builder => builder.Grouped("hex", @"\xff")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa", 0, builder => builder.Grouped("hex", @"\xaa")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\x00", 0, builder => builder.Grouped("hex", @"\x00")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\x99", 0, builder => builder.Grouped("hex", @"\x99")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xffff", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff-", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\xFFFF", 0, builder => builder.Grouped("hex", @"\xFF")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\xF", 0, builder => builder.Grouped("esc", @"\x")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\xFG", 0, builder => builder.Grouped("esc", @"\x")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\x0", 0, builder => builder.Grouped("esc", @"\x")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\x\x\x99", 0, builder => builder.Grouped("esc", @"\x\x")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return ExpectedMatchResult.CreateTestData(@"\uFFFF", 0, builder => builder.Grouped("uni", @"\uFFFF")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\uAAAA", 0, builder => builder.Grouped("uni", @"\uAAAA")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\uffff", 0, builder => builder.Grouped("uni", @"\uffff")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa", 0, builder => builder.Grouped("uni", @"\uaaaa")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\u9999", 0, builder => builder.Grouped("uni", @"\u9999")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\u0000", 0, builder => builder.Grouped("uni", @"\u0000")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000-", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\uFFF", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\uFF", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\uF", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\u999", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaa", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\u0", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\u\u\u0000", 0, builder => builder.Grouped("esc", @"\u\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return ExpectedMatchResult.CreateTestData(@"\p{C}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"C").Ungrouped(@"}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{C}\p{C}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"C").Ungrouped(@"}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}-", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return ExpectedMatchResult.CreateTestData(@"\p\p\p{C}", 0, builder => builder.Grouped("esc", @"\p\p")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc", 0, builder => builder.Grouped("esc", @"\p")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc }", 0, builder => builder.Grouped("esc", @"\p")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return ExpectedMatchResult.CreateTestData(@" ", 0, builder => builder.Grouped("lit", @" ")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@#", 0, builder => builder.Grouped("lit", @"abc123!@#")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"{", 0, builder => builder.Grouped("lit", @"{")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"}", 0, builder => builder.Grouped("lit", @"}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"{1}", 0, builder => builder.Grouped("lit", @"{1}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"{1,}", 0, builder => builder.Grouped("lit", @"{1,}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"{1,2}", 0, builder => builder.Grouped("lit", @"{1,2}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@#{}", 0, builder => builder.Grouped("lit", @"abc123!@#{}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return ExpectedMatchResult.CreateTestData(@"abc123!@#{,}", 0, builder => builder.Grouped("lit", @"abc123!@#{,}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@#{,3}", 0, builder => builder.Grouped("lit", @"abc123!@#{,3}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return ExpectedMatchResult.CreateTestData("()", 0, builder => builder.Nested("grp", "(", b => b.Grouped("ptn", "").Ungrouped(")"))
                .FailedGroup("anc", "alt", "any", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"(?(?=test)one|two)", 0, builder => builder.Nested("grp", "(", b => b.Grouped("ptn", @"?(?=test)one|two").Ungrouped(")"))
                .FailedGroup("anc", "alt", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"(?(?=#test)one|two)", 0, builder => builder.Nested("grp", "(", b => b.Grouped("ptn", @"?(?=#test)one|two").Ungrouped(")"))
                .FailedGroup("anc", "alt", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"(?\(?=test)one|two)", 0, builder => builder.Nested("grp", "(", b => b.Grouped("ptn", @"?\(?=test").Ungrouped(")"))
                .FailedGroup("anc", "alt", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            //.FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@".{33}", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@".{9,}", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@".{10,11}", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@".?", 0, builder => builder.Grouped("any", @".").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@".+", 0, builder => builder.Grouped("any", @".").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\e\f\v{33}", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\e\f\v{9,}", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\e\f\v{10,11}", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\e\f\v?", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\e\f\v+", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077{33}", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077{9,}", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077{10,11}", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{", b => 
                b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077?", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077+", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\0{33}", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\0{9,}", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\0{10,11}", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\0?", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\0+", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{33}", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{9,}", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{", 
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{10,11}", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL?", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL+", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{33}", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{9,}", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{10,11}", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", 
                r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff?", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff+", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{33}", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{9,}", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{", 
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{10,11}", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF?", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF+", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}{33}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}{9,}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}{10,11}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", 
                r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}?", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}+", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@#{1}", 0, builder => builder.Grouped("lit", @"abc123!@#").Nested("q", "{", b => b.Grouped("min", "1").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@#{1,}", 0, builder => builder.Grouped("lit", @"abc123!@#").Nested("q", "{", b => b.Grouped("min", "1").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@#{1,3}", 0, builder => builder.Grouped("lit", @"abc123!@#").Nested("q", "{", b => b.Grouped("min", "1").Nested("qr", ",", r => r.Grouped("max", "3")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@#?", 0, builder => builder.Grouped("lit", @"abc123!@#").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@#+", 0, builder => builder.Grouped("lit", @"abc123!@#").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy"));

            yield return ExpectedMatchResult.CreateTestData(@"#{1}", 0, builder => builder.Grouped("lit", @"#").Nested("q", "{", b => b.Grouped("min", "1").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"#{1,}\nTest", 0, builder => builder.Grouped("lit", @"#").Nested("q", "{", b => b.Grouped("min", "1").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData("#{1,3}Test", 0, builder => builder.Grouped("lit", "#").Nested("q", "{", b => b.Grouped("min", "1").Nested("qr", ",", r => r.Grouped("max", "3")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy"));

            yield return ExpectedMatchResult.CreateTestData(@"[]{1}", 0, builder => builder.Grouped("cc", @"[]").Nested("q", "{", b => b.Grouped("min", "1").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"[one|\]two]{1,}", 0, builder => builder.Nested("cc", "[", b => b.Grouped("ptn", @"one|\]two").Ungrouped("]")).Nested("q", "{", b => b.Grouped("min", "1").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"[]{1,3}", 0, builder => builder.Grouped("cc", @"[]").Nested("q", "{", b => b.Grouped("min", "1").Nested("qr", ",", r => r.Grouped("max", "3")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"[one|\]two]?", 0, builder => builder.Nested("cc", "[", b => b.Grouped("ptn", @"one|\]two").Ungrouped("]")).Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"[]+", 0, builder => builder.Grouped("cc", @"[]").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "min", "qr", "max", "lzy"));

            //.FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@".{33}?", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@".{9,}?", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max"));
            yield return ExpectedMatchResult.CreateTestData(@".{10,11}?", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt"));
            yield return ExpectedMatchResult.CreateTestData(@".??", 0, builder => builder.Grouped("any", @".").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@".+?", 0, builder => builder.Grouped("any", @".").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\e\f\v{33}?", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\e\f\v{9,}?", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\e\f\v{10,11}?", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", 
                r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt"));
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\e\f\v??", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\e\f\v+?", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077{33}?", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077{9,}?", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{", 
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077{10,11}?", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", 
                r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077??", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077+?", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\0{33}?", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\0{9,}?", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\0{10,11}?", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => 
                r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\0??", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\0+?", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{33}?", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{", 
                b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{9,}?", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{", 
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{10,11}?", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL??", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL+?", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{33}?", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{9,}?", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{", 
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{10,11}?", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", 
                r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff??", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff+?", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{33}?", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{", 
                b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{9,}?", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{", 
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{10,11}?", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF??", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF+?", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}{33}?", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}{9,}?", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{", 
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}{10,11}?", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",",
                r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}??", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}+?", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@#{1}?", 0, builder => builder.Grouped("lit", @"abc123!@#").Nested("q", "{", b => b.Grouped("min", "1").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@#{1,}?", 0, builder => builder.Grouped("lit", @"abc123!@#").Nested("q", "{", b => b.Grouped("min", "1").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@#{1,3}?", 0, builder => builder.Grouped("lit", @"abc123!@#").Nested("q", "{", b => b.Grouped("min", "1").Nested("qr", ",", 
                r => r.Grouped("max", "3")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@#??", 0, builder => builder.Grouped("lit", @"abc123!@#").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@#+?", 0, builder => builder.Grouped("lit", @"abc123!@#").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max"));

            yield return ExpectedMatchResult.CreateTestData(@"[]{1}?", 0, builder => builder.Grouped("cc", @"[]").Nested("q", "{", b => b.Grouped("min", "1").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"[one|\]two]{1,}?", 0,
                builder => builder.Nested("cc", "[", b => b.Grouped("ptn", @"one|\]two").Ungrouped("]")).Nested("q", "{", b => b.Grouped("min", "1").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"[]{1,3}?", 0, builder => builder.Grouped("cc", @"[]").Nested("q", "{", b => b.Grouped("min", "1").Nested("qr", ",", r => r.Grouped("max", "3")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt"));
            yield return ExpectedMatchResult.CreateTestData(@"[one|\]two]??", 0, builder => builder.Nested("cc", "[", b => b.Grouped("ptn", @"one|\]two").Ungrouped("]")).Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "mlt", "min", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"[]+?", 0, builder => builder.Grouped("cc", @"[]").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "min", "qr", "max"));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetSingleLinePatternTokenRegexTestData), DynamicDataSourceType.Method)]
        public void SingleLinePatternTokenRegexTestMethod(string input, int startAt, ExpectedMatchResult expectedMatch)
        {
            Match actualMatch = RegexParser.SingleLinePatternTokenRegex.Match(input, startAt);
            Assert.AreEqual(expectedMatch.Success, actualMatch.Success);
            if (actualMatch.Success)
            {
                string expectedMatchedNames = string.Join("\n", expectedMatch.Groups.Where(g => g.Success).OrderBy(g => g.Name).Select(g => g.Name).Distinct());
                string actualMatchedNames = string.Join("\n", expectedMatch.Groups.Where(g => actualMatch.Groups[g.Name].Success).OrderBy(g => g.Name).Select(g => g.Name).Distinct());
                Assert.AreEqual(expectedMatchedNames, actualMatchedNames);
                Assert.AreEqual(expectedMatch.Value, actualMatch.Value);
                Assert.AreEqual(expectedMatch.Index, actualMatch.Index);
                Assert.AreEqual(expectedMatch.Length, actualMatch.Length);
                foreach (ExpectedMatchGroupResult expectedGroup in expectedMatch.Groups)
                {
                    Group actualGroup = actualMatch.Groups[expectedGroup.Name];
                    Assert.AreEqual(expectedGroup.Success, actualGroup.Success, $"Group = {expectedGroup.Name}");
                    if (actualGroup.Success)
                    {
                        Assert.AreEqual(expectedGroup.Value, actualGroup.Value, $"Group = {expectedGroup.Name}");
                        Assert.AreEqual(expectedGroup.Index, actualGroup.Index, $"Group = {expectedGroup.Name}");
                        Assert.AreEqual(expectedGroup.Length, actualGroup.Length, $"Group = {expectedGroup.Name}");
                    }
                }
            }
        }

        public static IEnumerable<object[]> GetMultilinePatternTokenRegexTestData()
        {
            yield return ExpectedMatchResult.CreateTestData(@"^", 0, builder => builder.Grouped("anc", @"^")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"$", 0, builder => builder.Grouped("anc", @"$")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\G", 0, builder => builder.Grouped("anc", @"\G")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\A", 0, builder => builder.Grouped("anc", @"\A")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\Z", 0, builder => builder.Grouped("anc", @"\Z")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\z", 0, builder => builder.Grouped("anc", @"\z")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return ExpectedMatchResult.CreateTestData(@"|", 0, builder => builder.Grouped("alt", @"|")
                .FailedGroup("anc", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return ExpectedMatchResult.CreateTestData(@".", 0, builder => builder.Grouped("any", @".")
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\e\f\v", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return ExpectedMatchResult.CreateTestData(@"\[\\\^\$\.\|\?\*\+\(\)\{\}", 0, builder => builder.Grouped("em", @"\[\\\^\$\.\|\?\*\+\(\)\{\}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return ExpectedMatchResult.CreateTestData(@"\100", 0, builder => builder.Grouped("oct", @"\100")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\177", 0, builder => builder.Grouped("oct", @"\177")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\200", 0, builder => builder.Grouped("oct", @"\200")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\377", 0, builder => builder.Grouped("oct", @"\377")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\400", 0, builder => builder.Grouped("dbr", @"\40")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\700", 0, builder => builder.Grouped("dbr", @"\70")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\01", 0, builder => builder.Grouped("oct", @"\01")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\07", 0, builder => builder.Grouped("oct", @"\07")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\010", 0, builder => builder.Grouped("oct", @"\010")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\077", 0, builder => builder.Grouped("oct", @"\077")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077\1", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077-", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return ExpectedMatchResult.CreateTestData(@"\0", 0, builder => builder.Grouped("nul", @"\0")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\00", 0, builder => builder.Grouped("nul", @"\0")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\08", 0, builder => builder.Grouped("nul", @"\0\0\0\0")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\07", 0, builder => builder.Grouped("nul", @"\0\0\0")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return ExpectedMatchResult.CreateTestData(@"\cA", 0, builder => builder.Grouped("ctl", @"\cA")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ", 0, builder => builder.Grouped("ctl", @"\cZ")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return ExpectedMatchResult.CreateTestData(@"\c", 0, builder => builder.Grouped("esc", @"\c")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\ca", 0, builder => builder.Grouped("esc", @"\c")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\cz", 0, builder => builder.Grouped("esc", @"\c")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL\cM\cN\cO\cP\cQ\cR\cS\cT\cU\cV\cW\cX\cY", 0,
                builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL\cM\cN\cO\cP\cQ\cR\cS\cT\cU\cV\cW\cX\cY")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL\cm\cN", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL-", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return ExpectedMatchResult.CreateTestData(@"\c\c\cZ", 0, builder => builder.Grouped("esc", @"\c\c")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return ExpectedMatchResult.CreateTestData(@"\xFF", 0, builder => builder.Grouped("hex", @"\xFF")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\xAA", 0, builder => builder.Grouped("hex", @"\xAA")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\xff", 0, builder => builder.Grouped("hex", @"\xff")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa", 0, builder => builder.Grouped("hex", @"\xaa")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\x00", 0, builder => builder.Grouped("hex", @"\x00")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\x99", 0, builder => builder.Grouped("hex", @"\x99")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xffff", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff-", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\xFFFF", 0, builder => builder.Grouped("hex", @"\xFF")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\xF", 0, builder => builder.Grouped("esc", @"\x")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\xFG", 0, builder => builder.Grouped("esc", @"\x")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\x0", 0, builder => builder.Grouped("esc", @"\x")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\x\x\x99", 0, builder => builder.Grouped("esc", @"\x\x")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return ExpectedMatchResult.CreateTestData(@"\uFFFF", 0, builder => builder.Grouped("uni", @"\uFFFF")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\uAAAA", 0, builder => builder.Grouped("uni", @"\uAAAA")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\uffff", 0, builder => builder.Grouped("uni", @"\uffff")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa", 0, builder => builder.Grouped("uni", @"\uaaaa")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\u9999", 0, builder => builder.Grouped("uni", @"\u9999")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\u0000", 0, builder => builder.Grouped("uni", @"\u0000")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000-", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\uFFF", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\uFF", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\uF", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\u999", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaa", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\u0", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\u\u\u0000", 0, builder => builder.Grouped("esc", @"\u\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return ExpectedMatchResult.CreateTestData(@"\p{C}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"C").Ungrouped(@"}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{C}\p{C}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"C").Ungrouped(@"}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}-", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return ExpectedMatchResult.CreateTestData(@"\p\p\p{C}", 0, builder => builder.Grouped("esc", @"\p\p")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc", 0, builder => builder.Grouped("esc", @"\p")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc }", 0, builder => builder.Grouped("esc", @"\p")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return ExpectedMatchResult.CreateTestData(@" ", 0, builder => builder.Grouped("lit", @" ")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@", 0, builder => builder.Grouped("lit", @"abc123!@")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"{", 0, builder => builder.Grouped("lit", @"{")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"}", 0, builder => builder.Grouped("lit", @"}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"{1}", 0, builder => builder.Grouped("lit", @"{1}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"{1,}", 0, builder => builder.Grouped("lit", @"{1,}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"{1,2}", 0, builder => builder.Grouped("lit", @"{1,2}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@{}", 0, builder => builder.Grouped("lit", @"abc123!@{}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return ExpectedMatchResult.CreateTestData(@"abc123!@{,}", 0, builder => builder.Grouped("lit", @"abc123!@{,}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@{,3}", 0, builder => builder.Grouped("lit", @"abc123!@{,3}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return ExpectedMatchResult.CreateTestData(@"#{,3}", 0, builder => builder.Nested("cmt", "#", b => b.Grouped("t", "{,3}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData("#{,3}\nTest", 0, builder => builder.Nested("cmt", "#", b => b.Grouped("t", "{,3}")).Ungrouped("\n")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData("#\nTest", 0, builder => builder.Grouped("cmt", "#").Ungrouped("\n")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "t"));

            yield return ExpectedMatchResult.CreateTestData("()", 0, builder => builder.Nested("grp", "(", b => b.Grouped("ptn", "").Ungrouped(")"))
                .FailedGroup("anc", "alt", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData("(?(?=test)one|two)", 0, builder => builder.Nested("grp", "(", b => b.Grouped("ptn", "?(?=test)one|two").Ungrouped(")"))
                .FailedGroup("anc", "alt", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData("(?#(?=\ntest)one|two)", 0, builder => builder.Nested("grp", "(", b => b.Grouped("ptn", "?#(?=\ntest").Ungrouped(")"))
                .FailedGroup("anc", "alt", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"(?\(?=test)one|two)", 0, builder => builder.Nested("grp", "(", b => b.Grouped("ptn", @"?\(?=test").Ungrouped(")"))
                .FailedGroup("anc", "alt", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return ExpectedMatchResult.CreateTestData(@"[]", 0, builder => builder.Grouped("cc", @"[]")
                .FailedGroup("anc", "alt", "grp", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"[one|\]two]", 0, builder => builder.Nested("cc", "[", b => b.Grouped("ptn", @"one|\]two").Ungrouped("]"))
                .FailedGroup("anc", "alt", "grp", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            //.FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@".{33}", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@".{9,}", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@".{10,11}", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@".?", 0, builder => builder.Grouped("any", @".").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@".+", 0, builder => builder.Grouped("any", @".").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\e\f\v{33}", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\e\f\v{9,}", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\e\f\v{10,11}", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\e\f\v?", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\e\f\v+", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077{33}", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077{9,}", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077{10,11}", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{", b =>
                b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077?", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077+", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\0{33}", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\0{9,}", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\0{10,11}", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\0?", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\0+", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{33}", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{9,}", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{",
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{10,11}", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL?", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL+", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{33}", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{9,}", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{10,11}", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",",
                r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff?", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff+", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{33}", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{9,}", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{",
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{10,11}", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF?", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF+", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}{33}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}{9,}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}{10,11}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",",
                r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}?", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}+", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@{1}", 0, builder => builder.Grouped("lit", @"abc123!@").Nested("q", "{", b => b.Grouped("min", "1").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@{1,}", 0, builder => builder.Grouped("lit", @"abc123!@").Nested("q", "{", b => b.Grouped("min", "1").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@{1,3}", 0, builder => builder.Grouped("lit", @"abc123!@").Nested("q", "{", b => b.Grouped("min", "1").Nested("qr", ",", r => r.Grouped("max", "3")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@?", 0, builder => builder.Grouped("lit", @"abc123!@").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@+", 0, builder => builder.Grouped("lit", @"abc123!@").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return ExpectedMatchResult.CreateTestData(@"[]{1}", 0, builder => builder.Grouped("cc", @"[]").Nested("q", "{", b => b.Grouped("min", "1").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"[one|\]two]{1,}", 0, builder => builder.Nested("cc", "[", b => b.Grouped("ptn", @"one|\]two").Ungrouped("]")).Nested("q", "{", b => b.Grouped("min", "1").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"[]{1,3}", 0, builder => builder.Grouped("cc", @"[]").Nested("q", "{", b => b.Grouped("min", "1").Nested("qr", ",", r => r.Grouped("max", "3")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"[one|\]two]?", 0, builder => builder.Nested("cc", "[", b => b.Grouped("ptn", @"one|\]two").Ungrouped("]")).Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"[]+", 0, builder => builder.Grouped("cc", @"[]").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "min", "qr", "max", "lzy", "cmt", "t"));

            //.FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@".{33}?", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@".{9,}?", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@".{10,11}?", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@".??", 0, builder => builder.Grouped("any", @".").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@".+?", 0, builder => builder.Grouped("any", @".").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\e\f\v{33}?", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\e\f\v{9,}?", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\e\f\v{10,11}?", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",",
                r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\e\f\v??", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\r\n\t\a\e\f\v+?", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077{33}?", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077{9,}?", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{",
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077{10,11}?", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",",
                r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077??", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\200\177\377\100\010\077+?", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\0{33}?", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\0{9,}?", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\0{10,11}?", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r =>
                r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\0??", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\0\0\0\0+?", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{33}?", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{",
                b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{9,}?", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{",
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{10,11}?", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL??", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL+?", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{33}?", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{9,}?", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{",
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{10,11}?", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",",
                r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff??", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff+?", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{33}?", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{",
                b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{9,}?", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{",
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{10,11}?", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF??", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF+?", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}{33}?", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}{9,}?", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{",
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}{10,11}?", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",",
                r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}??", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"\p{Cc}+?", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@{1}?", 0, builder => builder.Grouped("lit", @"abc123!@").Nested("q", "{", b => b.Grouped("min", "1").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@{1,}?", 0, builder => builder.Grouped("lit", @"abc123!@").Nested("q", "{", b => b.Grouped("min", "1").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@{1,3}?", 0, builder => builder.Grouped("lit", @"abc123!@").Nested("q", "{", b => b.Grouped("min", "1").Nested("qr", ",",
                r => r.Grouped("max", "3")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@??", 0, builder => builder.Grouped("lit", @"abc123!@").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"abc123!@+?", 0, builder => builder.Grouped("lit", @"abc123!@").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "max", "cmt", "t"));

            yield return ExpectedMatchResult.CreateTestData(@"[]{1}?", 0, builder => builder.Grouped("cc", @"[]").Nested("q", "{", b => b.Grouped("min", "1").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"[one|\]two]{1,}?", 0,
                builder => builder.Nested("cc", "[", b => b.Grouped("ptn", @"one|\]two").Ungrouped("]")).Nested("q", "{", b => b.Grouped("min", "1").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"[]{1,3}?", 0, builder => builder.Grouped("cc", @"[]").Nested("q", "{", b => b.Grouped("min", "1").Nested("qr", ",", r => r.Grouped("max", "3")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"[one|\]two]??", 0, builder => builder.Nested("cc", "[", b => b.Grouped("ptn", @"one|\]two").Ungrouped("]")).Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "mlt", "min", "qr", "max", "max", "cmt", "t"));
            yield return ExpectedMatchResult.CreateTestData(@"[]+?", 0, builder => builder.Grouped("cc", @"[]").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "min", "qr", "max", "cmt", "t"));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetMultilinePatternTokenRegexTestData), DynamicDataSourceType.Method)]
        public void MultilinePatternTokenRegexTestMethod(string input, int startAt, ExpectedMatchResult expectedMatch)
        {
            Match actualMatch = RegexParser.MultilinePatternTokenRegex.Match(input, startAt);
            Assert.AreEqual(expectedMatch.Success, actualMatch.Success);
            if (actualMatch.Success)
            {
                string expectedMatchedNames = string.Join("\n", expectedMatch.Groups.Where(g => g.Success).OrderBy(g => g.Name).Select(g => g.Name).Distinct());
                string actualMatchedNames = string.Join("\n", expectedMatch.Groups.Where(g => actualMatch.Groups[g.Name].Success).OrderBy(g => g.Name).Select(g => g.Name).Distinct());
                Assert.AreEqual(expectedMatchedNames, actualMatchedNames);
                Assert.AreEqual(expectedMatch.Value, actualMatch.Value);
                Assert.AreEqual(expectedMatch.Index, actualMatch.Index);
                Assert.AreEqual(expectedMatch.Length, actualMatch.Length);
                foreach (ExpectedMatchGroupResult expectedGroup in expectedMatch.Groups)
                {
                    Group actualGroup = actualMatch.Groups[expectedGroup.Name];
                    Assert.AreEqual(expectedGroup.Success, actualGroup.Success, $"Group = {expectedGroup.Name}");
                    if (actualGroup.Success)
                    {
                        Assert.AreEqual(expectedGroup.Value, actualGroup.Value, $"Group = {expectedGroup.Name}");
                        Assert.AreEqual(expectedGroup.Index, actualGroup.Index, $"Group = {expectedGroup.Name}");
                        Assert.AreEqual(expectedGroup.Length, actualGroup.Length, $"Group = {expectedGroup.Name}");
                    }
                }
            }
        }

        public static IEnumerable<object[]> GetQuantifierRegexTestData()
        {
            yield return ExpectedMatchResult.CreateTestData(@"{1}", 0, "{", builder => builder.Grouped("min", "1").Ungrouped("}").FailedGroup("opt", "mlt", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"{1,}", 0, "{", builder => builder.Grouped("min", "1").Grouped("qr", ",").Ungrouped("}").FailedGroup("opt", "mlt", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"{1,3}", 0, "{", builder => builder.Grouped("min", "1").Nested("qr", ",", r => r.Grouped("max", "3")).Ungrouped("}").FailedGroup("opt", "mlt", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"?", 0, builder => builder.Grouped("opt", "?").FailedGroup("mlt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"+", 0, builder => builder.Grouped("mlt", "+").FailedGroup("opt", "min", "qr", "max", "lzy"));
            yield return ExpectedMatchResult.CreateTestData(@"{1}?", 0, "{", builder => builder.Grouped("min", "1").Ungrouped("}").Grouped("lzy", "?").FailedGroup("opt", "mlt", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"{1,}?", 0, "{", builder => builder.Grouped("min", "1").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?").FailedGroup("opt", "mlt", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"{1,3}?", 0, "{", builder => builder.Grouped("min", "1").Nested("qr", ",", r => r.Grouped("max", "3")).Ungrouped("}").Grouped("lzy", "?").FailedGroup("opt", "mlt"));
            yield return ExpectedMatchResult.CreateTestData(@"??", 0, builder => builder.Grouped("opt", "?").Grouped("lzy", "?").FailedGroup("mlt", "min", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"+?", 0, builder => builder.Grouped("mlt", "+").Grouped("lzy", "?").FailedGroup("opt", "min", "qr", "max"));
            yield return ExpectedMatchResult.CreateTestData(@"{}", 0, ExpectedMatchResult.Failed);
            yield return ExpectedMatchResult.CreateTestData(@"{1", 0, ExpectedMatchResult.Failed);
            yield return ExpectedMatchResult.CreateTestData(@"{1,", 0, ExpectedMatchResult.Failed);
            yield return ExpectedMatchResult.CreateTestData(@"{1,3", 0, ExpectedMatchResult.Failed);
            yield return ExpectedMatchResult.CreateTestData(@"{,3}", 0, ExpectedMatchResult.Failed);
            yield return ExpectedMatchResult.CreateTestData(@"{,}", 0, ExpectedMatchResult.Failed);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetQuantifierRegexTestData), DynamicDataSourceType.Method)]
        public void QuantifierRegexTestMethod(string input, int startAt, ExpectedMatchResult expectedMatch)
        {
            Match actualMatch = RegexParser.QuantifierRegex.Match(input, startAt);
            Assert.AreEqual(expectedMatch.Success, actualMatch.Success);
            if (actualMatch.Success)
            {
                string expectedMatchedNames = string.Join("\n", expectedMatch.Groups.Where(g => g.Success).OrderBy(g => g.Name).Select(g => g.Name).Distinct());
                string actualMatchedNames = string.Join("\n", expectedMatch.Groups.Where(g => actualMatch.Groups[g.Name].Success).OrderBy(g => g.Name).Select(g => g.Name).Distinct());
                Assert.AreEqual(expectedMatchedNames, actualMatchedNames);
                Assert.AreEqual(expectedMatch.Value, actualMatch.Value);
                Assert.AreEqual(expectedMatch.Index, actualMatch.Index);
                Assert.AreEqual(expectedMatch.Length, actualMatch.Length);
                foreach (ExpectedMatchGroupResult expectedGroup in expectedMatch.Groups)
                {
                    Group actualGroup = actualMatch.Groups[expectedGroup.Name];
                    Assert.AreEqual(expectedGroup.Success, actualGroup.Success, $"Group = {expectedGroup.Name}");
                    if (actualGroup.Success)
                    {
                        Assert.AreEqual(expectedGroup.Value, actualGroup.Value, $"Group = {expectedGroup.Name}");
                        Assert.AreEqual(expectedGroup.Index, actualGroup.Index, $"Group = {expectedGroup.Name}");
                        Assert.AreEqual(expectedGroup.Length, actualGroup.Length, $"Group = {expectedGroup.Name}");
                    }
                }
            }
        }

        public static IEnumerable<object[]> GetGroupTypeRegexTestData()
        {
            yield return ExpectedMatchResult.CreateTestData(@"?", 0, "?", builder => builder.FailedGroup("x", "ap", "an", "bp", "bn", "d", "ng", "m", "at", "c", "cr", "cn", "ptn"));
            yield return ExpectedMatchResult.CreateTestData(@"?:", 0, "?", builder => builder.Grouped("x", ":").FailedGroup("ap", "an", "bp", "bn", "d", "ng", "m", "at", "c", "cr", "cn", "ptn"));
            yield return ExpectedMatchResult.CreateTestData(@"?=", 0, "?", builder => builder.Grouped("ap", "=").FailedGroup("x", "an", "bp", "bn", "d", "ng", "m", "at", "c", "cr", "cn", "ptn"));
            yield return ExpectedMatchResult.CreateTestData(@"?!", 0, "?", builder => builder.Grouped("an", "!").FailedGroup("x", "ap", "bp", "bn", "d", "ng", "m", "at", "c", "cr", "cn", "ptn"));
            yield return ExpectedMatchResult.CreateTestData(@"?<=", 0, "?<", builder => builder.Grouped("bp", "=").FailedGroup("x", "ap", "an", "bn", "d", "ng", "m", "at", "c", "cr", "cn", "ptn"));
            yield return ExpectedMatchResult.CreateTestData(@"?<!", 0, "?<", builder => builder.Grouped("bn", "!").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn", "ptn"));
            yield return ExpectedMatchResult.CreateTestData(@"?<name>", 0, "?", builder => builder.Grouped("d", "<").Grouped("ng", "name").Ungrouped(">").FailedGroup("x", "ap", "an", "bp", "bn", "m", "at", "c", "cr", "cn", "ptn"));
            yield return ExpectedMatchResult.CreateTestData(@"?'name'", 0, "?", builder => builder.Grouped("d", "'").Grouped("ng", "name").Ungrouped("'").FailedGroup("x", "ap", "an", "bp", "bn", "m", "at", "c", "cr", "cn", "ptn"));
            yield return ExpectedMatchResult.CreateTestData(@"?ixsmn:", 0, "?", builder => builder.Grouped("m", "ixsmn").Grouped("x", ":").FailedGroup("ap", "an", "bp", "bn", "d", "ng", "at", "c", "cr", "cn", "ptn"));
            yield return ExpectedMatchResult.CreateTestData(@"?>", 0, "?", builder => builder.Grouped("at", ">").FailedGroup("x", "ap", "an", "bp", "bn", "d", "ng", "m", "c", "cr", "cn", "ptn"));
            yield return ExpectedMatchResult.CreateTestData(@"?(name)", 0, "?", builder => builder.Nested("c", "(", b => b.Grouped("cn", "name").Ungrouped(")")).FailedGroup("x", "ap", "an", "bp", "bn", "d", "ng", "m", "at", "cr", "ptn"));
            yield return ExpectedMatchResult.CreateTestData(@"?(12)", 0, "?", builder => builder.Nested("c", "(", b => b.Grouped("cr", "12").Ungrouped(")")).FailedGroup("x", "ap", "an", "bp", "bn", "d", "ng", "m", "at", "cn", "ptn"));

            yield return ExpectedMatchResult.CreateTestData(@"?name>", 0, "?", builder => builder.Grouped("ptn", "name>").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?<name", 0, "?", builder => builder.Grouped("ptn", "<name").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?name'", 0, "?", builder => builder.Grouped("ptn", "name'").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?'name", 0, "?", builder => builder.Grouped("ptn", "'name").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?name)", 0, "?", builder => builder.Grouped("ptn", "name)").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?(name", 0, "?", builder => builder.Grouped("ptn", "(name").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?12)", 0, "?", builder => builder.Grouped("ptn", "12)").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?(12", 0, "?", builder => builder.Grouped("ptn", "(12").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?ixsmni:", 0, "?", builder => builder.Grouped("ptn", "ixsmni:").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?ixsmnx:", 0, "?", builder => builder.Grouped("ptn", "ixsmnx:").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?ixsmns:", 0, "?", builder => builder.Grouped("ptn", "ixsmns:").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?ixsmnm:", 0, "?", builder => builder.Grouped("ptn", "ixsmnm:").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?ixsmnn:", 0, "?", builder => builder.Grouped("ptn", "ixsmnn:").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?aixsmn:", 0, "?", builder => builder.Grouped("ptn", "aixsmn:").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));

            yield return ExpectedMatchResult.CreateTestData(@"?Test", 0, "?", builder => builder.Grouped("ptn", "Test").FailedGroup("x", "ap", "an", "bp", "bn", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?:Test", 0, "?", builder => builder.Grouped("x", ":").Grouped("ptn", "Test").FailedGroup("ap", "an", "bp", "bn", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?=Test", 0, "?", builder => builder.Grouped("ap", "=").Grouped("ptn", "Test").FailedGroup("x", "an", "bp", "bn", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?!Test", 0, "?", builder => builder.Grouped("an", "!").Grouped("ptn", "Test").FailedGroup("x", "ap", "bp", "bn", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?<=Test", 0, "?<", builder => builder.Grouped("bp", "=").Grouped("ptn", "Test").FailedGroup("x", "ap", "an", "bn", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?<!Test", 0, "?<", builder => builder.Grouped("bn", "!").Grouped("ptn", "Test").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?<name>Test", 0, "?", builder => builder.Grouped("d", "<").Grouped("ng", "name").Ungrouped(">").Grouped("ptn", "Test")
                .FailedGroup("x", "ap", "an", "bp", "bn", "m", "at", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?'name'Test", 0, "?", builder => builder.Grouped("d", "'").Grouped("ng", "name").Ungrouped("'").Grouped("ptn", "Test")
                .FailedGroup("x", "ap", "an", "bp", "bn", "m", "at", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?ixsmn:Test", 0, "?", builder => builder.Grouped("m", "ixsmn").Grouped("x", ":").Grouped("ptn", "Test").FailedGroup("ap", "an", "bp", "bn", "d", "ng", "at", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?>Test", 0, "?", builder => builder.Grouped("at", ">").Grouped("ptn", "Test").FailedGroup("x", "ap", "an", "bp", "bn", "d", "ng", "m", "c", "cr", "cn"));
            yield return ExpectedMatchResult.CreateTestData(@"?(name)Test", 0, "?", builder => builder.Nested("c", "(", b => b.Grouped("cn", "name").Ungrouped(")")).Grouped("ptn", "Test")
                .FailedGroup("x", "ap", "an", "bp", "bn", "d", "ng", "m", "at", "cr"));
            yield return ExpectedMatchResult.CreateTestData(@"?(12)Test", 0, "?", builder => builder.Nested("c", "(", b => b.Grouped("cr", "12").Ungrouped(")")).Grouped("ptn", "Test")
                .FailedGroup("x", "ap", "an", "bp", "bn", "d", "ng", "m", "at", "cn"));
        }

        [DataTestMethod]
        [DynamicData(nameof(GetGroupTypeRegexTestData), DynamicDataSourceType.Method)]
        public void GroupTypeRegexTestMethod(string input, int startAt, ExpectedMatchResult expectedMatch)
        {
            Match actualMatch = RegexParser.GroupTypeRegex.Match(input, startAt);
            Assert.AreEqual(expectedMatch.Success, actualMatch.Success);
            if (actualMatch.Success)
            {
                string expectedMatchedNames = string.Join("\n", expectedMatch.Groups.Where(g => g.Success).OrderBy(g => g.Name).Select(g => g.Name).Distinct());
                string actualMatchedNames = string.Join("\n", expectedMatch.Groups.Where(g => actualMatch.Groups[g.Name].Success).OrderBy(g => g.Name).Select(g => g.Name).Distinct());
                Assert.AreEqual(expectedMatchedNames, actualMatchedNames);
                Assert.AreEqual(expectedMatch.Value, actualMatch.Value);
                Assert.AreEqual(expectedMatch.Index, actualMatch.Index);
                Assert.AreEqual(expectedMatch.Length, actualMatch.Length);
                foreach (ExpectedMatchGroupResult expectedGroup in expectedMatch.Groups)
                {
                    Group actualGroup = actualMatch.Groups[expectedGroup.Name];
                    Assert.AreEqual(expectedGroup.Success, actualGroup.Success, $"Group = {expectedGroup.Name}");
                    if (actualGroup.Success)
                    {
                        Assert.AreEqual(expectedGroup.Value, actualGroup.Value, $"Group = {expectedGroup.Name}");
                        Assert.AreEqual(expectedGroup.Index, actualGroup.Index, $"Group = {expectedGroup.Name}");
                        Assert.AreEqual(expectedGroup.Length, actualGroup.Length, $"Group = {expectedGroup.Name}");
                    }
                }
            }
        }

    }
}

using System.Collections.ObjectModel;
using System.Text;
using System.Text.RegularExpressions;
using LteDev.RegexParsing;


namespace LteDev.UnitTests;

public class RegexParserUnitTest
{
    public class CaptureResult
    {
        public int Index { get; }

        public int Length { get; }
        
        public bool Success { get; }
        
        public string Value { get; }
        
        protected CaptureResult()
        {
            Index = Length = 0;
            Success = false;
            Value = "";
        }

        protected CaptureResult(int index, string value)
        {
            Index = index;
            Length = (Value = value).Length;
            Success = true;
        }
    }

    public class MatchGroupResult : CaptureResult, IEquatable<MatchGroupResult>
    {
        public string Name { get; }

        public MatchGroupResult(string name) : base() { Name = name; }

        public MatchGroupResult(string name, int index, string value) : base(index, value) { Name = name; }

        public bool Equals(MatchGroupResult? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Success)
                return other.Success && Index == other.Index && Length == other.Length && Name == other.Name && Value != other.Value;
            return !other.Success;
        }

        public override bool Equals(object? obj) => Equals(obj as MatchGroupResult);

        public override int GetHashCode() => Success ? HashCode.Combine(Success, Name, Index, Length, Value) : 0;

        public override string ToString()
        {
            if (!Success) return "{ Success = False }";
            return $"{{ Success = True, Name = \"{Name}\", Index = {Index}, Length = {Length}, Value = \"{Value.Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\"", "\\\"")}\" }}";
        }
    }

    public class MatchResult : CaptureResult, IEquatable<MatchResult>
    {
        public ReadOnlyCollection<MatchGroupResult> Groups { get; }

        private MatchResult() : base() { Groups = new ReadOnlyCollection<MatchGroupResult>(new Collection<MatchGroupResult>()); }

        internal MatchResult(int index, string value, IList<MatchGroupResult> groups) : base(index, value)
        {
            Groups = new ReadOnlyCollection<MatchGroupResult>((groups == null) ? new Collection<MatchGroupResult>() : groups);
        }

        internal static readonly MatchResult Failed = new();

        internal static TestCaseData CreateTestData(string input, int startAt, MatchResult expected) => new TestCaseData(input, startAt).Returns(expected);

        internal static TestCaseData CreateTestData(string input, int startAt, Action<ExpectedRegexMatchBuilder> builderCallback)
        {
            ExpectedRegexMatchBuilder builder = new(startAt);
            builderCallback(builder);
            return new TestCaseData(input, startAt).Returns(builder.Build());
        }

        internal static TestCaseData CreateTestData(string input, int startAt, string initialUngroupedMatch, Action<ExpectedRegexMatchBuilder> builderCallback)
        {
            ExpectedRegexMatchBuilder builder = new(startAt, initialUngroupedMatch);
            builderCallback(builder);
            return new TestCaseData(input, startAt).Returns(builder.Build());
        }

        internal static MatchResult FromResult(Match actualMatch)
        {
            if (!actualMatch.Success) return Failed;
            
            return new MatchResult(actualMatch.Index, actualMatch.Value, actualMatch.Groups.Values.Where(g => g.Name != g.Index.ToString()).Select(g =>
            {
                if (!g.Success) return new MatchGroupResult(g.Name);
                return new MatchGroupResult(g.Name, g.Index, g.Value);
            }).ToArray());
        }

        public bool Equals(MatchResult? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Success)
            {
                if (!other.Success || Index != other.Index || Length != other.Length || Value != other.Value) return false;
                int c = Groups.Count;
                if (c != other.Groups.Count) return false;
                for (int i = 0; i < c; i++)
                {
                    if (!Groups[i].Equals(other.Groups[i])) return false;
                }
                return true;
            }
            return !other.Success;
        }

        public override bool Equals(object? obj) => Equals(obj as MatchGroupResult);

        public override int GetHashCode()
        {
            if (!Success) return 0;
            HashCode hc = new();
            foreach (var g in Groups)
                hc.Add(g.GetHashCode());
            return HashCode.Combine(Index, Length, Success, Value, Groups.Count, hc.ToHashCode());
        }

        public override string ToString()
        {
            if (!Success) return "{ Success = False }";
            StringBuilder sb = new StringBuilder("{\n  Success = True ").Append(", Index = ").Append(Index).Append(", Length = ").Append(Length).Append(Index).Append(", Value = ").Append(Value);
            using IEnumerator<MatchGroupResult> enumerator = Groups.GetEnumerator();
            if (!enumerator.MoveNext()) return sb.Append(",\n  Groups = []\n}").ToString();
            sb.Append(",\n    Groups = [\n    ").Append(enumerator.Current.ToString());
            while (enumerator.MoveNext())
                sb.Append(",\n    ").Append(enumerator.Current.ToString());
            return sb.Append("\n  ]\n}").ToString();
        }
    }

    public class ExpectedRegexMatchBuilder
    {
        private readonly int _startIndex;
        private int _nextIndex;
        private readonly StringBuilder _value = new();
        private readonly Collection<MatchGroupResult> _groups;

        private ExpectedRegexMatchBuilder(ExpectedRegexMatchBuilder outer, string? initialUngroupedMatch)
        {
            _groups = outer._groups;
            _startIndex = outer._nextIndex;
            if (string.IsNullOrEmpty(initialUngroupedMatch))
                _nextIndex = _startIndex;
            else
                _nextIndex = _startIndex + _value.Append(initialUngroupedMatch).Length;
        }

        public ExpectedRegexMatchBuilder(int index = 0, string? initialUngroupedMatch = null)
        {
            _groups = new Collection<MatchGroupResult>();
            _startIndex = index;
            if (string.IsNullOrEmpty(initialUngroupedMatch))
                _nextIndex = index;
            else
                _nextIndex = index + _value.Append(initialUngroupedMatch).Length;
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
            MatchGroupResult group = new(name, _nextIndex, match);
            _nextIndex += group.Length;
            _value.Append(group.Value);
            _groups.Add(group);
            return this;
        }

        public ExpectedRegexMatchBuilder FailedGroup(params string[] name)
        {
            foreach (string n in name)
                _groups.Add(new MatchGroupResult(n));
            return this;
        }

        public ExpectedRegexMatchBuilder Nested(string outerGroupName, Action<ExpectedRegexMatchBuilder> innerBuilderCallback)
        {
            ExpectedRegexMatchBuilder builder = new(this, null);
            innerBuilderCallback(builder);
            MatchGroupResult group = builder.Build(outerGroupName);
            _nextIndex += group.Length;
            _value.Append(group.Value);
            _groups.Add(group);
            return this;
        }

        public ExpectedRegexMatchBuilder Nested(string outerGroupName, string initialGroupedMatch, Action<ExpectedRegexMatchBuilder> innerBuilderCallback)
        {
            ExpectedRegexMatchBuilder builder = new(this, initialGroupedMatch);
            innerBuilderCallback(builder);
            MatchGroupResult group = builder.Build(outerGroupName);
            _nextIndex += group.Length;
            _value.Append(group.Value);
            _groups.Add(group);
            return this;
        }

        public MatchResult Build() => new(_startIndex, _value.ToString(), _groups);

        private MatchGroupResult Build(string groupName) => new(groupName, _startIndex, _value.ToString());
    }

    public static class TestData
    {
        public static System.Collections.IEnumerable GetCharacterClassFirstTokenRegexTestData()
        {
            yield return MatchResult.CreateTestData(@"^", 0, builder => builder.Grouped("neg", @"^").FailedGroup("lit", "em", "esc", "r"));
            yield return MatchResult.CreateTestData(@"-", 0, builder => builder.Grouped("lit", @"-").FailedGroup("neg", "em", "esc", "r"));
            yield return MatchResult.CreateTestData(@"\^", 0, builder => builder.Grouped("em", @"\^").FailedGroup("neg", "lit", "esc", "r"));
            yield return MatchResult.CreateTestData(@"\-", 0, builder => builder.Grouped("esc", @"\-").FailedGroup("neg", "lit", "em", "r"));
            yield return MatchResult.CreateTestData(@"\-\x\u\p\\", 0, builder => builder.Grouped("esc", @"\-\x\u\p").FailedGroup("neg", "lit", "em", "r"));
            yield return MatchResult.CreateTestData(@"--", 0, builder => builder.Grouped("lit", @"--").FailedGroup("neg", "em", "esc", "r"));
            yield return MatchResult.CreateTestData(@"--?", 0, builder => builder.Grouped("lit", @"-").Grouped("r", @"-").FailedGroup("neg", "em", "esc"));
            yield return MatchResult.CreateTestData(@"-[^$.|?*+(){}\", 0, builder => builder.Grouped("lit", @"-[^$.|?*+(){}").FailedGroup("neg", "em", "esc", "r"));
            yield return MatchResult.CreateTestData(@"-[^$.|?*+(){}\", 0, builder => builder.Grouped("lit", @"-[^$.|?*+(){}").FailedGroup("neg", "em", "esc", "r"));
            yield return MatchResult.CreateTestData(@"-[^$.|?*+(){}-", 0, builder => builder.Grouped("lit", @"-[^$.|?*+(){}-").FailedGroup("neg", "em", "esc", "r"));
            yield return MatchResult.CreateTestData(@"-[^$.|?*+(){}-\", 0, builder => builder.Grouped("lit", @"-[^$.|?*+(){}").Grouped("r", @"-").FailedGroup("neg", "em", "esc"));
            yield return MatchResult.CreateTestData(@"\^\\\-\b", 0, builder => builder.Grouped("em", @"\^\\\-").FailedGroup("neg", "lit", "esc", "r"));
            yield return MatchResult.CreateTestData(@"^-", 0, builder => builder.Grouped("neg", @"^").Grouped("lit", @"-").FailedGroup("em", "esc", "r"));
            yield return MatchResult.CreateTestData(@"^\-", 0, builder => builder.Grouped("neg", @"^").Grouped("esc", @"\-").FailedGroup("lit", "em", "r"));

            yield return MatchResult.CreateTestData(@"^\-\x\u\p\\", 0, builder => builder.Grouped("neg", @"^").Grouped("esc", @"\-\x\u\p").FailedGroup("lit", "em", "r"));
            yield return MatchResult.CreateTestData(@"^--", 0, builder => builder.Grouped("neg", @"^").Grouped("lit", @"--").FailedGroup("em", "esc", "r"));
            yield return MatchResult.CreateTestData(@"^--?", 0, builder => builder.Grouped("neg", @"^").Grouped("lit", @"-").Grouped("r", @"-").FailedGroup("em", "esc"));
            yield return MatchResult.CreateTestData(@"\^-", 0, builder => builder.Grouped("em", @"\^").FailedGroup("neg", "lit", "esc", "r"));
            yield return MatchResult.CreateTestData(@"\^-?", 0, builder => builder.Grouped("em", @"\^").Grouped("r", @"-").FailedGroup("neg", "lit", "esc"));
            yield return MatchResult.CreateTestData(@"^-[^$.|?*+(){}\", 0, builder => builder.Grouped("neg", @"^").Grouped("lit", @"-[^$.|?*+(){}").FailedGroup("em", "esc", "r"));
            yield return MatchResult.CreateTestData(@"^-[^$.|?*+(){}-", 0, builder => builder.Grouped("neg", @"^").Grouped("lit", @"-[^$.|?*+(){}-").FailedGroup("em", "esc", "r"));
            yield return MatchResult.CreateTestData(@"^-[^$.|?*+(){}-\", 0, builder => builder.Grouped("neg", @"^").Grouped("lit", @"-[^$.|?*+(){}").Grouped("r", @"-").FailedGroup("em", "esc"));
            yield return MatchResult.CreateTestData(@"\a", 0, MatchResult.Failed);
            yield return MatchResult.CreateTestData(@"]", 0, MatchResult.Failed);
            yield return MatchResult.CreateTestData(@" ", 0, MatchResult.Failed);
            yield return MatchResult.CreateTestData(@" ^", 0, MatchResult.Failed);
            yield return MatchResult.CreateTestData(@" -", 0, MatchResult.Failed);
        }

        public static System.Collections.IEnumerable GetSingleLinePatternTokenRegexTestData()
        {
            yield return MatchResult.CreateTestData(@"^", 0, builder => builder.Grouped("anc", @"^")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"$", 0, builder => builder.Grouped("anc", @"$")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\G", 0, builder => builder.Grouped("anc", @"\G")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\A", 0, builder => builder.Grouped("anc", @"\A")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\Z", 0, builder => builder.Grouped("anc", @"\Z")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\z", 0, builder => builder.Grouped("anc", @"\z")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return MatchResult.CreateTestData(@"|", 0, builder => builder.Grouped("alt", @"|")
                .FailedGroup("anc", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return MatchResult.CreateTestData(@".", 0, builder => builder.Grouped("any", @".")
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return MatchResult.CreateTestData(@"\r\n\t\a\e\f\v", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return MatchResult.CreateTestData(@"\[\\\^\$\.\|\?\*\+\(\)\{\}", 0, builder => builder.Grouped("em", @"\[\\\^\$\.\|\?\*\+\(\)\{\}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy"));

            yield return MatchResult.CreateTestData(@"\100", 0, builder => builder.Grouped("oct", @"\100")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\177", 0, builder => builder.Grouped("oct", @"\177")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\200", 0, builder => builder.Grouped("oct", @"\200")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\377", 0, builder => builder.Grouped("oct", @"\377")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\400", 0, builder => builder.Grouped("dbr", @"\40")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\700", 0, builder => builder.Grouped("dbr", @"\70")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\01", 0, builder => builder.Grouped("oct", @"\01")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\07", 0, builder => builder.Grouped("oct", @"\07")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\010", 0, builder => builder.Grouped("oct", @"\010")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\077", 0, builder => builder.Grouped("oct", @"\077")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077\1", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077-", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return MatchResult.CreateTestData(@"\0", 0, builder => builder.Grouped("nul", @"\0")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\00", 0, builder => builder.Grouped("nul", @"\0")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\0\0\0\08", 0, builder => builder.Grouped("nul", @"\0\0\0\0")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\0\0\0\07", 0, builder => builder.Grouped("nul", @"\0\0\0")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return MatchResult.CreateTestData(@"\cA", 0, builder => builder.Grouped("ctl", @"\cA")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\cZ", 0, builder => builder.Grouped("ctl", @"\cZ")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return MatchResult.CreateTestData(@"\c", 0, builder => builder.Grouped("esc", @"\c")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\ca", 0, builder => builder.Grouped("esc", @"\c")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\cz", 0, builder => builder.Grouped("esc", @"\c")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL\cM\cN\cO\cP\cQ\cR\cS\cT\cU\cV\cW\cX\cY", 0,
                builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL\cM\cN\cO\cP\cQ\cR\cS\cT\cU\cV\cW\cX\cY")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL\cm\cN", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL-", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return MatchResult.CreateTestData(@"\c\c\cZ", 0, builder => builder.Grouped("esc", @"\c\c")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return MatchResult.CreateTestData(@"\xFF", 0, builder => builder.Grouped("hex", @"\xFF")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\xAA", 0, builder => builder.Grouped("hex", @"\xAA")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\xff", 0, builder => builder.Grouped("hex", @"\xff")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\xaa", 0, builder => builder.Grouped("hex", @"\xaa")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\x00", 0, builder => builder.Grouped("hex", @"\x00")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\x99", 0, builder => builder.Grouped("hex", @"\x99")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xffff", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff-", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\xFFFF", 0, builder => builder.Grouped("hex", @"\xFF")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\xF", 0, builder => builder.Grouped("esc", @"\x")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\xFG", 0, builder => builder.Grouped("esc", @"\x")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\x0", 0, builder => builder.Grouped("esc", @"\x")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\x\x\x99", 0, builder => builder.Grouped("esc", @"\x\x")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return MatchResult.CreateTestData(@"\uFFFF", 0, builder => builder.Grouped("uni", @"\uFFFF")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\uAAAA", 0, builder => builder.Grouped("uni", @"\uAAAA")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\uffff", 0, builder => builder.Grouped("uni", @"\uffff")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\uaaaa", 0, builder => builder.Grouped("uni", @"\uaaaa")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\u9999", 0, builder => builder.Grouped("uni", @"\u9999")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\u0000", 0, builder => builder.Grouped("uni", @"\u0000")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000-", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\uFFF", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\uFF", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\uF", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\u999", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\uaa", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\u0", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\u\u\u0000", 0, builder => builder.Grouped("esc", @"\u\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return MatchResult.CreateTestData(@"\p{C}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"C").Ungrouped(@"}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\p{C}\p{C}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"C").Ungrouped(@"}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\p{Cc}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\p{Cc}-", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return MatchResult.CreateTestData(@"\p\p\p{C}", 0, builder => builder.Grouped("esc", @"\p\p")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\p{Cc", 0, builder => builder.Grouped("esc", @"\p")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\p{Cc }", 0, builder => builder.Grouped("esc", @"\p")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return MatchResult.CreateTestData(@" ", 0, builder => builder.Grouped("lit", @" ")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"abc123!@#", 0, builder => builder.Grouped("lit", @"abc123!@#")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"{", 0, builder => builder.Grouped("lit", @"{")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"}", 0, builder => builder.Grouped("lit", @"}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"{1}", 0, builder => builder.Grouped("lit", @"{1}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"{1,}", 0, builder => builder.Grouped("lit", @"{1,}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"{1,2}", 0, builder => builder.Grouped("lit", @"{1,2}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"abc123!@#{}", 0, builder => builder.Grouped("lit", @"abc123!@#{}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return MatchResult.CreateTestData(@"abc123!@#{,}", 0, builder => builder.Grouped("lit", @"abc123!@#{,}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"abc123!@#{,3}", 0, builder => builder.Grouped("lit", @"abc123!@#{,3}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            yield return MatchResult.CreateTestData("()", 0, builder => builder.Nested("grp", "(", b => b.Grouped("ptn", "").Ungrouped(")"))
                .FailedGroup("anc", "alt", "any", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"(?(?=test)one|two)", 0, builder => builder.Nested("grp", "(", b => b.Grouped("ptn", @"?(?=test)one|two").Ungrouped(")"))
                .FailedGroup("anc", "alt", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"(?(?=#test)one|two)", 0, builder => builder.Nested("grp", "(", b => b.Grouped("ptn", @"?(?=#test)one|two").Ungrouped(")"))
                .FailedGroup("anc", "alt", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"(?\(?=test)one|two)", 0, builder => builder.Nested("grp", "(", b => b.Grouped("ptn", @"?\(?=test").Ungrouped(")"))
                .FailedGroup("anc", "alt", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));

            //.FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@".{33}", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@".{9,}", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy"));
            yield return MatchResult.CreateTestData(@".{10,11}", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy"));
            yield return MatchResult.CreateTestData(@".?", 0, builder => builder.Grouped("any", @".").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@".+", 0, builder => builder.Grouped("any", @".").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\r\n\t\a\e\f\v{33}", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\r\n\t\a\e\f\v{9,}", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\r\n\t\a\e\f\v{10,11}", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy"));
            yield return MatchResult.CreateTestData(@"\r\n\t\a\e\f\v?", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\r\n\t\a\e\f\v+", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077{33}", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077{9,}", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077{10,11}", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{", b =>
                b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077?", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077+", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\0\0\0\0{33}", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\0\0\0\0{9,}", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\0\0\0\0{10,11}", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy"));
            yield return MatchResult.CreateTestData(@"\0\0\0\0?", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\0\0\0\0+", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{33}", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{9,}", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{",
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{10,11}", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL?", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL+", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{33}", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{9,}", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{10,11}", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",",
                r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff?", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff+", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{33}", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{9,}", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{",
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{10,11}", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF?", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF+", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\p{Cc}{33}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\p{Cc}{9,}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\p{Cc}{10,11}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",",
                r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy"));
            yield return MatchResult.CreateTestData(@"\p{Cc}?", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"\p{Cc}+", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"abc123!@#{1}", 0, builder => builder.Grouped("lit", @"abc123!@#").Nested("q", "{", b => b.Grouped("min", "1").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"abc123!@#{1,}", 0, builder => builder.Grouped("lit", @"abc123!@#").Nested("q", "{", b => b.Grouped("min", "1").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"abc123!@#{1,3}", 0, builder => builder.Grouped("lit", @"abc123!@#").Nested("q", "{", b => b.Grouped("min", "1").Nested("qr", ",", r => r.Grouped("max", "3")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy"));
            yield return MatchResult.CreateTestData(@"abc123!@#?", 0, builder => builder.Grouped("lit", @"abc123!@#").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"abc123!@#+", 0, builder => builder.Grouped("lit", @"abc123!@#").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy"));

            yield return MatchResult.CreateTestData(@"#{1}", 0, builder => builder.Grouped("lit", @"#").Nested("q", "{", b => b.Grouped("min", "1").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"#{1,}\nTest", 0, builder => builder.Grouped("lit", @"#").Nested("q", "{", b => b.Grouped("min", "1").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy"));
            yield return MatchResult.CreateTestData("#{1,3}Test", 0, builder => builder.Grouped("lit", "#").Nested("q", "{", b => b.Grouped("min", "1").Nested("qr", ",", r => r.Grouped("max", "3")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy"));

            yield return MatchResult.CreateTestData(@"[]{1}", 0, builder => builder.Grouped("cc", @"[]").Nested("q", "{", b => b.Grouped("min", "1").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"[one|\]two]{1,}", 0, builder => builder.Nested("cc", "[", b => b.Grouped("ptn", @"one|\]two").Ungrouped("]")).Nested("q", "{", b => b.Grouped("min", "1").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"[]{1,3}", 0, builder => builder.Grouped("cc", @"[]").Nested("q", "{", b => b.Grouped("min", "1").Nested("qr", ",", r => r.Grouped("max", "3")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt", "lzy"));
            yield return MatchResult.CreateTestData(@"[one|\]two]?", 0, builder => builder.Nested("cc", "[", b => b.Grouped("ptn", @"one|\]two").Ungrouped("]")).Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"[]+", 0, builder => builder.Grouped("cc", @"[]").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "min", "qr", "max", "lzy"));

            //.FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max"));
            yield return MatchResult.CreateTestData(@".{33}?", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max"));
            yield return MatchResult.CreateTestData(@".{9,}?", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max"));
            yield return MatchResult.CreateTestData(@".{10,11}?", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt"));
            yield return MatchResult.CreateTestData(@".??", 0, builder => builder.Grouped("any", @".").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max"));
            yield return MatchResult.CreateTestData(@".+?", 0, builder => builder.Grouped("any", @".").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max"));
            yield return MatchResult.CreateTestData(@"\r\n\t\a\e\f\v{33}?", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max"));
            yield return MatchResult.CreateTestData(@"\r\n\t\a\e\f\v{9,}?", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max"));
            yield return MatchResult.CreateTestData(@"\r\n\t\a\e\f\v{10,11}?", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",",
                r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt"));
            yield return MatchResult.CreateTestData(@"\r\n\t\a\e\f\v??", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max"));
            yield return MatchResult.CreateTestData(@"\r\n\t\a\e\f\v+?", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077{33}?", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077{9,}?", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{",
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077{10,11}?", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",",
                r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077??", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077+?", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max"));
            yield return MatchResult.CreateTestData(@"\0\0\0\0{33}?", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max"));
            yield return MatchResult.CreateTestData(@"\0\0\0\0{9,}?", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max"));
            yield return MatchResult.CreateTestData(@"\0\0\0\0{10,11}?", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r =>
                r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt"));
            yield return MatchResult.CreateTestData(@"\0\0\0\0??", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max"));
            yield return MatchResult.CreateTestData(@"\0\0\0\0+?", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{33}?", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{",
                b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{9,}?", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{",
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{10,11}?", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL??", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL+?", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{33}?", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{9,}?", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{",
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{10,11}?", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",",
                r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff??", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff+?", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{33}?", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{",
                b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{9,}?", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{",
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{10,11}?", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF??", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF+?", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max"));
            yield return MatchResult.CreateTestData(@"\p{Cc}{33}?", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max"));
            yield return MatchResult.CreateTestData(@"\p{Cc}{9,}?", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{",
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max"));
            yield return MatchResult.CreateTestData(@"\p{Cc}{10,11}?", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",",
                r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt"));
            yield return MatchResult.CreateTestData(@"\p{Cc}??", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max"));
            yield return MatchResult.CreateTestData(@"\p{Cc}+?", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max"));
            yield return MatchResult.CreateTestData(@"abc123!@#{1}?", 0, builder => builder.Grouped("lit", @"abc123!@#").Nested("q", "{", b => b.Grouped("min", "1").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max"));
            yield return MatchResult.CreateTestData(@"abc123!@#{1,}?", 0, builder => builder.Grouped("lit", @"abc123!@#").Nested("q", "{", b => b.Grouped("min", "1").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max"));
            yield return MatchResult.CreateTestData(@"abc123!@#{1,3}?", 0, builder => builder.Grouped("lit", @"abc123!@#").Nested("q", "{", b => b.Grouped("min", "1").Nested("qr", ",",
                r => r.Grouped("max", "3")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt"));
            yield return MatchResult.CreateTestData(@"abc123!@#??", 0, builder => builder.Grouped("lit", @"abc123!@#").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max"));
            yield return MatchResult.CreateTestData(@"abc123!@#+?", 0, builder => builder.Grouped("lit", @"abc123!@#").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max"));

            yield return MatchResult.CreateTestData(@"[]{1}?", 0, builder => builder.Grouped("cc", @"[]").Nested("q", "{", b => b.Grouped("min", "1").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt", "qr", "max"));
            yield return MatchResult.CreateTestData(@"[one|\]two]{1,}?", 0,
                builder => builder.Nested("cc", "[", b => b.Grouped("ptn", @"one|\]two").Ungrouped("]")).Nested("q", "{", b => b.Grouped("min", "1").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt", "max"));
            yield return MatchResult.CreateTestData(@"[]{1,3}?", 0, builder => builder.Grouped("cc", @"[]").Nested("q", "{", b => b.Grouped("min", "1").Nested("qr", ",", r => r.Grouped("max", "3")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt"));
            yield return MatchResult.CreateTestData(@"[one|\]two]??", 0, builder => builder.Nested("cc", "[", b => b.Grouped("ptn", @"one|\]two").Ungrouped("]")).Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "mlt", "min", "qr", "max"));
            yield return MatchResult.CreateTestData(@"[]+?", 0, builder => builder.Grouped("cc", @"[]").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "min", "qr", "max"));
        }

        public static System.Collections.IEnumerable GetMultilinePatternTokenRegexTestData()
        {
            yield return MatchResult.CreateTestData(@"^", 0, builder => builder.Grouped("anc", @"^")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"$", 0, builder => builder.Grouped("anc", @"$")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\G", 0, builder => builder.Grouped("anc", @"\G")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\A", 0, builder => builder.Grouped("anc", @"\A")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\Z", 0, builder => builder.Grouped("anc", @"\Z")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\z", 0, builder => builder.Grouped("anc", @"\z")
                .FailedGroup("alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return MatchResult.CreateTestData(@"|", 0, builder => builder.Grouped("alt", @"|")
                .FailedGroup("anc", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return MatchResult.CreateTestData(@".", 0, builder => builder.Grouped("any", @".")
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return MatchResult.CreateTestData(@"\r\n\t\a\e\f\v", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return MatchResult.CreateTestData(@"\[\\\^\$\.\|\?\*\+\(\)\{\}", 0, builder => builder.Grouped("em", @"\[\\\^\$\.\|\?\*\+\(\)\{\}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return MatchResult.CreateTestData(@"\100", 0, builder => builder.Grouped("oct", @"\100")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\177", 0, builder => builder.Grouped("oct", @"\177")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\200", 0, builder => builder.Grouped("oct", @"\200")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\377", 0, builder => builder.Grouped("oct", @"\377")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\400", 0, builder => builder.Grouped("dbr", @"\40")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\700", 0, builder => builder.Grouped("dbr", @"\70")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\01", 0, builder => builder.Grouped("oct", @"\01")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\07", 0, builder => builder.Grouped("oct", @"\07")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\010", 0, builder => builder.Grouped("oct", @"\010")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\077", 0, builder => builder.Grouped("oct", @"\077")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077\1", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077-", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return MatchResult.CreateTestData(@"\0", 0, builder => builder.Grouped("nul", @"\0")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\00", 0, builder => builder.Grouped("nul", @"\0")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\0\0\0\08", 0, builder => builder.Grouped("nul", @"\0\0\0\0")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\0\0\0\07", 0, builder => builder.Grouped("nul", @"\0\0\0")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return MatchResult.CreateTestData(@"\cA", 0, builder => builder.Grouped("ctl", @"\cA")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\cZ", 0, builder => builder.Grouped("ctl", @"\cZ")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return MatchResult.CreateTestData(@"\c", 0, builder => builder.Grouped("esc", @"\c")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\ca", 0, builder => builder.Grouped("esc", @"\c")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\cz", 0, builder => builder.Grouped("esc", @"\c")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL\cM\cN\cO\cP\cQ\cR\cS\cT\cU\cV\cW\cX\cY", 0,
                builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL\cM\cN\cO\cP\cQ\cR\cS\cT\cU\cV\cW\cX\cY")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL\cm\cN", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL-", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return MatchResult.CreateTestData(@"\c\c\cZ", 0, builder => builder.Grouped("esc", @"\c\c")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return MatchResult.CreateTestData(@"\xFF", 0, builder => builder.Grouped("hex", @"\xFF")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\xAA", 0, builder => builder.Grouped("hex", @"\xAA")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\xff", 0, builder => builder.Grouped("hex", @"\xff")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\xaa", 0, builder => builder.Grouped("hex", @"\xaa")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\x00", 0, builder => builder.Grouped("hex", @"\x00")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\x99", 0, builder => builder.Grouped("hex", @"\x99")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xffff", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff-", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\xFFFF", 0, builder => builder.Grouped("hex", @"\xFF")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\xF", 0, builder => builder.Grouped("esc", @"\x")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\xFG", 0, builder => builder.Grouped("esc", @"\x")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\x0", 0, builder => builder.Grouped("esc", @"\x")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\x\x\x99", 0, builder => builder.Grouped("esc", @"\x\x")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return MatchResult.CreateTestData(@"\uFFFF", 0, builder => builder.Grouped("uni", @"\uFFFF")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\uAAAA", 0, builder => builder.Grouped("uni", @"\uAAAA")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\uffff", 0, builder => builder.Grouped("uni", @"\uffff")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\uaaaa", 0, builder => builder.Grouped("uni", @"\uaaaa")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\u9999", 0, builder => builder.Grouped("uni", @"\u9999")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\u0000", 0, builder => builder.Grouped("uni", @"\u0000")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000-", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\uFFF", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\uFF", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\uF", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\u999", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\uaa", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\u0", 0, builder => builder.Grouped("esc", @"\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\u\u\u0000", 0, builder => builder.Grouped("esc", @"\u\u")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return MatchResult.CreateTestData(@"\p{C}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"C").Ungrouped(@"}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\p{C}\p{C}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"C").Ungrouped(@"}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\p{Cc}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\p{Cc}-", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return MatchResult.CreateTestData(@"\p\p\p{C}", 0, builder => builder.Grouped("esc", @"\p\p")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\p{Cc", 0, builder => builder.Grouped("esc", @"\p")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\p{Cc }", 0, builder => builder.Grouped("esc", @"\p")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return MatchResult.CreateTestData(@" ", 0, builder => builder.Grouped("lit", @" ")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"abc123!@", 0, builder => builder.Grouped("lit", @"abc123!@")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"{", 0, builder => builder.Grouped("lit", @"{")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"}", 0, builder => builder.Grouped("lit", @"}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"{1}", 0, builder => builder.Grouped("lit", @"{1}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"{1,}", 0, builder => builder.Grouped("lit", @"{1,}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"{1,2}", 0, builder => builder.Grouped("lit", @"{1,2}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"abc123!@{}", 0, builder => builder.Grouped("lit", @"abc123!@{}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return MatchResult.CreateTestData(@"abc123!@{,}", 0, builder => builder.Grouped("lit", @"abc123!@{,}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"abc123!@{,3}", 0, builder => builder.Grouped("lit", @"abc123!@{,3}")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return MatchResult.CreateTestData(@"#{,3}", 0, builder => builder.Nested("cmt", "#", b => b.Grouped("t", "{,3}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData("#{,3}\nTest", 0, builder => builder.Nested("cmt", "#", b => b.Grouped("t", "{,3}")).Ungrouped("\n")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData("#\nTest", 0, builder => builder.Grouped("cmt", "#").Ungrouped("\n")
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "t"));

            yield return MatchResult.CreateTestData("()", 0, builder => builder.Nested("grp", "(", b => b.Grouped("ptn", "").Ungrouped(")"))
                .FailedGroup("anc", "alt", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData("(?(?=test)one|two)", 0, builder => builder.Nested("grp", "(", b => b.Grouped("ptn", "?(?=test)one|two").Ungrouped(")"))
                .FailedGroup("anc", "alt", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData("(?#(?=\ntest)one|two)", 0, builder => builder.Nested("grp", "(", b => b.Grouped("ptn", "?#(?=\ntest").Ungrouped(")"))
                .FailedGroup("anc", "alt", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"(?\(?=test)one|two)", 0, builder => builder.Nested("grp", "(", b => b.Grouped("ptn", @"?\(?=test").Ungrouped(")"))
                .FailedGroup("anc", "alt", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return MatchResult.CreateTestData(@"[]", 0, builder => builder.Grouped("cc", @"[]")
                .FailedGroup("anc", "alt", "grp", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"[one|\]two]", 0, builder => builder.Nested("cc", "[", b => b.Grouped("ptn", @"one|\]two").Ungrouped("]"))
                .FailedGroup("anc", "alt", "grp", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));

            //.FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@".{33}", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@".{9,}", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@".{10,11}", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@".?", 0, builder => builder.Grouped("any", @".").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@".+", 0, builder => builder.Grouped("any", @".").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\r\n\t\a\e\f\v{33}", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\r\n\t\a\e\f\v{9,}", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\r\n\t\a\e\f\v{10,11}", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\r\n\t\a\e\f\v?", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\r\n\t\a\e\f\v+", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077{33}", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077{9,}", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077{10,11}", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{", b =>
                b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077?", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077+", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\0\0\0\0{33}", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\0\0\0\0{9,}", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\0\0\0\0{10,11}", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\0\0\0\0?", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\0\0\0\0+", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{33}", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{9,}", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{",
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{10,11}", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL?", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL+", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{33}", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{9,}", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{10,11}", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",",
                r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff?", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff+", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{33}", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{9,}", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{",
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{10,11}", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF?", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF+", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\p{Cc}{33}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\p{Cc}{9,}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\p{Cc}{10,11}", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",",
                r => r.Grouped("max", "11")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\p{Cc}?", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\p{Cc}+", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"abc123!@{1}", 0, builder => builder.Grouped("lit", @"abc123!@").Nested("q", "{", b => b.Grouped("min", "1").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"abc123!@{1,}", 0, builder => builder.Grouped("lit", @"abc123!@").Nested("q", "{", b => b.Grouped("min", "1").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"abc123!@{1,3}", 0, builder => builder.Grouped("lit", @"abc123!@").Nested("q", "{", b => b.Grouped("min", "1").Nested("qr", ",", r => r.Grouped("max", "3")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"abc123!@?", 0, builder => builder.Grouped("lit", @"abc123!@").Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"abc123!@+", 0, builder => builder.Grouped("lit", @"abc123!@").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "lzy", "cmt", "t"));

            yield return MatchResult.CreateTestData(@"[]{1}", 0, builder => builder.Grouped("cc", @"[]").Nested("q", "{", b => b.Grouped("min", "1").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"[one|\]two]{1,}", 0, builder => builder.Nested("cc", "[", b => b.Grouped("ptn", @"one|\]two").Ungrouped("]")).Nested("q", "{", b => b.Grouped("min", "1").Grouped("qr", ",").Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"[]{1,3}", 0, builder => builder.Grouped("cc", @"[]").Nested("q", "{", b => b.Grouped("min", "1").Nested("qr", ",", r => r.Grouped("max", "3")).Ungrouped("}"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"[one|\]two]?", 0, builder => builder.Nested("cc", "[", b => b.Grouped("ptn", @"one|\]two").Ungrouped("]")).Nested("q", b => b.Grouped("opt", "?"))
                .FailedGroup("anc", "alt", "grp", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "mlt", "min", "qr", "max", "lzy", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"[]+", 0, builder => builder.Grouped("cc", @"[]").Nested("q", b => b.Grouped("mlt", "+"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "min", "qr", "max", "lzy", "cmt", "t"));

            //.FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "q", "opt", "mlt", "min", "qr", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@".{33}?", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@".{9,}?", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@".{10,11}?", 0, builder => builder.Grouped("any", @".").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "cmt", "t"));
            yield return MatchResult.CreateTestData(@".??", 0, builder => builder.Grouped("any", @".").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@".+?", 0, builder => builder.Grouped("any", @".").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\r\n\t\a\e\f\v{33}?", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\r\n\t\a\e\f\v{9,}?", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\r\n\t\a\e\f\v{10,11}?", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",",
                r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\r\n\t\a\e\f\v??", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\r\n\t\a\e\f\v+?", 0, builder => builder.Grouped("ce", @"\r\n\t\a\e\f\v").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077{33}?", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077{9,}?", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{",
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077{10,11}?", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",",
                r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077??", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\200\177\377\100\010\077+?", 0, builder => builder.Grouped("oct", @"\200\177\377\100\010\077").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\0\0\0\0{33}?", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\0\0\0\0{9,}?", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\0\0\0\0{10,11}?", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r =>
                r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\0\0\0\0??", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\0\0\0\0+?", 0, builder => builder.Grouped("nul", @"\0\0\0\0").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{33}?", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{",
                b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{9,}?", 0, builder => builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{",
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL{10,11}?", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL??", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL+?", 0, builder =>
                builder.Grouped("ctl", @"\cZ\cA\cB\cC\cD\cE\cF\cG\cH\cI\cJ\cK\cL").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{33}?", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{9,}?", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{",
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff{10,11}?", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",",
                r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff??", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\xaa\x99\xFF\xAA\x00\xff+?", 0, builder => builder.Grouped("hex", @"\xaa\x99\xFF\xAA\x00\xff").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{33}?", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{",
                b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{9,}?", 0, builder => builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{",
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF{10,11}?", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",", r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF??", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF+?", 0, builder =>
                builder.Grouped("uni", @"\uaaaa\uffff\u9999\uAAAA\u0000\uFFFF").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\p{Cc}{33}?", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{", b => b.Grouped("min", "33").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\p{Cc}{9,}?", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{",
                b => b.Grouped("min", "9").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\p{Cc}{10,11}?", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", "{", b => b.Grouped("min", "10").Nested("qr", ",",
                r => r.Grouped("max", "11")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "mlt", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\p{Cc}??", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"\p{Cc}+?", 0, builder => builder.Nested("cat", @"\p{", b => b.Grouped("n", @"Cc").Ungrouped(@"}")).Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "lit", "em", "oct", "nul", "ctl", "hex", "uni", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"abc123!@{1}?", 0, builder => builder.Grouped("lit", @"abc123!@").Nested("q", "{", b => b.Grouped("min", "1").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"abc123!@{1,}?", 0, builder => builder.Grouped("lit", @"abc123!@").Nested("q", "{", b => b.Grouped("min", "1").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"abc123!@{1,3}?", 0, builder => builder.Grouped("lit", @"abc123!@").Nested("q", "{", b => b.Grouped("min", "1").Nested("qr", ",",
                r => r.Grouped("max", "3")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "mlt", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"abc123!@??", 0, builder => builder.Grouped("lit", @"abc123!@").Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "mlt", "min", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"abc123!@+?", 0, builder => builder.Grouped("lit", @"abc123!@").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "cc", "opt", "min", "qr", "max", "max", "cmt", "t"));

            yield return MatchResult.CreateTestData(@"[]{1}?", 0, builder => builder.Grouped("cc", @"[]").Nested("q", "{", b => b.Grouped("min", "1").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"[one|\]two]{1,}?", 0,
                builder => builder.Nested("cc", "[", b => b.Grouped("ptn", @"one|\]two").Ungrouped("]")).Nested("q", "{", b => b.Grouped("min", "1").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"[]{1,3}?", 0, builder => builder.Grouped("cc", @"[]").Nested("q", "{", b => b.Grouped("min", "1").Nested("qr", ",", r => r.Grouped("max", "3")).Ungrouped("}").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "ce", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "mlt", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"[one|\]two]??", 0, builder => builder.Nested("cc", "[", b => b.Grouped("ptn", @"one|\]two").Ungrouped("]")).Nested("q", b => b.Grouped("opt", "?").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "mlt", "min", "qr", "max", "max", "cmt", "t"));
            yield return MatchResult.CreateTestData(@"[]+?", 0, builder => builder.Grouped("cc", @"[]").Nested("q", b => b.Grouped("mlt", "+").Grouped("lzy", "?"))
                .FailedGroup("anc", "alt", "grp", "ptn", "any", "em", "oct", "nul", "ctl", "hex", "uni", "cat", "n", "dbr", "nbr", "esc", "opt", "min", "qr", "max", "cmt", "t"));
        }

        public static System.Collections.IEnumerable GetQuantifierRegexTestData()
        {
            yield return MatchResult.CreateTestData(@"{1}", 0, "{", builder => builder.Grouped("min", "1").Ungrouped("}").FailedGroup("opt", "mlt", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"{1,}", 0, "{", builder => builder.Grouped("min", "1").Grouped("qr", ",").Ungrouped("}").FailedGroup("opt", "mlt", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"{1,3}", 0, "{", builder => builder.Grouped("min", "1").Nested("qr", ",", r => r.Grouped("max", "3")).Ungrouped("}").FailedGroup("opt", "mlt", "lzy"));
            yield return MatchResult.CreateTestData(@"?", 0, builder => builder.Grouped("opt", "?").FailedGroup("mlt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"+", 0, builder => builder.Grouped("mlt", "+").FailedGroup("opt", "min", "qr", "max", "lzy"));
            yield return MatchResult.CreateTestData(@"{1}?", 0, "{", builder => builder.Grouped("min", "1").Ungrouped("}").Grouped("lzy", "?").FailedGroup("opt", "mlt", "qr", "max"));
            yield return MatchResult.CreateTestData(@"{1,}?", 0, "{", builder => builder.Grouped("min", "1").Grouped("qr", ",").Ungrouped("}").Grouped("lzy", "?").FailedGroup("opt", "mlt", "max"));
            yield return MatchResult.CreateTestData(@"{1,3}?", 0, "{", builder => builder.Grouped("min", "1").Nested("qr", ",", r => r.Grouped("max", "3")).Ungrouped("}").Grouped("lzy", "?").FailedGroup("opt", "mlt"));
            yield return MatchResult.CreateTestData(@"??", 0, builder => builder.Grouped("opt", "?").Grouped("lzy", "?").FailedGroup("mlt", "min", "qr", "max"));
            yield return MatchResult.CreateTestData(@"+?", 0, builder => builder.Grouped("mlt", "+").Grouped("lzy", "?").FailedGroup("opt", "min", "qr", "max"));
            yield return MatchResult.CreateTestData(@"{}", 0, MatchResult.Failed);
            yield return MatchResult.CreateTestData(@"{1", 0, MatchResult.Failed);
            yield return MatchResult.CreateTestData(@"{1,", 0, MatchResult.Failed);
            yield return MatchResult.CreateTestData(@"{1,3", 0, MatchResult.Failed);
            yield return MatchResult.CreateTestData(@"{,3}", 0, MatchResult.Failed);
            yield return MatchResult.CreateTestData(@"{,}", 0, MatchResult.Failed);
        }

        public static System.Collections.IEnumerable GetGroupTypeRegexTestData()
        {
            yield return MatchResult.CreateTestData(@"?", 0, "?", builder => builder.FailedGroup("x", "ap", "an", "bp", "bn", "d", "ng", "m", "at", "c", "cr", "cn", "ptn"));
            yield return MatchResult.CreateTestData(@"?:", 0, "?", builder => builder.Grouped("x", ":").FailedGroup("ap", "an", "bp", "bn", "d", "ng", "m", "at", "c", "cr", "cn", "ptn"));
            yield return MatchResult.CreateTestData(@"?=", 0, "?", builder => builder.Grouped("ap", "=").FailedGroup("x", "an", "bp", "bn", "d", "ng", "m", "at", "c", "cr", "cn", "ptn"));
            yield return MatchResult.CreateTestData(@"?!", 0, "?", builder => builder.Grouped("an", "!").FailedGroup("x", "ap", "bp", "bn", "d", "ng", "m", "at", "c", "cr", "cn", "ptn"));
            yield return MatchResult.CreateTestData(@"?<=", 0, "?<", builder => builder.Grouped("bp", "=").FailedGroup("x", "ap", "an", "bn", "d", "ng", "m", "at", "c", "cr", "cn", "ptn"));
            yield return MatchResult.CreateTestData(@"?<!", 0, "?<", builder => builder.Grouped("bn", "!").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn", "ptn"));
            yield return MatchResult.CreateTestData(@"?<name>", 0, "?", builder => builder.Grouped("d", "<").Grouped("ng", "name").Ungrouped(">").FailedGroup("x", "ap", "an", "bp", "bn", "m", "at", "c", "cr", "cn", "ptn"));
            yield return MatchResult.CreateTestData(@"?'name'", 0, "?", builder => builder.Grouped("d", "'").Grouped("ng", "name").Ungrouped("'").FailedGroup("x", "ap", "an", "bp", "bn", "m", "at", "c", "cr", "cn", "ptn"));
            yield return MatchResult.CreateTestData(@"?ixsmn:", 0, "?", builder => builder.Grouped("m", "ixsmn").Grouped("x", ":").FailedGroup("ap", "an", "bp", "bn", "d", "ng", "at", "c", "cr", "cn", "ptn"));
            yield return MatchResult.CreateTestData(@"?>", 0, "?", builder => builder.Grouped("at", ">").FailedGroup("x", "ap", "an", "bp", "bn", "d", "ng", "m", "c", "cr", "cn", "ptn"));
            yield return MatchResult.CreateTestData(@"?(name)", 0, "?", builder => builder.Nested("c", "(", b => b.Grouped("cn", "name").Ungrouped(")")).FailedGroup("x", "ap", "an", "bp", "bn", "d", "ng", "m", "at", "cr", "ptn"));
            yield return MatchResult.CreateTestData(@"?(12)", 0, "?", builder => builder.Nested("c", "(", b => b.Grouped("cr", "12").Ungrouped(")")).FailedGroup("x", "ap", "an", "bp", "bn", "d", "ng", "m", "at", "cn", "ptn"));

            yield return MatchResult.CreateTestData(@"?name>", 0, "?", builder => builder.Grouped("ptn", "name>").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?<name", 0, "?", builder => builder.Grouped("ptn", "<name").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?name'", 0, "?", builder => builder.Grouped("ptn", "name'").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?'name", 0, "?", builder => builder.Grouped("ptn", "'name").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?name)", 0, "?", builder => builder.Grouped("ptn", "name)").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?(name", 0, "?", builder => builder.Grouped("ptn", "(name").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?12)", 0, "?", builder => builder.Grouped("ptn", "12)").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?(12", 0, "?", builder => builder.Grouped("ptn", "(12").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?ixsmni:", 0, "?", builder => builder.Grouped("ptn", "ixsmni:").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?ixsmnx:", 0, "?", builder => builder.Grouped("ptn", "ixsmnx:").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?ixsmns:", 0, "?", builder => builder.Grouped("ptn", "ixsmns:").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?ixsmnm:", 0, "?", builder => builder.Grouped("ptn", "ixsmnm:").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?ixsmnn:", 0, "?", builder => builder.Grouped("ptn", "ixsmnn:").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?aixsmn:", 0, "?", builder => builder.Grouped("ptn", "aixsmn:").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));

            yield return MatchResult.CreateTestData(@"?Test", 0, "?", builder => builder.Grouped("ptn", "Test").FailedGroup("x", "ap", "an", "bp", "bn", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?:Test", 0, "?", builder => builder.Grouped("x", ":").Grouped("ptn", "Test").FailedGroup("ap", "an", "bp", "bn", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?=Test", 0, "?", builder => builder.Grouped("ap", "=").Grouped("ptn", "Test").FailedGroup("x", "an", "bp", "bn", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?!Test", 0, "?", builder => builder.Grouped("an", "!").Grouped("ptn", "Test").FailedGroup("x", "ap", "bp", "bn", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?<=Test", 0, "?<", builder => builder.Grouped("bp", "=").Grouped("ptn", "Test").FailedGroup("x", "ap", "an", "bn", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?<!Test", 0, "?<", builder => builder.Grouped("bn", "!").Grouped("ptn", "Test").FailedGroup("x", "ap", "an", "bp", "d", "ng", "m", "at", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?<name>Test", 0, "?", builder => builder.Grouped("d", "<").Grouped("ng", "name").Ungrouped(">").Grouped("ptn", "Test")
                .FailedGroup("x", "ap", "an", "bp", "bn", "m", "at", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?'name'Test", 0, "?", builder => builder.Grouped("d", "'").Grouped("ng", "name").Ungrouped("'").Grouped("ptn", "Test")
                .FailedGroup("x", "ap", "an", "bp", "bn", "m", "at", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?ixsmn:Test", 0, "?", builder => builder.Grouped("m", "ixsmn").Grouped("x", ":").Grouped("ptn", "Test").FailedGroup("ap", "an", "bp", "bn", "d", "ng", "at", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?>Test", 0, "?", builder => builder.Grouped("at", ">").Grouped("ptn", "Test").FailedGroup("x", "ap", "an", "bp", "bn", "d", "ng", "m", "c", "cr", "cn"));
            yield return MatchResult.CreateTestData(@"?(name)Test", 0, "?", builder => builder.Nested("c", "(", b => b.Grouped("cn", "name").Ungrouped(")")).Grouped("ptn", "Test")
                .FailedGroup("x", "ap", "an", "bp", "bn", "d", "ng", "m", "at", "cr"));
            yield return MatchResult.CreateTestData(@"?(12)Test", 0, "?", builder => builder.Nested("c", "(", b => b.Grouped("cr", "12").Ungrouped(")")).Grouped("ptn", "Test")
                .FailedGroup("x", "ap", "an", "bp", "bn", "d", "ng", "m", "at", "cn"));
        }
    }

    [SetUp]
    public void Setup()
    {
    }

    [TestCaseSource(typeof(TestData), nameof(TestData.GetCharacterClassFirstTokenRegexTestData))]
    public MatchResult CharacterClassNextTokenRegexTestMethod(string input, int startAt)
    {
        Match actualMatch = RegexParser.CharacterClassNextTokenRegex.Match(input, startAt);
        return MatchResult.FromResult(actualMatch);
    }

    [TestCaseSource(typeof(TestData), nameof(TestData.GetSingleLinePatternTokenRegexTestData))]
    public MatchResult SingleLinePatternTokenRegexTestMethod(string input, int startAt)
    {
        Match actualMatch = RegexParser.SingleLinePatternTokenRegex.Match(input, startAt);
        return MatchResult.FromResult(actualMatch);
    }

    [TestCaseSource(typeof(TestData), nameof(TestData.GetMultilinePatternTokenRegexTestData))]
    public MatchResult MultilinePatternTokenRegexTestMethod(string input, int startAt)
    {
        Match actualMatch = RegexParser.MultilinePatternTokenRegex.Match(input, startAt);
        return MatchResult.FromResult(actualMatch);
    }

    [TestCaseSource(typeof(TestData), nameof(TestData.GetQuantifierRegexTestData))]
    public MatchResult QuantifierRegexTestMethod(string input, int startAt)
    {
        Match actualMatch = RegexParser.QuantifierRegex.Match(input, startAt);
        return MatchResult.FromResult(actualMatch);
    }

    [TestCaseSource(typeof(TestData), nameof(TestData.GetGroupTypeRegexTestData))]
    public MatchResult GroupTypeRegexTestMethod(string input, int startAt)
    {
        Match actualMatch = RegexParser.GroupTypeRegex.Match(input, startAt);
        return MatchResult.FromResult(actualMatch);
    }
}
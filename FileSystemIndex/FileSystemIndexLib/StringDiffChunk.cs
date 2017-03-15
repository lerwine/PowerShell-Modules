using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace FileSystemIndexLib
{
    public class StringDiffChunk : UnifiedDiffChunk<StringDiffLine, string>
	{
        public StringDiffChunk(int originalStartLine, int newStartLine, IEnumerable<StringDiffLine> lines) : base(originalStartLine, newStartLine, lines) { }

		protected override StringDiffLine CreateDiffLine(string originalValue, string newValue)
        {
            return new StringDiffLine(originalValue, newValue);
        }

		protected override UnifiedDiffChunk<StringDiffLine, string> CreateDiffChunk(int originalStartLine, int newStartLine, IEnumerable<StringDiffLine> lines)
        {
            return new StringDiffChunk(originalStartLine, newStartLine, lines);
        }

		protected override IEqualityComparer<string> Comparer { get { return StringComparer.InvariantCulture; } }

		protected override bool IsAbsentValue(string value) { return value == null; }

		protected override string GetAbsentValue() { return null; }

		protected override string CoerceNonAbsentValue(string value) { return (value == null) ? "" : value; }
    }
}
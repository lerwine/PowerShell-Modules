using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace FileSystemIndexLib
{
    public class StringDiffChunkNCS : UnifiedDiffChunk<StringDiffLineNCS, string>
	{
        public StringDiffChunkNCS(int originalStartLine, int newStartLine, IEnumerable<StringDiffLineNCS> lines) : base(originalStartLine, newStartLine, lines) { }

		protected override StringDiffLineNCS CreateDiffLine(string originalValue, string newValue)
        {
            return new StringDiffLineNCS(originalValue, newValue);
        }

		protected override UnifiedDiffChunk<StringDiffLineNCS, string> CreateDiffChunk(int originalStartLine, int newStartLine, IEnumerable<StringDiffLineNCS> lines)
        {
            return new StringDiffChunkNCS(originalStartLine, newStartLine, lines);
        }

		protected override IEqualityComparer<string> Comparer { get { return StringComparer.InvariantCultureIgnoreCase; } }

		protected override bool IsAbsentValue(string value) { return value == null; }

		protected override string GetAbsentValue() { return null; }

		protected override string CoerceNonAbsentValue(string value) { return (value == null) ? "" : value; }
    }
}
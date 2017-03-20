using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace FileSystemIndexLib
{
    public class StringDiffChunk : DiffChunk<StringDiffLine, string>
	{
		public StringDiffChunk() : base() { }
		
        public StringDiffChunk(int originalStartLine, int newStartLine, IEnumerable<StringDiffLine> lines) : base(originalStartLine, newStartLine, lines) { }

        public StringDiffChunk(IEnumerable<string> originalValues, int originalStartLine, IEnumerable<string> newValues, int newStartLine) : base(originalValues, originalStartLine, newValues, newStartLine) { }

		protected override StringDiffLine CreateDiffItem(string originalValue, string newValue)
        {
            return new StringDiffLine(originalValue, newValue);
        }

		protected override IEqualityComparer<string> Comparer { get { return StringComparer.InvariantCulture; } }

		protected override bool IsAbsentValue(string value) { return value == null; }

		protected override string GetAbsentValue() { return null; }
    }
}
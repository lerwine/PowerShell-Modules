using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace FileSystemIndexLib
{
    public class StringDiffChunkNCS : DiffChunk<StringDiffLineNCS, string>
	{
		public StringDiffChunkNCS() : base() { }
		
        public StringDiffChunkNCS(int originalStartLine, int newStartLine, IEnumerable<StringDiffLineNCS> lines) : base(originalStartLine, newStartLine, lines) { }

        public StringDiffChunkNCS(IEnumerable<string> originalValues, int originalStartLine, IEnumerable<string> newValues, int newStartLine) : base(originalValues, originalStartLine, newValues, newStartLine) { }

		protected override StringDiffLineNCS CreateDiffItem(string originalValue, string newValue)
        {
            return new StringDiffLineNCS(originalValue, newValue);
        }

		protected override IEqualityComparer<string> Comparer { get { return StringComparer.InvariantCultureIgnoreCase; } }

		protected override bool IsAbsentValue(string value) { return value == null; }

		protected override string GetAbsentValue() { return null; }
    }
}
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace FileSystemIndexLib
{
    public class DiffFile : DiffChunkCollection<StringDiffChunkNCS, StringDiffLineNCS, string>
    {
		public DiffFile() { }
		
		public DiffFile(string originalName, DateTime originalModified, string newName, DateTime newModified, IEnumerable<StringDiffChunkNCS> chunks)
			: base(originalName, originalModified, newName, newModified, chunks) { }
		
		public DiffFile(string originalName, DateTime originalModified, string newName, DateTime newModified, IEnumerable<string> originalItems, IEnumerable<string> newItems)
			: base(originalName, originalModified, newName, newModified, originalItems, newItems) { }
		
		public override StringDiffChunkNCS CreateNewChunk() { return new StringDiffChunkNCS(); }
		
		protected override StringDiffChunkNCS CreateNewChunk(int originalStartLine, int newStartLine, IEnumerable<string> originalItems, IEnumerable<string> newItems)
		{
			return new StringDiffChunkNCS(originalStartLine, newStartLine, originalItems, newItems);
		}
		
		protected override string CoerceNonAbsentValue(string value) { return (value == null) ? "" : value; }
    }
}
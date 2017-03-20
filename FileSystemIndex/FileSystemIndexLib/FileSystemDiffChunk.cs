using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace FileSystemIndexLib
{
    public class FileSystemDiffChunk : DiffChunk<FileSystemDiffLine, FileSystemInfo>
	{
		public FileSystemDiffChunk() : base() { }
		
        public FileSystemDiffChunk(int originalStartLine, int newStartLine, IEnumerable<FileSystemDiffLine> lines) : base(originalStartLine, newStartLine, lines) { }

        public FileSystemDiffChunk(int originalStartLine, int newStartLine, IEnumerable<FileSystemInfo> originalValues, IEnumerable<FileSystemInfo> newValues) : base(originalValues, originalStartLine, newValues, newStartLine) { }

		protected override FileSystemDiffLine CreateDiffItem(FileSystemInfo originalValue, FileSystemInfo newValue)
        {
            return new FileSystemDiffLine(originalValue, newValue);
        }

		protected override IEqualityComparer<FileSystemInfo> Comparer { get { return FileSystemDiff.FileSystemComparer.Default; } }

		protected override bool IsAbsentValue(FileSystemInfo value) { return value == null; }

		protected override FileSystemInfo GetAbsentValue() { return null; }
    }
}
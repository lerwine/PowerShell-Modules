using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace FileSystemIndexLib
{
    public class FileSystemDiffLine : DiffItem<FileSystemInfo>
	{
		public FileSystemDiffLine() : base(null, null) { }
		
		public FileSystemDiffLine(FileSystemInfo originalValue, FileSystemInfo newValue) : base(originalValue, newValue) { }

		protected override bool AreEqual(FileSystemInfo originalValue, FileSystemInfo newValue)
        {
            return FileSystemDiff.FileSystemComparer.AreEqual(originalValue, newValue);
        }
		
		protected override bool IsAbsentValue(FileSystemInfo value) { return value == null; }
        
		protected override string AsStringValue(FileSystemInfo value)
		{
			if (value == null)
				return "\t\t\t\t";
			
			if (!value.Exists)
				return String.Format("{0}\t\t\t\t{1}", value.Name, (value is FileInfo) ? "f" : "d");
			
			if (value is FileInfo)
				return String.Format("{0}\t{1:yyyy-MM-dd HH:mm:ss.fffzzzzzz}\t{2:yyyy-MM-dd HH:mm:ss.fffzzzzzz}\t{3}\tf", value.Name, value.CreationTime, value.LastWriteTime, (value as FileInfo).Length);
			
			return String.Format("{0}\t{1:yyyy-MM-dd HH:mm:ss.fffzzzzzz}\t{2:yyyy-MM-dd HH:mm:ss.fffzzzzzz}\t\tf", value.Name, value.CreationTime, value.LastWriteTime);
		}
    }
}
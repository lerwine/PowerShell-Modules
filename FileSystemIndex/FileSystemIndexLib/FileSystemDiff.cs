using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace FileSystemIndexLib
{
    public class FileSystemDiff : DiffChunkCollection<FileSystemDiffChunk, FileSystemDiffLine, FileSystemInfo>
    {
		public FileSystemDiff() { }
		
		public FileSystemDiff(string originalDirectory, string newDirectory) : this(new DirectoryInfo(originalDirectory), new DirectoryInfo(newDirectory)) { }
		
		public FileSystemDiff(DirectoryInfo originalDirectory, DirectoryInfo newDirectory)
			: base(FileSystemComparer.GetFileSystemInfos(originalDirectory), originalDirectory.FullName, (originalDirectory.Exists) ? originalDirectory.LastWriteTime : DateTime.Now,
				FileSystemComparer.GetFileSystemInfos(newDirectory), newDirectory.FullName, (newDirectory.Exists) ? newDirectory.LastWriteTime : DateTime.Now) { }
		
		public override FileSystemDiffChunk CreateNewChunk() { return new FileSystemDiffChunk(); }
		
		protected override FileSystemDiffChunk CreateNewChunk(IEnumerable<FileSystemInfo> originalItems, int originalStartLine, IEnumerable<FileSystemInfo> newItems, int newStartLine)
		{
			return new FileSystemDiffChunk(originalStartLine, newStartLine, originalItems, newItems);
		}
		
		protected override FileSystemInfo CoerceNonAbsentValue(FileSystemInfo value)
		{
			if (value == null)
				throw new ArgumentNullException();
			
			return value;
		}
		
		public class FileSystemComparer : IEqualityComparer<FileSystemInfo>, IComparer<FileSystemInfo>
		{
			public static IEnumerable<FileSystemInfo> GetFileSystemInfos(DirectoryInfo source)
			{
				if (source == null || !source.Exists)
					return null;
				
				return AsSorted(source.GetFileSystemInfos());
			}
		
			public static IEnumerable<FileSystemInfo> AsSorted(IEnumerable<FileSystemInfo> source)
			{
				if (source == null)
					return source;
				
				return source.OrderBy(f => f, Default);
			}
			
			private static FileSystemComparer _default = null;
			
			public static FileSystemComparer Default
			{
				get
				{
					if (_default == null)
						_default = new FileSystemComparer();
					return _default;
				}
			}
			
			public static bool AreEqual(FileSystemInfo x, FileSystemInfo y)
			{
				if (x == null)
					return y == null;
				
				if (ReferenceEquals(x, y))
					return true;
				
				if (x is DirectoryInfo)
				{
					if (!(y is DirectoryInfo))
						return false;
				}
				else if (y is DirectoryInfo)
					return false;
				
				if (!StringComparer.InvariantCultureIgnoreCase.Equals(x.Name, y.Name))
					return false;
				
				if (!x.Exists)
					return !y.Exists;
				
				if (!(y.Exists && x.LastAccessTime.Equals(y.LastAccessTime) && x.CreationTime.Equals(y.CreationTime)))
					return false;
				
				if (x is FileInfo)
					return (x as FileInfo).Length.Equals((y as FileInfo).Length);
				
				return true;
			}
			
			public bool Equals(FileSystemInfo x, FileSystemInfo y) { return AreEqual(x, y); }
			
			public int Compare(FileSystemInfo x, FileSystemInfo y)
			{
				if (x == null)
					return (y == null) ? 0 : -1;
				
				if (y == null)
					return 1;
				
				if (ReferenceEquals(x, y))
					return 0;
				
				if (x is DirectoryInfo)
				{
					if (!(y is DirectoryInfo))
						return 1;
				}
				else if (y is DirectoryInfo)
					return -1;
				
				int result = StringComparer.InvariantCultureIgnoreCase.Compare(x.Name, y.Name);
				if (result != 0)
					return result;
				
				if (!x.Exists)
					return (y.Exists) ? -1 : 0;
				if (!y.Exists)
					return 1;
				if ((result = x.LastAccessTime.CompareTo(y.LastAccessTime)) != 0 || (result = x.CreationTime.CompareTo(y.CreationTime)) != 0 || !(x is FileInfo))
					return result;
				
				return (x as FileInfo).Length.CompareTo((y as FileInfo).Length);
			}
			
			public int GetHashCode(FileSystemInfo obj)
			{
				return ((obj == null) ? "" : ((obj is DirectoryInfo) ? "1" : "0") + obj.Name.ToLower()).GetHashCode();
			}
		}
    }
}
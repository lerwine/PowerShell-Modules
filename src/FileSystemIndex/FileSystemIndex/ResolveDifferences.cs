using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace FileSystemIndexLib
{
    public class WindowOwner : IWin32Window
    {
        private IntPtr _handle;
        public WindowOwner() : this(Process.GetCurrentProcess().MainWindowHandle) { }
        public WindowOwner(IntPtr handle) { _handle = handle; }
        public IntPtr Handle { get { return _handle; } }
    }
	public class DiffSelection
	{
		public const string ChoiceLabel_CopyLeft = "<";
		public const string ChoiceLabel_CopyRight = ">";
		public const string ChoiceLabel_Edit = "E";
		public const string ChoiceLabel_Delete = "X";
		public const string ChoiceLabel_Ignore = "I";
		public const string ChoiceLabel_None = "N";
		public FileSystemInfo Left { get; private set; }
		public FileSystemInfo Right { get; private set; }
		public DiffResult Diff { get; private set; }
		public bool IsSelected { get; private set; }
		public static Collection<DiffSelection> CopyToLeft(IEnumerable<DiffSelection> collection)
		{
			Collection<DiffSelection> result = new Collection<DiffSelection>();
			foreach (DiffSelection item in collection)
			{
				if (!item.IsSelected)
					result.Add(item);
				else if (item.Right is DirectoryInfo)
					CopyDirectory(item.Right as DirectoryInfo, item.Left as DirectoryInfo);
				else
					CopyFile(item.Right as FileInfo, item.Left as FileInfo);
			}
			return result;
		}
		public static Collection<DiffSelection> CopyToRight(IEnumerable<DiffSelection> collection)
		{
			Collection<DiffSelection> result = new Collection<DiffSelection>();
			foreach (DiffSelection item in collection)
			{
				if (!item.IsSelected)
					result.Add(item);
				else if (item.Left is DirectoryInfo)
					CopyDirectory(item.Left as DirectoryInfo, item.Right as DirectoryInfo);
				else
					CopyFile(item.Left as FileInfo, item.Right as FileInfo);
			}
			return result;
		}
		public static Collection<DiffSelection> Delete(IEnumerable<DiffSelection> collection)
		{
			Collection<DiffSelection> result = new Collection<DiffSelection>();
			foreach (DiffSelection item in collection)
			{
				if (!item.IsSelected)
					result.Add(item);
				else if (item.Left.Exists)
				{
					if (item.Left is DirectoryInfo)
						(item.Left as DirectoryInfo).Delete(true);
					else
						(item.Left as FileInfo).Delete();
				}
				else if (item.Right is DirectoryInfo)
					(item.Right as DirectoryInfo).Delete(true);
				else
					(item.Right as FileInfo).Delete();
			}
			return result;
		}
		public static Collection<DiffSelection> Refresh(IEnumerable<DiffSelection> collection, string leftRoot, string rightRoot)
		{
			Collection<DiffSelection> result = new Collection<DiffSelection>();
			DirectoryInfo leftDir = new DirectoryInfo(leftRoot);
			DirectoryInfo rightDir = new DirectoryInfo(rightRoot);
			foreach (DiffSelection item in collection)
			{
				if (item.IsSelected)
				{
					item.Diff.Refresh(leftDir, rightDir);
					if (item.Diff.Message.Length == 0)
						continue;
				}
				
				result.Add(item);
			}
			return result;
		}
		public static void CopyFile(FileInfo left, FileInfo right)
		{
			File.Copy(left.FullName, right.FullName, right.Exists);
		}
		public static void CopyDirectory(DirectoryInfo left, DirectoryInfo right)
		{
			FileInfo[] rightFiles;
			DirectoryInfo[] rightDirectories;
			if (!right.Exists)
			{
				right.Create();
				right.Refresh();
				rightFiles = new FileInfo[0];
				rightDirectories = new DirectoryInfo[0];
			}
			else
			{
				rightFiles = right.GetFiles();
				rightDirectories = right.GetDirectories();
			}
			
			if (rightFiles.Length == 0)
			{
				foreach (FileInfo file in left.GetFiles())
					File.Copy(file.FullName, System.IO.Path.Combine(right.FullName, file.Name));
			}
			else
			{
				Dictionary<string, FileInfo> leftFileTable = new Dictionary<string, FileInfo>(StringComparer.InvariantCultureIgnoreCase);
				foreach (FileInfo fileInfo in left.GetFiles())
					leftFileTable.Add(fileInfo.Name, fileInfo);
				Dictionary<string, FileInfo> rightFileTable = new Dictionary<string, FileInfo>(StringComparer.InvariantCultureIgnoreCase);
				foreach (FileInfo fileInfo in rightFiles)
					rightFileTable.Add(fileInfo.Name, fileInfo);
				foreach (string key in leftFileTable.Keys)
				{
					if (rightFileTable.ContainsKey(key))
						File.Copy(leftFileTable[key].FullName, rightFileTable[key].FullName, true);
					else
						File.Copy(leftFileTable[key].FullName, System.IO.Path.Combine(right.FullName, key), false);
				}
				foreach (string key in rightFileTable.Keys)
				{
					if (!leftFileTable.ContainsKey(key))
						rightFileTable[key].Delete();
				}
			}
			
			if (rightDirectories.Length == 0)
			{
				foreach (DirectoryInfo directory in left.GetDirectories())
					CopyDirectory(directory, new DirectoryInfo(System.IO.Path.Combine(right.FullName, directory.Name)));
				return;
			}
			
			Dictionary<string, DirectoryInfo> leftDirTable = new Dictionary<string, DirectoryInfo>(StringComparer.InvariantCultureIgnoreCase);
			foreach (DirectoryInfo directoryInfo in left.GetDirectories())
				leftDirTable.Add(directoryInfo.Name, directoryInfo);
			Dictionary<string, DirectoryInfo> rightDirTable = new Dictionary<string, DirectoryInfo>(StringComparer.InvariantCultureIgnoreCase);
			foreach (DirectoryInfo directoryInfo in rightDirectories)
				rightDirTable.Add(directoryInfo.Name, directoryInfo);
			foreach (string key in leftDirTable.Keys)
			{
				if (rightDirTable.ContainsKey(key))
					CopyDirectory(leftDirTable[key], rightDirTable[key]);
				else
					CopyDirectory(leftDirTable[key], new DirectoryInfo(System.IO.Path.Combine(right.FullName, key)));
			}
			foreach (string key in rightDirTable.Keys)
			{
				if (!leftDirTable.ContainsKey(key))
					rightDirTable[key].Delete(true);
			}
		}
		public static Collection<DiffSelection> Create(string leftRoot, string rightRoot, IEnumerable<DiffResult> allItems, IEnumerable<DiffResult> selectedItems)
		{
			Collection<DiffSelection> result = new Collection<DiffSelection>();
			DirectoryInfo leftDirectoryInfo = new DirectoryInfo(leftRoot);
			DirectoryInfo rightDirectoryInfo = new DirectoryInfo(rightRoot);
			foreach (DiffResult item in allItems)
				result.Add(new DiffSelection(item, leftDirectoryInfo, rightDirectoryInfo, selectedItems.Any(i => ReferenceEquals(i, item))));
			foreach (DiffResult item in selectedItems.Where(i => !allItems.Any(a => ReferenceEquals(a, i))))
				result.Add(new DiffSelection(item, leftDirectoryInfo, rightDirectoryInfo, true));
			return result;
		}
		private DiffSelection(DiffResult item, DirectoryInfo leftRoot, DirectoryInfo rightRoot, bool isSelected)
		{
			IsSelected = isSelected;
			Diff = item;
			if (item.IsDirectory)
			{
				Left = new DirectoryInfo(item.GetFullName(leftRoot));
				Right = new DirectoryInfo(item.GetFullName(rightRoot));
			}
			else
			{
				Left = new FileInfo(item.GetFullName(leftRoot));
				Right = new FileInfo(item.GetFullName(rightRoot));
			}
		}
		public static string GetUserChoice(PSHostUserInterface ui, IEnumerable<DiffSelection> selections)
		{
			if (selections.Count(i => i.IsSelected) == 0)
				return ChoiceLabel_None;
			StringBuilder message = new StringBuilder();
			bool canDeleteLeft = true;
			bool canMoveRight = true;
			bool canDeleteRight = true;
			bool canMoveLeft = true;
			bool canEdit = true;
			foreach (DiffSelection item in selections.Where(i => i.IsSelected))
			{
				if (message.Length > 0)
					message.AppendLine();
				message.AppendLine(item.Diff.Message);
				if (item.Right.Exists)
				{
					canDeleteLeft = false;
					if (item.Left.Exists)
					{
						canDeleteRight = false;
						message.AppendLine(String.Format("\t{0} ({1})", item.Left.FullName, item.Left.LastWriteTime));
					}
					else
					{
						canEdit = false;
						canMoveRight = false;
						message.AppendLine(String.Format("\t[{0}]", item.Left.FullName));
					}
					message.Append(String.Format("\t=> {0} ({1})", item.Right.FullName, item.Right.LastWriteTime));
				}
				else
				{
					canEdit = false;
					canMoveLeft = false;
					canDeleteRight = false;
					message.AppendLine(String.Format("\t{0} ({1})", item.Left.FullName, item.Left.LastWriteTime));
					message.Append(String.Format("\t[{0}]", item.Right.FullName));
				}
			}
			Collection<ChoiceDescription> choices = new Collection<ChoiceDescription>();
			if (canMoveLeft)
				choices.Add(new ChoiceDescription(ChoiceLabel_CopyLeft, "Copy to left"));
			if (canMoveRight)
				choices.Add(new ChoiceDescription(ChoiceLabel_CopyRight, "Copy to right"));
			if (canEdit)
				choices.Add(new ChoiceDescription(ChoiceLabel_Edit, "Edit files"));
			if (canDeleteLeft || canDeleteRight)
				choices.Add(new ChoiceDescription(ChoiceLabel_Delete, "Delete files"));
			choices.Add(new ChoiceDescription(ChoiceLabel_Ignore, "Ignore"));
			choices.Add(new ChoiceDescription(ChoiceLabel_None, "Take no action"));
			int index = ui.PromptForChoice("Choose action", message.ToString(), choices, choices.Count - 1);
			if (index < 0 || index >= choices.Count)
				return ChoiceLabel_None;
			return choices[index].Label;
		}
	}
	public class DiffResult
	{
        public string Path { get; private set; }
        public string Name { get; private set; }
        public string Message { get; private set; }
        public bool IsDirectory { get; private set; }
        public long LeftLen { get; private set; }
        public long RightLen { get; private set; }
        public DateTime? LeftMod { get; private set; }
        public DateTime? RightMod { get; private set; }
		public static Collection<DiffResult> GetResults(long sourceId, int activityId, string activity, PSHostUserInterface ui, string leftPath, string rightPath)
		{
			Collection<DiffResult> results = new Collection<DiffResult>();
			ProgressRecord progressRecord = new ProgressRecord(activityId, activity, "Initializing");
			progressRecord.PercentComplete = 0;
			ui.WriteProgress(sourceId, progressRecord);
			try
			{
				DirectoryInfo leftDirectoryInfo = new DirectoryInfo(leftPath);
				DirectoryInfo rightDirectoryInfo = new DirectoryInfo(rightPath);
				List<DiffSource> source = DiffSource.Create(leftDirectoryInfo, rightDirectoryInfo);
				var itemWithSize = source.Select(s => new { Source = s, Length = (s.LeftExists && s.RightExists && !s.DoNotProcessLines) ? s.LeftLen + s.RightLen : 0L }).ToArray();
				long totalLength = itemWithSize.Sum(s => s.Length) + source.Count;
				long completedLength = 0L;
				foreach (var item in itemWithSize)
				{
					progressRecord = new ProgressRecord(activityId, activity, "Comparing");
					progressRecord.PercentComplete = Convert.ToInt32((Convert.ToDouble(completedLength) * 100.0) / Convert.ToDouble(totalLength));
					progressRecord.CurrentOperation = item.Source.GetRelativeName();
					ui.WriteProgress(sourceId, progressRecord);
					if (item.Length == 0)
						completedLength++;
					else
						completedLength += item.Length + 1;
					DiffResult r = new DiffResult(item.Source, leftDirectoryInfo, rightDirectoryInfo);
					if (r.Message.Length > 0)
						results.Add(r);
				}
			}
			finally
			{
				progressRecord = new ProgressRecord(activityId, activity, "Finished");
				progressRecord.PercentComplete = 100;
				progressRecord.RecordType = ProgressRecordType.Completed;
				ui.WriteProgress(sourceId, progressRecord);
			}
			return results;
		}
		public string GetRelativeName()
		{
			if (Path.Length == 0)
				return Name;
			return System.IO.Path.Combine(Path, Name);
		}
		internal void Refresh(DirectoryInfo leftRoot, DirectoryInfo rightRoot)
		{
			ReInitialize(DiffSource.Create(this, leftRoot, rightRoot), leftRoot, rightRoot);
		}
		public string GetFullName(DirectoryInfo root)
		{
			if (Path.Length == 0)
				return System.IO.Path.Combine(root.FullName, Name);
			return System.IO.Path.Combine(root.FullName, Path, Name);
		}
		public DiffResult(DiffSource source, DirectoryInfo leftRoot, DirectoryInfo rightRoot)
		{
			Path = source.Path;
			Name = source.Name;
			IsDirectory = source.IsDirectory;
			ReInitialize(source, leftRoot, rightRoot);
		}
		
		private void ReInitialize(DiffSource source, DirectoryInfo leftRoot, DirectoryInfo rightRoot)
		{
			RightMod = source.RightMod;
			LeftMod = source.LeftMod;
		
			if (!source.LeftExists)
			{
				LeftLen = -1;
				RightLen = (source.RightExists) ? source.RightLen : -1;
				Message = (source.IsDirectory) ? "Left folder does not exist." : "Left file does not exist.";
				return;
			}
			
			LeftLen = source.LeftLen;
			
			if (!source.RightExists)
			{
				RightLen = -1;
				Message = (source.IsDirectory) ? "Right folder does not exist." : "Right file does not exist.";
				return;
			}
			
			RightLen = source.RightLen;
			
			if (source.DoNotProcessLines)
			{
				if (source.LeftLen != source.RightLen)
					Message = String.Format("Length {0} != {1}", source.LeftLen, source.RightLen);
				else
					Message = "";
				return;
			}
			
			using (StreamReader leftReader = new StreamReader(source.GetFullName(leftRoot)))
			{
				using (StreamReader rightReader = new StreamReader(source.GetFullName(rightRoot)))
				{
					int lineNumber = 1;
					while (!(leftReader.EndOfStream || rightReader.EndOfStream))
					{
						
						if (leftReader.ReadLine() != rightReader.ReadLine())
							break;
						lineNumber++;
					}
					
					if (leftReader.EndOfStream && rightReader.EndOfStream)
						Message = "";
					else
						Message = String.Format("Difference starting at line {0}.", lineNumber);
				}
			}
		}
	}
    public class DiffSource
    {
        private long? _leftLen;
        private long? _rightLen;
        public string Path { get; private set; }
        public string Name { get; private set; }
        public bool IsDirectory { get { return !(_leftLen.HasValue || _rightLen.HasValue); } }
        public bool LeftExists { get { return LeftMod.HasValue; } }
        public bool RightExists { get { return RightMod.HasValue; } }
        public long LeftLen { get { return (_leftLen.HasValue) ? _leftLen.Value : 0L; } }
        public long RightLen { get { return (_rightLen.HasValue) ? _rightLen.Value : 0L; } }
        public DateTime? LeftMod { get; private set; }
        public DateTime? RightMod { get; private set; }
		public bool DoNotProcessLines { get; private set; }
		public string GetFullName(DirectoryInfo root)
		{
			if (Path.Length == 0)
				return System.IO.Path.Combine(root.FullName, Name);
			return System.IO.Path.Combine(root.FullName, Path, Name);
		}
		public static List<DiffSource> Create(DirectoryInfo left, DirectoryInfo right)
		{
			List<DiffSource> list = new List<DiffSource>();
			if (!right.Exists)
			{
				if (left.Exists)
					list.Add(new DiffSource("", left.Name, false, null, null, left.LastWriteTime, null));
			}
			else if (!left.Exists)
				list.Add(new DiffSource("", right.Name, false, null, null, null, right.LastWriteTime));	
			else
				Create(list, "", left, right);
			return list;
		}
		internal static DiffSource Create(DiffResult diffResult, DirectoryInfo leftRoot, DirectoryInfo rightRoot)
		{
			if (diffResult.IsDirectory)
			{
				DirectoryInfo leftDirectoryInfo = new DirectoryInfo(diffResult.GetFullName(leftRoot));
				DirectoryInfo rightDirectoryInfo = new DirectoryInfo(diffResult.GetFullName(rightRoot));
				if (leftDirectoryInfo.Exists)
				{
					if (rightDirectoryInfo.Exists)
						return new DiffSource(diffResult.Path, leftDirectoryInfo.Name, false, null, null, leftDirectoryInfo.LastWriteTime, rightDirectoryInfo.LastWriteTime);
					return new DiffSource(diffResult.Path, leftDirectoryInfo.Name, false, null, null, leftDirectoryInfo.LastWriteTime, null);
				}
				
				return new DiffSource(diffResult.Path, rightDirectoryInfo.Name, false, null, null, null, rightDirectoryInfo.LastWriteTime);
			}
			
			FileInfo leftFileInfo = new FileInfo(diffResult.GetFullName(leftRoot));
			FileInfo rightFileInfo = new FileInfo(diffResult.GetFullName(rightRoot));
			if (leftFileInfo.Exists)
			{
				if (rightFileInfo.Exists)
					return new DiffSource(diffResult.Path, leftFileInfo.Name, GetDoNotProcessLines(leftFileInfo.Extension), leftFileInfo.Length, rightFileInfo.Length, leftFileInfo.LastWriteTime, rightFileInfo.LastWriteTime);
				return new DiffSource(diffResult.Path, leftFileInfo.Name, false, leftFileInfo.Length, null, leftFileInfo.LastWriteTime, null);
			}
			
			return new DiffSource(diffResult.Path, rightFileInfo.Name, false, null, rightFileInfo.Length, null, rightFileInfo.LastWriteTime);
		}
		private static void Create(List<DiffSource> list, string commonPath, DirectoryInfo leftParent, DirectoryInfo rightParent)
		{
			FileInfo[] leftFiles = leftParent.GetFiles();
			FileInfo[] rightFiles = rightParent.GetFiles();
			List<string> allNames;
			
			if (rightFiles == null || rightFiles.Length == 0)
			{
				if (leftFiles != null &&leftFiles.Length > 0)
				{
					foreach (FileInfo fileInfo in leftFiles)
						list.Add(new DiffSource(commonPath, fileInfo.Name, false, fileInfo.Length, null, fileInfo.LastWriteTime, null));
				}
			}
			else if (leftFiles == null || leftFiles.Length == 0)
			{
				foreach (FileInfo fileInfo in rightFiles)
					list.Add(new DiffSource(commonPath, fileInfo.Name, false, null, fileInfo.Length, null, fileInfo.LastWriteTime));
			}
			else
			{
				Dictionary<string, FileInfo> leftFileTable = new Dictionary<string, FileInfo>(StringComparer.InvariantCultureIgnoreCase);
				foreach (FileInfo fileInfo in leftFiles)
					leftFileTable.Add(fileInfo.Name, fileInfo);
				Dictionary<string, FileInfo> rightFileTable = new Dictionary<string, FileInfo>(StringComparer.InvariantCultureIgnoreCase);
				foreach (FileInfo fileInfo in rightFiles)
					rightFileTable.Add(fileInfo.Name, fileInfo);
				allNames = new List<string>(leftFileTable.Keys);
				foreach (string key in rightFileTable.Keys)
				{
					if (!leftFileTable.ContainsKey(key))
						allNames.Add(key);
				}
				allNames.Sort(StringComparer.OrdinalIgnoreCase);
				foreach (string key in allNames)
				{
					if (leftFileTable.ContainsKey(key))
					{
						if (rightFileTable.ContainsKey(key))
							list.Add(new DiffSource(commonPath, leftFileTable[key].Name, GetDoNotProcessLines(leftFileTable[key].Extension), leftFileTable[key].Length, rightFileTable[key].Length, leftFileTable[key].LastWriteTime, rightFileTable[key].LastWriteTime));
						else
							list.Add(new DiffSource(commonPath, leftFileTable[key].Name, false, leftFileTable[key].Length, null, leftFileTable[key].LastWriteTime, null));
					}
					else
						list.Add(new DiffSource(commonPath, rightFileTable[key].Name, false, null, rightFileTable[key].Length, null, rightFileTable[key].LastWriteTime));
				}
			}
			
			DirectoryInfo[] leftDirectories = leftParent.GetDirectories();
			DirectoryInfo[] rightDirectories = rightParent.GetDirectories();
			if (rightDirectories == null || rightDirectories.Length == 0)
			{
				if (leftDirectories != null && leftDirectories.Length > 0)
				{
					foreach (DirectoryInfo directoryInfo in leftDirectories)
						list.Add(new DiffSource(commonPath, directoryInfo.Name, false, null, null, directoryInfo.LastWriteTime, null));
				}
				return;
			}
			
			if (leftDirectories == null || leftDirectories.Length == 0)
			{
				foreach (DirectoryInfo directoryInfo in rightDirectories)
					list.Add(new DiffSource(commonPath, directoryInfo.Name, false, null, null, null, directoryInfo.LastWriteTime));
				return;
			}
			
			Dictionary<string, DirectoryInfo> leftDirTable = new Dictionary<string, DirectoryInfo>(StringComparer.InvariantCultureIgnoreCase);
			foreach (DirectoryInfo directoryInfo in leftDirectories)
				leftDirTable.Add(directoryInfo.Name, directoryInfo);
			Dictionary<string, DirectoryInfo> rightDirTable = new Dictionary<string, DirectoryInfo>(StringComparer.InvariantCultureIgnoreCase);
			foreach (DirectoryInfo directoryInfo in rightDirectories)
				rightDirTable.Add(directoryInfo.Name, directoryInfo);
			allNames = new List<string>(leftDirTable.Keys);
			foreach (string key in rightDirTable.Keys)
			{
				if (!leftDirTable.ContainsKey(key))
					allNames.Add(key);
			}
			allNames.Sort(StringComparer.OrdinalIgnoreCase);
			foreach (string key in allNames)
			{
				if (leftDirTable.ContainsKey(key))
				{
					if (rightDirTable.ContainsKey(key))
						Create(list, System.IO.Path.Combine(commonPath, key), leftDirTable[key], rightDirTable[key]);
					else
						list.Add(new DiffSource(commonPath, leftDirTable[key].Name, false, null, null, leftDirTable[key].LastWriteTime, null));
				}
				else
					list.Add(new DiffSource(commonPath, rightDirTable[key].Name, false, null, null, null, rightDirTable[key].LastWriteTime));
			}
		}
		public string GetRelativeName()
		{
			if (Path.Length == 0)
				return Name;
			return System.IO.Path.Combine(Path, Name);
		}
		public static bool GetDoNotProcessLines(string extension) { return StringComparer.InvariantCultureIgnoreCase.Equals(".dll"); }
		private DiffSource(string path, string name, bool shouldProcessLines, long? leftLen, long? rightLen, DateTime? leftMod, DateTime? rightMod)
		{
			Path = path;
			Name = name;
			DoNotProcessLines = shouldProcessLines;
			_leftLen = leftLen;
			_rightLen = rightLen;
			LeftMod = leftMod;
			RightMod = rightMod;
		}
	}
}
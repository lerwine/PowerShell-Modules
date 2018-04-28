using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

namespace FileSystemIndexLib
{
    /// <summary>
    /// Maintains state to recursively iterate through subdirectories.
    /// </summary>
	[Serializable]
    public class DirectoryCrawlIterator : IXmlSerializable
    {
        /// <summary>
        /// Element name for serializing <see cref="MaxRecurseDepth" /> to XML.
        /// </summary>
		public const string AttributeName_MaxRecurseDepth = "MaxRecurseDepth";
		
        /// <summary>
        /// Element name for serializing curent iteration index to XML.
        /// </summary>
		protected const string AttributeName_CurrentDirectoryIndex = "CurrentDirectoryIndex";
		
        /// <summary>
        /// Element name for serializing path iteration indexes to XML.
        /// </summary>
		protected const string AttributeName_Index = "Index";
		
        /// <summary>
        /// Element name for serializing <see cref="StartDirectory" /> to XML.
        /// </summary>
		public const string ElementName_StartDirectory = "StartDirectory";
		
        /// <summary>
        /// Element name for serializing <see cref="FileSearchPattern" /> to XML.
        /// </summary>
		public const string ElementName_FileSearchPattern = "FileSearchPattern";
		
        /// <summary>
        /// Element name for serializing <see cref="DirectorySearchPattern" /> to XML.
        /// </summary>
		public const string ElementName_DirectorySearchPattern = "DirectorySearchPattern";
		
        /// <summary>
        /// Element name for serializing current directory content state items to XML.
        /// </summary>
		protected const string ElementName_CurrentDirectoryList = "CurrentDirectoryList";
		
        /// <summary>
        /// Element name for serializing iterated content state items to XML.
        /// </summary>
		protected const string ElementName_DirectoryStack = "DirectoryStack";
		
        /// <summary>
        /// Element name for serializing path state stack to XML.
        /// </summary>
		protected const string ElementName_Stack = "Stack";
		
        /// <summary>
        /// Element name for serializing path state items to XML.
        /// </summary>
		protected const string ElementName_Item = "Item";
		
        /// <summary>
        /// Element name for serializing <see cref="LastIterationError" /> to XML.
        /// </summary>
		public const string ElementName_lastIterationError = "LastIterationError";
		
        /// <summary>
        /// Element name for serializing <see cref="LastErrorDirectory" /> to XML.
        /// </summary>
		public const string ElementName_LastErrorDirectory = "LastErrorDirectory";
		
		[NonSerialized()]
		private object _syncRoot = new object();
		private string _startDirectory;
        private string _fileSearchPattern;
		private string _directorySearchPattern;
		private int _maxRecurseDepth;
        private Stack<string[]> _directoryListStack = new Stack<string[]>();
        private Stack<int> _directoryIndexStack = new Stack<int>();
        private string[] _currentDirectoryList = new string[0];
		private int _currentDirectoryIndex = -1;
		private Exception _lastIterationError = null;
		private string _lastErrorDirectory = null;
		[NonSerialized()]
		private DirectoryInfo _currentDirectory = null;
		[NonSerialized()]
		private FileInfo[] _currentFileList = null;
		
        /// <summary>
        /// Root directory for iteration.
        /// </summary>
		public string StartDirectory { get { return _startDirectory; } }
		
        /// <summary>
        /// Pattern of files to search for.
        /// </summary>
		public string FileSearchPattern { get { return _fileSearchPattern; } }
		
        /// <summary>
        /// Pattern of subdirectories to iterate through.
        /// </summary>
		public string DirectorySearchPattern { get { return _directorySearchPattern; } }
		
        /// <summary>
        /// Maximum depth of subdirectories to recurse into.
        /// </summary>
		public int MaxRecurseDepth { get { return _maxRecurseDepth; } }
		
        /// <summary>
        /// Current directory in iteration.
        /// </summary>
		public DirectoryInfo CurrentDirectory
		{
			get
			{
				lock (_syncRoot)
					return AssertCurrentDirectory();
			}
		}
		
        /// <summary>
        /// Error (if any) from previous iteration advancement.
        /// </summary>
		public Exception LastIterationError { get { return _lastIterationError; } }
		
        /// <summary>
        /// Directory of last iteration if it produced an Exception.
        /// </summary>
		public string LastErrorDirectory { get { return _lastErrorDirectory; } }
		
        /// <summary>
        /// List of files in current directory.
        /// </summary>
		public FileInfo[] CurrentFileList
		{
			get
			{
				lock (_syncRoot)
				{
					if (_currentFileList == null)
					{
						DirectoryInfo directoryInfo;
						if ((directoryInfo = AssertCurrentDirectory()) != null && directoryInfo.Exists)
						{
							if (_fileSearchPattern.Length == 0)
								_currentFileList = directoryInfo.GetFiles();
							else
								_currentFileList = directoryInfo.GetFiles(_fileSearchPattern, SearchOption.TopDirectoryOnly);
						}
						else
							_currentFileList = new FileInfo[0];
					}
				}
				
				return _currentFileList;
			}
		}
		
        /// <summary>
        /// Initialize new <see cref="DirectoryCrawlIterator" />.
        /// </summary>
        /// <param name="startDirectory">Root directory for iteration.</param>
        /// <param name="fileSearchPattern">Pattern of files to search for.</param>
        /// <param name="directorySearchPattern">Pattern of subdirectories to iterate through.</param>
        /// <param name="maxRecurseDepth">Maximum depth of subdirectories to recurse into.</param>
        public DirectoryCrawlIterator(string startDirectory, string fileSearchPattern, string directorySearchPattern, int maxRecurseDepth)
        {
			if (startDirectory == null)
				throw new ArgumentNullException("startDirectory");
			
			if (startDirectory.Trim().Length == 0)
				throw new ArgumentException("Start directory cannot be empty", "startDirectory");
			
			_startDirectory = startDirectory;
			_fileSearchPattern = (fileSearchPattern == null) ? "" : fileSearchPattern.Trim();
			_directorySearchPattern = (directorySearchPattern == null) ? "" : directorySearchPattern.Trim();
			_maxRecurseDepth = maxRecurseDepth;
			
			if (Directory.Exists(startDirectory))
				_currentDirectoryList = new string[] { startDirectory };
        }

        /// <summary>
        /// Initialize new <see cref="DirectoryCrawlIterator" />.
        /// </summary>
        /// <param name="startDirectory">Root directory for iteration.</param>
        /// <param name="fileSearchPattern">Pattern of files to search for.</param>
        /// <param name="directorySearchPattern">Pattern of subdirectories to iterate through.</param>
        public DirectoryCrawlIterator(string startDirectory, string fileSearchPattern, string directorySearchPattern) : this(startDirectory, fileSearchPattern, directorySearchPattern, -1) { }

        /// <summary>
        /// Initialize new <see cref="DirectoryCrawlIterator" />.
        /// </summary>
        /// <param name="startDirectory">Root directory for iteration.</param>
        /// <param name="fileSearchPattern">Pattern of files to search for.</param>
        /// <param name="maxRecurseDepth">Maximum depth of subdirectories to recurse into.</param>
        public DirectoryCrawlIterator(string startDirectory, string fileSearchPattern, int maxRecurseDepth) : this(startDirectory, fileSearchPattern, null, maxRecurseDepth) { }

        /// <summary>
        /// Initialize new <see cref="DirectoryCrawlIterator" />.
        /// </summary>
        /// <param name="startDirectory">Root directory for iteration.</param>
        /// <param name="fileSearchPattern">Pattern of files to search for.</param>
        public DirectoryCrawlIterator(string startDirectory, string fileSearchPattern) : this(startDirectory, fileSearchPattern, null) { }

        /// <summary>
        /// Initialize new <see cref="DirectoryCrawlIterator" />.
        /// </summary>
        /// <param name="startDirectory">Root directory for iteration.</param>
        /// <param name="maxRecurseDepth">Maximum depth of subdirectories to recurse into.</param>
        public DirectoryCrawlIterator(string startDirectory, int maxRecurseDepth) : this(startDirectory, null, maxRecurseDepth) { }

        /// <summary>
        /// Initialize new <see cref="DirectoryCrawlIterator" />.
        /// </summary>
        /// <param name="startDirectory">Root directory for iteration.</param>
        public DirectoryCrawlIterator(string startDirectory) : this(startDirectory, null) { }
		
		private DirectoryInfo AssertCurrentDirectory()
		{
			if (_currentDirectory == null && _currentDirectoryList.Length > 0)
				_currentDirectory = new DirectoryInfo(_currentDirectoryList[_currentDirectoryIndex]);
			return _currentDirectory;
		}
		
        /// <summary>
        /// Advances to the next subdirectory.
        /// </summary>
        /// <returns>true if there was a following subdirectory to iterate into or false if there are no more subdirectories.</returns>
		public bool MoveNext()
		{
			lock (_syncRoot)
			{
				if (_currentDirectoryIndex == -1)
				{
					if (_currentDirectoryList.Length == 0)
						return false;
					_currentDirectoryIndex = 0;
					return true;
				}
				
				_lastErrorDirectory = null;
				_currentDirectory = null;
				_currentFileList = null;
				_lastIterationError = null;
				
				try
				{
					DirectoryInfo directoryInfo;
					if ((_maxRecurseDepth < 0 || _directoryIndexStack.Count < _maxRecurseDepth) && (directoryInfo = AssertCurrentDirectory()) != null && directoryInfo.Exists)
					{
						DirectoryInfo[] diList = (_directorySearchPattern.Length == 0) ? directoryInfo.GetDirectories() : directoryInfo.GetDirectories(_directorySearchPattern, SearchOption.TopDirectoryOnly);
						if (diList.Length > 0)
						{
							_directoryIndexStack.Push(_currentDirectoryIndex);
							_currentDirectoryIndex = 0;
							_directoryListStack.Push(_currentDirectoryList);
							List<string> currentDirectoryList = new List<string>();
							foreach (DirectoryInfo di in diList)
								currentDirectoryList.Add(di.FullName);
							_currentDirectoryList = currentDirectoryList.ToArray();
							return true;
						}
					}
				}
				catch (Exception exception)
				{
					_lastIterationError = exception;
					_lastErrorDirectory = _currentDirectoryList[_currentDirectoryIndex];
				}
				
				_currentDirectoryIndex++;
				while (_currentDirectoryIndex == _currentDirectoryList.Length)
				{
					if (_directoryIndexStack.Count == 0)
						return false;
					_currentDirectoryIndex = _directoryIndexStack.Pop();
					_currentDirectoryIndex++;
					_currentDirectoryList = _directoryListStack.Pop();
				}
			}
			
			return true;
		}
		
        /// <summary>
        /// Resets iterator.
        /// </summary>
		public void Reset()
		{
			lock (_syncRoot)
			{
				_lastErrorDirectory = null;
				_lastIterationError = null;
				_currentDirectory = null;
				_currentFileList = null;
				_currentDirectoryIndex = -1;
				_directoryIndexStack.Clear();
				_directoryListStack.Clear();
				if (Directory.Exists(_startDirectory))
					_currentDirectoryList = new string[] { _startDirectory };
				else
					_currentDirectoryList = new string[0];
			}
		}

		System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema() { return null; }
		
        /// <summary>
        /// Deserialize properties of current <see cref="DirectoryCrawlIterator" /> from XML data.
        /// </summary>
		public void ReadXml(XmlReader reader)
		{
			reader.MoveToContent();
			_maxRecurseDepth = XmlConvert.ToInt32(reader.GetAttribute(AttributeName_MaxRecurseDepth));
			_currentDirectoryIndex = XmlConvert.ToInt32(reader.GetAttribute(AttributeName_CurrentDirectoryIndex));
			
			if (!reader.Read())
				throw new Exception("Unexpected end of XML");
			
			do
			{
				while (reader.NodeType != XmlNodeType.Element && reader.NodeType != XmlNodeType.EndElement)
				{
					if (!reader.Read())
						throw new Exception("Unexpected end of XML");
				}
				
				if (reader.NodeType == XmlNodeType.EndElement)
					break;
				
				_lastErrorDirectory = "";
				_currentDirectoryList = new string[0];
				_directoryListStack.Clear();
				_directoryIndexStack.Clear();
				switch (reader.Name)
				{
					case ElementName_StartDirectory:
						_startDirectory = reader.ReadElementContentAsString();
						break;
						
					case ElementName_FileSearchPattern:
						_fileSearchPattern = reader.ReadElementContentAsString();
						break;
						
					case ElementName_DirectorySearchPattern:
						_directorySearchPattern = reader.ReadElementContentAsString();
						break;
						
					case ElementName_LastErrorDirectory:
						_lastErrorDirectory = reader.ReadElementContentAsString();
						break;
						
					case ElementName_lastIterationError:
						using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(reader.ReadElementContentAsString().Trim())))
						{
							BinaryFormatter formatter = new BinaryFormatter();
							_lastIterationError = formatter.Deserialize(stream) as Exception;
						}
						break;
						
					case ElementName_CurrentDirectoryList:
						List<string> currentDirectoryList = new List<string>();
						reader.Read();
						while (reader.NodeType != XmlNodeType.EndElement)
						{
							while (reader.NodeType != XmlNodeType.Element && reader.NodeType != XmlNodeType.EndElement)
							{
								if (!reader.Read())
									throw new Exception("Unexpected end of XML");
							}
							
							if (reader.NodeType == XmlNodeType.EndElement)
								break;
							
							if (reader.Name == ElementName_Item)
								currentDirectoryList.Add(reader.ReadElementContentAsString());
							else if (!reader.Read())
								break;
						}
						if (reader.NodeType == XmlNodeType.EndElement)
							reader.Read();
						_currentDirectoryList = currentDirectoryList.ToArray();
						break;
						
					case ElementName_DirectoryStack:
						reader.Read();
						while (reader.NodeType != XmlNodeType.EndElement)
						{
							while (reader.NodeType != XmlNodeType.Element && reader.NodeType != XmlNodeType.EndElement)
							{
								if (!reader.Read())
									throw new Exception("Unexpected end of XML");
							}
							
							if (reader.NodeType == XmlNodeType.EndElement)
								break;
							if (reader.Name == ElementName_Stack)
							{
								_directoryIndexStack.Push(XmlConvert.ToInt32(reader.GetAttribute(AttributeName_Index)));
								List<string> names = new List<string>();
								if (!reader.Read())
								{
									_directoryIndexStack.Pop();
									break;
								}
									
								while (reader.NodeType != XmlNodeType.EndElement)
								{
									while (reader.NodeType != XmlNodeType.Element && reader.NodeType != XmlNodeType.EndElement)
									{
										if (!reader.Read())
											throw new Exception("Unexpected end of XML");
									}
									
									if (reader.NodeType == XmlNodeType.EndElement)
										break;
									if (reader.Name == ElementName_Item)
										names.Add(reader.ReadElementContentAsString());
									else if (!reader.Read())
										break;
								}
								
								if (reader.NodeType == XmlNodeType.EndElement)
									reader.Read();
								
								_directoryListStack.Push(names.ToArray());
							}
							
							if (!reader.Read())
								break;
						}
						if (reader.NodeType == XmlNodeType.EndElement)
							reader.Read();
						break;
				}
			} while (reader.NodeType != XmlNodeType.EndElement);
			
			if (reader.NodeType == XmlNodeType.EndElement)
				reader.Read();
		}
		
        /// <summary>
        /// Serialize properties of current <see cref="DirectoryCrawlIterator" /> to XML data.
        /// </summary>
		public void WriteXml(XmlWriter writer)
		{
			lock (_syncRoot)
			{
				writer.WriteAttributeString(AttributeName_MaxRecurseDepth, XmlConvert.ToString(_maxRecurseDepth));
				writer.WriteAttributeString(AttributeName_CurrentDirectoryIndex, XmlConvert.ToString(_currentDirectoryIndex));
				writer.WriteElementString(ElementName_StartDirectory, _startDirectory);
				writer.WriteElementString(ElementName_FileSearchPattern, _fileSearchPattern);
				writer.WriteElementString(ElementName_DirectorySearchPattern, _directorySearchPattern);
				
				if (_lastErrorDirectory != null)
					writer.WriteElementString(ElementName_LastErrorDirectory, _lastErrorDirectory);
				
				writer.WriteStartElement(ElementName_CurrentDirectoryList);
				try
				{
					foreach (string s in _currentDirectoryList)
						writer.WriteElementString(ElementName_Item, s);
				}
				finally { writer.WriteEndElement(); }
				
				writer.WriteStartElement(ElementName_DirectoryStack);
				try
				{
					int[] indexes = _directoryIndexStack.ToArray();
					string[][] names = _directoryListStack.ToArray();
					for (int i = indexes.Length - 1; i > -1; i--)
					{
						writer.WriteStartElement(ElementName_Stack);
						try
						{
							writer.WriteAttributeString(AttributeName_Index, XmlConvert.ToString(indexes[i]));
							foreach (string s in names[i])
								writer.WriteElementString(ElementName_Item, s);
						}
						finally { writer.WriteEndElement(); }
					}
				}
				finally { writer.WriteEndElement(); }
				
				if (_lastIterationError == null)
					return;
				
				writer.WriteStartElement(ElementName_lastIterationError);
				try
				{
					writer.WriteAttributeString("Type", _lastIterationError.GetType().FullName);
					try { writer.WriteAttributeString("Message", _lastIterationError.Message); } catch { }
					try
					{
						if (_lastIterationError.InnerException != null)
						{
							writer.WriteAttributeString("InnerExceptionType", _lastIterationError.GetType().FullName);
							try { writer.WriteAttributeString("InnerExceptionMessage", _lastIterationError.Message); } catch { }
						}
					} catch { }
					using (MemoryStream stream = new MemoryStream())
					{
						BinaryFormatter formatter = new BinaryFormatter();
						formatter.Serialize(stream, _lastIterationError);
						writer.WriteCData(Convert.ToBase64String(stream.ToArray(), Base64FormattingOptions.InsertLineBreaks));
					}
				}
				finally { writer.WriteEndElement(); }
			}
		}
	}
}

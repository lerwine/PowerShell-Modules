using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace FileSystemIndexLib
{
    [Serializable]
    [XmlRoot(ElementName = ElementName_IterationState, Namespace = XmlSerialization_NamespaceUri)]
    public class IterationState
    {
        #region Fields

        private object _syncRoot = new object();
        private int _index = -1;
        private int _fileIndex = -1;
        private string[] _fileNames = new string[0];
        private string[] _subdirectoryNames = new string[0];
        private string _path = "";
        private IterationState _parent = null;
        [NonSerialized]
        private object _currentFile = null;
        [NonSerialized]
        private object _currentDirectory = null;
        private Exception _error = null;

        #endregion

        #region Constant definitions for serialization nodes

        /// <summary>
        /// Attribute name use when serializing the <see cref="Index"/> property to XML.
        /// </summary>
        public const string AttributeName_Index = "Index";

        /// <summary>
        /// Attribute name use when serializing the <see cref="FileIndex"/> property to XML.
        /// </summary>
        public const string AttributeName_FileIndex = "FileIndex";

        /// <summary>
        /// Attribute name which will contain the type name of <see cref="Error"/> if it is not null. This will also contain the type of the <seealso cref="Exception.InnerException"/> if it is not null.
        /// </summary>
        public const string AttributeName_Type = "Type";

        /// <summary>
        /// XML Namespace URI used for XML serialization.
        /// </summary>
        public const string XmlSerialization_NamespaceUri = "urn:Erwine.Leonard.T:PowerShellModules/FileSystemIndex/IterationState.xsd";

        /// <summary>
        /// Root XML element name when serializing the root <see cref="IterationState"/> object.
        /// </summary>
        public const string ElementName_IterationState = "IterationState";

        /// <summary>
        /// Element name for serializing the <see cref="FileNames"/> property.
        /// </summary>
        public const string ElementName_FileNames = "FileNames";

        /// <summary>
        /// Element name for serializing individual elements within the <see cref="FileNames"/> and <see cref="SubdirectoryNames"/> properties.
        /// </summary>
        public const string ElementName_Name = "Name";

        /// <summary>
        /// Element name for serializing the <see cref="SubdirectoryNames"/> property.
        /// </summary>
        public const string ElementName_SubdirectoryNames = "SubdirectoryNames";

        /// <summary>
        /// Element name for serializing the <see cref="Path"/> property.
        /// </summary>
        public const string ElementName_Path = "Path";

        /// <summary>
        /// Element name for serializing the <see cref="Parent"/> property.
        /// </summary>
        public const string ElementName_Parent = "Parent";

        /// <summary>
        /// Element name for serializing the <see cref="Error"/> property.
        /// </summary>
        public const string ElementName_Error = "Error";

        /// <summary>
        /// Element name for serializing the <seealso cref="Exception.Message"/> property of the <see cref="Error"/> property as well as the <seealso cref="Exception.Message"/> property of its <seealso cref="Exception.InnerException"/> if it is not null.
        /// </summary>
        public const string ElementName_Message = "Message";

        /// <summary>
        /// Element name for serializing the <seealso cref="Exception.InnerException"/> property of the <see cref="Error"/> property.
        /// </summary>
        public const string ElementName_InnerException = "InnerException";

        /// <summary>
        /// Element name for storing the serialized the binary data (in base-64 format) of <seealso cref="Error"/>, which can be used to deserialize the object.
        /// </summary>
        public const string ElementName_Data = "Data";

        #endregion

        #region Properties

        /// <summary>
        /// Index of item within <see cref="SubdirectoryNames"/> of <see cref="Parent"/> which was used to create the value for the <see cref="CurrentDirectoryInfo"/> property.
        /// </summary>
        [XmlAttribute(AttributeName = AttributeName_Index)]
        public int Index
        {
            get { return _index; }
            set
            {
                lock (_syncRoot)
                    _index = value;
            }
        }

        /// <summary>
        /// Index of item within <see cref="FileNames"/> which was used to create the value for the <see cref="CurrentFileInfo"/> property.
        /// </summary>
        [XmlAttribute(AttributeName = AttributeName_FileIndex)]
        public int FileIndex
        {
            get { return _fileIndex; }
            set
            {
                lock (_syncRoot)
                    _fileIndex = value;
            }
        }

        /// <summary>
        /// Names of files within the currently iterated subdirectory represented by <see cref="CurrentDirectoryInfo"/>.
        /// </summary>
        [XmlArray(ElementName = ElementName_IterationState, Namespace = XmlSerialization_NamespaceUri)]
        [XmlArrayItem(ElementName = ElementName_Name, Namespace = XmlSerialization_NamespaceUri)]
        public string[] FileNames
        {
            get { return _fileNames ?? new string[0]; }
            set { _fileNames = value ?? new string[0]; }
        }

        /// <summary>
        /// Names of nested subdirectories contained by the currently iterated subdirectory.
        /// </summary>
        [XmlArray(ElementName = ElementName_SubdirectoryNames, Namespace = XmlSerialization_NamespaceUri)]
        [XmlArrayItem(ElementName = ElementName_Name, Namespace = XmlSerialization_NamespaceUri)]
        public string[] SubdirectoryNames
        {
            get { return _subdirectoryNames ?? new string[0]; }
            set { _subdirectoryNames = value ?? new string[0]; }
        }

        /// <summary>
        /// Path or name of currently iterated subdirectory.
        /// </summary>
        /// <remarks>If <see cref="Parent"/> is null, then this wil contain the full path. Otherwise, this will contain just the directory name.</remarks>
        [XmlElement(ElementName = ElementName_Path, Namespace = XmlSerialization_NamespaceUri)]
        public string Path
        {
            get { return _path; }
            set
            {
                string s = value ?? "";

                lock (_syncRoot)
                {
                    if (s == _path || _parent == null)
                        return;

                    if (_path.Length > 0)
                        throw new InvalidOperationException("Path cannot be changed once it has been set.");

                    if (_parent._subdirectoryNames.Contains(s))
                        throw new InvalidOperationException("Parent does not contain a subdirectory with this name.");

                    _path = s;
                }
            }
        }

        /// <summary>
        /// Represents parent subdirectory in current iteration.
        /// </summary>
        [XmlElement(ElementName = ElementName_Path, Namespace = XmlSerialization_NamespaceUri, IsNullable = true)]
        public IterationState Parent
        {
            get { return _parent; }
            set
            {
                lock (_syncRoot)
                {
                    if (_path == "" || (value == null) ? _parent == null : _parent != null && ReferenceEquals(_parent, value))
                        return;

                    if (_parent != null)
                        throw new InvalidOperationException("Cannot change parent object once it has been set.");

                    if (!_parent._fileNames.Contains(_path))
                        throw new InvalidOperationException("Parent does not contain a subdirectory with this name.");

                    _parent = value;
                }
            }
        }

        /// <summary>
        /// This will contain any exception encountered while attempting to iterate to the current item.
        /// </summary>
        [XmlIgnore]
        public Exception Error { get { return _error; } }

        /// <summary>
        /// This property is intended to support serialization of the error and is not intended to be used directly.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [XmlElement(ElementName = ElementName_Error, Namespace = XmlSerialization_NamespaceUri, IsNullable = true)]
        public XmlElement _Error
        {
            get
            {
                Exception exception = Error;
                if (exception == null)
                    return null;

                XmlDocument xmlDocument = new XmlDocument();
                XmlElement xmlElement = xmlDocument.AppendChild(xmlDocument.CreateElement(ElementName_Error, XmlSerialization_NamespaceUri)) as XmlElement;
                xmlElement.Attributes.Append(xmlDocument.CreateAttribute(AttributeName_Type)).Value = exception.GetType().FullName;
                string message;
                try { message = exception.Message; } catch { message = null; }
                if (message != null)
                    xmlElement.AppendChild(xmlDocument.CreateElement(ElementName_Message, XmlSerialization_NamespaceUri)).AppendChild(xmlDocument.CreateTextNode(message));
                if (exception.InnerException != null)
                {
                    XmlElement innerElement = xmlElement.AppendChild(xmlDocument.CreateElement(ElementName_InnerException, XmlSerialization_NamespaceUri)) as XmlElement;
                    innerElement.Attributes.Append(xmlDocument.CreateAttribute(AttributeName_Type)).Value = exception.InnerException.GetType().FullName;
                    try { message = exception.InnerException.Message; } catch { message = null; }
                    if (message != null)
                        innerElement.AppendChild(xmlDocument.CreateElement(ElementName_Message, XmlSerialization_NamespaceUri)).AppendChild(xmlDocument.CreateTextNode(message));
                }
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(memoryStream, exception);
                    xmlElement.AppendChild(xmlDocument.CreateElement(ElementName_Data, XmlSerialization_NamespaceUri)).AppendChild(xmlDocument.CreateCDataSection(Convert.ToBase64String(memoryStream.ToArray(), Base64FormattingOptions.InsertLineBreaks)));
                }
                // TODO: Serialize error
#warning Not implemented
                throw new NotImplementedException();
            }
            set
            {
                // TODO: Deserialize error
#warning Not implemented
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Currently iterated file.
        /// </summary>
        /// <remarks>This value may be null if the current iteration is an empty subdirectory or if there was an error getting details of the currently iterated file.</remarks>
        [XmlIgnore]
        public FileInfo CurrentFileInfo
        {
            get
            {
                lock (_syncRoot)
                    return AssertCurrentFile() as FileInfo;
            }
        }

        /// <summary>
        /// Contains any exception that was thrown while iterating to the current file.
        /// </summary>
        [XmlIgnore]
        public Exception CurrentFileError
        {
            get
            {
                lock (_syncRoot)
                    return AssertCurrentFile() as Exception;
            }
        }

        /// <summary>
        /// Currently iterated subdirectory.
        /// </summary>
        /// <remarks>This value may be null if the end of iteration has been reached or if there was an error getting details of the currently iterated subdirectory.</remarks>
        [XmlIgnore]
        public DirectoryInfo CurrentDirectoryInfo
        {
            get
            {
                lock (_syncRoot)
                    return AssertCurrentDirectory() as DirectoryInfo;
            }
        }

        /// <summary>
        /// Contains any exception that was thrown while iterating to the current file.
        /// </summary>
        [XmlIgnore]
        public Exception CurrentDirectoryError
        {
            get
            {
                lock (_syncRoot)
                    return AssertCurrentDirectory() as Exception;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of <see cref="IterationState"/> to represent the root of a subdirectory iteration.
        /// </summary>
        /// <param name="path">Root path to be iterated</param>
        public IterationState(string path)
        {
#warning Not implemented
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates an empty instance of <see cref="IterationState"/>.
        /// </summary>
        public IterationState() { }

        private object AssertCurrentDirectory()
        {
            if (_currentDirectory != null)
                return _currentDirectory;

            DirectoryInfo directoryInfo = null;
            if (_fileIndex < 0 || _fileIndex >= _fileNames.Length || (Parent != null && (directoryInfo = Parent.CurrentDirectoryInfo) == null))
                _currentDirectory = new object();
            else
            {
                try
                {
                    _currentDirectory = new DirectoryInfo((directoryInfo == null) ? _path : System.IO.Path.Combine(directoryInfo.FullName, _path));
                }
                catch (Exception exception)
                {
                    _currentDirectory = exception;
                    _error = exception;
                }
            }

            return _currentDirectory;
        }

        private object AssertCurrentFile()
        {
            if (_currentFile != null)
                return _currentFile;

            DirectoryInfo directoryInfo;
            if (_fileIndex < 0 || _fileIndex >= _fileNames.Length || (directoryInfo = CurrentDirectoryInfo) == null)
                _currentFile = new object();
            else
            {
                try
                {
                    _currentFile = new FileInfo(System.IO.Path.Combine(directoryInfo.FullName, _fileNames[_fileIndex]));
                }
                catch (Exception exception)
                {
                    _currentFile = exception;
                    _error = exception;
                }
            }

            return _currentFile;
        }

        /// <summary>
        /// Gets full name of currently iterated subdirectory.
        /// </summary>
        /// <returns>Full name of currently iterated subdirectory.</returns>
        public string GetDirectoryFullName()
        {
            if (_path.Length == 0)
                throw new InvalidOperationException("Path not defined.");

            return (Parent == null) ? _path : System.IO.Path.Combine(Parent.GetDirectoryFullName(), _path);
        }
        
        /// <summary>
        /// Attempts to reload information for the currently iterated item.
        /// </summary>
        /// <returns>True if information was successfully refreshed or false if there was an error.</returns>
        public bool TryRefresh()
        {
#warning Not implemented
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Iterates to next matching filesystem item.
        /// </summary>
        /// <param name="options">Defines options for filtering iterated items.</param>
        /// <param name="directoryChanged">This gets set to true if the iteration resulted a change to the <see cref="CurrentDirectoryInfo"/>.</param>
        /// <returns>True if there was a following item to iterate to; otherwise false to indicate end of iteration.</returns>
        /// <remarks>This will always advance to the next iteration, even if an exception was encountered.</remarks>
        public bool TryMoveNext(IterationOptions options, out bool directoryChanged)
        {
#warning Not implemented
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Iterates to next filesystem item.
        /// </summary>
        /// <param name="directoryChanged">This gets set to true if the iteration resulted a change to the <see cref="CurrentDirectoryInfo"/>.</param>
        /// <returns>True if there was a following item to iterate to; otherwise false to indicate end of iteration.</returns>
        /// <remarks>This will always advance to the next iteration, even if an exception was encountered.</remarks>
        public bool TryMoveNext(out bool directoryChanged)
        {
#warning Not implemented
            throw new System.NotImplementedException();
        }
    }
}
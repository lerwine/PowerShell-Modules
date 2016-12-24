$Script:DbPath = 'C:\Users\lerwi\Documents\FileCatalog.sdf';
$Script:ProviderFactory = [System.Data.Common.DbProviderFactories]::GetFactory('System.Data.SqlServerCe.4.0');
Add-Type -Path $Script:ProviderFactory.GetType().Assembly.Location;
Add-Type -TypeDefinition @'
namespace DupCheckCLR
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlServerCe;
    using System.Text;
    
    [Serializable()]
    public class VolumeInformation : IEquatable<VolumeInformation>
    {
        private string _rootPathName = "";
        private string _volumeName = "";
        private string _fileSystemName = "";
        private uint _volumeSerialNumber = 0;
        private uint _maximumComponentLength = 0;
        private FileSystemFeature _fileSystemFlags = FileSystemFeature.None;
        
        /// <summary>
        /// The root directory of the volume.
        /// </summary>
        public string RootPathName
        {
            get { return _rootPathName; }
            set { _rootPathName = value ?? ""; }
        }
        
        /// <summary>
        /// The name of the volume.
        /// </summary>
        public string VolumeName
        {
            get { return _volumeName; }
            set { _volumeName = value ?? ""; }
        }
        
        /// <summary>
        /// The name of the file system.
        /// </summary>
        public string FileSystemName
        {
            get { return _fileSystemName; }
            set { _fileSystemName = value ?? ""; }
        }
        
        /// <summary>
        /// Serial number assigned when operating system formatted volume.
        /// </summary>
        public uint VolumeSerialNumber
        {
            get { return _volumeSerialNumber; }
            set { _volumeSerialNumber = value; }
        }

        /// <summary>
        /// The maximum length, in TCHARs, of a file name component that the file system supports.
        /// </summary>
        public uint MaximumComponentLength
        {
            get { return _maximumComponentLength; }
            set { _maximumComponentLength = value; }
        }
        
        /// <summary>
        /// Flags describing the capabilities of the filesystem.
        /// </summary>
        public FileSystemFeature FileSystemFlags
        {
            get { return _fileSystemFlags; }
            set { _fileSystemFlags = value; }
        }

        private void SetFlag(FileSystemFeature flag, bool value)
        {
            if (value)
                _fileSystemFlags |= flag;
            else
                _fileSystemFlags &= ~flag;
        }

        /// <summary>
        /// The file system preserves the case of file names when it places a name on disk.
        /// </summary>
        public bool CasePreservedNames
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.CasePreservedNames); }
            set { SetFlag(FileSystemFeature.CasePreservedNames, value); }
        }

        /// <summary>
        /// The file system supports case-sensitive file names.
        /// </summary>
        public bool CaseSensitiveSearch
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.CaseSensitiveSearch); }
            set { SetFlag(FileSystemFeature.CaseSensitiveSearch, value); }
        }

        /// <summary>
        /// The specified volume is a direct access (DAX) volume. This flag was introduced in Windows 10, version 1607.
        /// </summary>
        public bool DaxVolume
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.DaxVolume); }
            set { SetFlag(FileSystemFeature.DaxVolume, value); }
        }

        /// <summary>
        /// The file system supports file-based compression.
        /// </summary>
        public bool FileCompression
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.FileCompression); }
            set { SetFlag(FileSystemFeature.FileCompression, value); }
        }

        /// <summary>
        /// The file system supports named streams.
        /// </summary>
        public bool NamedStreams
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.NamedStreams); }
            set { SetFlag(FileSystemFeature.NamedStreams, value); }
        }

        /// <summary>
        /// The file system preserves and enforces access control lists (ACL).
        /// </summary>
        public bool PersistentACLS
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.PersistentACLS); }
            set { SetFlag(FileSystemFeature.PersistentACLS, value); }
        }
        
        /// <summary>
        /// The specified volume is read-only.
        /// </summary>
        public bool ReadOnlyVolume
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.ReadOnlyVolume); }
            set { SetFlag(FileSystemFeature.ReadOnlyVolume, value); }
        }

        /// <summary>
        /// The volume supports a single sequential write.
        /// </summary>
        public bool SequentialWriteOnce
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.SequentialWriteOnce); }
            set { SetFlag(FileSystemFeature.SequentialWriteOnce, value); }
        }

        /// <summary>
        /// The file system supports the Encrypted File System (EFS).
        /// </summary>
        public bool SupportsEncryption
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.SupportsEncryption); }
            set { SetFlag(FileSystemFeature.SupportsEncryption, value); }
        }

        /// <summary>
        /// The specified volume supports extended attributes. An extended attribute is a piece of
        /// application-specific metadata that an application can associate with a file and is not part
        /// of the file's data.
        /// </summary>
        public bool SupportsExtendedAttributes
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.SupportsExtendedAttributes); }
            set { SetFlag(FileSystemFeature.SupportsExtendedAttributes, value); }
        }

        /// <summary>
        /// The specified volume supports hard links. For more information, see Hard Links and Junctions.
        /// </summary>
        public bool SupportsHardLinks
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.SupportsHardLinks); }
            set { SetFlag(FileSystemFeature.SupportsHardLinks, value); }
        }

        /// <summary>
        /// The file system supports object identifiers.
        /// </summary>
        public bool SupportsObjectIDs
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.SupportsObjectIDs); }
            set { SetFlag(FileSystemFeature.SupportsObjectIDs, value); }
        }

        /// <summary>
        /// The file system supports open by FileID. For more information, see FILE_ID_BOTH_DIR_INFO.
        /// </summary>
        public bool SupportsOpenByFileId
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.SupportsOpenByFileId); }
            set { SetFlag(FileSystemFeature.SupportsOpenByFileId, value); }
        }

        /// <summary>
        /// The file system supports re-parse points.
        /// </summary>
        public bool SupportsReparsePoints
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.SupportsReparsePoints); }
            set { SetFlag(FileSystemFeature.SupportsReparsePoints, value); }
        }

        /// <summary>
        /// The file system supports sparse files.
        /// </summary>
        public bool SupportsSparseFiles
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.SupportsSparseFiles); }
            set { SetFlag(FileSystemFeature.SupportsSparseFiles, value); }
        }

        /// <summary>
        /// The volume supports transactions.
        /// </summary>
        public bool SupportsTransactions
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.SupportsTransactions); }
            set { SetFlag(FileSystemFeature.SupportsTransactions, value); }
        }

        /// <summary>
        /// The specified volume supports update sequence number (USN) journals. For more information,
        /// see Change Journal Records.
        /// </summary>
        public bool SupportsUsnJournal
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.SupportsUsnJournal); }
            set { SetFlag(FileSystemFeature.SupportsUsnJournal, value); }
        }

        /// <summary>
        /// The file system supports Unicode in file names as they appear on disk.
        /// </summary>
        public bool UnicodeOnDisk
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.UnicodeOnDisk); }
            set { SetFlag(FileSystemFeature.UnicodeOnDisk, value); }
        }

        /// <summary>
        /// The specified volume is a compressed volume, for example, a DoubleSpace volume.
        /// </summary>
        public bool VolumeIsCompressed
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.VolumeIsCompressed); }
            set { SetFlag(FileSystemFeature.VolumeIsCompressed, value); }
        }

        /// <summary>
        /// The file system supports disk quotas.
        /// </summary>
        public bool VolumeQuotas
        {
            get { return _fileSystemFlags.HasFlag(FileSystemFeature.VolumeQuotas); }
            set { SetFlag(FileSystemFeature.VolumeQuotas, value); }
        }

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool GetVolumeInformation(string rootPathName, StringBuilder volumeNameBuffer, int volumeNameSize, out uint volumeSerialNumber,
            out uint maximumComponentLength, out FileSystemFeature fileSystemFlags, StringBuilder fileSystemNameBuffer, int nFileSystemNameSize);

        public VolumeInformation() { }

        public VolumeInformation(string rootPathName)
        {
            if (rootPathName == null)
                throw new ArgumentNullException("rootPathName");

            if (rootPathName.Length == 0)
                throw new ArgumentException("Root path name cannot be empty.", "rootPathName");

            RootPathName = rootPathName;
            Refresh();
        }

        public void Refresh()
        {
            if (RootPathName.Length == 0)
                throw new InvalidOperationException("Root path name is empty.");

            StringBuilder volumeNameBuffer = new StringBuilder(261);
            StringBuilder fileSystemNameBuffer = new StringBuilder(261);
            uint volumeSerialNumber, maximumComponentLength;
            FileSystemFeature fileSystemFlags;
            if (!GetVolumeInformation(RootPathName, volumeNameBuffer, volumeNameBuffer.Capacity, out volumeSerialNumber, out maximumComponentLength,
                    out fileSystemFlags, fileSystemNameBuffer, fileSystemNameBuffer.Capacity))
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

            VolumeName = volumeNameBuffer.ToString();
            FileSystemName = fileSystemNameBuffer.ToString();
            VolumeSerialNumber = volumeSerialNumber;
            MaximumComponentLength = maximumComponentLength;
            FileSystemFlags = fileSystemFlags;
        }

        public override string ToString() { return String.Format("{0:X4}-{1:X4}", _volumeSerialNumber >> 16, _volumeSerialNumber & 0x0000FFFF); }

        public override int GetHashCode() { return _volumeSerialNumber.GetHashCode(); }

        public bool Equals(VolumeInformation other)
        {
            return other != null && (ReferenceEquals(this, other) || (_volumeSerialNumber == other._volumeSerialNumber && _fileSystemFlags == other._fileSystemFlags &&
                _maximumComponentLength == other._maximumComponentLength && _rootPathName == other._rootPathName && _volumeName == other._volumeName &&
                _fileSystemName == other._fileSystemName));
        }

        public override bool Equals(object obj) { return Equals(obj as VolumeInformation); }
    }

    [Flags]
    public enum FileSystemFeature : uint
    {
        /// <summary>
        /// Indicates that no flags are set.
        /// </summary>
        None = 0,

        /// <summary>
        /// The file system preserves the case of file names when it places a name on disk.
        /// </summary>
        CasePreservedNames = 2,

        /// <summary>
        /// The file system supports case-sensitive file names.
        /// </summary>
        CaseSensitiveSearch = 1,

        /// <summary>
        /// The specified volume is a direct access (DAX) volume. This flag was introduced in Windows 10, version 1607.
        /// </summary>
        DaxVolume = 0x20000000,

        /// <summary>
        /// The file system supports file-based compression.
        /// </summary>
        FileCompression = 0x10,

        /// <summary>
        /// The file system supports named streams.
        /// </summary>
        NamedStreams = 0x40000,

        /// <summary>
        /// The file system preserves and enforces access control lists (ACL).
        /// </summary>
        PersistentACLS = 8,

        /// <summary>
        /// The specified volume is read-only.
        /// </summary>
        ReadOnlyVolume = 0x80000,

        /// <summary>
        /// The volume supports a single sequential write.
        /// </summary>
        SequentialWriteOnce = 0x100000,

        /// <summary>
        /// The file system supports the Encrypted File System (EFS).
        /// </summary>
        SupportsEncryption = 0x20000,

        /// <summary>
        /// The specified volume supports extended attributes. An extended attribute is a piece of
        /// application-specific metadata that an application can associate with a file and is not part
        /// of the file's data.
        /// </summary>
        SupportsExtendedAttributes = 0x00800000,

        /// <summary>
        /// The specified volume supports hard links. For more information, see Hard Links and Junctions.
        /// </summary>
        SupportsHardLinks = 0x00400000,

        /// <summary>
        /// The file system supports object identifiers.
        /// </summary>
        SupportsObjectIDs = 0x10000,

        /// <summary>
        /// The file system supports open by FileID. For more information, see FILE_ID_BOTH_DIR_INFO.
        /// </summary>
        SupportsOpenByFileId = 0x01000000,

        /// <summary>
        /// The file system supports re-parse points.
        /// </summary>
        SupportsReparsePoints = 0x80,

        /// <summary>
        /// The file system supports sparse files.
        /// </summary>
        SupportsSparseFiles = 0x40,

        /// <summary>
        /// The volume supports transactions.
        /// </summary>
        SupportsTransactions = 0x200000,

        /// <summary>
        /// The specified volume supports update sequence number (USN) journals. For more information,
        /// see Change Journal Records.
        /// </summary>
        SupportsUsnJournal = 0x02000000,

        /// <summary>
        /// The file system supports Unicode in file names as they appear on disk.
        /// </summary>
        UnicodeOnDisk = 4,

        /// <summary>
        /// The specified volume is a compressed volume, for example, a DoubleSpace volume.
        /// </summary>
        VolumeIsCompressed = 0x8000,

        /// <summary>
        /// The file system supports disk quotas.
        /// </summary>
        VolumeQuotas = 0x20
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct HashCodeEx : IEquatable<HashCodeEx>
    {
        [FieldOffset(0)]
        private int _sha256HashCode;
        [FieldOffset(0)]
        private ushort _w0;
        [FieldOffset(0)]
        private long _longHashCode;
        [FieldOffset(16)]
        private ushort _w1;
        [FieldOffset(32)]
        private long _length;
        [FieldOffset(32)]
        private ushort _w2;
        [FieldOffset(48)]
        private ushort _w3;
        
        public int Sha256HashCode { get { return _sha256HashCode; } }
        public long LongHashCode { get { return _longHashCode; } }
        public long Length { get { return _length; } }

        public HashCodeEx(int sha256HashCode, long length)
        {
            _w0 = 0;
            _w1 = 0;
            _w2 = 0;
            _w3 = 0;
            _longHashCode = 0;
            _sha256HashCode = sha256HashCode;
            _length = length;
        }

        public override string ToString() { return String.Format("{0:X4}-{1:X4}-{2:X4}-{3:X4}", _w0, _w1, _w2, _w3); }

        public override int GetHashCode() { return _sha256HashCode; }

        public bool Equals(HashCodeEx other)
        {
            return _sha256HashCode == other._sha256HashCode && _length == other._length;
        }

        public override bool Equals(object obj) { return obj != null && obj is HashCodeEx && Equals((HashCodeEx)obj); }
    }

    public class FileInfoEx : IEquatable<FileInfoEx>
    {
        private string _name = "";
        private string _directoryName = "";
        private DateTime _creationTime;
        private DateTime _lastWriteTime;
        private HashCodeEx _hashCodeEx;
         
        public string Name
        {
            get { return _name; }
            set { _name = value ?? ""; }
        }
        
        public string DirectoryName
        {
            get { return _directoryName; }
            set { _directoryName = value ?? ""; }
        }
        
        public long Length
        {
            get { return _hashCodeEx.Length; }
            set
            {
                if (_hashCodeEx.Length != value)
                    _hashCodeEx = new HashCodeEx(_hashCodeEx.Sha256HashCode, value);
            }
        }
        
        public DateTime CreationTime
        {
            get { return _creationTime; }
            set { _creationTime = value; }
        }

        public DateTime LastWriteTime
        {
            get { return _lastWriteTime; }
            set { _lastWriteTime = value; }
        }

        public int Sha256HashCode
        {
            get { return _hashCodeEx.Sha256HashCode; }
            set
            {
                if (_hashCodeEx.Sha256HashCode != value)
                    _hashCodeEx = new HashCodeEx(value, _hashCodeEx.Length);
            }
        }

        public long LongHashCode { get { return _hashCodeEx.LongHashCode; } }

        public FileInfoEx() { }
        
        public FileInfoEx(string name, string directoryName, long length, DateTime creationTime, DateTime lastWriteTime, int sha256HashCode)
        {
            Name = name;
            DirectoryName = directoryName;
            CreationTime = creationTime;
            LastWriteTime = lastWriteTime;
            _hashCodeEx = new HashCodeEx(sha256HashCode, length);
        }

        public FileInfoEx(FileInfo fileInfo, int sha256HashCode)
        {
            if (fileInfo == null)
                throw new ArgumentNullException("fileInfo");

            Name = fileInfo.Name;
            DirectoryName = fileInfo.DirectoryName;
            CreationTime = fileInfo.CreationTime;
            LastWriteTime = fileInfo.LastWriteTime;
            _hashCodeEx = new HashCodeEx(sha256HashCode, fileInfo.Length);
        }

        public static FileInfoEx Create(FileInfo fileInfo, SHA256Managed algorithm)
        {
            if (fileInfo == null)
                throw new ArgumentNullException("fileInfo");

            if (algorithm == null)
                throw new ArgumentNullException("algorithm");

            using (FileStream fileStream = fileInfo.OpenRead())
                return new FileInfoEx(fileInfo, GetChecksum(fileStream, algorithm));
        }
        
        public static FileInfoEx Create(FileInfo fileInfo)
        {
            if (fileInfo == null)
                throw new ArgumentNullException("fileInfo");

            using (FileStream fileStream = fileInfo.OpenRead())
                return new FileInfoEx(fileInfo, GetChecksum(fileStream));
        }

        public override string ToString() { return String.Format("{0}: {1}", _hashCodeEx.ToString(), Path.Combine(_directoryName, _name)); }

        public override int GetHashCode() { return _hashCodeEx.Sha256HashCode; }
        
        public bool Equals(FileInfoEx other)
        {
            return other != null && (ReferenceEquals(this, other) || (_hashCodeEx.Equals(other._hashCodeEx) && _name == other._name &&
                _directoryName == other._directoryName && _creationTime == other._creationTime && _lastWriteTime == other._lastWriteTime));
        }

        public override bool Equals(object obj) { return Equals(obj as FileInfoEx); }

        public static SHA256Managed GetHashAlgorithm() { return new SHA256Managed(); }
        
        public static int GetChecksum(Stream stream, SHA256Managed algorithm)
        {
            if (algorithm == null)
                throw new ArgumentNullException("algorithm");

            if (stream == null)
                throw new ArgumentNullException("stream");

            byte[] checksum = algorithm.ComputeHash(stream);
            return BitConverter.ToInt32(checksum, 0);
        }

        public static int GetChecksum(string file, SHA256Managed algorithm)
        {
            if (file == null)
                throw new ArgumentNullException("file");
                
            if (file.Length == 0)
                throw new ArgumentException("File name cannot be empty.", "file");

            using (FileStream stream = File.OpenRead(file))
                return GetChecksum(file, algorithm);
        }
        
        public static int GetChecksum(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
                
            using (SHA256Managed algorithm = GetHashAlgorithm())
                return GetChecksum(stream, algorithm);
        }

        public static int GetChecksum(string file)
        {
            using (SHA256Managed algorithm = GetHashAlgorithm())
                return GetChecksum(file, algorithm);
        }

        public static bool HasFlag(FileAttributes value, FileAttributes flag) { return value.HasFlag(flag); }
    }
    
    public abstract class BaseRecord
    {
        public const string ColName_Id = "Id";

        private bool _isNew = true;
        private bool _hasChanges = true;
        private Guid? _id = null;
        
        public Guid Id { get { return (_id.HasValue) ? _id.Value : Guid.Empty; } }
        
        public override int GetHashCode() { return (_id.HasValue) ? 0 : _id.Value.GetHashCode(); }
        
        public bool IsNew() { return _isNew; }

        public bool HasChanges() { return _hasChanges; }
        
        protected void SetChanged() { _hasChanges = true; }
        
        protected virtual OnLoad(SqlCeDataReader dataReader) { }
        
        protected virtual void AddInsertParameters(SqlCeParameterCollection parameters) { }

        protected virtual void AddUpdateParameters(SqlCeParameterCollection parameters) { }
        
        protected abstract string GetTableName();
        
        protected abstract string GetSelectQueryFields();

        protected abstract string GetInsertQueryPlaceHolders();
        
        protected abstract string GetUpdateQueryFieldAndPlaceHolders();

        protected void AddSelectIdQueryField(SqlCeCommand command)
        {
            if (!_id.HasValue)
                _id = Guid.NewGuid();
            command.Parameters.AddWithValue(ColName_Id, _id.Value);
        }

        protected bool SelectById(SqlCeConnection dbConnection, Guid id)
        {
            string sql = String.Format("SELECT [{0}], {1} FROM [{2}] WHERE ([{0}] = @Id)", ColName_Id, GetSelectQueryFields(), GetTableName());
            using (SqlCeCommand command = dbConnection.CreateCommand())
            {
                command.CommandText = sql;
                AddSelectIdQueryField(command);
                using (SqlCeDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.HasRows && dataReader.Read())
                    {
                        OnLoad(dataReader);
                        _id = id;
                        _isNew = false;
                        _hasChanges = false;
                        return true;
                    }
                }
            }
            return false;
        }
        
        protected static bool TryLoad<TResult>(SqlCeCommand command, out List<TResult> result)
            where TResult : BaseRecord, new()
        {
            using (SqlCeDataReader dataReader = command.ExecuteReader())
                return TryLoad<TResult>(dataReader, out result);
        }
        
        protected static bool TryLoad<TResult>(SqlCeDataReader dataReader, out List<TResult> results)
            where TResult : BaseRecord, new()
        {
            results = new List<TResult>();
            
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    TResult item = new TResult();
                    item._id = dataReader.GetGuid(dataReader.GetOrdinal(ColName_Id));
                    item.OnLoad(dataReader);
                    item._isNew = false;
                    item._hasChanges = false;
                    results.Add(item);
                }
            }

            return results.Count > 0;
        }
        
        public void SaveChanges(SqlCeConnection dbConnection)
        {
            string sql = (_isNew) ? String.Format("INSERT INTO [{0}] ({1}, {2}) VALUES (@{1}, {3})", GetTableName(), ColName_Id, GetSelectQueryFields(), GetInsertQueryPlaceHolders()) :
                String.Format("UPDATE [{0}] SET {1} WHERE ({2} = @{2})", GetTableName(), GetUpdateQueryFieldAndPlacdeHolders(), ColName_Id);
            using (SqlCeCommand command = dbConnection.CreateCommand())
            {
                command.CommandText = sql;
                if (_isNew)
                {
                    AddSelectIdQueryField(command);
                    AddInsertParameters(command);
                }
                else
                {
                    AddUpdateParameters(command);
                    AddSelectIdQueryField(command);
                }
                command.ExecuteNonQuery();
            }
            _isNew = false;
            _hasChanges = false;
        }
        
        public void Delete(SqlCeConnection dbConnection)
        {
            string sql = String.Format("DELETE FROM [{0}] WHERE ({1} = @{1})", GetTableName(), ColName_Id);
            using (SqlCeCommand command = dbConnection.CreateCommand())
            {
                AddSelectIdQueryField(command);
                command.ExecuteNonQuery();
            }
            _id = null;
            _isNew = true;
            _hasChanges = true;
        }
    }

    public class VolumeRecord : BaseRecord, IEquatable<VolumeRecord>
    {
        public const string TableName_Volume = "Volume";
        public const string ColName_SerialNumber = "SerialNumber";
        public const string ColName_MaxComponentLen = "MaxComponentLen";
        public const string ColName_FileSystemFlags = "FileSystemFlags";
        public const string ColName_Name = "Name";
        public const string ColName_RootPath = "RootPath";
        public const string ColName_RootPathLC = "RootPathLC";
        public const string ColName_FileSystemName = "FileSystemName";
        public const string ColName_CaseSensitive = "CaseSensitive";
        public const string ColName_ReadOnly = "ReadOnly";

        private uint _serialNumber = 0;
        private uint _maxComponentLen = 0;
        private FileSystemFeature _fileSystemFlags = FileSystemFeature.None;
        private string _name = "";
        private string _rootPath = "";
        private string _fileSystemName = "";
        
        public uint SerialNumber { get { return _serialNumber; } }
        
        public uint MaxComponentLen { get { return _maxComponentLen; } }

        public FileSystemFeature FileSystemFlags { get { return _fileSystemFlags; } }
        
        public string Name { get { return _name; } }

        public string RootPath { get { return _rootPath; } }
        
        public string FileSystemName { get { return _fileSystemName; } }
        
        public bool CaseSensitive { get { return _fileSystemFlags.HasFlag(FileSystemFeature.CaseSensitiveSearch); } }

        public bool ReadOnly { get { return _fileSystemFlags.HasFlagFileSystemFeature.(ReadOnlyVolume); } }
        
        public static VolumeRecord Load(SqlCeConnection dbConnection, Guid id)
        {
            VolumeRecord result = new VolumeRecord();
            if (result.SelectById(dbConnection, id))
                return result;

            return null;
        }
        
        public static VolumeRecord Load(SqlCeConnection dbConnection, uint serialNumber)
        {
            string sql = String.Format("SELECT [{0}], {1} FROM [{2}] WHERE ([{3}] = @{3})", ColName_Id, GetSelectQueryFields(), GetTableName(), ColName_SerialNumber);
            using (SqlCeCommand command = dbConnection.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.AddWithValue(ColName_SerialNumber, serialNumber);
                AddSelectIdQueryField(command);
                using (SqlCeDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.HasRows && dataReader.Read())
                    {
                        VolumeRecord result = new VolumeRecord();
                        OnLoad(dataReader);
                        result._id = dataReader.GetGuid(dataReader.GetOrdinal(ColName_Id));
                        return result;
                    }
                }
            }
            return null;
        }
        
        public static VolumeRecord Load(SqlCeConnection dbConnection, DirectoryInfo directoryInfo)
        {
            VolumeInformation volumeInformation = new VolumeInformation(directoryInfo.Root.FullName);
            return Load(dbConnection, volumeInformation.VolumeSerialNumber);
        }
        
        public void Update(SqlCeConnection dbConnection, VolumeInformation volumeInformation)
        {
            result._maxComponentLen = volumeInformation.MaximumComponentLength;
            result._fileSystemFlags = volumeInformation.FileSystemFlags;
            result._name = volumeInformation.VolumeName;
            result._rootPath = volumeInformation.RootPath;
            result._fileSystemName = volumeInformation.FileSystemName;
            if (result.HasChanges())
                result.SaveChanges(dbConnection);
        }
        
        public static VolumeRecord LoadOrCreate(SqlCeConnection dbConnection, DirectoryInfo directoryInfo)
        {
            VolumeInformation volumeInformation = new VolumeInformation(directoryInfo.Root.FullName);
            VolumeRecord result = Load(dbConnection, volumeInformation.VolumeSerialNumber);
            if (result != null)
            {
                result.Update(dbConnection, volumeInformation);
                return result;
            }

            result = new VolumeRecord();
            result._serialNumber = volumeInformation.VolumeSerialNumber;
            result._maxComponentLen = volumeInformation.MaximumComponentLength;
            result._fileSystemFlags = volumeInformation.FileSystemFlags;
            result._name = volumeInformation.VolumeName;
            result._rootPath = volumeInformation.RootPath;
            result._fileSystemName = volumeInformation.FileSystemName;
            result.SaveChanges(dbConnection);
            return result;
        }
        
        public override string ToString() { return String.Format("{0:X4}-{1:X4} {2}", SerialNumber >> 16, SerialNumber & 0X0000FFFF, _name); }

        public override int GetHashCode() { return SerialNumber.GetHashCode(); }
        
        public bool Equals(VolumeRecord other)
        {
            return other != null && (ReferenceEquals(this, other) || (_serialNumber == other._serialNumber && _maxComponentLen == other._maxComponentLen && 
                _fileSystemFlags == other._fileSystemFlags && _name == other._name && _rootPath == other._rootPath && _fileSystemName == other._fileSystemName &&
                (IsNew()) ? other.IsNew() : !IsNew() && Id.Equals(other.Id)));
        }

        public override bool Equals(object obj) { return Equals(obj as VolumeRecord); }
        
        protected override OnLoad(SqlCeDataReader dataReader)
        {
            _serialNumber = (uint)(dataReader.GetInt64(dataReader.GetOrdinal(ColName_SerialNumber)));
            _maxComponentLen = (uint)(dataReader.GetInt64(dataReader.GetOrdinal(ColName_MaxComponentLen)));
            _fileSystemFlags = (FileSystemFeature)((uint)(dataReader.GetInt64(dataReader.GetOrdinal(ColName_FileSystemFlags))));
            _name = dataReader.GetString(dataReader.GetOrdinal(ColName_Name));
            _rootPath = dataReader.GetString(dataReader.GetOrdinal(ColName_RootPath));
            _fileSystemName = dataReader.GetString(dataReader.GetOrdinal(ColName_FileSystemName));
        }
        
        protected override void AddInsertParameters(SqlCeParameterCollection parameters)
        {
            parameters.AddWithValue(ColName_SerialNumber, (long)(_serialNumber));
            parameters.AddWithValue(ColName_MaxComponentLen, (long)(_maxComponentLen));
            parameters.AddWithValue(ColName_FileSystemFlags, (long)(_fileSystemFlags));
            parameters.AddWithValue(ColName_Name, _name);
            parameters.AddWithValue(ColName_RootPath, _rootPath);
            parameters.AddWithValue(ColName_RootPathLC, _rootPath.ToLower());
            parameters.AddWithValue(ColName_FileSystemName, _fileSystemName);
            parameters.AddWithValue(ColName_CaseSensitive, CaseSensitive);
            parameters.AddWithValue(ColName_ReadOnly, ReadOnly);
        }

        protected override void AddUpdateParameters(SqlCeParameterCollection parameters) 
        {
            parameters.AddWithValue(ColName_MaxComponentLen, (long)(_maxComponentLen));
            parameters.AddWithValue(ColName_FileSystemFlags, (long)(_fileSystemFlags));
            parameters.AddWithValue(ColName_Name, _name);
            parameters.AddWithValue(ColName_RootPath, _rootPath);
            parameters.AddWithValue(ColName_RootPathLC, _rootPath.ToLower());
            parameters.AddWithValue(ColName_FileSystemName, _fileSystemName);
            parameters.AddWithValue(ColName_CaseSensitive, CaseSensitive);
            parameters.AddWithValue(ColName_ReadOnly, ReadOnly);
        }
        
        protected override string GetTableName() { return TableName_Volume; }
        
        protected override string GetSelectQueryFields()
        {
            return String.Format("[{0}], [{1}], [{2}], [{3}], [{4}], [{5}], [{6}], [{7}], [{8}]", ColName_SerialNumber, ColName_MaxComponentLen, ColName_FileSystemFlags, 
                ColName_Name, ColName_RootPath, ColName_RootPathLC, ColName_FileSystemName, ColName_CaseSensitive, ColName_ReadOnly );
        }

        protected override string GetInsertQueryPlaceHolders() 
        {
            return String.Format("@{0}, @{1}, @{2}, @{3}, @{4},@{5}, @{6}, @{7}], @{8}", ColName_SerialNumber, ColName_MaxComponentLen, ColName_FileSystemFlags, 
                ColName_Name, ColName_RootPath, ColName_RootPathLC, ColName_FileSystemName, ColName_CaseSensitive, ColName_ReadOnly );
        }
        
        protected override string GetUpdateQueryFieldAndPlaceHolders() 
        {
            return String.Format("[{0}] = @{0}, [{1}] = @{1}, [{2}] = @{2}, [{3}] = @{3}, [{4}] = @{4}, [{5}] = @{5}, [{6}] = @{6}, [{7}] = @{7}, [{8}] = @{8}", ColName_SerialNumber, ColName_MaxComponentLen, ColName_FileSystemFlags, 
                ColName_Name, ColName_RootPath, ColName_RootPathLC, ColName_FileSystemName, ColName_CaseSensitive, ColName_ReadOnly );
        }
    }

    public abstract class FsRecord : BaseRecord
    {
        public const string ColName_ParentId = "ParentId";
        public const string ColName_Name = "Name";

        private Guid _parentId = Guid.Empty;
        private string _name = "";
        
        public Guid ParentId { get { return _id; } }

        public string Name { get { return _name; } }
        
        protected override OnLoad(SqlCeDataReader dataReader)
        {
            int ordinal = dataReader.GetOrdinal(ColName_ParentId);
            if (dataReader.IsDBNull(ordinal))
                _parentId = Guid.Empty;
            else
                _parentId = dataReader.GetGuid(ordinal);
            _name = dataReader.GetString(dataReader.GetOrdinal(ColName_Name));
        }
        
        protected FsRecord() { }
        
        protected FsRecord(string name, Guid parentId)
        {
            _name = name ?? "";
            _parentId = parentId;
        }

        protected abstract override string GetTableName();
        
        protected override string GetSelectQueryFields() { return String.Format("[{0}], [{1}]", ColName_ParentId, ColName_Name); }

        protected override string GetInsertQueryPlaceHolders() { return String.Format("@{0}, @{1}", ColName_ParentId, ColName_Name); }
        
        protected override string GetUpdateQueryFieldAndPlaceHolders() { return String.Format("[{0}] = @{0}, [{1}] = @{1}", ColName_ParentId, ColName_Name); }
    }

    public class DirectoryRecord : FsRecord, IEquatable<DirectoryRecord>
    {
        public const string TableName_Directory = "Directory";
        public const string ColName_VolumeId = "VolumeId";

        private bool _isRoot = true;
        private Guid _volumeId = Guid.Empty;
        
        public new Guid? ParentId { get { return (_isRoot) ? null : base.ParentId as Guid?; } }

        public new Guid VolumeId { get { return _volumeId; } }

        public static DirectoryRecord Load(SqlCeConnection dbConnection, Guid id)
        {
            DirectoryRecord result = new DirectoryRecord();
            if (result.SelectById(dbConnection, id))
                return result;

            return null;
        }
        
        public static DirectoryRecord Load(SqlCeConnection dbConnection, string name, Guid volumeId)
        {
            string sql = String.Format("SELECT [{0}], {1} FROM [{2}] WHERE ([{3}] = @{3} AND [{4}] = @{4})", ColName_Id, GetSelectQueryFields(), GetTableName(), ColName_Name, ColName_VolumeId);
            using (SqlCeCommand command = dbConnection.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.AddWithValue(ColName_Name, name);
                command.Parameters.AddWithValue(ColName_VolumeId, volumeId);
                using (SqlCeDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.HasRows && dataReader.Read())
                    {
                        DirectoryRecord result = new DirectoryRecord();
                        OnLoad(dataReader);
                        result._id = dataReader.GetGuid(dataReader.GetOrdinal(ColName_Id));
                        return result;
                    }
                }
            }

            return null;
        }
        
        public static DirectoryRecord Load(SqlCeConnection dbConnection, Guid parentId, string name)
        {
            string sql = String.Format("SELECT [{0}], {1} FROM [{2}] WHERE ([{3}] = @{3} AND [{4}] = @{4})", ColName_Id, GetSelectQueryFields(), GetTableName(), ColName_ParentId, ColName_Name);
            using (SqlCeCommand command = dbConnection.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.AddWithValue(ColName_ParentId, parentId);
                command.Parameters.AddWithValue(ColName_Name, name);
                using (SqlCeDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.HasRows && dataReader.Read())
                    {
                        DirectoryRecord result = new DirectoryRecord();
                        OnLoad(dataReader);
                        result._id = dataReader.GetGuid(dataReader.GetOrdinal(ColName_Id));
                        return result;
                    }
                }
            }

            return null;
        }
        
        public static DirectoryRecord LoadOrCreate(SqlCeConnection dbConnection, DirectoryInfo directoryInfo, Guid volumeId)
        {
            DirectoryRecord result;
            if (directoryInfo.Parent == null)
            {
                result = Load(directoryInfo.Name, volumeId);
                if (result == null)
                {
                    result = new DirectoryRecord(directoryInfo.Name, volumeId, null);
                    result.SaveChanges(dbConnection);
                }
                return result;
            }

            DirectoryRecord parent = LoadOrCreate(dbConnection, directoryInfo.Parent, volumeId);
            result = Load(dbConnection, parent.Id, directoryInfo.Name);
            if (result == null)
            {
                result = new DirectoryRecord(directoryInfo.Name, volumeId, parent.Id);
                result.SaveChanges(dbConnection);
            }
            return result;
        }

        public static DirectoryRecord LoadOrCreate(SqlCeConnection dbConnection, DirectoryInfo directoryInfo)
        {
            VolumeRecord volumeRecord = LoadOrCreate(dbConnection, directoryInfo);
            return LoadOrCreate(dbConnection, directoryInfo, volumeRecord.Id)
        }
        
        public DirectoryRecord() { }
        
        public DirectoryRecord(string name, Guid volumeId, Guid? parentId) : base(name, (parentId.HasValue) ? parentId.Value : Guid.Empty)
        {
            _isRoot = !parentId.HasValue;
            _volumeId = volumeId;
        }

        public override string ToString() { return String.Format("Directory: {0}", Name); }

        public bool Equals(DirectoryRecord other)
        {
            return other != null && (ReferenceEquals(this, other) || (_maxComponentLen == other._maxComponentLen && 
                _fileSystemFlags == other._fileSystemFlags && _name == other._name && _rootPath == other._rootPath && _fileSystemName == other._fileSystemName &&
                (IsNew()) ? other.IsNew() : !IsNew() && Id.Equals(other.Id)));
        }

        public override bool Equals(object obj) { return Equals(obj as VolumeRecord); }

        protected override OnLoad(SqlCeDataReader dataReader)
        {
            _isRoot = dataReader.IsDBNull(dataReader.GetOrdinal(ColName_ParentId)))
            _volumeId = dataReader.GetGuid(dataReader.GetOrdinal(ColName_VolumeId)))
        }
        
        protected override void AddInsertParameters(SqlCeParameterCollection parameters)
        {
            if (!_isRoot)
                parameters.AddWithValue(ColName_ParentId, base.ParentId);
            parameters.AddWithValue(ColName_Name, Name);
            parameters.AddWithValue(ColName_VolumeId, VolumeId);
        }

        protected override void AddUpdateParameters(SqlCeParameterCollection parameters) { }
        
        protected override string GetTableName() { return TableName_Directory; }
        
        protected override string GetSelectQueryFields() { return String.Format("{0}, [{1}]", base.GetInsertQueryPlaceHolders(), ColName_VolumeId); }
        
        protected override string GetInsertQueryPlaceHolders() { return String.Format("{0}, @{1}", base.GetInsertQueryPlaceHolders(), ColName_VolumeId); }
        
        protected override string GetUpdateQueryFieldAndPlaceHolders() { return String.Format("{0}, [{1}] = @{1}", base.GetInsertQueryPlaceHolders(), ColName_VolumeId); }
    }

    public class HashSetRecord : BaseRecord, IEquatable<HashSetRecord>
    {
        public const string TableName_HashSet = "HashSet";
        public const string ColName_IsValidated = "IsValidated";
        public const string ColName_Length = "Length";
        public const string ColName_Sha256 = "Sha256";

        private bool _isValidated = false;
        private long _length = 0L;
        private int _sha256 = 0;
        
        public HashSetRecord() { }
        
        public bool IsValidated
        {
            get { return _isValidated; }
            set
            {
                if (value == _isValidated)
                    return;

                _isValidated = value;
                SetChanged();
            }
        }
        
        public long Length
        {
            get { return _length; }
            set
            {
                if (value == _length)
                    return;

                _length = value;
                SetChanged();
            }
        }

        public int Sha256
        {
            get { return _sha256; }
            set
            {
                if (value == _sha256)
                    return;

                _sha256 = value;
                SetChanged();
            }
        }
        
        public static HashSetRecord Load(SqlCeConnection dbConnection, Guid id)
        {
            HashSetRecord result = new HashSetRecord();
            if (result.SelectById(dbConnection, id))
                return result;

            return null;
        }
        
        public static HashSetRecord Load(SqlCeConnection dbConnection, long length, int sha256, bool isValidated)
        {
            string sql = String.Format("SELECT [{0}], {1} FROM [{2}] WHERE ([{3}] = @{3} AND [{4}] = @{4} AND [{5}] = @{5})", ColName_Id, GetSelectQueryFields(),
                TableName_HashSet, ColName_Length, ColName_Sha256, ColName_IsValidated);
            using (SqlCeCommand command = dbConnection.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.AddWithValue(ColName_Length, length);
                command.Parameters.AddWithValue(ColName_Sha256, sha256);
                command.Parameters.AddWithValue(ColName_IsValidated, isValidated);
                using (SqlCeDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.HasRows && dataReader.Read())
                    {
                        HashSetRecord result = new HashSetRecord();
                        OnLoad(dataReader);
                        result._id = dataReader.GetGuid(dataReader.GetOrdinal(ColName_Id));
                        return result;
                    }
                }
            }

            return null;
        }
        
        public static HashSetRecord LoadOrCreate(SqlCeConnection dbConnection, bool isValidated, long length, int sha256)
        {
            HashSetRecord result = Load(dbConnection, isValidated, length, sha256);
            if (result == null)
            {
                result = new HashSetRecord();
                result._isValidated = isValidated;
                result._length = length;
                result._sha256 = sha256;
                result.SaveChanges(dbConnection);
            }
            
            return result;
        }
        
        public override string ToString() { return String.Format("{0:X4}-{1:X4}: {2}", Sha256 >> 16, Sha256 & 0x0000ffff, Name); }

        public override int GetHashCode() { return _sha256; }
        
        public bool Equals(HashSetRecord other)
        {
            return other != null && (ReferenceEquals(this, other) || (_isValidated == other._isValidated && _length == other._length && 
                _sha256 == other._sha256 && (IsNew()) ? other.IsNew() : !IsNew() && Id.Equals(other.Id)));
        }

        public override bool Equals(object obj) { return Equals(obj as HashSetRecord); }

        protected override OnLoad(SqlCeDataReader dataReader)
        {
            _isValidated = dataReader.GetBoolean(dataReader.GetOrdinal(ColName_IsValidated));
            _length = dataReader.GetInt64(dataReader.GetOrdinal(ColName_Length));
            _sha256 = dataReader.GetInt32(dataReader.GetOrdinal(ColName_Sha256));
        }
        
        protected override void AddInsertParameters(SqlCeParameterCollection parameters)
        {
            parameters.AddWithValue(ColName_IsValidated, IsValidated);
            parameters.AddWithValue(ColName_Length, Length);
            parameters.AddWithValue(ColName_Sha256, Sha256);
        }

        protected override void AddUpdateParameters(SqlCeParameterCollection parameters)
        {
            parameters.AddWithValue(ColName_IsValidated, IsValidated);
            parameters.AddWithValue(ColName_Length, Length);
            parameters.AddWithValue(ColName_Sha256, Sha256);
        }
        
        protected override string GetTableName() { return TableName_HashSet; }
        
        protected override string GetSelectQueryFields() { return String.Format("[{0}], [{1}], [{2}]", ColName_IsValidated, ColName_Length, ColName_Sha256); }
        
        protected override string GetInsertQueryPlaceHolders() { return String.Format("@{0}, @{1}, @{2}", ColName_IsValidated, ColName_Length, ColName_Sha256); }
        
        protected override string GetUpdateQueryFieldAndPlaceHolders() { return String.Format("[{0}] = @{0}, [{1}] = @{1}, [{2}] = @{2}", ColName_IsValidated, ColName_Length, ColName_Sha256); }
    }

    public class FileRecord : FsRecord, IEquatable<File>
    {
        public const string TableName_File = "File";
        public const string ColName_HashSetId = "HashSetId";
        public const string ColName_CreationTime = "CreationTime";
        public const string ColName_LastWriteTime = "LastWriteTime";

        private Guid _hashSetId = Guid.Empty;
        private DateTime _creationTime = DateTime.MinValue;
        private DateTime _lastWriteTime = DateTime.MinValue;
        
        public Guid HashSetId
        {
            get { return _hashSetId; }
            set
            {
                if (value == _hashSetId)
                    return;

                _hashSetId = value;
                SetChanged();
            }
        }

        public DateTime CreationTime
        {
            get { return _creationTime; }
            set
            {
                if (value == _creationTime)
                    return;

                _creationTime = value;
                SetChanged();
            }
        }

        public DateTime LastWriteTime
        {
            get { return _lastWriteTime; }
            set
            {
                if (value == _lastWriteTime)
                    return;

                _lastWriteTime = value;
                SetChanged();
            }
        }
        
        public static FileRecord Load(SqlCeConnection dbConnection, Guid id)
        {
            FileRecord result = new FileRecord();
            if (result.SelectById(dbConnection, id))
                return result;

            return null;
        }
        
        public static FileRecord Load(SqlCeConnection dbConnection, Guid parentId, string name)
        {
            string sql = String.Format("SELECT [{0}], {1} FROM [{2}] WHERE ([{3}] = @{3} AND [{4}] = @{4})", ColName_Id, GetSelectQueryFields(),
                TableName_File, ColName_ParentId, ColName_Name);
            using (SqlCeCommand command = dbConnection.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.AddWithValue(ColName_ParentId, parentId);
                command.Parameters.AddWithValue(ColName_Name, name);
                using (SqlCeDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.HasRows && dataReader.Read())
                    {
                        FileRecord result = new FileRecord();
                        OnLoad(dataReader);
                        result._id = dataReader.GetGuid(dataReader.GetOrdinal(ColName_Id));
                        return result;
                    }
                }
            }

            return null;
        }

        public static FileRecord LoadOrCreate(SqlCeConnection dbConnection, FileInfoEx fileInfoEx)
        {
            /* HashSetRecord result = Load(dbConnection, isValidated, length, sha256);
            if (result == null)
            {
                result = new HashSetRecord();
                result._isValidated = isValidated;
                result._length = length;
                result._sha256 = sha256;
                result.SaveChanges(dbConnection);
            }
            
            return result; */
            throw new NotImplementedException();
        }

        public static FileRecord LoadOrCreate(SqlCeConnection dbConnection, FileInfo fileInfo)
        {
            /* HashSetRecord result = Load(dbConnection, isValidated, length, sha256);
            if (result == null)
            {
                result = new HashSetRecord();
                result._isValidated = isValidated;
                result._length = length;
                result._sha256 = sha256;
                result.SaveChanges(dbConnection);
            }
            
            return result; */
            throw new NotImplementedException();
        }
        
        public FileRecord() { }
        
        public FileRecord(string name, Guid parentId) : base(name, parentId) { }

        public override string ToString() { return String.Format("File: {0}", Name); }

        public bool Equals(FileRecord other)
        {
            return other != null && (ReferenceEquals(this, other) || (Name == other.Name && _hashSetId == other._hashSetId && _creationTime == other._creationTime &&
                _lastWriteTime == other._lastWriteTime && (IsNew()) ? other.IsNew() : !IsNew() && Id.Equals(other.Id)));
        }

        public override bool Equals(object obj) { return Equals(obj as FileRecord); }
        
        protected override OnLoad(SqlCeDataReader dataReader)
        {
            _hashSetId = dataReader.GetGuid(dataReader.GetOrdinal(ColName_HashSetId));
            _creationTime = dataReader.GetDateTime(dataReader.GetOrdinal(ColName_CreationTime));
            _lastWriteTime = dataReader.GetDateTime(dataReader.GetOrdinal(ColName_LastWriteTime));
        }
        
        protected override void AddInsertParameters(SqlCeParameterCollection parameters)
        {
            parameters.AddWithValue(ColName_ParentId, base.ParentId);
            parameters.AddWithValue(ColName_Name, Name);
            parameters.AddWithValue(ColName_HashSetId, HashSetId);
            parameters.AddWithValue(ColName_CreationTime, CreationTime);
            parameters.AddWithValue(ColName_LastWriteTime, LastWriteTime);
        }

        protected override void AddUpdateParameters(SqlCeParameterCollection parameters)
        {
            parameters.AddWithValue(ColName_HashSetId, HashSetId);
            parameters.AddWithValue(ColName_CreationTime, CreationTime);
            parameters.AddWithValue(ColName_LastWriteTime, LastWriteTime);
        }

        protected override string GetTableName() { return TableName_File; }
        
        protected override string GetSelectQueryFields() { return String.Format("{1}, [{1}], [{2}], [{3}]", base.GetSelectQueryFields(), ColName_HashSetId, ColName_CreationTime, ColName_LastWriteTime); }

        protected override string GetInsertQueryPlaceHolders() { return String.Format("{0}, @{1}, @{2}, @{3}", base.GetInsertQueryPlaceHolders(), ColName_HashSetId, ColName_CreationTime, ColName_LastWriteTime); }
        
        protected override string GetUpdateQueryFieldAndPlaceHolders() { return String.Format("{0}, [{1}] = @{1}, [{2}] = @{2}, [{3}] = @{3}", base.GetUpdateQueryFieldAndPlaceHolders(), ColName_HashSetId, ColName_CreationTime, ColName_LastWriteTime); }
    }
}
'@

Function Get-SqlConnectionString {
    [CmdletBinding()]
    Param()
    $SqlCeConnectionStringBuilder = New-Object -TypeName 'System.Data.SqlServerCe.SqlCeConnectionStringBuilder';
    $SqlCeConnectionStringBuilder.DataSource = $Script:DbPath;
    $SqlCeConnectionStringBuilder.ConnectionString;
}

Function Get-SqlCeEngine {
    [CmdletBinding()]
    Param()
    New-Object -TypeName 'System.Data.SqlServerCe.SqlCeEngine' -ArgumentList (Get-SqlConnectionString);
}

Function Get-DbConnection {
    [CmdletBinding()]
    Param()
    $SqlCeConnectionStringBuilder = New-Object -TypeName 'System.Data.SqlServerCe.SqlCeConnectionStringBuilder';
    $SqlCeConnectionStringBuilder.DataSource = $Script:DbPath;
    if (-not ($Script:DbPath | Test-Path)) {
        $SqlCeEngine = Get-SqlCeEngine;
        try { $SqlCeEngine.CreateDatabase() } catch { throw } finally { $SqlCeEngine.Dispose() }
        $DbConnection = New-Object -TypeName 'System.Data.SqlServerCe.SqlCeConnection' -ArgumentList (Get-SqlConnectionString);
        $DbConnection.Open();
        $DbCommand = $DbConnection.CreateCommand();
        $DbCommand.CommandText = @'
CREATE TABLE [{0}] (
    [{1}] UniqueIdentifier NOT NULL ROWGUIDCOL,
    [{2}] BigInt NOT NULL,
    [{3}] BigInt NOT NULL,
    [{4}] BigInt NOT NULL,
    [{5}] NVarChar(450) NOT NULL,
    [{6}] NVarChar(450) NOT NULL,
    [{7}] NVarChar(450) NOT NULL,
    [{8}] NVarChar(450) NOT NULL,
    [{9}] Bit NOT NULL,
    [{10}] Bit NOT NULL
);
'@ -f [DupCheckCLR.VolumeRecord]::TableName_Volume, [DupCheckCLR.VolumeRecord]::ColName_Id, [DupCheckCLR.VolumeRecord]::ColName_SerialNumber,
            [DupCheckCLR.VolumeRecord]::ColName_MaxComponentLen, [DupCheckCLR.VolumeRecord]::ColName_FileSystemFlags, [DupCheckCLR.VolumeRecord]::ColName_Name,
            [DupCheckCLR.VolumeRecord]::ColName_RootPath, [DupCheckCLR.VolumeRecord]::ColName_RootPathLC, [DupCheckCLR.VolumeRecord]::ColName_FileSystemName,
            [DupCheckCLR.VolumeRecord]::ColName_CaseSensitive, [DupCheckCLR.VolumeRecord]::ColName_ReadOnly;
        $DbCommand.ExecuteNonQuery() | Out-Null;
        $DbCommand.Dispose();

        $DbCommand = $DbConnection.CreateCommand();
        $DbCommand.CommandText = 'ALTER TABLE [{0}] ADD CONSTRAINT [PK_{0}] PRIMARY KEY ([Id]);' -f [DupCheckCLR.VolumeRecord]::TableName_Volume, [DupCheckCLR.VolumeRecord]::ColName_Id;
        $DbCommand.ExecuteNonQuery() | Out-Null;
        $DbCommand.Dispose();
        
        $DbCommand = $DbConnection.CreateCommand();
        $DbCommand.CommandText = 'CREATE UNIQUE INDEX [IX_{0}_{1}] ON [Volume] ([{1}] ASC)' -f [DupCheckCLR.VolumeRecord]::TableName_Volume, [DupCheckCLR.VolumeRecord]::ColName_SerialNumber;
        $DbCommand.ExecuteNonQuery() | Out-Null;
        $DbCommand.Dispose();

        $DbCommand = $DbConnection.CreateCommand();
        $DbCommand.CommandText = 'CREATE INDEX [IX_{0}_{1}] ON [Volume] ([{1}] ASC)' -f [DupCheckCLR.VolumeRecord]::TableName_Volume, [DupCheckCLR.VolumeRecord]::ColName_RootPathLC;
        $DbCommand.ExecuteNonQuery() | Out-Null;
        $DbCommand.Dispose();

        $DbCommand = $DbConnection.CreateCommand();
        $DbCommand.CommandText = @'
CREATE TABLE [{0}] (
    [{1}] UniqueIdentifier NOT NULL ROWGUIDCOL,
    [{2}] UniqueIdentifier NOT NULL,
    [{3}] UniqueIdentifier NULL,
    [{4}] NVarChar(450) NOT NULL,
    [{5}] NVarChar(450) NOT NULL
);
'@ -f [DupCheckCLR.DirectoryRecord]::TableName_Directory, [DupCheckCLR.DirectoryRecord]::ColName_Id, [DupCheckCLR.DirectoryRecord]::ColName_VolumeId,
            [DupCheckCLR.DirectoryRecord]::ColName_ParentId, [DupCheckCLR.DirectoryRecord]::ColName_Name, [DupCheckCLR.DirectoryRecord]::ColName_LCName;
        $DbCommand.ExecuteNonQuery() | Out-Null;
        $DbCommand.Dispose();
        
        $DbCommand = $DbConnection.CreateCommand();
        $DbCommand.CommandText = 'ALTER TABLE [{0}] ADD CONSTRAINT [PK_{0}] PRIMARY KEY ([{1}]);' -f [DupCheckCLR.DirectoryRecord]::TableName_Directory, [DupCheckCLR.DirectoryRecord]::ColName_VolumeId;
        $DbCommand.ExecuteNonQuery() | Out-Null;
        $DbCommand.Dispose();
        
        $DbCommand = $DbConnection.CreateCommand();
        $DbCommand.CommandText = 'ALTER TABLE [{0}] ADD CONSTRAINT [FK_{1}_{0}] FOREIGN KEY ([{2}]) REFERENCES [{1}] ([{3}]);' -f [DupCheckCLR.DirectoryRecord]::TableName_Directory,
            [DupCheckCLR.VolumeRecord]::TableName_Volume, [DupCheckCLR.DirectoryRecord]::ColName_VolumeId, [DupCheckCLR.VolumeRecord]::ColName_Id;
        $DbCommand.ExecuteNonQuery() | Out-Null;
        $DbCommand.Dispose();

        $DbCommand = $DbConnection.CreateCommand();
        $DbCommand.CommandText = 'ALTER TABLE [{0}] ADD CONSTRAINT [FK_Parent_{0}] FOREIGN KEY ([{1}]) REFERENCES [{0}] ([{2}]);' -f [DupCheckCLR.DirectoryRecord]::TableName_Directory,
            [DupCheckCLR.DirectoryRecord]::ColName_ParentId, [DupCheckCLR.DirectoryRecord]::ColName_Id;
        $DbCommand.ExecuteNonQuery() | Out-Null;
        $DbCommand.Dispose();

        $DbCommand = $DbConnection.CreateCommand();
        $DbCommand.CommandText = 'CREATE INDEX [IX_{0}_{1}] ON [{0}] ([{1}] ASC)' -f [DupCheckCLR.DirectoryRecord]::TableName_Directory, [DupCheckCLR.VolumeRecord]::ColName_LCName;
        $DbCommand.ExecuteNonQuery() | Out-Null;
        $DbCommand.Dispose();

        $DbCommand = $DbConnection.CreateCommand();
        $DbCommand.CommandText = @'
CREATE TABLE [{0}] (
    [Id] UniqueIdentifier NOT NULL ROWGUIDCOL,
    [IsValidated] Bit NOT NULL,
    [Length] BigInt NOT NULL,
    [Sha256] Int NOT NULL
);
'@ -f [DupCheckCLR.HashSetRecord]::TableName_HashSet, [DupCheckCLR.HashSetRecord]::ColName_Id, [DupCheckCLR.HashSetRecord]::ColName_IsValidated,
            [DupCheckCLR.HashSetRecord]::ColName_Length, [DupCheckCLR.HashSetRecord]::ColName_Sha256;
        $DbCommand.ExecuteNonQuery() | Out-Null;
        $DbCommand.Dispose();

        $DbCommand = $DbConnection.CreateCommand();
        $DbCommand.CommandText = 'ALTER TABLE [{0}] ADD CONSTRAINT [PK_{0}] PRIMARY KEY ([{1}]);' -f [DupCheckCLR.HashSetRecord]::TableName_HashSet, [DupCheckCLR.HashSetRecord]::ColName_Id;
        $DbCommand.ExecuteNonQuery() | Out-Null;
        $DbCommand.Dispose();

        $DbCommand = $DbConnection.CreateCommand();
        $DbCommand.CommandText = 'CREATE INDEX [IX_{0}_{1}] ON [{0}] ([{1}] ASC)' -f [DupCheckCLR.HashSetRecord]::TableName_HashSett, [DupCheckCLR.HashSetRecord]::ColName_Length;
        $DbCommand.ExecuteNonQuery() | Out-Null;
        $DbCommand.Dispose();

        $DbCommand = $DbConnection.CreateCommand();
        $DbCommand.CommandText = 'CREATE INDEX [IX_{0}_{1}] ON [{0}] ([{1}] ASC)' -f [DupCheckCLR.HashSetRecord]::TableName_HashSett, [DupCheckCLR.HashSetRecord]::ColName_Sha256;
        $DbCommand.ExecuteNonQuery() | Out-Null;
        $DbCommand.Dispose();

        $DbCommand = $DbConnection.CreateCommand();
        $DbCommand.CommandText = @'
CREATE TABLE [{0}] (
    [{1}] UniqueIdentifier NOT NULL ROWGUIDCOL,
    [{2}] UniqueIdentifier NOT NULL,
    [{3}] UniqueIdentifier NOT NULL,
    [{4}] NVarChar(450) NOT NULL,
    [{5}] NVarChar(450) NOT NULL,
    [{6}] DateTime NOT NULL,
    [{7}] DateTime NOT NULL
);
'@ -f [DupCheckCLR.FileRecord]::TableName_File, [DupCheckCLR.FileRecord]::ColName_Id, [DupCheckCLR.FileRecord]::ColName_ParentId, [DupCheckCLR.FileRecord]::ColName_HashSetId,
            [DupCheckCLR.FileRecord]::ColName_Name, [DupCheckCLR.FileRecord]::ColName_LCName, [DupCheckCLR.FileRecord]::ColName_CreationTime,
            [DupCheckCLR.FileRecord]::ColName_LastWriteTime;
        $DbCommand.ExecuteNonQuery() | Out-Null;
        $DbCommand.Dispose();

        $DbCommand = $DbConnection.CreateCommand();
        $DbCommand.CommandText = 'ALTER TABLE [{0}] ADD CONSTRAINT [PK_{0}] PRIMARY KEY ([{1}]);' -f [DupCheckCLR.FileRecord]::TableName_File, [DupCheckCLR.FileRecord]::ColName_Id;
        $DbCommand.ExecuteNonQuery() | Out-Null;
        $DbCommand.Dispose();
        
        $DbCommand = $DbConnection.CreateCommand();
        $DbCommand.CommandText = 'ALTER TABLE [{0}] ADD CONSTRAINT [FK_{1}_{0}] FOREIGN KEY ([{2}]) REFERENCES [{1}] ([{3}]);' -f [DupCheckCLR.FileRecord]::TableName_File,
            [DupCheckCLR.DirectoryRecord]::TableName_Directory, [DupCheckCLR.FileRecord]::ColName_Id, [DupCheckCLR.FileRecord]::ColName_ParentId,
            [DupCheckCLR.DirectoryRecord]::ColName_Id;
        $DbCommand.ExecuteNonQuery() | Out-Null;
        $DbCommand.Dispose();

        $DbCommand = $DbConnection.CreateCommand();
        $DbCommand.CommandText = 'CREATE INDEX [IX_{0}_{1}] ON [{0}] ([{1}] ASC)' -f [DupCheckCLR.FileRecord]::TableName_File, [DupCheckCLR.FileRecord]::ColName_LCName;
        $DbCommand.ExecuteNonQuery() | Out-Null;
        $DbCommand.Dispose();
    } else {
        $DbConnection = New-Object -TypeName 'System.Data.SqlServerCe.SqlCeConnection' -ArgumentList $SqlCeConnectionStringBuilder.ConnectionString;
        $DbConnection.Open();
    }
    $DbConnection | Write-Output;
}

Function Import-FSFile {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [System.IO.FileInfo]$FileInfo,
        
        [Parameter(Mandatory = $true)]
        [System.Guid]$ParentId
    )

    Process {
        if (-not [DupCheckCLR.FileInfoEx]::HasFlag($FileInfo.Attributes, [System.IO.FileAttributes]::Hidden)) {
            $FileInfo.FullName | Write-Host;
            $Properties = @{
                Id = $null;
                OldHashSetId = $null;
                NewHashSetId = $null;
                IsChanged = $false;
            };
            $DbCommand = $Script:DbConnection.CreateCommand();
            try {
                $DbCommand.CommandText = 'SELECT [Id], [HashSetId], [CreationTime], [LastWriteTime] FROM [File] WHERE ([ParentId]=@ParentId AND [LCName]=@LCName)';
                $DbCommand.Parameters.AddWithValue('ParentId', $ParentId) | Out-Null;
                $DbCommand.Parameters.AddWithValue('LCName', $FileInfo.Name.ToLower()) | Out-Null;
                $DbReader = $DbCommand.ExecuteReader();
                try {
                    if ($DbReader.HasRows -and $DbReader.Read()) {
                        $Properties.Id = $DbReader.GetGuid($DbReader.GetOrdinal('Id'));
                        $Properties.OldHashSetId = $DbReader.GetGuid($DbReader.GetOrdinal('HashSetId'));
                        $CreationTime = $DbReader.GetDateTime($DbReader.GetOrdinal('CreationTime'));
                        if ($CreationTime -ne $FileInfo.CreationTime) {
                            $Properties.IsChanged = $true;
                        }
                        $LastWriteTime = $DbReader.GetDateTime($DbReader.GetOrdinal('LastWriteTime'));
                        if ($LastWriteTime -ne $FileInfo.LastWriteTime) {
                            $Properties.IsChanged = $true;
                        }
                    }
                } catch { throw } finally { $DbReader.Dispose() }
            } catch { throw } finally { $DbCommand.Dispose() }
            $Properties.NewHashSetId = $Properties.OldHashSetId;
            if ($Properties.OldHashSetId -ne $null) {
                $DbCommand.CommandText = 'SELECT [Length] FROM [HashSet] WHERE ([Id]=@Id)';
                $DbCommand.Parameters.AddWithValue('Id', $Properties.OldHashSetId) | Out-Null;
                $DbReader = $DbCommand.ExecuteReader();
                try {
                    $DbReader.Read();
                    if ($DbReader.GetInt64($DbReader.GetOrdinal('Length')) -ne $FileInfoEx.Length) {
                        $Properties.NewHashSetId = $null;
                        $Properties.IsChanged = $true;
                    }
                } catch { throw } finally { $DbReader.Dispose() }
            }
            if ($Properties.IsChanged -or $Properties.Id -eq $null) {
                $FileInfoEx = [DupCheckCLR.FileInfoEx]::Create($FileInfo, $Script:HashAlgorithm);
                $DbCommand = $Script:DbConnection.CreateCommand();
                if ($Properties.OldHashSetId -eq $null) {
                    $DbCommand.CommandText = 'SELECT [Id] FROM [HashSet] WHERE ([IsValidated]=0 AND [Length]=@Length AND [Sha256]=@Sha256)';
                    $DbCommand.Parameters.AddWithValue('Length', $FileInfoEx.Length) | Out-Null;
                    $DbCommand.Parameters.AddWithValue('Sha256', $FileInfoEx.Sha256HashCode) | Out-Null;
                    $DbReader = $DbCommand.ExecuteReader();
                    try {
                        if ($DbReader.HasRows -and $DbReader.Read()) {
                            $Properties.NewHashSetId = $DbReader.GetGuid($DbReader.GetOrdinal('Id'));
                        }
                    } catch { throw } finally { $DbReader.Dispose() }
                } else {
                    $DbCommand.CommandText = 'SELECT [Sha256] FROM [HashSet] WHERE ([Id]=@Id)';
                    $DbCommand.Parameters.AddWithValue('Id', $Properties.OldHashSetId) | Out-Null;
                    $DbReader = $DbCommand.ExecuteReader();
                    try {
                        $DbReader.Read();
                        if ($DbReader.GetInt32($DbReader.GetOrdinal('Sha256')) -ne $FileInfoEx.Sha256HashCode) {
                            $Properties.NewHashSetId = $null;
                        }
                    } catch { throw } finally { $DbReader.Dispose() }

                }

                if ($Properties.NewHashSetId -eq $null) {
                    $Properties.NewHashSetId = [System.Guid]::NewGuid();
                    $DbCommand = $Script:DbConnection.CreateCommand();
                    $DbCommand.CommandText = 'INSERT INTO [HashSet] ([Id], [IsValidated], [Length], [Sha256]) VALUES (@Id, @IsValidated, @Length, @Sha256)';
                    $DbCommand.Parameters.AddWithValue('Id', $Properties.NewHashSetId) | Out-Null;
                    $DbCommand.Parameters.AddWithValue('IsValidated', $false) | Out-Null;
                    $DbCommand.Parameters.AddWithValue('Length', $FileInfoEx.Length) | Out-Null;
                    $DbCommand.Parameters.AddWithValue('Sha256', $FileInfoEx.Sha256HashCode) | Out-Null;
                    $DbCommand.ExecuteNonQuery() | Out-Null;
                }

                if ($Properties.Id -eq $null) {
                    $DbCommand = $Script:DbConnection.CreateCommand();
                    $DbCommand.CommandText = 'INSERT INTO [File] ([Id], [ParentId], [HashSetId], [Name], [LCName], [CreationTime], [LastWriteTime]) VALUES (@Id, @ParentId, @HashSetId, @Name, @LCName, @CreationTime, @LastWriteTime)';
                    $DbCommand.Parameters.AddWithValue('Id', [System.Guid]::NewGuid()) | Out-Null;
                    $DbCommand.Parameters.AddWithValue('ParentId', $ParentId) | Out-Null;
                    $DbCommand.Parameters.AddWithValue('HashSetId', $Properties.NewHashSetId) | Out-Null;
                    $DbCommand.Parameters.AddWithValue('Name', $FileInfo.Name) | Out-Null;
                    $DbCommand.Parameters.AddWithValue('LCName', $FileInfo.Name.ToLower()) | Out-Null;
                    $DbCommand.Parameters.AddWithValue('CreationTime', $FileInfo.CreationTime) | Out-Null;
                    $DbCommand.Parameters.AddWithValue('LastWriteTime', $FileInfo.LastWriteTime) | Out-Null;
                    $DbCommand.ExecuteNonQuery() | Out-Null;
                } else {
                    $DbCommand = $Script:DbConnection.CreateCommand();
                    $DbCommand.CommandText = 'UPDATE [File] SET [HashSetId] = @HashSetId, [Name] = @Name, [LCName] = @LCName, [CreationTime] = @CreationTime, [LastWriteTime] = @LastWriteTime) WHERE ([Id]=@Id)';
                    $DbCommand.Parameters.AddWithValue('HashSetId', $Properties.NewHashSetId) | Out-Null;
                    $DbCommand.Parameters.AddWithValue('Name', $FileInfo.Name) | Out-Null;
                    $DbCommand.Parameters.AddWithValue('LCName', $FileInfo.Name.ToLower()) | Out-Null;
                    $DbCommand.Parameters.AddWithValue('CreationTime', $FileInfo.CreationTime) | Out-Null;
                    $DbCommand.Parameters.AddWithValue('LastWriteTime', $FileInfo.LastWriteTime) | Out-Null;
                    $DbCommand.Parameters.AddWithValue('Id', $Properties.Id) | Out-Null;
                    $DbCommand.ExecuteNonQuery() | Out-Null;
                }

                <# TODO: Remove old hash set if it has no more related items
                if ($Properties.OldHashSetId -ne $null -and $Properties.NewHashSetId -ne $Properties.OldHashSetId) {
                }
                #>
            }
        }
    }
}

Function Get-FSFolderId {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true)]
        [System.IO.DirectoryInfo]$DirectoryInfo,
        
        [Parameter(Mandatory = $true)]
        [System.Guid]$VolumeId,

        [System.Guid]$ParentId
    )
    
    $p = $null;
    $DbCommand = $Script:DbConnection.CreateCommand();
    if ($PSBoundParameters.ContainsKey('ParentId')) {
        $p = $ParentId;
        $DbCommand.CommandText = 'SELECT [Id] FROM [Directory] WHERE ([ParentId]=@ParentId AND [LCName]=@Name)';
        $DbCommand.Parameters.AddWithValue('ParentId', $p) | Out-Null;
        $DbCommand.Parameters.AddWithValue('Name', $DirectoryInfo.Name.ToLower()) | Out-Null;
    } else {
        if ($DirectoryInfo.Parent -eq $null) {
            $DbCommand.CommandText = 'SELECT [Id] FROM [Directory] WHERE ([VolumeId]=@VolumeId AND [ParentId] IS NULL AND [LCName]=@Name)';
            $DbCommand.Parameters.AddWithValue('VolumeId', $VolumeId) | Out-Null;
            $DbCommand.Parameters.AddWithValue('Name', $DirectoryInfo.Name.ToLower()) | Out-Null;
        } else {
            $DbCommand.CommandText = 'SELECT [Id] FROM [Directory] WHERE ([ParentId]=@ParentId AND [LCName]=@Name)';
            $p = Get-FSFolderId -DirectoryInfo $DirectoryInfo.Parent -VolumeId $VolumeId;
            $DbCommand.Parameters.AddWithValue('ParentId', $p) | Out-Null;
            $DbCommand.Parameters.AddWithValue('Name', $DirectoryInfo.Name.ToLower()) | Out-Null;
        }
    }
    $DbReader = $DbCommand.ExecuteReader();
    
    $Id = $null;
    try {
        if ($DbReader.HasRows -and $DbReader.Read()) { $Id = $DbReader.GetGuid($DbReader.GetOrdinal('Id')) }
    } catch { throw } finally { $DbReader.Dispose() }
    if ($Id -eq $null) {
        $DbCommand = $Script:DbConnection.CreateCommand();
        try {
            $Id = [System.Guid]::NewGuid();
            $DbCommand.CommandText = 'INSERT INTO [Directory] ([Id], [VolumeId], [ParentId], [Name], [LCName]) VALUES (@Id, @VolumeId, @ParentId, @Name, @LCName)';
            $DbCommand.Parameters.AddWithValue('Id', $Id) | Out-Null;
            $DbCommand.Parameters.AddWithValue('VolumeId', $VolumeId) | Out-Null;
            if ($p -eq $null) {
                $DbCommand.Parameters.AddWithValue('ParentId', [System.DBNull]::Value) | Out-Null;
            } else {
                $DbCommand.Parameters.AddWithValue('ParentId', $p) | Out-Null;
            }
            $Name = $DirectoryInfo.Name;
            $DbCommand.Parameters.AddWithValue('Name', $Name) | Out-Null;
            $DbCommand.Parameters.AddWithValue('LCName', $Name.ToLower()) | Out-Null;
            $DbCommand.ExecuteNonQuery() | Out-Null;
        } catch { throw } finally { $DbCommand.Dispose() }
    }
    $Id | Write-Output;
}

Function Import-FSFolder {
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
        [System.IO.DirectoryInfo]$DirectoryInfo,

        [System.Guid]$VolumeId,

        [System.Guid]$ParentId
    )

    Process {
        if (-not $PSBoundParameters.ContainsKey('VolumeId')) {
            $Id = $null;
            $DbCommand = $Script:DbConnection.CreateCommand();
            try {
                $DbCommand.CommandText = 'SELECT [Id] FROM [Volume] WHERE ([RootPathLC]=@RootPathLC)';
                $DbCommand.Parameters.AddWithValue('RootPathLC', $DirectoryInfo.Root.FullName.ToLower()) | Out-Null;
                $DbReader = $DbCommand.ExecuteReader();
                try {
                    if ($DbReader.HasRows -and $DbReader.Read()) { $Id = $DbReader.GetGuid($DbReader.GetOrdinal('Id')) }
                } catch { throw } finally { $DbReader.Dispose() }
            } catch { throw } finally { $DbCommand.Dispose() }
            
            if ($Id -eq $null) {
                $DbCommand = $Script:DbConnection.CreateCommand();
                try {
                    $Id = [System.Guid]::NewGuid();
                    $VolumeInformation = New-Object -TypeName 'DupCheckCLR.VolumeInformation' -ArgumentList $DirectoryInfo.Root.FullName;
                    $DbCommand.CommandText = 'INSERT INTO [Volume] ([Id], [SerialNumber], [MaxComponentLen], [FileSystemFlags], [Name], [RootPath], [RootPathLC], [FileSystemName], [CaseSensitive], [ReadOnly]) VALUES (@Id, @SerialNumber, @MaxComponentLen, @FileSystemFlags, @Name, @RootPath, @RootPathLC, @FileSystemName, @CaseSensitive, @ReadOnly)';
                    $DbCommand.Parameters.AddWithValue('Id', $Id) | Out-Null;
                    [System.Int64]$Value = $VolumeInformation.VolumeSerialNumber;
                    $DbCommand.Parameters.AddWithValue('SerialNumber', $Value) | Out-Null;
                    [System.Int64]$Value = $VolumeInformation.MaximumComponentLength;
                    $DbCommand.Parameters.AddWithValue('MaxComponentLen', $Value) | Out-Null;
                    [System.Int64]$Value = $VolumeInformation.FileSystemFlags;
                    $DbCommand.Parameters.AddWithValue('FileSystemFlags', $Value) | Out-Null;
                    $DbCommand.Parameters.AddWithValue('Name', $VolumeInformation.VolumeName) | Out-Null;
                    $DbCommand.Parameters.AddWithValue('RootPath', $VolumeInformation.RootPathName) | Out-Null;
                    $DbCommand.Parameters.AddWithValue('RootPathLC', $VolumeInformation.RootPathName.ToLower()) | Out-Null;
                    $DbCommand.Parameters.AddWithValue('FileSystemName', $VolumeInformation.FileSystemName) | Out-Null;
                    $DbCommand.Parameters.AddWithValue('CaseSensitive', $VolumeInformation.CaseSensitiveSearch) | Out-Null;
                    $DbCommand.Parameters.AddWithValue('ReadOnly', $VolumeInformation.ReadOnlyVolume) | Out-Null;
                    $DbCommand.ExecuteNonQuery() | Out-Null;
                } catch { throw } finally { $DbCommand.Dispose() }
            }
            $VolumeId = $Id;
        }
        $DirectoryInfo.FullName | Write-Host;
        $Id = Get-FSFolderId -DirectoryInfo $DirectoryInfo -VolumeId $VolumeId;
        $ChildDirectories = @();
        try { $ChildDirectories = @($DirectoryInfo.GetDirectories()) } catch { $ChildDirectories = @() }
        if ($ChildDirectories.Count -gt 0) {
            $ChildDirectories | Import-FSFolder -VolumeId $VolumeId -ParentId $Id;
        }
        $Files = @();
        try { $Files = @($DirectoryInfo.GetFiles()) } catch { $Files = @() }
        if ($Files.Count -gt 0) {
            $Files | Import-FSFile -ParentId $Id;
        }
    }
}

$Script:HashAlgorithm = [DupCheckCLR.FileInfoEx]::GetHashAlgorithm();
$Script:DbConnection = Get-DbConnection;
try {
    @('C:\Users\lerwi\Documents', 'C:\Users\lerwi\Downloads', 'C:\Users\lerwi\Google Drive', 'C:\Users\lerwi\OneDrive', 'C:\Users\lerwi\Music', 'C:\Users\lerwi\Pictures',
        'C:\Users\lerwi\Videos', 'F:\Backup') | ForEach-Object { New-Object -TypeName 'System.IO.DirectoryInfo' -ArgumentList $_ } | Import-FSFolder;
} catch { throw } finally {
    $Script:DbConnection.Close();
    $Script:DbConnection.Dispose();
    $Script:HashAlgorithm.Dispose();
}

$SqlCeEngine = Get-SqlCeEngine;
try { $SqlCeEngine.Compact() } catch { throw } finally { $SqlCeEngine.Dispose() }
using System;
using System.Data.SqlServerCe;
using System.IO;

namespace FileSystemIndexLib
{
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

        public VolumeRecord() { }
        
        public VolumeRecord(Guid id, uint serialNumber) : base(id) { _serialNumber = serialNumber; }

        public uint SerialNumber { get { return _serialNumber; } }

        public uint MaxComponentLen { get { return _maxComponentLen; } set { _maxComponentLen = value; } }

        public FileSystemFeature FileSystemFlags { get { return _fileSystemFlags; } set { _fileSystemFlags = value; } }

        public string Name { get { return _name; } set { _name = value; } }

        public string RootPath { get { return _rootPath; } set { _rootPath = value; } }

        public string FileSystemName { get { return _fileSystemName; } set { _fileSystemName = value; } }

        public bool CaseSensitive { get { return _fileSystemFlags.HasFlag(FileSystemFeature.CaseSensitiveSearch); } }

        public bool ReadOnly { get { return _fileSystemFlags.HasFlag(FileSystemFeature.ReadOnlyVolume); } }

        public static VolumeRecord Load(SqlCeConnection dbConnection, Guid id)
        {
            VolumeRecord result = new VolumeRecord();
            if (result.SelectById(dbConnection, id))
                return result;

            return null;
        }

        public static VolumeRecord Load(SqlCeConnection dbConnection, uint serialNumber)
        {
            VolumeRecord result = new VolumeRecord();
            string sql = String.Format("SELECT [{0}], {1} FROM [{2}] WHERE ([{3}] = @{3})", ColName_Id, result.GetSelectQueryFields(), result.GetTableName(), ColName_SerialNumber);
            using (SqlCeCommand command = dbConnection.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.AddWithValue(ColName_SerialNumber, serialNumber);
                result.AddSelectIdQueryField(command);
                using (SqlCeDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.HasRows && dataReader.Read())
                    {
                        result = new VolumeRecord(dataReader.GetGuid(dataReader.GetOrdinal(ColName_Id)), serialNumber);
                        result.OnLoad(dataReader);
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
            _maxComponentLen = volumeInformation.MaximumComponentLength;
            _fileSystemFlags = volumeInformation.FileSystemFlags;
            _name = volumeInformation.VolumeName;
            _rootPath = volumeInformation.RootPathName;
            _fileSystemName = volumeInformation.FileSystemName;
            if (IsChanged())
                SaveChanges(dbConnection);
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
            result._rootPath = volumeInformation.RootPathName;
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
        
        protected override void OnLoad(SqlCeDataReader dataReader)
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
                ColName_Name, ColName_RootPath, ColName_RootPathLC, ColName_FileSystemName, ColName_CaseSensitive, ColName_ReadOnly);
        }

        protected override string GetInsertQueryPlaceHolders()
        {
            return String.Format("@{0}, @{1}, @{2}, @{3}, @{4},@{5}, @{6}, @{7}], @{8}", ColName_SerialNumber, ColName_MaxComponentLen, ColName_FileSystemFlags,
                ColName_Name, ColName_RootPath, ColName_RootPathLC, ColName_FileSystemName, ColName_CaseSensitive, ColName_ReadOnly);
        }

        protected override string GetUpdateQueryFieldAndPlaceHolders()
        {
            return String.Format("[{0}] = @{0}, [{1}] = @{1}, [{2}] = @{2}, [{3}] = @{3}, [{4}] = @{4}, [{5}] = @{5}, [{6}] = @{6}, [{7}] = @{7}, [{8}] = @{8}", ColName_SerialNumber, ColName_MaxComponentLen, ColName_FileSystemFlags,
                ColName_Name, ColName_RootPath, ColName_RootPathLC, ColName_FileSystemName, ColName_CaseSensitive, ColName_ReadOnly);
        }
    }
}

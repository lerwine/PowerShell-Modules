using System;
using System.Data.SqlServerCe;
using System.IO;

namespace FileSystemIndexLib
{
    public class FileRecord : FsRecord, IEquatable<FileRecord>
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
            FileRecord result = new FileRecord();
            string sql = String.Format("SELECT [{0}], {1} FROM [{2}] WHERE ([{3}] = @{3} AND [{4}] = @{4})", ColName_Id, result.GetSelectQueryFields(),
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
                        result = new FileRecord(dataReader.GetGuid(dataReader.GetOrdinal(ColName_Id)), name, parentId);
                        result.OnLoad(dataReader);
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

        public FileRecord(Guid id, string name, Guid parentId) : base(id, name, parentId) { }

        public FileRecord(string name, Guid parentId) : base(name, parentId) { }

        public FileRecord() { }

        public override string ToString() { return String.Format("File: {0}", Name); }

        public override int GetHashCode() { return Name.GetHashCode(); }

        public bool Equals(FileRecord other)
        {
            return other != null && (ReferenceEquals(this, other) || (Name == other.Name && _hashSetId == other._hashSetId && _creationTime == other._creationTime &&
                _lastWriteTime == other._lastWriteTime && (IsNew()) ? other.IsNew() : !IsNew() && Id.Equals(other.Id)));
        }

        public override bool Equals(object obj) { return Equals(obj as FileRecord); }

        protected override void OnLoad(SqlCeDataReader dataReader)
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

        protected override void RestorePropertyValues(BaseRecord originalValuesObject)
        {
            throw new NotImplementedException();
        }

        protected override BaseRecord CreateClone()
        {
            throw new NotImplementedException();
        }
    }
}

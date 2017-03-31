using System;
using System.Data.SqlServerCe;
using System.IO;

namespace FileSystemIndexLib
{
    public class DirectoryRecord : FsRecord, IEquatable<DirectoryRecord>
    {
        public const string TableName_Directory = "Directory";
        public const string ColName_VolumeId = "VolumeId";

        private bool _isRoot = true;
        private Guid _volumeId = Guid.Empty;

        public new Guid? ParentId { get { return (_isRoot) ? null : base.ParentId as Guid?; } }

        public Guid VolumeId { get { return _volumeId; } }

        public static DirectoryRecord Load(SqlCeConnection dbConnection, Guid id)
        {
            DirectoryRecord result = new DirectoryRecord();
            if (result.SelectById(dbConnection, id))
                return result;

            return null;
        }

        public static DirectoryRecord Load(SqlCeConnection dbConnection, string name, Guid volumeId)
        {
            DirectoryRecord result = new DirectoryRecord();

            string sql = String.Format("SELECT [{0}], {1} FROM [{2}] WHERE ([{3}] = @{3} AND [{4}] = @{4})", ColName_Id, result.GetSelectQueryFields(), result.GetTableName(), ColName_Name, ColName_VolumeId);
            using (SqlCeCommand command = dbConnection.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.AddWithValue(ColName_Name, name);
                command.Parameters.AddWithValue(ColName_VolumeId, volumeId);
                using (SqlCeDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.HasRows && dataReader.Read())
                    {
                        result = new DirectoryRecord(dataReader.GetGuid(dataReader.GetOrdinal(ColName_Id)), name, volumeId, null);
                        result.OnLoad(dataReader);
                        return result;
                    }
                }
            }

            return null;
        }

        public static DirectoryRecord Load(SqlCeConnection dbConnection, Guid parentId, string name)
        {
            DirectoryRecord result = new DirectoryRecord();

            string sql = String.Format("SELECT [{0}], {1} FROM [{2}] WHERE ([{3}] = @{3} AND [{4}] = @{4})", ColName_Id, result.GetSelectQueryFields(), result.GetTableName(), ColName_ParentId, ColName_Name);
            using (SqlCeCommand command = dbConnection.CreateCommand())
            {
                command.CommandText = sql;
                command.Parameters.AddWithValue(ColName_ParentId, parentId);
                command.Parameters.AddWithValue(ColName_Name, name);
                using (SqlCeDataReader dataReader = command.ExecuteReader())
                {
                    if (dataReader.HasRows && dataReader.Read())
                    {
                        result = new DirectoryRecord(dataReader.GetGuid(dataReader.GetOrdinal(ColName_Id)), name, dataReader.GetGuid(dataReader.GetOrdinal(ColName_VolumeId)), parentId);
                        result.OnLoad(dataReader);
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
                result = Load(dbConnection, directoryInfo.Name, volumeId);
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
            VolumeRecord volumeRecord = VolumeRecord.LoadOrCreate(dbConnection, directoryInfo.Root);
            return LoadOrCreate(dbConnection, directoryInfo, volumeRecord.Id);
        }

        public DirectoryRecord() { }

        public DirectoryRecord(Guid id, string name, Guid volumeId, Guid? parentId) : base(id, name, (parentId.HasValue) ? parentId.Value : Guid.Empty)
        {
            _isRoot = !parentId.HasValue;
            _volumeId = volumeId;
        }


        public DirectoryRecord(string name, Guid volumeId, Guid? parentId) : base(name, (parentId.HasValue) ? parentId.Value : Guid.Empty)
        {
            _isRoot = !parentId.HasValue;
            _volumeId = volumeId;
        }

        public override int GetHashCode() { return Name.GetHashCode(); }

        public override string ToString() { return String.Format("Directory: {0}", Name); }

        public bool Equals(DirectoryRecord other)
        {
            return other != null && (ReferenceEquals(this, other) || (((ParentId.HasValue) ? other.ParentId.HasValue && ParentId.Value.Equals(other.ParentId.Value) : !other.ParentId.HasValue) &&
                _volumeId == other._volumeId && Name == other.Name && (IsNew()) ? other.IsNew() : !IsNew() && Id.Equals(other.Id)));
        }

        public override bool Equals(object obj) { return Equals(obj as VolumeRecord); }

        protected override void OnLoad(SqlCeDataReader dataReader)
        {
            _isRoot = dataReader.IsDBNull(dataReader.GetOrdinal(ColName_ParentId));
            _volumeId = dataReader.GetGuid(dataReader.GetOrdinal(ColName_VolumeId));
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

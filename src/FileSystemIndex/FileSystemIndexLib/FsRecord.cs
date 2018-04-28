using System;
using System.Data.SqlServerCe;

namespace FileSystemIndexLib
{
    public abstract class FsRecord : BaseRecord
    {
        public const string ColName_ParentId = "ParentId";
        public const string ColName_Name = "Name";

        private Guid _parentId = Guid.Empty;
        private string _name = "";

        public Guid ParentId { get { return _parentId; } }

        public string Name { get { return _name; } }

        protected override void OnLoad(SqlCeDataReader dataReader)
        {
            int ordinal = dataReader.GetOrdinal(ColName_ParentId);
            if (dataReader.IsDBNull(ordinal))
                _parentId = Guid.Empty;
            else
                _parentId = dataReader.GetGuid(ordinal);
            _name = dataReader.GetString(dataReader.GetOrdinal(ColName_Name));
        }

        protected FsRecord() { }

        protected FsRecord(Guid id, string name, Guid parentId) : base(id)
        {
            _name = name ?? "";
            _parentId = parentId;
        }

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
}

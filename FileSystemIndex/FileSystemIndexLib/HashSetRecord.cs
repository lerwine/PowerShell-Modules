using System;
using System.Data.SqlServerCe;

namespace FileSystemIndexLib
{
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

        public HashSetRecord(Guid id) : base(id) { }

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
            HashSetRecord result = new HashSetRecord();

            string sql = String.Format("SELECT [{0}], {1} FROM [{2}] WHERE ([{3}] = @{3} AND [{4}] = @{4} AND [{5}] = @{5})", ColName_Id, result.GetSelectQueryFields(),
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
                        result = new HashSetRecord(dataReader.GetGuid(dataReader.GetOrdinal(ColName_Id)));
                        result.OnLoad(dataReader);
                        return result;
                    }
                }
            }

            return null;
        }

        public static HashSetRecord LoadOrCreate(SqlCeConnection dbConnection, bool isValidated, long length, int sha256)
        {
            HashSetRecord result = Load(dbConnection, length, sha256, isValidated);
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

        public override string ToString() { return String.Format("{0:X4}-{1:X4}: {2}", Sha256 >> 16, Sha256 & 0x0000ffff, Length); }

        public override int GetHashCode() { return _sha256; }

        public bool Equals(HashSetRecord other)
        {
            return other != null && (ReferenceEquals(this, other) || (_isValidated == other._isValidated && _length == other._length &&
                _sha256 == other._sha256 && (IsNew()) ? other.IsNew() : !IsNew() && Id.Equals(other.Id)));
        }

        public override bool Equals(object obj) { return Equals(obj as HashSetRecord); }

        protected override void OnLoad(SqlCeDataReader dataReader)
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
}

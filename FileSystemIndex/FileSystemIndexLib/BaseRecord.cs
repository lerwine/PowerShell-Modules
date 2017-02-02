using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.Threading;

namespace FileSystemIndexLib
{
    public abstract class BaseRecord : INotifyPropertyChanged, INotifyPropertyChanging, ICloneable, IDataErrorInfo, IEditableObject, IChangeTracking
    {
        #region Constant column name values

        public const string ColName_Id = "Id";

        #endregion

        #region Fields

        private object _syncRoot = new object();
        private bool _isNew = true;
        private bool _isChanged = true;
        private Guid? _id = null;
        BaseRecord _originalValues = null;
        private bool _changingEditMode = false;
        private Dictionary<string, string> _errors = new Dictionary<string, string>();

        #endregion

        #region Events Handlers

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        #endregion

        #region Constructors

        protected BaseRecord() { }

        protected BaseRecord(Guid id) { _id = id; }

        #endregion

        #region Properties

        #region Database value properties

        public Guid Id { get { return (_id.HasValue) ? _id.Value : Guid.Empty; } }

        #endregion

        #region Explicit properties

        string IDataErrorInfo.Error { get { return GetErrorMessage(); } }
        
        bool IChangeTracking.IsChanged { get { return IsChanged(); } }

        string IDataErrorInfo.this[string columnName] { get { return GetErrorMessage(columnName); } }

        #endregion

        #endregion

        #region Error tracking methods

        public bool HasErrors()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetErrors(string propertyName)
        {
            throw new NotImplementedException();
        }
        
        public string GetErrorMessage(string columnName)
        {
            throw new NotImplementedException();
        }

        public string GetErrorMessage()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Editing and Change tracking methods

        public void BeginEdit()
        {
            Monitor.Enter(_syncRoot);

            try
            {
                if (_changingEditMode)
                    throw new InvalidOperationException("Edit mode cannot be changed in this state");

                _changingEditMode = true;
                try
                {
                    BaseRecord originalValuesObject = Clone();
                    _originalValues = originalValuesObject;
                    OnClonedForBeginEdit(originalValuesObject);
                }
                catch { throw; }
                finally { _changingEditMode = false; }
            }
            catch { throw; }
            finally { Monitor.Exit(_syncRoot); }
        }

        protected virtual void OnClonedForBeginEdit(BaseRecord originalValuesObject) { }

        public void EndEdit()
        {
            Monitor.Enter(_syncRoot);

            try
            {
                if (_changingEditMode)
                    throw new InvalidOperationException("Edit mode cannot be changed in this state");

                _changingEditMode = true;
                try
                {
                    BaseRecord originalValuesObject = _originalValues;
                    _originalValues = _originalValues._originalValues;
                    OnEndEdit(originalValuesObject);
                }
                catch { throw; }
                finally { _changingEditMode = false; }
            }
            catch { throw; }
            finally { Monitor.Exit(_syncRoot); }
        }

        protected virtual void OnEndEdit(BaseRecord originalValuesObject) { }

        public void CancelEdit()
        {
            Monitor.Enter(_syncRoot);

            try
            {
                if (_changingEditMode)
                    throw new InvalidOperationException("Edit mode cannot be changed in this state");

                _changingEditMode = true;
                try
                {
                    BaseRecord originalValuesObject = _originalValues;
                    _originalValues = _originalValues._originalValues;

                    // TODO: Check to see if errors collections have changed and raise events if appropriate.

                    _id = originalValuesObject._id;
                    Dictionary<string, string> prevErrors = new Dictionary<string, string>();
                    foreach (string k in _errors.Keys)
                        prevErrors.Add(k, _errors[k]);

                    Dictionary<string, string> errors = new Dictionary<string, string>();
                    _errors.Clear();
                    foreach (string k in originalValuesObject._errors.Keys)
                    {
                        errors.Add(k, originalValuesObject._errors[k]);
                        _errors.Add(k, originalValuesObject._errors[k]);
                    }

                    bool isChanged = originalValuesObject._isChanged;
                    _isChanged = isChanged;
                    bool isNew = originalValuesObject._isNew;
                    _isNew = isNew;

                    RestorePropertyValues(originalValuesObject);

                    _isChanged = isChanged;
                    _isNew = isNew;
                    _errors.Clear();
                    foreach (string k in errors.Keys)
                        _errors.Add(k, errors[k]);
                }
                catch { throw; }
                finally { _changingEditMode = false; }
            }
            catch { throw; }
            finally { Monitor.Exit(_syncRoot); }
        }

        /// <summary>
        /// This gets called to restore property values on cancelling edit mode.
        /// </summary>
        /// <param name="originalValuesObject">Original property values to restore.</param>
        protected abstract void RestorePropertyValues(BaseRecord originalValuesObject);

        /// <summary>
        /// Indicates whether the item is a new item which has never been saved to the database.
        /// </summary>
        /// <returns>true if the item has ever been saved to the database; otherwise false.</returns>
        public bool IsNew() { return _isNew; }

        public bool IsChanged() { return _isChanged; }

        protected void SetChanged() { _isChanged = true; }

        public void AcceptChanges()
        {
            throw new NotImplementedException();
        }

        #endregion

        public override int GetHashCode() { return (_id.HasValue) ? 0 : _id.Value.GetHashCode(); }

        #region Database interactivity methods

        /// <summary>
        /// This get called whenever data is to be loaded from the database.
        /// </summary>
        /// <param name="dataReader">Data reader object</param>
        protected virtual void OnLoad(SqlCeDataReader dataReader) { }

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
            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = ColName_Id;
            parameter.Value = _id.Value;
            parameter.DbType = System.Data.DbType.Guid;
            command.Parameters.Add(parameter);
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
                        _isChanged = false;
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
                    item._isChanged = false;
                    results.Add(item);
                }
            }

            return results.Count > 0;
        }

        /// <summary>
        /// Saves any changes to the database.
        /// </summary>
        public void SaveChanges(SqlCeConnection dbConnection)
        {
            string sql = (_isNew) ? String.Format("INSERT INTO [{0}] ({1}, {2}) VALUES (@{1}, {3})", GetTableName(), ColName_Id, GetSelectQueryFields(), GetInsertQueryPlaceHolders()) :
                String.Format("UPDATE [{0}] SET {1} WHERE ({2} = @{2})", GetTableName(), GetUpdateQueryFieldAndPlaceHolders(), ColName_Id);
            using (SqlCeCommand command = dbConnection.CreateCommand())
            {
                command.CommandText = sql;
                if (_isNew)
                {
                    AddSelectIdQueryField(command);
                    AddInsertParameters(command.Parameters);
                }
                else
                {
                    AddUpdateParameters(command.Parameters);
                    AddSelectIdQueryField(command);
                }
                command.ExecuteNonQuery();
            }
            _isNew = false;
            _isChanged = false;
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
            _isChanged = true;
        }

        #endregion

        protected abstract BaseRecord CreateClone();

        public BaseRecord Clone() { return CreateClone(); }

        object ICloneable.Clone() { return CreateClone(); }
    }

    public abstract class BaseRecord<TImplemented> : BaseRecord
        where TImplemented : BaseRecord<TImplemented>, new()
    {
        /// <summary>
        /// Creates a new <typeparamref name="TImplemented"/> object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new <typeparamref name="TImplemented"/> object that is a copy of this instance.</returns>
        public new TImplemented Clone() { return CreateClone() as TImplemented; }

        protected override BaseRecord CreateClone()
        {
            TImplemented clone = new TImplemented();
            clone.Initialize((TImplemented)this);
            return clone;
        }

        internal abstract void Initialize(TImplemented source);
    }
}

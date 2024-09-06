using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace LteDev
{
    /// <summary>
    /// Represents a dataset which stores file comparison information
    /// </summary>
    public class FileInfoCatalogDataSet : DataSet
    {
        /// <summary>
        /// XML Prefix for elements within <see cref="FileInfoCatalogDataSet" />.
        /// </summary>
        public const string FileInfoCatalog_Prefix = "";
        
        /// <summary>
        /// XML Namespace for elements within <see cref="FileInfoCatalogDataSet" />.
        /// </summary>
        public const string FileInfoCatalog_Namespace = "";
        
        /// <summary>
        /// XML element name for the root element of <see cref="FileInfoCatalogDataSet" />.
        /// </summary>
        public const string FileInfoCatalog_Name = "FileInfoCatalog";
        
        /// <summary>
        /// XML element name for the <see cref="DirectoryStructureDataTable" />.
        /// </summary>
        public const string TableName_DirectoryStructure = "DirectoryStructure";
        
        /// <summary>
        /// XML element name for the <see cref="FileInfoDataTable" />.
        /// </summary>
        public const string TableName_FileInfo = "FileInfo";

        private readonly DirectoryStructureDataTable _directoryStructure;
        private readonly FileInfoDataTable _files;
        private readonly DataRelation _fk_ParentDirectory;
        private readonly DataRelation _fk_FileDirectory;
        
        /// <summary>
        /// Table containing directory structure in information.
        /// </summary>
        public DirectoryStructureDataTable DirectoryStructure { get { return _directoryStructure; } }

        /// <summary>
        /// Table containing file information.
        /// </summary>
        public FileInfoDataTable Files { get { return _files; } }

        /// <summary>
        /// Relationship for nested directory structures.
        /// </summary>
        public DataRelation FK_ParentDirectory { get { return _fk_ParentDirectory; } }

        /// <summary>
        /// Relationship for a file's parent directory.
        /// </summary>
        /// <value></value>
        public DataRelation FK_FileDirectory { get { return _fk_FileDirectory; } }
        
        /// <summary>
        /// Creates a new <see cref="FileInfoCatalogDataSet" />.
        /// </summary>
        /// <param name="caseSensitive">True if paths are case-sensitive; otherwise, false.</param>
        public FileInfoCatalogDataSet(bool caseSensitive) : base(FileInfoCatalog_Name)
        {
            Namespace = FileInfoCatalog_Namespace;
            Prefix = FileInfoCatalog_Prefix;
            CaseSensitive = caseSensitive;
            _directoryStructure = new DirectoryStructureDataTable();
            _directoryStructure.CaseSensitive = caseSensitive;
            _files = new FileInfoDataTable();
            _files.CaseSensitive = caseSensitive;
            Tables.Add(_directoryStructure);
            Tables.Add(_files);
            _fk_ParentDirectory = Relations.Add("FK_ParentDirectory", _directoryStructure.IDDataColumn, _directoryStructure.ParentIDDataColumn, true);
            _fk_ParentDirectory.Nested = true;
            _fk_FileDirectory = Relations.Add("FK_FileDirectory", _directoryStructure.IDDataColumn, _files.IDDataColumn, true);
        }

        /// <summary>
        /// Deserialize instance of <see cref="FileInfoCatalogDataSet"/>.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected FileInfoCatalogDataSet(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <summary>
        /// Deserialize instance of <see cref="FileInfoCatalogDataSet"/>.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        /// <param name="ConstructSchema"></param>
        protected FileInfoCatalogDataSet(SerializationInfo info, StreamingContext context, bool ConstructSchema) : base(info, context, ConstructSchema) { }

        /// <summary>
        /// Gets the <seealso cref="DirectoryStructureDataRow" /> associated with a <seealso cref="DirectoryInfo" /> object.
        /// </summary>
        /// <param name="directoryInfo">A <seealso cref="DirectoryInfo" /> object.</param>
        /// <param name="doNotAdd">If true, and no data row exists matching the directory, then no row is added; otherwise a new row is inserted and returned.</param>
        /// <returns>The associated <seealso cref="DirectoryStructureDataRow" /> object or null if the row didn't exist and <paramref name="doNotAdd" /> was true,</returns>
        public DirectoryStructureDataRow GetDirectoryRow(DirectoryInfo directoryInfo, bool doNotAdd)
        {
            if (directoryInfo == null)
                return null;
            StringComparer comparer = (CaseSensitive) ? StringComparer.InvariantCulture : StringComparer.InvariantCultureIgnoreCase;
            DirectoryStructureDataRow dataRow;
            if (directoryInfo.Parent == null || directoryInfo.Parent.FullName == directoryInfo.FullName)
            {
                dataRow = GetAllDirectories().FirstOrDefault(d => !d.ParentID.HasValue && comparer.Equals(d.Name, directoryInfo.Name));
                if (dataRow == null && !doNotAdd)
                {
                    dataRow = DirectoryStructure.NewRow();
                    dataRow[DirectoryStructure.NameDataColumn] = directoryInfo.Name;
                    DirectoryStructure.Rows.Add(dataRow);
                    dataRow.SetParentRow(null, _fk_ParentDirectory);
                    dataRow.AcceptChanges();
                }
                return dataRow;
            }
            DirectoryStructureDataRow parentRow = GetDirectoryRow(directoryInfo.Parent, doNotAdd);
            if (parentRow == null)
                return null;
            dataRow = GetAllDirectories().FirstOrDefault(d => d.ParentID.HasValue && d.ParentID.Value.Equals(parentRow.ID) && comparer.Equals(d.Name, directoryInfo.Name));
            if (dataRow == null && !doNotAdd)
            {
                dataRow = DirectoryStructure.NewRow();
                dataRow[DirectoryStructure.NameDataColumn] = directoryInfo.Name;
                DirectoryStructure.Rows.Add(dataRow);
                dataRow.SetParentRow(parentRow, _fk_ParentDirectory);
                dataRow.AcceptChanges();
            }
            return dataRow;
        }

        /// <summary>
        /// Gets the <seealso cref="FileInfoDataRow" /> associated with a <seealso cref="FileInfo" /> object.
        /// </summary>
        /// <param name="fileInfo">A <seealso cref="FileInfo" /> object.</param>
        /// <param name="doNotAdd">If true, and no data row exists matching the directory, then no row is added; otherwise a new row is inserted and returned.</param>
        /// <returns>The associated <seealso cref="FileInfoDataRow" /> object or null if the row didn't exist and <paramref name="doNotAdd" /> was true,</returns>
        public FileInfoDataRow GetFileInfoDataRow(FileInfo fileInfo, bool doNotAdd)
        {
            if (fileInfo == null)
                return null;
            StringComparer comparer = (CaseSensitive) ? StringComparer.InvariantCulture : StringComparer.InvariantCultureIgnoreCase;
            DirectoryStructureDataRow parentRow = GetDirectoryRow(fileInfo.Directory, doNotAdd);
            if (parentRow == null)
                return null;
            FileInfoDataRow dataRow = GetAllFiles().FirstOrDefault(f => f.ParentID.Equals(parentRow.ID) && comparer.Equals(f.Name, fileInfo.Name));
            if (dataRow != null)
            {
                if (dataRow.Name != fileInfo.Name)
                    dataRow[Files.NameDataColumn] = fileInfo.Name;
                if (dataRow.Length != fileInfo.Length)
                    dataRow[Files.LengthDataColumn] = fileInfo.Length;
                DateTime dateTime = FileInfoCatalogUtility.ToDataSetDateTime(fileInfo.CreationTimeUtc, Files.CreationTimeDataColumn.DateTimeMode);
                if (dataRow.CreationTime != dateTime)
                    dataRow[Files.CreationTimeDataColumn] = dateTime;
                dateTime = FileInfoCatalogUtility.ToDataSetDateTime(fileInfo.LastWriteTimeUtc, Files.LastWriteTimeDataColumn.DateTimeMode);
                if (dataRow.LastWriteTime != dateTime)
                    dataRow[Files.LastWriteTimeDataColumn] = dateTime;

            }
            else if (!doNotAdd)
            {
                dataRow = Files.NewRow();
                dataRow[Files.NameDataColumn] = fileInfo.Name;
                dataRow[Files.LengthDataColumn] = fileInfo.Length;
                dataRow[Files.CreationTimeDataColumn] = FileInfoCatalogUtility.ToDataSetDateTime(fileInfo.CreationTimeUtc, Files.CreationTimeDataColumn.DateTimeMode);
                dataRow[Files.LastWriteTimeDataColumn] = FileInfoCatalogUtility.ToDataSetDateTime(fileInfo.LastWriteTimeUtc, Files.LastWriteTimeDataColumn.DateTimeMode);
                Files.Rows.Add(dataRow);
                dataRow.SetParentRow(parentRow, _fk_ParentDirectory);
                dataRow.AcceptChanges();
            }
            return dataRow;
        }

        /// <summary>
        /// Gets all directory rows.
        /// </summary>
        /// <returns>All directory rows.</returns>
        public IEnumerable<DirectoryStructureDataRow> GetAllDirectories() { return DirectoryStructure.Rows.OfType<DirectoryStructureDataRow>(); }

        /// <summary>
        /// Gets all file rows.
        /// </summary>
        /// <returns>All file rows.</returns>
        public IEnumerable<FileInfoDataRow> GetAllFiles() { return Files.Rows.OfType<FileInfoDataRow>(); }

        /// <summary>
        /// Get child rows from a parent.
        /// </summary>
        /// <param name="parentID">ID of parent or null to get root-level directories.</param>
        /// <returns>Child rows.</returns>
        public IEnumerable<IFileSystemDataRow> GetChildren(Guid? parentID)
        {
            if (parentID.HasValue)
                return GetAllDirectories().Where(d => d.ParentID.HasValue && d.ParentID.Value.Equals(parentID.Value)).Cast<IFileSystemDataRow>()
                    .Concat(GetAllFiles().Where(f => f.ParentID.Equals(parentID.Value)).Cast<IFileSystemDataRow>());
            return GetAllDirectories().Where(d => !d.ParentID.HasValue).Cast<IFileSystemDataRow>();
        }

        /// <summary>
        /// Prevents parent directory relationships from being removed.
        /// </summary>
        /// <param name="relation"></param>
        protected override void OnRemoveRelation(DataRelation relation)
        {
            if (relation != null && (ReferenceEquals(relation, _fk_ParentDirectory) || ReferenceEquals(relation, _fk_FileDirectory)))
                throw new NotSupportedException();
        }

        /// <summary>
        /// Prevents file and directory structure tables from being removed.
        /// </summary>
        /// <param name="table"></param>
        protected override void OnRemoveTable(DataTable table)
        {
            if (table != null && (ReferenceEquals(table, _directoryStructure) || ReferenceEquals(table, _files)))
                throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Interface for file and directory rows.
    /// </summary>
    public interface IFileSystemDataRow
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        Guid ID { get; }

        /// <summary>
        /// Unique identifier of parent.
        /// </summary>
        Guid? ParentID { get; }

        /// <summary>
        /// Name of file or directory.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Current state of the row.
        /// </summary>
        DataRowState RowState { get; }

        /// <summary>
        /// Row error.
        /// </summary>
        string RowError { get; set; }

        /// <summary>
        /// Determines whether row has errors.
        /// </summary>
        bool HasErrors { get; }

        /// <summary>
        /// Determines whether the row has a specific version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        bool HasVersion(DataRowVersion version);
    }

    /// <summary>
    /// Data table containing nested directory structure.
    /// </summary>
    public class DirectoryStructureDataTable : DataTable
    {
        #region  Columns

        #region ID

        private readonly DataColumn _idDataColumn;

        /// <summary>
        /// Data column for unique identifier.
        /// </summary>
        public DataColumn IDDataColumn { get { return _idDataColumn; } }

        #endregion

        #region ParentID

        private readonly DataColumn _parentIDDataColumn;

        /// <summary>
        /// Data column for the ID of the parent directory row.
        /// </summary>
        public DataColumn ParentIDDataColumn { get { return _parentIDDataColumn; } }

        #endregion

        #region Name

        private readonly DataColumn _nameDataColumn;

        /// <summary>
        /// Data column for the directory name.
        /// </summary>
        public DataColumn NameDataColumn { get { return _nameDataColumn; } }

        #endregion

        #region UpdatedOn

        private readonly DataColumn _updatedOnDataColumn;

        /// <summary>
        /// Data column for date/time when row was last updated.
        /// </summary>
        public DataColumn UpdatedOnDataColumn { get { return _updatedOnDataColumn; } }

        #endregion

        #endregion

        /// <summary>
        /// Create new directory structure data table.
        /// </summary>
        public DirectoryStructureDataTable() : base(FileInfoCatalogDataSet.TableName_DirectoryStructure, FileInfoCatalogDataSet.FileInfoCatalog_Namespace)
        {
            Prefix = FileInfoCatalogDataSet.FileInfoCatalog_Prefix;
            _idDataColumn = Columns.Add("ID", typeof(Guid));
            _idDataColumn.AllowDBNull = false;
            _idDataColumn.Caption = "ID";
            _idDataColumn.ColumnMapping = MappingType.Attribute;
            _idDataColumn.Namespace = FileInfoCatalogDataSet.FileInfoCatalog_Namespace;
            _idDataColumn.Prefix = FileInfoCatalogDataSet.FileInfoCatalog_Prefix;
            _idDataColumn.ReadOnly = true;
            _nameDataColumn = Columns.Add("Name", typeof(string));
            _nameDataColumn.AllowDBNull = false;
            _nameDataColumn.Caption = "Name";
            _nameDataColumn.ColumnMapping = MappingType.Attribute;
            _nameDataColumn.Namespace = FileInfoCatalogDataSet.FileInfoCatalog_Namespace;
            _nameDataColumn.Prefix = FileInfoCatalogDataSet.FileInfoCatalog_Prefix;
            _nameDataColumn.ReadOnly = false;
            _parentIDDataColumn = Columns.Add("ParentID", typeof(Guid));
            _parentIDDataColumn.AllowDBNull = true;
            _parentIDDataColumn.Caption = "Parent ID";
            _parentIDDataColumn.ColumnMapping = MappingType.Attribute;
            _parentIDDataColumn.Namespace = FileInfoCatalogDataSet.FileInfoCatalog_Namespace;
            _parentIDDataColumn.Prefix = FileInfoCatalogDataSet.FileInfoCatalog_Prefix;
            _parentIDDataColumn.ReadOnly = false;
            _updatedOnDataColumn = Columns.Add("UpdatedOn", typeof(DateTime));
            _updatedOnDataColumn.AllowDBNull = true;
            _updatedOnDataColumn.Caption = "Updated";
            _updatedOnDataColumn.ColumnMapping = MappingType.Attribute;
            _updatedOnDataColumn.Namespace = FileInfoCatalogDataSet.FileInfoCatalog_Namespace;
            _updatedOnDataColumn.Prefix = FileInfoCatalogDataSet.FileInfoCatalog_Prefix;
            _updatedOnDataColumn.ReadOnly = false;
            Constraints.Add("PK_DirectoryStructureID", _idDataColumn, true);
        }

        /// <summary>
        /// Deserialize instance of <see cref="DirectoryStructureDataTable"/>
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected DirectoryStructureDataTable(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <summary>
        /// Create new data row for a directory structure row.
        /// </summary>
        /// <returns></returns>
        public new DirectoryStructureDataRow NewRow() { return (DirectoryStructureDataRow)(base.NewRow()); }

        /// <summary>
        /// Creates a new instance of <see cref="DirectoryStructureDataTable"/>.
        /// </summary>
        /// <returns></returns>
        protected override DataTable CreateInstance() { return new DirectoryStructureDataTable(); }

        /// <summary>
        /// Gets the type of row for this table.
        /// </summary>
        /// <returns></returns>
        protected override Type GetRowType() { return typeof(DirectoryStructureDataRow); }

        /// <summary>
        /// Create new row.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        protected override DataRow NewRowFromBuilder(DataRowBuilder builder) { return new DirectoryStructureDataRow(builder); }
        
        /// <summary>
        /// Prevents required columns from being removed.
        /// </summary>
        /// <param name="column"></param>
        protected override void OnRemoveColumn(DataColumn column)
        {
            if (column != null && (Object.ReferenceEquals(column, _idDataColumn) || Object.ReferenceEquals(column, _nameDataColumn) || Object.ReferenceEquals(column, _parentIDDataColumn)))
                throw new InvalidOperationException();
        }
        
        /// <summary>
        /// Initializes new data rows.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTableNewRow(DataTableNewRowEventArgs e)
        {
            e.Row[_idDataColumn] = Guid.NewGuid();
        }

        /// <summary>
        /// Ensures new data/time values are normalized.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnColumnChanging(DataColumnChangeEventArgs e)
        {
            if ((typeof(DateTime)).IsAssignableFrom(e.Column.DataType) && e.ProposedValue != null && e.ProposedValue is DateTime)
            {
                DateTime dateTime = (DateTime)(e.ProposedValue);
                if (e.Column.DateTimeMode == DataSetDateTime.Local)
                {
                    if (dateTime.Kind == DateTimeKind.Utc)
                        dateTime = dateTime.ToLocalTime();
                    else if (dateTime.Kind == DateTimeKind.Unspecified)
                        dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
                }
                else if (dateTime.Kind == DateTimeKind.Local)
                    dateTime = dateTime.ToUniversalTime();
                else if (dateTime.Kind == DateTimeKind.Unspecified)
                    dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local).ToUniversalTime();
                e.ProposedValue = (dateTime.Millisecond == 0) ? dateTime : new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, 0, dateTime.Kind);
            }
            base.OnColumnChanging(e);
        }

        /// <summary>
        /// Updates row tracking variables.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnColumnChanged(DataColumnChangeEventArgs e)
        {
            base.OnColumnChanged(e);
        }
    }
    
    /// <summary>
    /// Row representing a subdirectory.
    /// </summary>
    public class DirectoryStructureDataRow : DataRow, IFileSystemDataRow
    {
        #region  Properties

        /// <summary>
        /// Parent data table.
        /// </summary>
        public new DirectoryStructureDataTable Table { get { return (DirectoryStructureDataTable)(base.Table); } }
        
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public Guid ID { get { return (Guid)(this[Table.IDDataColumn]); } }

        /// <summary>
        /// Unique identifier of parent row.
        /// </summary>
        public Guid? ParentID
        {
            get
            {
                if (IsNull(Table.ParentIDDataColumn))
                    return null;
                return (Guid)(this[Table.ParentIDDataColumn]);
            }
        }

        /// <summary>
        /// Name of subdirectory.
        /// </summary>
        public string Name { get { return (string)(this[Table.NameDataColumn]); } }

        /// <summary>
        /// Date/time when row was last updated.
        /// </summary>
        public DateTime? UpdatedOn
        {
            get
            {
                DataColumn col = Table.UpdatedOnDataColumn;
                if (IsNull(col))
                    return null;
                DateTime value = (DateTime)(this[col]);
                if (col.DateTimeMode == DataSetDateTime.Local)
                {
                    if (value.Kind == DateTimeKind.Utc)
                        value = value.ToLocalTime();
                    else if (value.Kind == DateTimeKind.Unspecified)
                        value = DateTime.SpecifyKind(value, DateTimeKind.Local);
                }
                else if (value.Kind == DateTimeKind.Utc)
                    value = value.ToLocalTime();
                else if (value.Kind == DateTimeKind.Unspecified)
                    value = DateTime.SpecifyKind(value, DateTimeKind.Utc).ToLocalTime();
                return (value.Millisecond == 0) ? value : new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, 0, value.Kind);
            }
            set
            {
                if (value.HasValue)
                    this[Table.UpdatedOnDataColumn] = value;
                else
                    this.SetNull(Table.UpdatedOnDataColumn);
            }
        }

        #endregion

        /// <summary>
        /// Initializes new <see cref="DirectoryStructureDataRow"/>.
        /// </summary>
        /// <param name="builder"></param>
        protected internal DirectoryStructureDataRow(DataRowBuilder builder) : base(builder) { }

        /// <summary>
        /// Gets parent directory.
        /// </summary>
        /// <returns></returns>
        public DirectoryStructureDataRow? GetParentDirectory()
        {
            DataSet dataSet = Table.DataSet;
            if (dataSet != null && dataSet is FileInfoCatalogDataSet)
                return (DirectoryStructureDataRow)(GetParentRow(((FileInfoCatalogDataSet)dataSet).FK_ParentDirectory));
            return null;
        }

        /// <summary>
        /// Sets parent directory.
        /// </summary>
        /// <param name="row"></param>
        public void SetParentDirectory(DirectoryStructureDataRow row)
        {
            DataSet dataSet = Table.DataSet;
            if (dataSet == null || !(dataSet is FileInfoCatalogDataSet))
                throw new InvalidOperationException("Table does not belong to a FileInfoCatalogDataSet");
            DataRelation dataRelation = ((FileInfoCatalogDataSet)dataSet).FK_ParentDirectory;
            if (row != null)
            {
                DataRow r = GetParentRow(dataRelation);
                if (r != null && ReferenceEquals(r, row))
                    return;
                r = row;
                do
                {
                    if (ReferenceEquals(r, this))
                        throw new InvalidOperationException("Circular directory references cannot be added.");
                } while ((r = r.GetParentRow(dataRelation)) != null);
            }
            else if (!ParentID.HasValue)
                return;
            SetParentRow(row, dataRelation);
        }
    }
    
    /// <summary>
    /// Data table containing file information.
    /// </summary>
    public class FileInfoDataTable : DataTable
    {
        #region  Columns

        #region ID

        private readonly DataColumn _idDataColumn;

        /// <summary>
        /// Data column for unique identifier.
        /// </summary>
        public DataColumn IDDataColumn { get { return _idDataColumn; } }

        #endregion

        #region DuplicateID

        private readonly DataColumn _duplicateIDDataColumn;

        /// <summary>
        /// Data column for identifier of duplicate files
        /// </summary>
        public DataColumn DuplicateIDDataColumn { get { return _duplicateIDDataColumn; } }

        #endregion

        #region ParentID

        private readonly DataColumn _parentIDDataColumn;

        /// <summary>
        /// Data column for the unique identifier of the parent directory row.
        /// </summary>
        public DataColumn ParentIDDataColumn { get { return _parentIDDataColumn; } }

        #endregion

        #region Name

        private readonly DataColumn _nameDataColumn;

        /// <summary>
        /// Data column for the file name.
        /// </summary>
        public DataColumn NameDataColumn { get { return _nameDataColumn; } }
        
        #endregion

        #region CreationTime

        private readonly DataColumn _creationTimeDataColumn;

        /// <summary>
        /// Data column for file creation date/time.
        /// </summary>
        public DataColumn CreationTimeDataColumn { get { return _creationTimeDataColumn; } }

        #endregion

        #region LastWriteTime

        private readonly DataColumn _lastWriteTimeDataColumn;

        /// <summary>
        /// Data column for the last file emodification date/time .
        /// </summary>
        public DataColumn LastWriteTimeDataColumn { get { return _lastWriteTimeDataColumn; } }

        #endregion

        #region UpdatedOn

        private readonly DataColumn _updatedOnDataColumn;

        /// <summary>
        /// Data column for the date/time when the data row was last updated.
        /// </summary>
        public DataColumn UpdatedOnDataColumn { get { return _updatedOnDataColumn; } }

        #endregion

        #region Length

        private readonly DataColumn _lengthDataColumn;

        /// <summary>
        /// Date column for file length.
        /// </summary>
        public DataColumn LengthDataColumn { get { return _lengthDataColumn; } }

        #endregion

        #region MD5High

        private readonly DataColumn _md5HighDataColumn;

        /// <summary>
        /// Data column for the upper 32 bits of the MD5 hash code.
        /// </summary>
        public DataColumn MD5HighDataColumn { get { return _md5HighDataColumn; } }

        #endregion

        #region MD5Low

        private readonly DataColumn _md5LowDataColumn;

        /// <summary>
        /// Data column for the lower 32 bits of the MD5 hash code.
        /// </summary>
        public DataColumn MD5LowDataColumn { get { return _md5LowDataColumn; } }

        #endregion

        #endregion

        /// <summary>
        /// Create new data table for file information.
        /// </summary>
        public FileInfoDataTable() : base(FileInfoCatalogDataSet.TableName_FileInfo, FileInfoCatalogDataSet.FileInfoCatalog_Namespace)
        {
            Prefix = FileInfoCatalogDataSet.FileInfoCatalog_Prefix;
            _idDataColumn = Columns.Add("ID", typeof(Guid));
            _idDataColumn.AllowDBNull = false;
            _idDataColumn.Caption = "ID";
            _idDataColumn.ColumnMapping = MappingType.Attribute;
            _idDataColumn.Namespace = FileInfoCatalogDataSet.FileInfoCatalog_Namespace;
            _idDataColumn.Prefix = FileInfoCatalogDataSet.FileInfoCatalog_Prefix;
            _idDataColumn.ReadOnly = true;
            _nameDataColumn = Columns.Add("Name", typeof(string));
            _nameDataColumn.AllowDBNull = false;
            _nameDataColumn.Caption = "Name";
            _nameDataColumn.ColumnMapping = MappingType.Attribute;
            _nameDataColumn.Namespace = FileInfoCatalogDataSet.FileInfoCatalog_Namespace;
            _nameDataColumn.Prefix = FileInfoCatalogDataSet.FileInfoCatalog_Prefix;
            _nameDataColumn.ReadOnly = false;
            _parentIDDataColumn = Columns.Add("ParentID", typeof(Guid));
            _parentIDDataColumn.AllowDBNull = false;
            _parentIDDataColumn.Caption = "Parent ID";
            _parentIDDataColumn.ColumnMapping = MappingType.Attribute;
            _parentIDDataColumn.Namespace = FileInfoCatalogDataSet.FileInfoCatalog_Namespace;
            _parentIDDataColumn.Prefix = FileInfoCatalogDataSet.FileInfoCatalog_Prefix;
            _parentIDDataColumn.ReadOnly = false;
            _duplicateIDDataColumn = Columns.Add("ComparisonGroupID", typeof(Guid));
            _duplicateIDDataColumn.AllowDBNull = true;
            _duplicateIDDataColumn.Caption = "Comparison Group ID";
            _duplicateIDDataColumn.ColumnMapping = MappingType.Attribute;
            _duplicateIDDataColumn.Namespace = FileInfoCatalogDataSet.FileInfoCatalog_Namespace;
            _duplicateIDDataColumn.Prefix = FileInfoCatalogDataSet.FileInfoCatalog_Prefix;
            _duplicateIDDataColumn.ReadOnly = false;
            _creationTimeDataColumn = Columns.Add("CreationTime", typeof(DateTime));
            _creationTimeDataColumn.AllowDBNull = false;
            _creationTimeDataColumn.Caption = "Created";
            _creationTimeDataColumn.ColumnMapping = MappingType.Attribute;
            _creationTimeDataColumn.Namespace = FileInfoCatalogDataSet.FileInfoCatalog_Namespace;
            _creationTimeDataColumn.Prefix = FileInfoCatalogDataSet.FileInfoCatalog_Prefix;
            _creationTimeDataColumn.ReadOnly = false;
            _lastWriteTimeDataColumn = Columns.Add("LastWriteTime", typeof(DateTime));
            _lastWriteTimeDataColumn.AllowDBNull = false;
            _lastWriteTimeDataColumn.Caption = "Modifed";
            _lastWriteTimeDataColumn.ColumnMapping = MappingType.Attribute;
            _lastWriteTimeDataColumn.Namespace = FileInfoCatalogDataSet.FileInfoCatalog_Namespace;
            _lastWriteTimeDataColumn.Prefix = FileInfoCatalogDataSet.FileInfoCatalog_Prefix;
            _lastWriteTimeDataColumn.ReadOnly = false;
            _updatedOnDataColumn = Columns.Add("UpdatedOn", typeof(DateTime));
            _updatedOnDataColumn.AllowDBNull = true;
            _updatedOnDataColumn.Caption = "Updated";
            _updatedOnDataColumn.ColumnMapping = MappingType.Attribute;
            _updatedOnDataColumn.Namespace = FileInfoCatalogDataSet.FileInfoCatalog_Namespace;
            _updatedOnDataColumn.Prefix = FileInfoCatalogDataSet.FileInfoCatalog_Prefix;
            _updatedOnDataColumn.ReadOnly = false;
            _lengthDataColumn = Columns.Add("Length", typeof(long));
            _lengthDataColumn.AllowDBNull = false;
            _lengthDataColumn.Caption = "Length";
            _lengthDataColumn.ColumnMapping = MappingType.Attribute;
            _lengthDataColumn.Namespace = FileInfoCatalogDataSet.FileInfoCatalog_Namespace;
            _lengthDataColumn.Prefix = FileInfoCatalogDataSet.FileInfoCatalog_Prefix;
            _lengthDataColumn.ReadOnly = false;
            _md5HighDataColumn = Columns.Add("MD5High", typeof(long));
            _md5HighDataColumn.AllowDBNull = false;
            _md5HighDataColumn.Caption = "MD5 High";
            _md5HighDataColumn.ColumnMapping = MappingType.Attribute;
            _md5HighDataColumn.Namespace = FileInfoCatalogDataSet.FileInfoCatalog_Namespace;
            _md5HighDataColumn.Prefix = FileInfoCatalogDataSet.FileInfoCatalog_Prefix;
            _md5HighDataColumn.ReadOnly = false;
            _md5LowDataColumn = Columns.Add("MD5Low", typeof(long));
            _md5LowDataColumn.AllowDBNull = false;
            _md5LowDataColumn.Caption = "MD5 Low";
            _md5LowDataColumn.ColumnMapping = MappingType.Attribute;
            _md5LowDataColumn.Namespace = FileInfoCatalogDataSet.FileInfoCatalog_Namespace;
            _md5LowDataColumn.Prefix = FileInfoCatalogDataSet.FileInfoCatalog_Prefix;
            _md5LowDataColumn.ReadOnly = false;
            Constraints.Add("PK_FileInfoID", _idDataColumn, true);
        }

        /// <summary>
        /// Deserialize instance of <see cref="FileInfoDataTable"/>.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected FileInfoDataTable(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <summary>
        /// Create new file information data row.
        /// </summary>
        /// <returns></returns>
        public new FileInfoDataRow NewRow() { return (FileInfoDataRow)(base.NewRow()); }

        /// <summary>
        /// Create new file information data table.
        /// </summary>
        /// <returns></returns>
        protected override DataTable CreateInstance() { return new FileInfoDataTable(); }

        /// <summary>
        /// Get data row type.
        /// </summary>
        /// <returns></returns>
        protected override Type GetRowType() { return typeof(FileInfoDataRow); }

        /// <summary>
        /// Create new data row.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        protected override DataRow NewRowFromBuilder(DataRowBuilder builder) { return new FileInfoDataRow(builder); }
        
        /// <summary>
        /// Prevents required columns from being removed.
        /// </summary>
        /// <param name="column"></param>
        protected override void OnRemoveColumn(DataColumn column)
        {
            if (column != null && (Object.ReferenceEquals(column, _idDataColumn) || Object.ReferenceEquals(column, _duplicateIDDataColumn) || Object.ReferenceEquals(column, _creationTimeDataColumn) || Object.ReferenceEquals(column, _lastWriteTimeDataColumn) || Object.ReferenceEquals(column, _lengthDataColumn) || Object.ReferenceEquals(column, _md5HighDataColumn) || Object.ReferenceEquals(column, _md5LowDataColumn) || Object.ReferenceEquals(column, _nameDataColumn) || Object.ReferenceEquals(column, _parentIDDataColumn)))
                throw new InvalidOperationException();
        }

        /// <summary>
        /// Initializes new data rows.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTableNewRow(DataTableNewRowEventArgs e)
        {
            e.Row[_idDataColumn] = Guid.NewGuid();
        }

        /// <summary>
        /// Ensures new data/time values are normalized.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnColumnChanging(DataColumnChangeEventArgs e)
        {
            if ((typeof(DateTime)).IsAssignableFrom(e.Column.DataType) && e.ProposedValue != null && e.ProposedValue is DateTime)
            {
                DateTime dateTime = (DateTime)(e.ProposedValue);
                if (e.Column.DateTimeMode == DataSetDateTime.Local)
                {
                    if (dateTime.Kind == DateTimeKind.Utc)
                        dateTime = dateTime.ToLocalTime();
                    else if (dateTime.Kind == DateTimeKind.Unspecified)
                        dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
                }
                else if (dateTime.Kind == DateTimeKind.Local)
                    dateTime = dateTime.ToUniversalTime();
                else if (dateTime.Kind == DateTimeKind.Unspecified)
                    dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local).ToUniversalTime();
                e.ProposedValue = (dateTime.Millisecond == 0) ? dateTime : new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, 0, dateTime.Kind);
            }
            base.OnColumnChanging(e);
        }

        /// <summary>
        /// Updates row tracking variables.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnColumnChanged(DataColumnChangeEventArgs e)
        {
            base.OnColumnChanged(e);
        }
    }
    
    /// <summary>
    /// Data row containing file information.
    /// </summary>
    public class FileInfoDataRow : DataRow, IFileSystemDataRow
    {
        #region  Properties

        /// <summary>
        /// Parent data table.
        /// </summary>
        public new FileInfoDataTable Table { get { return (FileInfoDataTable)(base.Table); } }

        /// <summary>
        /// Unique identifier for file row.
        /// </summary>
        public Guid ID { get { return (Guid)(this[Table.IDDataColumn]); } }

        /// <summary>
        /// Unique identifier of parent directory row.
        /// </summary>
        public Guid ParentID  { get { return (Guid)(this[Table.ParentIDDataColumn]); } }

        Guid? IFileSystemDataRow.ParentID { get { return this.ParentID; } }

        /// <summary>
        /// Name of file.
        /// </summary>
        public string Name { get { return (string)(this[Table.NameDataColumn]); } }
        
        /// <summary>
        /// Unique identifier that is used to associate duplicate files.
        /// </summary>
        public Guid? DuplicateID
        {
            get
            {
                if (IsNull(Table.DuplicateIDDataColumn))
                    return null;
                return (Guid)(this[Table.DuplicateIDDataColumn]);
            }
            set
            {
                if (value.HasValue)
                    this[Table.DuplicateIDDataColumn] = value.Value;
                else
                    this.SetNull(Table.DuplicateIDDataColumn);
            }
        }

        /// <summary>
        /// Date/Time of file creation.
        /// </summary>
        public DateTime CreationTime
        {
            get
            {
                DataColumn col = Table.CreationTimeDataColumn;
                DateTime value = (DateTime)(this[col]);
                if (col.DateTimeMode == DataSetDateTime.Local)
                {
                    if (value.Kind == DateTimeKind.Utc)
                        value = value.ToLocalTime();
                    else if (value.Kind == DateTimeKind.Unspecified)
                        value = DateTime.SpecifyKind(value, DateTimeKind.Local);
                }
                else if (value.Kind == DateTimeKind.Utc)
                    value = value.ToLocalTime();
                else if (value.Kind == DateTimeKind.Unspecified)
                    value = DateTime.SpecifyKind(value, DateTimeKind.Utc).ToLocalTime();
                return (value.Millisecond == 0) ? value : new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, 0, value.Kind);
            }
            set { this[Table.CreationTimeDataColumn] = value; }
        }

        /// <summary>
        /// Date/time of lst file modification.
        /// </summary>
        public DateTime LastWriteTime
        {
            get
            {
                DataColumn col = Table.LastWriteTimeDataColumn;
                DateTime value = (DateTime)(this[col]);
                if (col.DateTimeMode == DataSetDateTime.Local)
                {
                    if (value.Kind == DateTimeKind.Utc)
                        value = value.ToLocalTime();
                    else if (value.Kind == DateTimeKind.Unspecified)
                        value = DateTime.SpecifyKind(value, DateTimeKind.Local);
                }
                else if (value.Kind == DateTimeKind.Utc)
                    value = value.ToLocalTime();
                else if (value.Kind == DateTimeKind.Unspecified)
                    value = DateTime.SpecifyKind(value, DateTimeKind.Utc).ToLocalTime();
                return (value.Millisecond == 0) ? value : new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, 0, value.Kind);
            }
            set { this[Table.LastWriteTimeDataColumn] = value; }
        }
        
        /// <summary>
        /// Date/Time when row was last updated.
        /// </summary>
        public DateTime? UpdatedOn
        {
            get
            {
                DataColumn col = Table.UpdatedOnDataColumn;
                if (IsNull(col))
                    return null;
                DateTime value = (DateTime)(this[col]);
                if (col.DateTimeMode == DataSetDateTime.Local)
                {
                    if (value.Kind == DateTimeKind.Utc)
                        value = value.ToLocalTime();
                    else if (value.Kind == DateTimeKind.Unspecified)
                        value = DateTime.SpecifyKind(value, DateTimeKind.Local);
                }
                else if (value.Kind == DateTimeKind.Utc)
                    value = value.ToLocalTime();
                else if (value.Kind == DateTimeKind.Unspecified)
                    value = DateTime.SpecifyKind(value, DateTimeKind.Utc).ToLocalTime();
                return (value.Millisecond == 0) ? value : new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, 0, value.Kind);
            }
            set
            {
                if (value.HasValue)
                    this[Table.UpdatedOnDataColumn] = value;
                else
                    this.SetNull(Table.UpdatedOnDataColumn);
            }
        }

        /// <summary>
        /// Length of file
        /// </summary>
        public long Length
        {
            get { return (long)(this[Table.LengthDataColumn]); }
            set { this[Table.LengthDataColumn] = value; }
        }

        /// <summary>
        /// Upper 32 bits of MD5 checksum or zero if no checksum was calculated.
        /// </summary>
        public long MD5High
        {
            get { return (long)(this[Table.MD5HighDataColumn]); }
            set { this[Table.MD5HighDataColumn] = value; }
        }

        /// <summary>
        /// Lower 32 bits of MD5 checksum or zero if no checksum was calculated.
        /// </summary>
        public long MD5Low
        {
            get { return (long)(this[Table.MD5LowDataColumn]); }
            set { this[Table.MD5LowDataColumn] = value; }
        }

        #endregion

        /// <summary>
        /// Initialize new <see cref="FileInfoDataRow"/>.
        /// </summary>
        /// <param name="builder"></param>
        protected internal FileInfoDataRow(DataRowBuilder builder) : base(builder) { }

        /// <summary>
        /// Get prent directory row.
        /// </summary>
        /// <returns></returns>
        public DirectoryStructureDataRow GetParentDirectory()
        {
            DataSet dataSet = Table.DataSet;
            if (dataSet != null && dataSet is FileInfoCatalogDataSet)
                return (DirectoryStructureDataRow)(GetParentRow(((FileInfoCatalogDataSet)dataSet).FK_FileDirectory));
            return null;
        }

        /// <summary>
        /// Set parent directory row.
        /// </summary>
        /// <param name="row"></param>
        public void SetParentDirectory(DirectoryStructureDataRow row)
        {
            DataSet dataSet = Table.DataSet;
            if (dataSet == null || !(dataSet is FileInfoCatalogDataSet))
                throw new InvalidOperationException("Table does not belong to a FileInfoCatalogDataSet");
            DataRelation dataRelation = ((FileInfoCatalogDataSet)dataSet).FK_FileDirectory;
            if (row != null)
            {
                DataRow r = GetParentRow(dataRelation);
                if (r != null && ReferenceEquals(r, row))
                    return;
                r = row;
                do
                {
                    if (ReferenceEquals(r, this))
                        throw new InvalidOperationException("Circular directory references cannot be added.");
                } while ((r = r.GetParentRow(dataRelation)) != null);
            }
            SetParentRow(row, dataRelation);
        }
    }
    
    /// <summary>
    /// Utility methods
    /// </summary>
    public static class FileInfoCatalogUtility
    {
        /// <summary>
        /// Parses file base name and extension.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string GetNameParts(string name, out string extension)
        {
            ArgumentNullException.ThrowIfNull(name);
            int i = name.LastIndexOf('.');
            if (i < 1)
            {
                extension = "";
                return name;
            }

            extension = (i == name.Length - 1) ? "." : name[(i + 1)..];
            return name[..i];
        }

        /// <summary>
        /// Normalizes date/time value so it has zero milliseconds.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime NormalizeDateTime(DateTime value)
        {
            if (value.Millisecond == 0)
                return value;
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, 0, value.Kind);
        }

        /// <summary>
        /// Convert to local time.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="assumeUnspecifiedIsLocal"></param>
        /// <returns></returns>
        public static DateTime ToLocalTime(DateTime value, bool assumeUnspecifiedIsLocal = false)
        {
            if (value.Kind == DateTimeKind.Unspecified)
            {
                if (assumeUnspecifiedIsLocal)
                    return NormalizeDateTime(DateTime.SpecifyKind(value, DateTimeKind.Local));
                return NormalizeDateTime(DateTime.SpecifyKind(value, DateTimeKind.Utc).ToLocalTime());
            }

            return NormalizeDateTime((value.Kind == DateTimeKind.Local) ? value : value.ToLocalTime());   
        }

        /// <summary>
        /// Converts from data set date/time
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static DateTime FromDataSetDateTime(DateTime value, DataSetDateTime mode)
        {
            if (value.Kind == DateTimeKind.Unspecified)
            {
                if (mode == DataSetDateTime.Local || mode == DataSetDateTime.UnspecifiedLocal)
                    return NormalizeDateTime(DateTime.SpecifyKind(value, DateTimeKind.Local));
                return NormalizeDateTime(DateTime.SpecifyKind(value, DateTimeKind.Utc).ToLocalTime());
            }
            
            return NormalizeDateTime((value.Kind == DateTimeKind.Local) ? value : value.ToLocalTime());
        }

        /// <summary>
        /// Convert to universal time.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="assumeUnspecifiedIsUtc"></param>
        /// <returns></returns>
        public static DateTime ToUniversalTime(DateTime value, bool assumeUnspecifiedIsUtc = false)
        {
            if (value.Kind == DateTimeKind.Unspecified)
            {
                if (assumeUnspecifiedIsUtc)
                    return NormalizeDateTime(DateTime.SpecifyKind(value, DateTimeKind.Utc));
                return NormalizeDateTime(DateTime.SpecifyKind(value, DateTimeKind.Local).ToUniversalTime());
            }

            return NormalizeDateTime((value.Kind == DateTimeKind.Utc) ? value : value.ToUniversalTime());   
        }

        /// <summary>
        /// Converts to data set date/time
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mode"></param>
        /// <param name="assumeUnspecifiedIsLocal"></param>
        /// <returns></returns>
        public static DateTime ToDataSetDateTime(DateTime value, DataSetDateTime mode, bool assumeUnspecifiedIsLocal = false)
        {
            if (value.Kind == DateTimeKind.Unspecified)
            {
                if (assumeUnspecifiedIsLocal)
                {
                    if (mode == DataSetDateTime.Local || mode == DataSetDateTime.UnspecifiedLocal)
                        return NormalizeDateTime(DateTime.SpecifyKind(value, DateTimeKind.Local));
                    return NormalizeDateTime(DateTime.SpecifyKind(value, DateTimeKind.Local).ToUniversalTime());
                }
                
                if (mode == DataSetDateTime.Local || mode == DataSetDateTime.UnspecifiedLocal)
                    return NormalizeDateTime(DateTime.SpecifyKind(value, DateTimeKind.Utc).ToLocalTime());
                return NormalizeDateTime(DateTime.SpecifyKind(value, DateTimeKind.Utc));
            }
            
            if (mode == DataSetDateTime.Local || mode == DataSetDateTime.UnspecifiedLocal)
                return NormalizeDateTime((value.Kind == DateTimeKind.Utc) ? value.ToLocalTime() : value);

            return NormalizeDateTime((value.Kind == DateTimeKind.Utc) ? value : value.ToUniversalTime());
        }

        /// <summary>
        /// Determines whether a string needs to be encoded to be a valid column name.
        /// </summary>
        public static readonly Regex ColumnEncodeNameTestRegex = new Regex(@"^\d|[~()#\\/=><+\-*%&|^'""\[\]]", RegexOptions.Compiled);

        /// <summary>
        /// Characters to escape when encoding column names.
        /// </summary>
        public static readonly Regex ColumnNameReplaceRegex = new Regex(@"[\\\[\]]", RegexOptions.Compiled);

        /// <summary>
        /// Replace pattern for encoding LIKE statements.
        /// </summary>
        public static readonly Regex LikeWCReplaceRegex = new Regex(@"[*%\[\]]", RegexOptions.Compiled);

        /// <summary>
        /// Escapes text for inclusion in a LIKE statement.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EscapeLikeText(string text)
        {
            ArgumentNullException.ThrowIfNull(text);
            return LikeWCReplaceRegex.Replace(text, m => "[" + m.Value + "]");
        }

        /// <summary>
        /// Encodes a string as a column name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string EncodeColumnName(string name)
        {
            ArgumentNullException.ThrowIfNull(name);
            if (name.Length == 0 || ColumnEncodeNameTestRegex.IsMatch(name))
                return "[" + ColumnNameReplaceRegex.Replace(name, e => "\\" + e.Value) + "]";
            return name;
        }

        /// <summary>
        /// Encodes a query string value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="noEnclosingQuotes"></param>
        /// <returns></returns>
        public static string EncodeQueryValue(string value, bool noEnclosingQuotes)
        {
            ArgumentNullException.ThrowIfNull(value);
            return (noEnclosingQuotes) ? value.Replace("'", "''") : "'" + value.Replace("'", "''") + "'";
        }

        /// <summary>
        /// Encodes a query string value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeQueryValue(string value) { return EncodeQueryValue(value, false); }

        /// <summary>
        /// Encodes a query byte value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeQueryValue(byte value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Encodes a query signed byte value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeQueryValue(sbyte value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Encodes a query short integer value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeQueryValue(short value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Encodes a query unsigned short integer value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeQueryValue(ushort value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Encodes a query integer value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeQueryValue(int value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Encodes a query unsigned integer value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeQueryValue(uint value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Encodes a query long value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeQueryValue(long value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Encodes a query unsigned long value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeQueryValue(ulong value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture); 
        }

        /// <summary>
        /// Encodes a query single-precision value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeQueryValue(float value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Encodes a query double-precision value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeQueryValue(double value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Encodes a query decimal value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeQueryValue(decimal value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Encodes a query boolean value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeQueryValue(bool value) { return (value) ? "true" : "false"; }

        /// <summary>
        /// Encodes a query character value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="noEnclosingQuotes"></param>
        /// <returns></returns>
        public static string EncodeQueryValue(char value, bool noEnclosingQuotes)
        {
            if (value == '\'')
                return (noEnclosingQuotes) ? "''" : "''''";
            return new string((noEnclosingQuotes) ? new char[] { value } : new char[] { '\'', value, '\''});
        }

        /// <summary>
        /// Encodes a query character value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeQueryValue(char value) { return EncodeQueryValue(value, false); }

        /// <summary>
        /// Encodes a query DateTime value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeQueryValue(DateTime value)
        {
            return ToUniversalTime(value).ToString("#yyyy-MM-dd HH:mm:ss#");
        }

        /// <summary>
        /// Encodes a convertible query value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeQueryValue(IConvertible value) { return EncodeQueryValue(value, false); }

        /// <summary>
        /// Encodes a query value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="noEnclosingQuotes"></param>
        /// <returns></returns>
        public static string EncodeQueryValue(IConvertible value, bool noEnclosingQuotes)
        {
            if (value is string)
                return EncodeQueryValue((string)value, noEnclosingQuotes);
            if (value is bool)
                return EncodeQueryValue((string)value);
            if (value is char)
                return EncodeQueryValue((string)value, noEnclosingQuotes);
            if (value is byte)
                return EncodeQueryValue((string)value);
            if (value is sbyte)
                return EncodeQueryValue((string)value);
            if (value is short)
                return EncodeQueryValue((string)value);
            if (value is ushort)
                return EncodeQueryValue((string)value);
            if (value is int)
                return EncodeQueryValue((string)value);
            if (value is uint)
                return EncodeQueryValue((string)value);
            if (value is long)
                return EncodeQueryValue((string)value);
            if (value is ulong)
                return EncodeQueryValue((string)value);
            if (value is float)
                return EncodeQueryValue((string)value);
            if (value is double)
                return EncodeQueryValue((string)value);
            if (value is decimal)
                return EncodeQueryValue((string)value);
            if (value is DateTime)
                return EncodeQueryValue((string)value);
            switch (value.GetTypeCode())
            {
                case TypeCode.Boolean:
                    return EncodeQueryValue(Convert.ToBoolean(value, CultureInfo.InvariantCulture));
                case TypeCode.Byte:
                    return EncodeQueryValue(Convert.ToByte(value, CultureInfo.InvariantCulture));
                case TypeCode.Char:
                    return EncodeQueryValue(Convert.ToChar(value, CultureInfo.InvariantCulture), noEnclosingQuotes);
                case TypeCode.DateTime:
                    return EncodeQueryValue(Convert.ToDateTime(value, CultureInfo.InvariantCulture));
                case TypeCode.Decimal:
                    return EncodeQueryValue(Convert.ToDecimal(value, CultureInfo.InvariantCulture));
                case TypeCode.Double:
                    return EncodeQueryValue(Convert.ToDouble(value, CultureInfo.InvariantCulture));
                case TypeCode.Int16:
                    return EncodeQueryValue(Convert.ToInt16(value, CultureInfo.InvariantCulture));
                case TypeCode.Int32:
                    return EncodeQueryValue(Convert.ToInt32(value, CultureInfo.InvariantCulture));
                case TypeCode.Int64:
                    return EncodeQueryValue(Convert.ToInt64(value, CultureInfo.InvariantCulture));
                case TypeCode.SByte:
                    return EncodeQueryValue(Convert.ToSByte(value, CultureInfo.InvariantCulture));
                case TypeCode.Single:
                    return EncodeQueryValue(Convert.ToSingle(value, CultureInfo.InvariantCulture));
                case TypeCode.String:
                case TypeCode.Object:
                    return EncodeQueryValue(Convert.ToString(value, CultureInfo.InvariantCulture), noEnclosingQuotes);
                case TypeCode.UInt16:
                    return EncodeQueryValue(Convert.ToUInt16(value, CultureInfo.InvariantCulture));
                case TypeCode.UInt32:
                    return EncodeQueryValue(Convert.ToUInt32(value, CultureInfo.InvariantCulture));
                case TypeCode.UInt64:
                    return EncodeQueryValue(Convert.ToUInt64(value, CultureInfo.InvariantCulture));
            }
            return "";
        }

        /// <summary>
        /// Encodes a query byte value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="noEnclosingQuotes"></param>
        /// <returns></returns>
        public static string EncodeQueryValue(object value, bool noEnclosingQuotes)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value is IConvertible convertible)
                return EncodeQueryValue(convertible, noEnclosingQuotes);
            return EncodeQueryValue(value.ToString() ?? "", noEnclosingQuotes);
        }

        /// <summary>
        /// Encodes a query value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeQueryValue(object value) { return EncodeQueryValue(value, false); }

        /// <summary>
        /// Gets a query string to test of a value is null.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static string GetIsNullQueryString(string column)
        {
            return "IsNull(" + EncodeColumnName(column) + ")";
        }
        
        /// <summary>
        /// Gets a query string to test whether a value is not null.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static string GetIsNotNullQueryString(string column)
        {
            return "NOT IsNull(" + EncodeColumnName(column) + ")";
        }

        /// <summary>
        /// Gets a LIKE query string.
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetLikeQueryString(string column, string value)
        {
            ArgumentNullException.ThrowIfNull(value);

            return EncodeColumnName(column) + " LIKE '" + value.Replace("'", "''") + "'";
        }

        /// <summary>
        /// Gets a query string
        /// </summary>
        /// <param name="column"></param>
        /// <param name="op"></param>
        /// <param name="value"></param>
        /// <param name="isEscaped"></param>
        /// <returns></returns>
        public static string GetQueryString(string column, QueryOperator op, string value, bool isEscaped)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (isEscaped)
            {
                switch (op)
                {
                    case QueryOperator.LessThan:
                            return EncodeColumnName(column) + "=" + value;
                    case QueryOperator.GreaterThan:
                            return EncodeColumnName(column) + "=" + value;
                    case QueryOperator.NotEquals:
                        return EncodeColumnName(column) + "=" + value;
                    case QueryOperator.NotLessThan:
                        return EncodeColumnName(column) + "=" + value;
                    case QueryOperator.NotGreaterThan:
                        return EncodeColumnName(column) + "=" + value;
                }
                
                return EncodeColumnName(column) + "=" + value;
            }
            
            switch (op)
            {
                case QueryOperator.LessThan:
                        return EncodeColumnName(column) + "=" + EncodeQueryValue(value);
                case QueryOperator.GreaterThan:
                        return EncodeColumnName(column) + "=" + EncodeQueryValue(value);
                case QueryOperator.NotEquals:
                    return EncodeColumnName(column) + "=" + EncodeQueryValue(value);
                case QueryOperator.NotLessThan:
                    return EncodeColumnName(column) + "=" + EncodeQueryValue(value);
                case QueryOperator.NotGreaterThan:
                    return EncodeColumnName(column) + "=" + EncodeQueryValue(value);
            }
            
            return EncodeColumnName(column) + "=" + EncodeQueryValue(value);
        }

        /// <summary>
        /// Gets a query string
        /// </summary>
        /// <param name="column"></param>
        /// <param name="op"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetQueryString(string column, QueryOperator op, object value)
        {
            if (value == null || value is DBNull)
                throw new ArgumentNullException(nameof(value));
            switch (op)
            {
                case QueryOperator.LessThan:
                        return EncodeColumnName(column) + "=" + EncodeQueryValue(value);
                case QueryOperator.GreaterThan:
                        return EncodeColumnName(column) + "=" + EncodeQueryValue(value);
                case QueryOperator.NotEquals:
                    return EncodeColumnName(column) + "=" + EncodeQueryValue(value);
                case QueryOperator.NotLessThan:
                    return EncodeColumnName(column) + "=" + EncodeQueryValue(value);
                case QueryOperator.NotGreaterThan:
                    return EncodeColumnName(column) + "=" + EncodeQueryValue(value);
            }
            
            return EncodeColumnName(column) + "=" + EncodeQueryValue(value);
        }
    }

    /// <summary>
    /// Query operators
    /// </summary>
    public enum QueryOperator
    {
        /// <summary>
        /// 
        /// </summary>
        Equals,

        /// <summary>
        /// 
        /// </summary>
        LessThan,

        /// <summary>
        /// 
        /// </summary>
        GreaterThan,

        /// <summary>
        /// 
        /// </summary>
        NotEquals,

        /// <summary>
        /// 
        /// </summary>
        NotLessThan,

        /// <summary>
        /// 
        /// </summary>
        NotGreaterThan
    }
}
using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;

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

        private DirectoryStructureDataTable _directoryStructure;
        private FileInfoDataTable _files;
        private DataRelation _fk_ParentDirectory;
        private DataRelation _fk_FileDirectory;
        
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
            this.Namespace = FileInfoCatalog_Namespace;
            this.Prefix = FileInfoCatalog_Prefix;
            this.CaseSensitive = caseSensitive;
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
                {
                    dataRow[Files.NameDataColumn] = fileInfo.Name;
                    dataRow.AcceptChanges();
                }
                if (dataRow.Length != fileInfo.Length)
                    dataRow[Files.LengthDataColumn] = fileInfo.Length;
                DateTime dateTime = FileInfoCatalogUtility.ToDataSetDateTime(fileInfo.CreationTimeUtc, Files.CreationTimeDataColumn.DateTimeMode);
                if (dataRow.CreationTime != dateTime)
                    dataRow[Files.CreationTimeDataColumn] = dateTime;
                dateTime = FileInfoCatalogUtility.ToDataSetDateTime(fileInfo.LastWriteTimeUtc, Files.LastWriteTimeDataColumn.DateTimeMode);
                if (dataRow.LastWriteTime != dateTime)
                    dataRow[Files.LastWriteTimeDataColumn] = dateTime;
                if (dataRow.ComparisonGroupID.HasValue && GetComparisonGroup(dataRow.ComparisonGroupID).Any(f => f.ID.Equals(dataRow.ID)))
                {
                    dataRow.SetParentRow(null, )
                }
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

        public string GetFullName(DirectoryStructureDataRow dataRow)
        {
            if (dataRow == null)
                return "";

            DirectoryStructureDataRow parentRow = (DirectoryStructureDataRow)(dataRow.GetParentRow(_fk_ParentDirectory));
            if (parentRow == null)
                return dataRow.Name;
            return Path.Combine(GetFullName(parentRow), dataRow.Name);
        }

        public string GetFullName(FileInfoDataRow dataRow)
        {
            if (dataRow == null)
                return "";

            return Path.Combine(GetFullName((DirectoryStructureDataRow)(dataRow.GetParentRow(_fk_FileDirectory))), dataRow.Name);
        }

        public IEnumerable<DirectoryStructureDataRow> GetAllDirectories() { return DirectoryStructure.Rows.OfType<DirectoryStructureDataRow>(); }

        public IEnumerable<FileInfoDataRow> GetAllFiles() { return Files.Rows.OfType<FileInfoDataRow>(); }

        public IEnumerable<IFileSystemDataRow> GetChildren(Guid? parentID)
        {
            if (parentID.HasValue)
                return GetAllDirectories().Where(d => d.ParentID.HasValue && d.ParentID.Value.Equals(parentID.Value)).Cast<IFileSystemDataRow>()
                    .Concat(GetAllFiles().Where(f => f.ParentID.Equals(parentID.Value)).Cast<IFileSystemDataRow>());
            return GetAllDirectories().Where(d => !d.ParentID.HasValue).Cast<IFileSystemDataRow>();
        }

        public IEnumerable<FileInfoDataRow> GetComparisonGroup(Guid? comparisonGroupID)
        {
            if (comparisonGroupID.HasValue)
                return GetAllFiles().Where(f => f.ComparisonGroupID.HasValue && f.ComparisonGroupID.Value.Equals(comparisonGroupID.Value));
            return GetAllFiles().Where(f => !f.ComparisonGroupID.HasValue);
        }

        public void SetFilesEqual(FileInfoDataRow x, FileInfoDataRow y)
        {
            if (x == null)
            {
                if (y == null)
                    return;
                x = y;
                y = null;
            }
            else if (y != null)
            {
                if (x.ID.Equals(y.ID))
                    y = null;
                else
                {
                    if (x.Length != y.Length)
                        throw new InvalidOperationException("Length does not match");
                    if (x.MD5High != y.MD5High || x.MD5Low != y.MD5Low)
                        throw new InvalidOperationException("MD5 hash does not match");
                    if (x.ComparisonGroupID.HasValue)
                    {
                        if (y.ComparisonGroupID.HasValue)
                        {
                            if (x.ComparisonGroupID.Value.Equals(y.ComparisonGroupID.Value))
                                return;
                            throw new InvalidOperationException("Both files belong to other comparison groups");
                        }
                        y.ComparisonGroupID = x.ComparisonGroupID;
                    }
                    else
                    {
                        if (!y.ComparisonGroupID.HasValue)
                            y.ComparisonGroupID = Guid.NewGuid();
                        x.ComparisonGroupID = y.ComparisonGroupID;
                    }
                    return;
                }
            }

            FileInfoDataRow[] comparisonGroup = GetComparisonGroup(x.ComparisonGroupID).Where(i => !i.ID.Equals(x.ID)).Take(2).ToArray();
            x.ComparisonGroupID = null;
            if (comparisonGroup.Length == 1)
                comparisonGroup[0].ComparisonGroupID = null;
        }

        protected FileInfoCatalogDataSet(SerializationInfo info, StreamingContext context) : base(info, context) { }

        protected FileInfoCatalogDataSet(SerializationInfo info, StreamingContext context, bool ConstructSchema) : base(info, context, ConstructSchema) { }

        protected override void OnRemoveRelation(DataRelation relation)
        {
            if (relation != null && (ReferenceEquals(relation, _fk_ParentDirectory) || ReferenceEquals(relation, _fk_FileDirectory)))
                throw new NotSupportedException();
        }

        protected override void OnRemoveTable(DataTable table)
        {
            if (table != null && (ReferenceEquals(table, _directoryStructure) || ReferenceEquals(table, _files)))
                throw new NotSupportedException();
        }
    }

    public interface IFileSystemDataRow
    {
        Guid ID { get; }
        Guid? ParentID { get; }
        string Name { get; }
        DataRowState RowState { get; }
        string RowError { get; set; }
        bool HasErrors { get; }
        bool HasVersion(DataRowVersion version);
    }

    public class DirectoryStructureDataTable : DataTable
    {
        #region  Columns

        #region ID
        private DataColumn _idDataColumn = null;
        public DataColumn IDDataColumn { get { return _idDataColumn; } }

        #endregion

        #region ParentID
        private DataColumn _parentIDDataColumn = null;
        public DataColumn ParentIDDataColumn { get { return _parentIDDataColumn; } }

        #endregion

        #region Name
        private DataColumn _nameDataColumn = null;
        public DataColumn NameDataColumn { get { return _nameDataColumn; } }

        #endregion

        #endregion

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
            Constraints.Add("PK_DirectoryStructureID", _idDataColumn, true);
        }

        protected DirectoryStructureDataTable(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public new DirectoryStructureDataRow NewRow() { return (DirectoryStructureDataRow)(base.NewRow()); }

        protected override DataTable CreateInstance() { return new DirectoryStructureDataTable(); }

        protected override Type GetRowType() { return typeof(DirectoryStructureDataRow); }

        protected override DataRow NewRowFromBuilder(DataRowBuilder builder) { return new DirectoryStructureDataRow(builder); }
        
        protected override void OnRemoveColumn(DataColumn column)
        {
            if (column != null && (Object.ReferenceEquals(column, _idDataColumn) || Object.ReferenceEquals(column, _nameDataColumn) || Object.ReferenceEquals(column, _parentIDDataColumn)))
                throw new InvalidOperationException();
        }
        
        protected override void OnTableNewRow(DataTableNewRowEventArgs e)
        {
            e.Row[_idDataColumn] = Guid.NewGuid();
        }
    }
    
    public class DirectoryStructureDataRow : DataRow, IFileSystemDataRow
    {
        #region  Properties

        public new DirectoryStructureDataTable Table { get { return (DirectoryStructureDataTable)(base.Table); } }
        
        public Guid ID { get { return (Guid)(this[Table.IDDataColumn]); } }

        public Guid? ParentID
        {
            get
            {
                if (IsNull(Table.ParentIDDataColumn))
                    return null;
                return (Guid)(this[Table.ParentIDDataColumn]);
            }
        }

        public string Name { get { return (string)(this[Table.NameDataColumn]); } }

        #endregion
        
        protected internal DirectoryStructureDataRow(DataRowBuilder builder) : base(builder) { }
    }
    
    public class FileInfoDataTable : DataTable
    {
        #region  Columns

        #region ID
        private DataColumn _idDataColumn = null;
        public DataColumn IDDataColumn { get { return _idDataColumn; } }

        #endregion

        #region ComparisonGroupID
        private DataColumn _comparisonGroupIDDataColumn = null;
        public DataColumn ComparisonGroupIDDataColumn { get { return _comparisonGroupIDDataColumn; } }

        #endregion

        #region ParentID
        private DataColumn _parentIDDataColumn = null;
        public DataColumn ParentIDDataColumn { get { return _parentIDDataColumn; } }

        #endregion

        #region Name
        private DataColumn _nameDataColumn = null;
        public DataColumn NameDataColumn { get { return _nameDataColumn; } }

        #endregion

        #region CreationTime
        private DataColumn _creationTimeDataColumn = null;
        public DataColumn CreationTimeDataColumn { get { return _creationTimeDataColumn; } }

        #endregion

        #region LastWriteTime
        private DataColumn _lastWriteTimeDataColumn = null;
        public DataColumn LastWriteTimeDataColumn { get { return _lastWriteTimeDataColumn; } }

        #endregion

        #region UpdatedOn
        private DataColumn _updatedOnDataColumn = null;
        public DataColumn UpdatedOnDataColumn { get { return _updatedOnDataColumn; } }

        #endregion

        #region Length
        private DataColumn _lengthDataColumn = null;
        public DataColumn LengthDataColumn { get { return _lengthDataColumn; } }

        #endregion

        #region MD5High
        private DataColumn _md5HighDataColumn = null;
        public DataColumn MD5HighDataColumn { get { return _md5HighDataColumn; } }

        #endregion

        #region MD5Low
        private DataColumn _md5LowDataColumn = null;
        public DataColumn MD5LowDataColumn { get { return _md5LowDataColumn; } }

        #endregion

        #endregion

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
            _comparisonGroupIDDataColumn = Columns.Add("ComparisonGroupID", typeof(Guid));
            _comparisonGroupIDDataColumn.AllowDBNull = true;
            _comparisonGroupIDDataColumn.Caption = "Comparison Group ID";
            _comparisonGroupIDDataColumn.ColumnMapping = MappingType.Attribute;
            _comparisonGroupIDDataColumn.Namespace = FileInfoCatalogDataSet.FileInfoCatalog_Namespace;
            _comparisonGroupIDDataColumn.Prefix = FileInfoCatalogDataSet.FileInfoCatalog_Prefix;
            _comparisonGroupIDDataColumn.ReadOnly = false;
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

        protected FileInfoDataTable(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public new FileInfoDataRow NewRow() { return (FileInfoDataRow)(base.NewRow()); }

        protected override DataTable CreateInstance() { return new FileInfoDataTable(); }

        protected override Type GetRowType() { return typeof(FileInfoDataRow); }

        protected override DataRow NewRowFromBuilder(DataRowBuilder builder) { return new FileInfoDataRow(builder); }
        
        protected override void OnRemoveColumn(DataColumn column)
        {
            if (column != null && (Object.ReferenceEquals(column, _idDataColumn) || Object.ReferenceEquals(column, _comparisonGroupIDDataColumn) || Object.ReferenceEquals(column, _creationTimeDataColumn) || Object.ReferenceEquals(column, _lastWriteTimeDataColumn) || Object.ReferenceEquals(column, _lengthDataColumn) || Object.ReferenceEquals(column, _md5HighDataColumn) || Object.ReferenceEquals(column, _md5LowDataColumn) || Object.ReferenceEquals(column, _nameDataColumn) || Object.ReferenceEquals(column, _parentIDDataColumn)))
                throw new InvalidOperationException();
        }
        
        protected override void OnTableNewRow(DataTableNewRowEventArgs e)
        {
            e.Row[_idDataColumn] = Guid.NewGuid();
        }
    }
    
    public class FileInfoDataRow : DataRow, IFileSystemDataRow
    {
        #region  Properties

        public new FileInfoDataTable Table { get { return (FileInfoDataTable)(base.Table); } }

        public Guid ID { get { return (Guid)(this[Table.IDDataColumn]); } }

        public Guid ParentID  { get { return (Guid)(this[Table.ParentIDDataColumn]); } }

        Guid? IFileSystemDataRow.ParentID { get { return this.ParentID; } }

        public string Name { get { return (string)(this[Table.NameDataColumn]); } }
        
        public Guid? ComparisonGroupID
        {
            get
            {
                if (IsNull(Table.ComparisonGroupIDDataColumn))
                    return null;
                return (Guid)(this[Table.ComparisonGroupIDDataColumn]);
            }
            set
            {
                if (value.HasValue)
                    this[Table.ComparisonGroupIDDataColumn] = value.Value;
                else
                    this.SetNull(Table.ComparisonGroupIDDataColumn);
            }
        }

        public DateTime CreationTime
        {
            get { return FileInfoCatalogUtility.FromDataSetDateTime((DateTime)(this[Table.CreationTimeDataColumn]), Table.CreationTimeDataColumn.DateTimeMode); }
            set { this[Table.CreationTimeDataColumn] = FileInfoCatalogUtility.ToDataSetDateTime(value, Table.CreationTimeDataColumn.DateTimeMode); }
        }

        public DateTime LastWriteTime
        {
            get { return FileInfoCatalogUtility.FromDataSetDateTime((DateTime)(this[Table.LastWriteTimeDataColumn]), Table.LastWriteTimeDataColumn.DateTimeMode); }
            set { this[Table.LastWriteTimeDataColumn] = FileInfoCatalogUtility.ToDataSetDateTime(value, Table.LastWriteTimeDataColumn.DateTimeMode); }
        }

        public DateTime? UpdatedOn
        {
            get
            {
                if (IsNull(Table.UpdatedOnDataColumn))
                    return null;
                return FileInfoCatalogUtility.FromDataSetDateTime((DateTime)(this[Table.UpdatedOnDataColumn]), Table.UpdatedOnDataColumn.DateTimeMode);
            }
            set
            {
                if (value.HasValue)
                    this[Table.UpdatedOnDataColumn] = FileInfoCatalogUtility.ToDataSetDateTime(value.Value, Table.UpdatedOnDataColumn.DateTimeMode);
                else
                    this.SetNull(Table.UpdatedOnDataColumn);
            }
        }

        public long Length
        {
            get { return (long)(this[Table.LengthDataColumn]); }
            set { this[Table.LengthDataColumn] = value; }
        }

        public long MD5High
        {
            get { return (long)(this[Table.MD5HighDataColumn]); }
            set { this[Table.MD5HighDataColumn] = value; }
        }

        public long MD5Low
        {
            get { return (long)(this[Table.MD5LowDataColumn]); }
            set { this[Table.MD5LowDataColumn] = value; }
        }

        #endregion

        protected internal FileInfoDataRow(DataRowBuilder builder) : base(builder) { }
    }
    
    public static class FileInfoCatalogUtility
    {
        public static string GetNameParts(string name, out string extension)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            int i = name.LastIndexOf('.');
            if (i < 1)
            {
                extension = "";
                return name;
            }

            extension = (i == name.Length - 1) ? "." : name.Substring(i + 1);
            return name.Substring(0, i);
        }
        public static DateTime NormalizeDateTime(DateTime value)
        {
            if (value.Millisecond == 0)
                return value;
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, 0, value.Kind);
        }

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

        public static readonly Regex ColumnEncodeNameTestRegex = new Regex(@"^\d|[~()#\\/=><+\-*%&|^'""\[\]]", RegexOptions.Compiled);
        public static readonly Regex ColumnNameReplaceRegex = new Regex(@"[\\\[\]]", RegexOptions.Compiled);
        public static readonly Regex LikeWCReplaceRegex = new Regex(@"[*%\[\]]", RegexOptions.Compiled);
        public static string EscapeLikeText(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            return LikeWCReplaceRegex.Replace(text, m => "[" + m.Value + "]");
        }

        public static string EncodeColumnName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0 || ColumnEncodeNameTestRegex.IsMatch(name))
                return "[" + ColumnNameReplaceRegex.Replace(name, e => "\\" + e.Value) + "]";
            return name;
        }
        public static string EncodeQueryValue(string value, bool noEnclosingQuotes)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            return (noEnclosingQuotes) ? value.Replace("'", "''") : "'" + value.Replace("'", "''") + "'";
        }
        public static string EncodeQueryValue(string value) { return EncodeQueryValue(value, false); }
        public static string EncodeQueryValue(byte value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string EncodeQueryValue(sbyte value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string EncodeQueryValue(short value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string EncodeQueryValue(ushort value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string EncodeQueryValue(int value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string EncodeQueryValue(uint value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string EncodeQueryValue(long value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string EncodeQueryValue(ulong value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture); 
        }
        public static string EncodeQueryValue(float value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string EncodeQueryValue(double value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string EncodeQueryValue(decimal value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string EncodeQueryValue(bool value) { return (value) ? "true" : "false"; }
        public static string EncodeQueryValue(char value, bool noEnclosingQuotes)
        {
            if (value == '\'')
                return (noEnclosingQuotes) ? "''" : "''''";
            return new String((noEnclosingQuotes) ? new char[] { value } : new char[] { '\'', value, '\''});
        }
        public static string EncodeQueryValue(char value) { return EncodeQueryValue(value, false); }
        public static string EncodeQueryValue(DateTime value)
        {
            return ToUniversalTime(value).ToString("#yyyy-MM-dd HH:mm:ss#");
        }
        public static string EncodeQueryValue(IConvertible value) { return EncodeQueryValue(value, false); }
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
        public static string EncodeQueryValue(object value, bool noEnclosingQuotes)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (value is IConvertible)
                return EncodeQueryValue((IConvertible)value, noEnclosingQuotes);
            return EncodeQueryValue(value.ToString(), noEnclosingQuotes);
        }
        public static string EncodeQueryValue(object value) { return EncodeQueryValue(value, false); }

        public static string GetIsNullQueryString(string column)
        {
            return "IsNull(" + EncodeColumnName(column) + ")";
        }
        
        public static string GetIsNotNullQueryString(string column)
        {
            return "NOT IsNull(" + EncodeColumnName(column) + ")";
        }

        public static string GetLikeQueryString(string column, string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            
            return EncodeColumnName(column) + " LIKE '" + value.Replace("'", "''") + "'";
        }

        public static string GetQueryString(string column, QueryOperator op, string value, bool isEscaped)
        {
            if (value == null)
                throw new ArgumentNullException("value");
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
        public static string GetQueryString(string column, QueryOperator op, object value)
        {
            if (value == null || value is DBNull)
                throw new ArgumentNullException("value");
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

    public enum QueryOperator
    {
        Equals,
        LessThan,
        GreaterThan,
        NotEquals,
        NotLessThan,
        NotGreaterThan
    }
}
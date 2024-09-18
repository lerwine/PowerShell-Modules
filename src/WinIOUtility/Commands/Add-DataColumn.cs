using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WinIOUtility.Commands
{
    /// <summary>
    /// Adds a new DataColumn object to a DataTable.
    /// </summary>
    /// <details>Creates a new instance of the System.Data.DataColumn class and adds it to a System.Data.DataTable object.</details>
    [Cmdlet(VerbsCommon.Add, "DataColumn")]
    [OutputType(typeof(CodeDomProvider))]
    public class Add_DataColumn : PSCmdlet
    {
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
        public const string ParameterSetName_Opt = "Opt";
        public const string ParameterSetName_Expression = "Expression";
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member

        private MappingType? _type = null;

        #region Properties

        /// <summary>
        /// DataTable to add the DataColumn to.
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [ValidateNotNull()]
        [Alias("DataTable")]
        public DataTable Table { get; set; } = null!;

        /// <summary>
        /// A string that represents the name of the column to be added.
        /// </summary>
        [Parameter(Position = 1, ParameterSetName = ParameterSetName_Opt)]
        [Parameter(Mandatory = true, Position = 1, ParameterSetName = ParameterSetName_Expression)]
        [ValidateNotNullOrEmpty()]
        [Alias("Name")]
        public string ColumnName { get; set; } = null!;

        /// <summary>
        /// A supported column type.
        /// </summary>
        [Parameter(Position = 2, ParameterSetName = ParameterSetName_Opt)]
        [Parameter(Mandatory = true, Position = 2, ParameterSetName = ParameterSetName_Expression)]
        [ValidateNotNull()]
        public Type DataType { get; set; } = null!;

        /// <summary>
        /// An expression to calculate the value of a column, or create an aggregate column. The return type of an expression is determined by the System.Data.DataColumn.DataType of the column
        /// </summary>
        [Parameter(Mandatory = true, Position = 3, ParameterSetName = ParameterSetName_Expression)]
        [Alias("Expression")]
        [ValidateNotNullOrEmpty()]
        public string Expr { get; set; } = null!;

        /// <summary>
        /// One of the System.Data.MappingType values
        /// </summary>
        [Parameter(Position = 3, ParameterSetName = ParameterSetName_Opt)]
        [Parameter(Position = 4, ParameterSetName = ParameterSetName_Expression)]
        [Alias("MappingType")]
        public MappingType Type
        {
            get { return (_type.HasValue) ? _type.Value : MappingType.Element; }
            set { _type = value; }
        }

        /// <summary>
        /// The caption for the column.
        /// </summary>
        [Parameter()]
        [ValidateNotNullOrEmpty()]
        public string Caption { get; set; } = null!;

        /// <summary>
        /// Indicates whether null values are allowed in this column for rows that belong to the table.
        /// </summary>
        [Parameter()]
        public SwitchParameter AllowDBNull { get; set; }

        /// <summary>
        /// Indicates whether the column automatically increments the value of the column for new rows added to the table.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSetName_Opt)]
        public SwitchParameter AutoIncrement { get; set; }

        private long? _autoIncrementSeed = null;
        
        /// <summary>
        /// The starting value for a column when AutoIncrement is set.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSetName_Opt)]
        public long AutoIncrementSeed
        {
            get { return (_autoIncrementSeed.HasValue) ? _autoIncrementSeed.Value : 0L; }
            set { _autoIncrementSeed = value; }
        }

        private long? _autoIncrementStep = null;
        
        /// <summary>
        /// The starting value for a column when AutoIncrement is set.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSetName_Opt)]
        public long AutoIncrementStep
        {
            get { return (_autoIncrementStep.HasValue) ? _autoIncrementStep.Value : 1L; }
            set { _autoIncrementStep = value; }
        }

        private DataSetDateTime? _dateTimeMode = null;
        
        /// <summary>
        /// The DateTime mode for the column.
        /// </summary>
        [Parameter()]
        public DataSetDateTime DateTimeMode
        {
            get { return (_dateTimeMode.HasValue) ? _dateTimeMode.Value : DataSetDateTime.UnspecifiedLocal; }
            set { _dateTimeMode = value; }
        }

        /// <summary>
        /// The default value for the column when you are creating new rows.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSetName_Opt)]
        [AllowNull()]
        [AllowEmptyString()]
        public object DefaultValue { get; set; } = null!;

        private int? _maxLength = null;
        
        /// <summary>
        /// The maximum length of a text column.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSetName_Opt)]
        public int MaxLength
        {
            get { return (_maxLength.HasValue) ? _maxLength.Value : int.MaxValue; }
            set { _maxLength = value; }
        }

        /// <summary>
        /// Indicates whether the column allows for changes as soon as a row has been added to the table.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSetName_Opt)]
        public SwitchParameter ReadOnly { get; set; }

        /// <summary>
        /// Indicates whether the values in each row of the column must be unique.
        /// </summary>
        [Parameter(ParameterSetName = ParameterSetName_Opt)]
        public SwitchParameter Unique { get; set; }

        /// <summary>
        /// Indicates whether the new data column is returned.
        /// </summary>
        [Parameter()]
        public SwitchParameter PassThru { get; set; }

        #endregion

        #region Overrides

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
        protected override void ProcessRecord()
        {
            DataColumn? dataColumn;
            if (String.IsNullOrEmpty(Expr))
            {
                if (DataType != null)
                {
                    try { dataColumn = new DataColumn(ColumnName, DataType); }
                    catch (Exception ex)
                    {
                        try { dataColumn = new DataColumn(ColumnName); } catch { dataColumn = null; }
                        if (dataColumn != null)
                            WriteError(new ErrorRecord(ex, "Invalid", ErrorCategory.InvalidArgument, DataType));
                        else
                            WriteError(new ErrorRecord(ex, "Invalid", ErrorCategory.InvalidArgument, ColumnName));
                        return;
                    }
                }
                else if (String.IsNullOrEmpty(ColumnName))
                    dataColumn = new DataColumn();
                else
                {
                    try { dataColumn = new DataColumn(ColumnName); }
                    catch (Exception ex)
                    {
                        WriteError(new ErrorRecord(ex, "Invalid", ErrorCategory.InvalidArgument, ColumnName));
                        return;
                    }
                }
            }
            else if (_type.HasValue)
            {
                try { dataColumn = new DataColumn(ColumnName, DataType, Expr, Type); }
                catch (Exception ex)
                {
                    try { dataColumn = new DataColumn(ColumnName, DataType, Expr); } catch { dataColumn = null; }
                    if (dataColumn != null)
                        WriteError(new ErrorRecord(ex, "Invalid", ErrorCategory.InvalidArgument, Type));
                    else
                    {
                        try { dataColumn = new DataColumn(ColumnName, DataType); } catch { dataColumn = null; }
                        if (dataColumn != null)
                            WriteError(new ErrorRecord(ex, "Invalid", ErrorCategory.InvalidArgument, Expr));
                        else
                        {
                            try { dataColumn = new DataColumn(ColumnName); } catch { dataColumn = null; }
                            if (dataColumn != null)
                                WriteError(new ErrorRecord(ex, "Invalid", ErrorCategory.InvalidArgument, DataType));
                            else
                                WriteError(new ErrorRecord(ex, "Invalid", ErrorCategory.InvalidArgument, ColumnName));
                        }
                    }
                    return;
                }
            }
            else
            {
                try { dataColumn = new DataColumn(ColumnName, DataType, Expr); }
                catch (Exception ex)
                {
                    try { dataColumn = new DataColumn(ColumnName, DataType); } catch { dataColumn = null; }
                    if (dataColumn != null)
                        WriteError(new ErrorRecord(ex, "Invalid", ErrorCategory.InvalidArgument, Expr));
                    else
                    {
                        try { dataColumn = new DataColumn(ColumnName); } catch { dataColumn = null; }
                        if (dataColumn != null)
                            WriteError(new ErrorRecord(ex, "Invalid", ErrorCategory.InvalidArgument, DataType));
                        else
                            WriteError(new ErrorRecord(ex, "Invalid", ErrorCategory.InvalidArgument, ColumnName));
                    }
                    return;
                }
            }
            
            dataColumn.AllowDBNull = AllowDBNull.IsPresent;
            dataColumn.AutoIncrement = AutoIncrement.IsPresent;
            if (_autoIncrementSeed.HasValue)
                dataColumn.AutoIncrementSeed = AutoIncrementSeed;
            if (_autoIncrementStep.HasValue)
                dataColumn.AutoIncrementStep = AutoIncrementStep;
            dataColumn.ReadOnly = ReadOnly.IsPresent;
            dataColumn.Unique = Unique.IsPresent;
            if (!String.IsNullOrEmpty(Caption))
                dataColumn.Caption = Caption;
            if (_dateTimeMode.HasValue)
                dataColumn.DateTimeMode = DateTimeMode;
            object obj = DefaultValue;
            if (obj != null)
            {
                if (obj is PSObject)
                    obj = ((PSObject)obj).BaseObject;
                dataColumn.DefaultValue = obj;
            }
            if (_maxLength.HasValue)
                dataColumn.MaxLength = MaxLength;

            Table.Columns.Add(dataColumn);
            if (PassThru.IsPresent)
                WriteObject(Table, false);
        }

        #endregion
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}

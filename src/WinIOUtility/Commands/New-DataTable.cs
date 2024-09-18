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
    /// Creates a new DataTable object.
    /// </summary>
    /// <details>Initializes a new instance of the System.Data.DataTable class.</details>
    [Cmdlet(VerbsCommon.New, "DataTable")]
    [OutputType(typeof(CodeDomProvider))]
    public class New_DataTable : PSCmdlet
    {
#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
        public const string ParameterSetName_Opt = "Opt";
        public const string ParameterSetName_Namespace = "Namespace";
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member

        #region Properties

        /// <summary>
        /// The name to give the table.
        /// </summary>
        [Parameter(Position = 0, ParameterSetName = ParameterSetName_Opt)]
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = ParameterSetName_Namespace)]
        [ValidateNotNullOrEmpty()]
        [Alias("Name")]
        public string TableName { get; set; } = null!;

        /// <summary>
        /// The namespace for the XML representation of the data stored in the DataTable.
        /// </summary>
        [Parameter(Mandatory = true, Position = 1, ParameterSetName = ParameterSetName_Namespace)]
        [Alias("Namespace")]
        [ValidateNotNull()]
        [AllowEmptyString()]
        public string TableNamespace { get; set; } = null!;

        /// <summary>
        /// The namespace prefix for the XML representation of the data stored in the System.Data.DataTable
        /// </summary>
        [Parameter(Position = 2, ParameterSetName = ParameterSetName_Namespace)]
        [ValidateNotNull()]
        [AllowEmptyString()]
        public string Prefix { get; set; } = null!;

        /// <summary>
        /// Indicates whether string comparisons within the table are case-sensitive
        /// </summary>
        [Parameter()]
        public SwitchParameter CaseSensitive { get; set; }

        #endregion

        #region Overrides

#pragma warning disable 1591 // Missing XML comment for publicly visible type or member
        protected override void ProcessRecord()
        {
            DataTable? table;
            
            if (TableNamespace == null)
            {
                if (String.IsNullOrEmpty(TableName))
                    table = new DataTable();
                else
                {
                    try { table = new DataTable(TableName); }
                    catch (Exception ex)
                    {
                        WriteError(new ErrorRecord(ex, "InvalidTableName", ErrorCategory.InvalidArgument, TableName));
                        return;
                    }
                }
            }
            else
            {
                try { table = new DataTable(TableName, TableNamespace); }
                catch (Exception ex)
                {
                    try { table = new DataTable(TableName); } catch { table = null; }
                    if (table == null)
                        WriteError(new ErrorRecord(ex, "InvalidTableName", ErrorCategory.InvalidArgument, TableName));
                    else
                        WriteError(new ErrorRecord(ex, "InvalidTableNamespace", ErrorCategory.InvalidArgument, TableNamespace));
                    return;
                }
                if (!String.IsNullOrEmpty(Prefix))
                {
                    try { table.Prefix = Prefix; }
                    catch (Exception ex)
                    {
                        WriteError(new ErrorRecord(ex, "InvalidPrefix", ErrorCategory.InvalidArgument, Prefix));
                        return;
                    }
                }
                table.CaseSensitive = CaseSensitive.IsPresent;
            }
            WriteObject(table, false);
        }

        #endregion
    }
#pragma warning restore 1591 // Missing XML comment for publicly visible type or member
}

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Windows.Forms;

namespace Erwine.Leonard.T.WinForms.Commands
{
    [Cmdlet(VerbsCommon.New, "TableLayoutPanel", DefaultParameterSetName = "LayoutStyle")]
    [OutputType(typeof(TableLayoutPanel))]
    public class New_TableLayoutPanel : New_Control<TableLayoutPanel>
    {
        [Parameter(Mandatory = false, ValueFromPipeline = true, ParameterSetName = "LayoutStyle")]
        [AllowNull]
        [AllowEmptyCollection]
        public TableLayoutStyle[] Layout { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, ParameterSetName = "ExplicitType")]
        [AllowNull]
        [AllowEmptyCollection]
        [Alias("Row")]
        public RowStyle[] Rows { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, ParameterSetName = "ExplicitType")]
        [AllowNull]
        [AllowEmptyCollection]
        [Alias("Column")]
        public ColumnStyle[] Columns { get; set; }

        private Collection<RowStyle> _rows = new Collection<RowStyle>();
        private Collection<ColumnStyle> _columns = new Collection<ColumnStyle>();

        protected override bool ShouldPerformLayout { get { return true; } }

        protected override bool ShouldSuspendLayout { get { return true; } }

        protected override void BeginProcessing()
        {
            this._rows.Clear();
            this._columns.Clear();
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            if (this.Layout == null)
                return;

            if (this.ParameterSetName == "ExplicitType")
            {
                foreach (RowStyle row in this.Rows.Where(i => i != null))
                    this._rows.Add(row);

                foreach (ColumnStyle column in this.Columns.Where(i => i != null))
                    this._columns.Add(column);

                return;
            }

            foreach (TableLayoutStyle layout in this.Layout.Where(i => i != null))
            {
                if (layout is RowStyle)
                    this._rows.Add(layout as RowStyle);
                else
                {
                    try
                    {
                        this._columns.Add((ColumnStyle)layout);
                    }
                    catch (Exception exc)
                    {
                        this.WriteError(new ErrorRecord(exc, "AddLayoutStyle", ErrorCategory.InvalidArgument, layout));
                    }
                }
            }
        }

        protected override bool FinalizeControl()
        {
            try
            {
                this.Control.ColumnCount = this._columns.Count;
                this.Control.RowCount = this._rows.Count;
            }
            catch (Exception exc)
            {
                this.WriteError(new ErrorRecord(exc, "InitializeTableLayoutPanel", ErrorCategory.WriteError, this.Control));
            }

            foreach (ColumnStyle column in this._columns)
            {
                try
                {
                    this.Control.ColumnStyles.Add(column);
                }
                catch (Exception exc)
                {
                    this.WriteError(new ErrorRecord(exc, "AddTableLayoutPanelColumn", ErrorCategory.InvalidArgument, column));
                }
            }

            foreach (RowStyle row in this._rows)
            {
                try
                {
                    this.Control.RowStyles.Add(row);
                }
                catch (Exception exc)
                {
                    this.WriteError(new ErrorRecord(exc, "AddTableLayoutPanelRow", ErrorCategory.InvalidArgument, row));
                }
            }

            return true;
        }
    }
}

using Excel = Microsoft.Office.Interop.Excel;
using System.Management.Automation;

namespace MsExcelUtil
{
    public sealed class PSExcelWorkbook : PSObject
    {
        internal WorkbookDependency.Instance DependencyInstance { get; private set; }

        internal PSExcelWorkbook(WorkbookDependency.Instance dependencyInstance)
            : base(dependencyInstance.Workbook)
        {
            DependencyInstance = dependencyInstance;
        }
    }
    public sealed class PSExcelRange : PSObject
    {
    }
    public sealed class PSExcelCell
    {
        Excel.Range _cell;
        public int Row => _cell.Row;
        public int Column => _cell.Column;
        public bool HasFormula => _cell.HasFormula;
        public string Text => _cell.Text;
        public PSExcelCell(Excel.Worksheet worksheet, int rowIndex, int columnIndex)
        {
            _cell = worksheet.Rows[rowIndex, columnIndex];
        }
    }
}

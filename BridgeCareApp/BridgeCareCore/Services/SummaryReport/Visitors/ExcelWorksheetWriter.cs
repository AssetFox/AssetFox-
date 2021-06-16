using BridgeCareCore.Services.SummaryReport.Models;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.Visitors
{
    public class ExcelWorksheetWriter: IExcelWorksheetModelVisitor<ExcelWorksheet, ExcelWorksheet>
    {
        public ExcelWorksheet Visit(RowBasedExcelWorksheetModel model, ExcelWorksheet worksheet)
        {
            var writer = new ExcelWriter();
            writer.WriteWorksheet(worksheet, model.Region, model.StartRow, model.StartColumn);
            return worksheet;
        }

        public ExcelWorksheet Visit(AutoFitColumnsExcelWorksheetContentModel model, ExcelWorksheet worksheet)
        {
            worksheet.Cells.AutoFitColumns(model.MinWidth);
            return worksheet;
        }
    }
}

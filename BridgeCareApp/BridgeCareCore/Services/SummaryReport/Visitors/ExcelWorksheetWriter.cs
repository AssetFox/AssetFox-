using BridgeCareCore.Services.SummaryReport.Models;
using OfficeOpenXml;

namespace BridgeCareCore.Services.SummaryReport.Visitors
{
    public class ExcelWorksheetWriter: IExcelWorksheetModelVisitor<ExcelWorksheet, ExcelWorksheet>
    {
        public ExcelWorksheet Visit(RowBasedExcelWorksheetContentModel model, ExcelWorksheet worksheet)
        {
            var writer = new ExcelWriter();
            writer.WriteWorksheet(worksheet, model);
            return worksheet;
        }
    }
}

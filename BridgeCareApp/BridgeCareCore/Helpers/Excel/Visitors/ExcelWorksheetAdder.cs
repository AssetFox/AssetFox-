using OfficeOpenXml;

<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.Reporting/Services/BAMSSummaryReport/Visitors/ExcelWorksheetAdder.cs
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.Worksheets;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.Visitors
========
namespace BridgeCareCore.Helpers.Excel.Visitors
>>>>>>>> faa0cabf (Moved a whole lot of files to a more-sensible namespace):BridgeCareApp/BridgeCareCore/Helpers/Excel/Visitors/ExcelWorksheetAdder.cs
{
    public static class ExcelWorksheetAdder
    {
        public static ExcelWorksheet AddWorksheet(ExcelWorkbook workbook, ExcelWorksheetModel model)
        {
            var writer = new ExcelWorksheetWriter();
            var r = workbook.Worksheets.Add(model.TabName);
            foreach (var content in model.Content)
            {
                content.Accept(writer, r);
            }
            r.Cells.Calculate();
            return r;
        }
    }
}

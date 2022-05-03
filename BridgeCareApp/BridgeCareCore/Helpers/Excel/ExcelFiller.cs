using OfficeOpenXml;

<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.Reporting/Services/BAMSSummaryReport/ExcelFiller.cs
namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport
========
namespace BridgeCareCore.Helpers.Excel
>>>>>>>> faa0cabf (Moved a whole lot of files to a more-sensible namespace):BridgeCareApp/BridgeCareCore/Helpers/Excel/ExcelFiller.cs
{
    public static class ExcelFiller
    {
        public static void FillVertically(ExcelWorksheet worksheet, int fromRow, int fromColumn, params object[] values)
        {
            for (var rowDelta = 0; rowDelta < values.Length; rowDelta++)
            {
                worksheet.Cells[rowDelta + fromRow, fromColumn].Value = values[rowDelta];

            }
        }
    }
}

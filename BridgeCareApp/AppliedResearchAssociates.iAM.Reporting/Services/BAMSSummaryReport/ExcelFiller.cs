using OfficeOpenXml;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport
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

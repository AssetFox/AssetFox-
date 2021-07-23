using OfficeOpenXml;

namespace BridgeCareCore.Utils
{
    public static class ExcelWorksheetExtensions
    {
        public static T GetCellValue<T>(this ExcelWorksheet worksheet, int row, int col) =>
            worksheet.Cells[row, col].GetValue<T>();
    }
}

using System.Drawing;
using OfficeOpenXml;

namespace BridgeCareCore.Interfaces.SummaryReport
{
    public enum ExcelHelperCellFormat
    {
        NegativeCurrency,
        Number,
        Percentage,
        PercentageDecimal2,
        DecimalPrecision3,
    }

    public interface IExcelHelper
    {
        void ApplyBorder(ExcelRange cells);

        void ApplyColor(ExcelRange cells, Color color);

        void ApplyStyle(ExcelRange cells);

        void MergeCells(ExcelWorksheet worksheet, int fromRow, int fromColumn, int toRow, int toColumn, bool makeTextBold = true);

        void SetCurrencyFormat(ExcelRange cells);

        void SetCustomFormat(ExcelRange cells, ExcelHelperCellFormat type);

        void SetTextColor(ExcelRange cells, Color color);

        void HorizontalCenterAlign(ExcelRange cells);
    }
}

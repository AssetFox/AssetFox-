using System.Drawing;
using BridgeCareCore.Interfaces.SummaryReport;
using BridgeCareCore.Services.SummaryReport.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BridgeCareCore.Services.SummaryReport
{
    public enum ExcelHelperCellFormat
    {
        NegativeCurrency,
        Number,
        Percentage,
        PercentageDecimal2,
        DecimalPrecision3,
    }
    public static class ExcelHelper
    {
        /// <summary>
        ///     Merge given cells
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="fromRow"></param>
        /// <param name="fromColumn"></param>
        /// <param name="toRow"></param>
        /// <param name="toColumn"></param>
        public static void MergeCells(ExcelWorksheet worksheet, int fromRow, int fromColumn, int toRow, int toColumn, bool makeTextBold = true)
        {
            using (var cells = worksheet.Cells[fromRow, fromColumn, toRow, toColumn])
            {
                cells.Merge = true;
                if (makeTextBold == true)
                {
                    ApplyStyle(cells);
                }
                else
                {
                    cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    cells.Style.WrapText = true;
                }
            }
        }

        /// <summary>
        ///     Apply style to given cells
        /// </summary>
        /// <param name="cells"></param>
        public static void ApplyStyle(ExcelRange cells)
        {
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.WrapText = true;
            cells.Style.Font.Bold = true;
        }

        /// <summary>
        ///     Apply border to given cells
        /// </summary>
        /// <param name="cells"></param>
        public static void ApplyBorder(ExcelRange cells)
        {
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        }

        public static void ApplyLeftBorder(ExcelRange cells)
        {
            cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        }
        public static void ApplyRightBorder(ExcelRange cells)
        {
            cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }

        /// <summary>
        ///     Set currency format for given cells
        /// </summary>
        /// <param name="cells"></param>
        public static void SetCurrencyFormat(ExcelRange cells)
        {
            cells.Style.Numberformat.Format = ExcelFormatStrings.Currency;
        }

        /// <summary>
        ///     Set custom format for given cells
        /// </summary>
        /// <param name="cells"></param>
        public static void SetCustomFormat(ExcelRange cells, ExcelHelperCellFormat type)
        {
            switch (type)
            {
            case ExcelHelperCellFormat.NegativeCurrency:
                cells.Style.Numberformat.Format = ExcelFormatStrings.NegativeCurrency;
                break;

            case ExcelHelperCellFormat.Number:
                cells.Style.Numberformat.Format = "_-* #,##0_-;* (#,##0)_-;_-* \"-\"??_-;_-@_-";
                break;

            case ExcelHelperCellFormat.Percentage:
                cells.Style.Numberformat.Format = "#0%";
                break;
            case ExcelHelperCellFormat.PercentageDecimal2:
                cells.Style.Numberformat.Format = "#0.00%";
                break;
            case ExcelHelperCellFormat.DecimalPrecision3:
                cells.Style.Numberformat.Format = "#0.000";
                break;
            }
        }

        public static void ApplyColor(ExcelRange cells, Color color)
        {
            cells.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cells.Style.Fill.BackgroundColor.SetColor(color);
        }

        public static void SetTextColor(ExcelRange cells, Color color)
        {
            cells.Style.Font.Color.SetColor(color);
        }

        public static void HorizontalCenterAlign(ExcelRange cells)
        {
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }
    }
}

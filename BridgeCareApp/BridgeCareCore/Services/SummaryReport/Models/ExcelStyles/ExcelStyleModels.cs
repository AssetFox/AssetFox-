using System.Drawing;
using OfficeOpenXml.Style;

namespace BridgeCareCore.Services.SummaryReport.Models
{
    /// <summary>The excel-spreadsheet-filling engine treats styles
    /// the same way as values. Both conform to the same interface. However,
    /// the programmer may want to think about the two differently, so
    /// we generate the styles in a different class.</summary>
    public static class ExcelStyleModels
    {

        public static ExcelBoldModel Bold
            => new ExcelBoldModel
            {
                Bold = true,
            };

        public static ExcelBoldModel NotBold
            => new ExcelBoldModel
            {
                Bold = false,
            };

        public static ExcelBorderModel ThinBorder
            => new ExcelBorderModel
            {
                BorderStyle = ExcelBorderStyle.Thin,
            };

        public static ExcelBorderModel MediumBorder
            => new ExcelBorderModel
            {
                BorderStyle = ExcelBorderStyle.Medium,
            };

        public static ExcelHorizontalAlignmentModel HorizontalCenter
            => new ExcelHorizontalAlignmentModel
            {
                Alignment = ExcelHorizontalAlignment.Center
            };

        public static ExcelHorizontalAlignmentModel Right
            => new ExcelHorizontalAlignmentModel
            {
                Alignment = ExcelHorizontalAlignment.Right,
            };

        public static ExcelFontColorModel FontColor(Color color)
            => new ExcelFontColorModel
            {
                Color = color,
            };

        public static ExcelFontColorModel WhiteText
            => FontColor(Color.White);

        public static ExcelFillModel BackgroundColor(Color color)
            => new ExcelFillModel
            {
                Color = color,
            };

        public static ExcelNumberFormatModel CurrencyFormat =>
            new ExcelNumberFormatModel
            {
                Format = ExcelFormatStrings.Currency
            };

        public static ExcelNumberFormatModel PercentageFormat(int digitsAfterDecimalPlace)
            => new ExcelNumberFormatModel
            {
                Format = ExcelFormatStrings.Percentage(digitsAfterDecimalPlace),
            };

    }
}

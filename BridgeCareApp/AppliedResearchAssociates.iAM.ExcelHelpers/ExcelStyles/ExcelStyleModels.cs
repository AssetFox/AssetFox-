using System.Drawing;
using OfficeOpenXml.Style;

<<<<<<<< HEAD:BridgeCareApp/AppliedResearchAssociates.iAM.ExcelHelpers/ExcelStyles/ExcelStyleModels.cs
namespace AppliedResearchAssociates.iAM.ExcelHelpers
========
namespace BridgeCareCore.Helpers.Excel
>>>>>>>> master:BridgeCareApp/BridgeCareCore/Helpers/Excel/ExcelStyles/ExcelStyleModels.cs
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

        public static StackedExcelModel CenteredHeader
            => StackedExcelModels.Stacked(
                Bold,
                HorizontalCenter,
                VerticalCenter);

        public static StackedExcelModel CenteredHeaderWrap
            => StackedExcelModels.Stacked(
                Bold,
                HorizontalCenter,
                VerticalCenter,
                WrapText);

        public static StackedExcelModel LeftHeader
            => StackedExcelModels.Stacked(
                Bold,
                Left,
                VerticalCenter);


        public static StackedExcelModel LeftHeaderWrap
            => StackedExcelModels.Stacked(
                Bold,
                Left,
                VerticalCenter,
                WrapText);

        public static StackedExcelModel RightHeader
            => StackedExcelModels.Stacked(
                Bold,
                Right,
                VerticalCenter,
                WrapText);

        public static ExcelHorizontalAlignmentModel Left
            => new ExcelHorizontalAlignmentModel
            {
                Alignment = ExcelHorizontalAlignment.Left,
            };

        public static ExcelItalicModel Italic
            => new ExcelItalicModel
            {
                Italic = true,
            };

        public static ExcelWrapTextModel WrapText
            => new ExcelWrapTextModel
            {
                Wrap = true,
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

        public static ExcelSingleBorderModel ThinBottomBorder()
            => new()
            {
                BorderStyle = ExcelBorderStyle.Thin,
                Edge = RectangleEdge.Bottom,
            };

        public static ExcelHorizontalAlignmentModel HorizontalCenter
            => new ExcelHorizontalAlignmentModel
            {
                Alignment = ExcelHorizontalAlignment.Center
            };


        public static ExcelVerticalAlignmentModel VerticalCenter
            => new ExcelVerticalAlignmentModel
            {
                Alignment = ExcelVerticalAlignment.Center
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

        public static ExcelFontSizeModel FontSize(float fontSize)
            => new ExcelFontSizeModel
            {
                FontSize = fontSize,
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

        public static ExcelNumberFormatModel CurrencyWithoutCentsFormat =>
            new ExcelNumberFormatModel
            {
                Format = ExcelFormatStrings.CurrencyWithoutCents
            };

        public static ExcelNumberFormatModel PercentageFormat(int digitsAfterDecimalPlace)
            => new ExcelNumberFormatModel
            {
                Format = ExcelFormatStrings.Percentage(digitsAfterDecimalPlace),
            };

    }
}

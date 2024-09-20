using System.Drawing;
using AppliedResearchAssociates.iAM.ExcelHelpers;

namespace AppliedResearchAssociates.iAM.Reporting.Services.PAMSSummaryReport.ShortNameGlossary
{
    internal class ShortNameGlossaryColorKeyModels
    {

        private static Color PureGreen => Color.FromArgb(0, 255, 0);

        internal static RowBasedExcelRegionModel ColorKeyRows() => RowBasedExcelRegionModels.WithRows(
                CenteredHeader("Color Key"),
                TwoByOneRow(ExcelValueModels.Nothing),

                CenteredHeader("Work Done Columns"),
                TwoByOneRow(ColoredText("PMS Treatment is being cashed flowed.", Color.Red, PureGreen)),
                TwoByOneRow(ColoredText("MPMS Project selected for consecutive years.", Color.White, Color.Orange)),

                TwoByOneRow(ExcelValueModels.Nothing),
                CenteredHeader("Details Columns"),
                TwoByOneRow(ColoredText("Project is being cashed flowed.", Color.Red, PureGreen))
        );

        private static ExcelRowModel CenteredHeader(string text)
            => ExcelRowModels.WithCells(
                new RelativeExcelRangeModel
                {
                    Content = StackedExcelModels.Stacked(
                        ExcelValueModels.String(text),
                        ExcelStyleModels.Bold,
                        ExcelStyleModels.HorizontalCenter),
                    Size = new ExcelRangeSize(2, 1)
                });

        private static IExcelModel ColoredText(string text, Color textColor, Color fillColor)
            => StackedExcelModels.Stacked(
                ExcelValueModels.String(text),
                ExcelStyleModels.FontColor(textColor),
                ExcelStyleModels.BackgroundColor(fillColor),
                ExcelStyleModels.ThinBorder);


        private static ExcelRowModel TwoByOneRow(IExcelModel content)
            => ExcelRowModels.WithCells(TwoByOne(content));

        private static IExcelModel ItalicYear(int year)
            => StackedExcelModels.Stacked(
                ExcelValueModels.Integer(year),
                ExcelStyleModels.HorizontalCenter,
                ExcelStyleModels.Italic);

        private static RelativeExcelRangeModel TwoByOne(IExcelModel content)
            => new RelativeExcelRangeModel
            {
                Content = content,
                Size = new ExcelRangeSize(2, 1)
            };

    }
}

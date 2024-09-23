using System.Drawing;
using AppliedResearchAssociates.iAM.ExcelHelpers;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.ShortNameGlossary
{
    internal class ShortNameGlossaryColorKeyModels
    {

        private static Color PureGreen => Color.FromArgb(0, 255, 0);

        internal static RowBasedExcelRegionModel ColorKeyRows()
            => RowBasedExcelRegionModels.WithRows(
                CenteredHeader("Color Key"),
                ExcelRowModels.Empty,
                CenteredHeader("Work Done Columns"),
                TwoByOneRow(ColoredText("Bridge project is being cashed flowed.", Color.Red, PureGreen)),
                TwoByOneRow(ColoredText("MPMS Project is being Cash Flowed.", Color.White, Color.Orange)),
                TwoByOneRow(ExcelValueModels.Nothing),
                CenteredHeader("Details Columns"),
                TwoByOneRow(ColoredText("Bridge project is being cashed flowed.", Color.Red, PureGreen)),
                TwoByOneRow(ColoredText("MPMS Project is being Cash Flowed.", Color.White, Color.Orange)),
                TwoByOneRow(ColoredText("Min Condition is less than or equal to 3.5", Color.White, Color.Purple)),
                ExcelRowModels.Empty,
                ExcelRowModels.Empty,
                ExcelRowModels.WithEntries(
                    ExcelValueModels.String("example:")
                    ),
                ExcelRowModels.Empty,
                ExcelRowModels.WithEntries(ItalicYear(2021), ItalicYear(2022), ItalicYear(2023)),
                ExcelRowModels.WithEntries(
                    ColoredText("Brdg_Repl", Color.Red, PureGreen),
                    ColoredText("Brdg_Repl ", Color.Red, PureGreen),
                    ColoredText("Brdg_Repl", Color.Red, PureGreen)
                    ),
                ExcelRowModels.WithEntries(                    
                    ExcelValueModels.String("Bridge replacement is cash flowed over 3 years.")
                    )
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

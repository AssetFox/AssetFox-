using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Services.SummaryReport.BridgeData;
using BridgeCareCore.Services.SummaryReport.Models;
using BridgeCareCore.Services.SummaryReport.Models.Worksheets;

namespace BridgeCareCore.Services.SummaryReport.ShortNameGlossary
{
    public static class ShortNameGlossaryModels
    {
        public static List<IExcelWorksheetContentModel> Content
            => new List<IExcelWorksheetContentModel>
            {
                TreatmentsRegion,
                ExcelWorksheetContentModels.AutoFitColumns(70),
                ConditionRangeRegion,
                GlossaryRegion,
                ColorKeyRegion
            };

        public static AnchoredExcelRegionModel TreatmentsRegion
            => new AnchoredExcelRegionModel
            {
                Region = TreatmentsRows(),
                StartColumn = 1,
                StartRow = 1,
            };

        public static RowBasedExcelRegionModel TreatmentsRows()
        {
            var r = RowBasedExcelRegionModels.WithRows(
                    ExcelRowModels.WithEntries(
                        StackedExcelModels.LeftHeader("Bridge Care Work Type"),
                        StackedExcelModels.LeftHeader("Short Bridge Care Work type"))
                );
            var abbreviatedTreatmentNames = ShortNamesForTreatments.GetShortNamesForTreatments();
            foreach (var treatment in abbreviatedTreatmentNames)
            {
                var row = ExcelRowModels.WithEntries(
                    ExcelValueModels.String(treatment.Key),
                    ExcelValueModels.String(treatment.Value));
                row.EveryCell = ExcelStyleModels.ThinBorder;
                r.Rows.Add(row);
            }
            return r;
        }

        public static AnchoredExcelRegionModel ConditionRangeRegion
            => new AnchoredExcelRegionModel
            {
                Region = ConditionRangeContent(),
                StartRow = 1,
                StartColumn = 4
            };

        private static ExcelRowModel RowWithTwoColumnsThenOne(IExcelModel left, decimal number)
            => ExcelRowModels.WithCells(
                new RelativeExcelRangeModel
                {
                    Content = left,
                    Size = new ExcelRangeSize(2, 1)
                },
                new RelativeExcelRangeModel
                {
                    Content = StackedExcelModels.Stacked(
                        ExcelValueModels.Decimal(number),
                        ExcelStyleModels.Right
                        )
                });

        private static RowBasedExcelRegionModel ConditionRangeContent() =>
            new RowBasedExcelRegionModel
            {
                Rows = new List<ExcelRowModel>
                {
                    ExcelRowModels.WithCells(
                        new RelativeExcelRangeModel {
                            Content =
                        StackedExcelModels.Stacked(
                            ExcelValueModels.String($"Posted / Closed Bridge Condition\r\n Range"),
                            ExcelStyleModels.Bold,
                            ExcelStyleModels.HorizontalCenter,
                            ExcelStyleModels.WrapText),
                            Size = new ExcelRangeSize(3, 1)
                        }),
                    ExcelRowModels.Empty,
                    RowWithTwoColumnsThenOne(ExcelValueModels.String("Posted Condition"), 4.1m),
                    RowWithTwoColumnsThenOne(ExcelValueModels.String("Closed Condition"), 3m),
                    ExcelRowModels.Empty,
                    ExcelRowModels.WithCells(
                        new RelativeExcelRangeModel {
                            Content = StackedExcelModels.Stacked(
                                ExcelValueModels.String("Condition Limits"),
                                ExcelStyleModels.Bold,
                                ExcelStyleModels.HorizontalCenter),
                            Size = new ExcelRangeSize(3, 1)
                        }),

                    ExcelRowModels.Empty,
                    RowWithTwoColumnsThenOne(StackedExcelModels.Stacked(
                        ExcelValueModels.String("Good Min Cond >"),
                        ExcelStyleModels.HorizontalCenter),
                        7),
                    RowWithTwoColumnsThenOne(StackedExcelModels.Stacked(
                        ExcelValueModels.String("Poor Min Cond <"),
                        ExcelStyleModels.HorizontalCenter),
                        5),
                }
            };

        private static AnchoredExcelRegionModel ColorKeyRegion
            => new AnchoredExcelRegionModel
            {
                Region = ColorKeyRows(),
                StartRow = 39,
                StartColumn = 1
            };

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

        private static RelativeExcelRangeModel TwoByOne(IExcelModel content)
            => new RelativeExcelRangeModel
            {
                Content = content,
                Size = new ExcelRangeSize(2, 1)
            };

        private static ExcelRowModel TwoByOneRow(IExcelModel content)
            => ExcelRowModels.WithCells(TwoByOne(content));

        private static IExcelModel ItalicYear(int year)
            => StackedExcelModels.Stacked(
                ExcelValueModels.Integer(year),
                ExcelStyleModels.HorizontalCenter,
                ExcelStyleModels.Italic);

        private static Color PureGreen => Color.FromArgb(0, 255, 0);

        private static RowBasedExcelRegionModel ColorKeyRows()
            => RowBasedExcelRegionModels.WithRows(
                CenteredHeader("Color Key"),
                ExcelRowModels.Empty,
                CenteredHeader("Work Done Columns"),
                TwoByOneRow(ColoredText("Bridge project is being cashed flowed.", Color.Red, PureGreen)),
                TwoByOneRow(ColoredText("MPMS Project being Cach Flowed", Color.White, Color.Orange)),
                TwoByOneRow(ExcelValueModels.Nothing),
                CenteredHeader("Details Columns"),
                TwoByOneRow(ColoredText("Bridge project is being cashed flowed.", Color.Red, PureGreen)),
                TwoByOneRow(ColoredText("MPMS Project being Cach Flowed", Color.White, Color.Orange)),
                TwoByOneRow(StackedExcelModels.Stacked(ColoredText("Min Condition is less than or equal to 3.5", Color.White, Color.Purple),
                                                       ExcelStyleModels.HorizontalCenter)),
                ExcelRowModels.Empty,
                ExcelRowModels.WithEntries(ExcelValueModels.Nothing, ItalicYear(2021), ItalicYear(2022), ItalicYear(2023)),
                ExcelRowModels.WithEntries(
                    ExcelValueModels.String("example:"),
                    ColoredText("Brdg_Repl", Color.Red, PureGreen),
                    ColoredText("--", Color.Red, PureGreen),
                    ColoredText("--", Color.Red, PureGreen)
                    ),
                ExcelRowModels.WithEntries(
                    ExcelValueModels.Nothing,
                    ExcelValueModels.String("    (Bridge being replaced also has a parallel bridge.  Bridge replacement is cash flowed over 3 years.")
                    )
                );

        public static AnchoredExcelRegionModel GlossaryRegion
            => new AnchoredExcelRegionModel
            {
                Region = GlossaryColumn(),
                StartRow = 1,
                StartColumn = 8,
            };
        public static RowBasedExcelRegionModel GlossaryColumn()
        {
            var content = TabDefinitionsContent();
            return RowBasedExcelRegionModels.Column(content);
        }

        private static List<IExcelModel> TabDefinitionsContent()
            => new List<IExcelModel>
            {
                TabDefinitions,
                BoldThenNot("Parameters:", "Summary of simulation data inputs."),
                BoldThenNot("Bridge Data:", "Complete recommended treatments for all structures throughout the analysis period"),
                BoldThenNot("Unfunded Treatment \u2014 Final List", "First year of eligible but unfunded treatments by BRKEY."),
                BoldThenNot("Unfunded Treatments \u2014 Time:", "List of unfunded treatments through time."),
                BoldThenNot("Bridge Work Summary:", "Summary of projected costs for treatments throughout simulation period for all bridges. Summary of projected condition changes throughout analysis period."),
                BoldThenNot("Bridge Work Summary by Budget:", "Summary of projected costs for treatments throughout simulation period for all bridges by specific budget."),
                BoldThenNot("District Totals:", "Projected spending per district each year throughout analysis period."),
                BoldThenNot("NHS Count:", "Graph of projected bridge conditions for structures on the NHS by bridge count."),
                BoldThenNot("NHS DA:", "Graph of projected bridge conditions for structures on the NHS by deck area."),
                BoldThenNot("Non-NHS Count:", "Graph of projected bridge conditions for structures not on the NHS by bridge count."),
                BoldThenNot("Non-NHS DA:", "Graph of projected bridge conditions for structures not on the NHS by deck area."),
                BoldThenNot("Combined Count:", "Graph of projected bridge conditions for all structures by bridge count."),
                BoldThenNot("Combined DA:", "Graph of projected bridge conditions for all structures by deck area."),
                BoldThenNot("Poor Count:", "Graph showing projected number of structures to be in poor condition throughout analysis period."),
                BoldThenNot("Poor DA:", "Graph showing projected deck area of structures to be in poor condition throughout analysis period."),
                BoldThenNot("Poor DA by BPN:", "Graph showing projected deck area of structures to be in poor condition, by BPN, throughout analysis period."),
                BoldThenNot("Posted BPN Count:", "Graph showing projected number of structures to be posted based on off the GCR Super and not by load capacity calculations. Since it is not possible to perform load calculations ant this level, this can be considered a proxy and should be used only for only long-term evaluation of in change in total quantity."),
                BoldThenNot("Posted BPN DA:", "Graph showing projected deck area of structures to be posted based on the GCR Super and not by load capacity calculations. Since it is not possible to perform load calculations at this level, this can be considered a proxy and should be used only for long-term evaluation of change in total quantity."),
            };

        private static IExcelModel TabDefinitions
            => ExcelValueModels.RichString("Tab Definitions", true);

        private static IExcelModel BoldThenNot(string bold, string notBold)
            => StackedExcelModels.Stacked(
                ExcelValueModels.RichString(bold, true),
                ExcelValueModels.RichString($" {notBold}")
            );
    }
}

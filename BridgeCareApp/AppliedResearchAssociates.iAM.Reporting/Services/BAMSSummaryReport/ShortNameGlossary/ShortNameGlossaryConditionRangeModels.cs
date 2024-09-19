using System.Collections.Generic;

using AppliedResearchAssociates.iAM.ExcelHelpers;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.ShortNameGlossary
{
    public static class ShortNameGlossaryConditionRangeModels
    {
        internal static RowBasedExcelRegionModel ConditionRangeContent() =>
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
                    RowWithTwoColumnsThenOne(ExcelValueModels.String("Posted Condition <"), 4.1m),
                    RowWithTwoColumnsThenOne(ExcelValueModels.String("Closed Condition <"), 3m),
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

    }
}

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
                    ExcelRowModels.Empty,
                    ExcelRowModels.WithCells(
                        new RelativeExcelRangeModel {
                            Content = StackedExcelModels.Stacked(
                            ExcelValueModels.String($"Posted / Closed Bridge Condition Range"),
                            ExcelStyleModels.Bold,
                            ExcelStyleModels.HorizontalCenter,
                            ExcelStyleModels.WrapText),
                            Size = new ExcelRangeSize(3, 2)
                        }),
                    ExcelRowModels.Empty,
                    RowWithTwoColumnsThenOne(StackedExcelModels.Stacked(
                        ExcelValueModels.String("Posted Condition <"),
                        ExcelStyleModels.ThinBorder),
                        4.1m),
                    RowWithTwoColumnsThenOne(StackedExcelModels.Stacked(
                        ExcelValueModels.String("Closed Condition <"),
                        ExcelStyleModels.ThinBorder),
                        3m),
                    ExcelRowModels.Empty,
                    ExcelRowModels.Empty,
                    ExcelRowModels.WithCells(
                        new RelativeExcelRangeModel {
                            Content = StackedExcelModels.Stacked(
                                ExcelValueModels.String("Condition Limits"),
                                ExcelStyleModels.Bold,
                                ExcelStyleModels.HorizontalCenter,
                                ExcelStyleModels.ThinBorder),                            
                            Size = new ExcelRangeSize(3, 1)
                        }),
                    RowWithTwoColumnsThenOne(StackedExcelModels.Stacked(
                        ExcelValueModels.String("Good Min Cond >"),
                        ExcelStyleModels.HorizontalCenter,
                        ExcelStyleModels.ThinBorder),
                        7),
                    RowWithTwoColumnsThenOne(StackedExcelModels.Stacked(
                        ExcelValueModels.String("Poor Min Cond <"),
                        ExcelStyleModels.HorizontalCenter,
                        ExcelStyleModels.ThinBorder),
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
                        ExcelStyleModels.Right,
                        ExcelStyleModels.ThinBorder
                        )
                });

    }
}

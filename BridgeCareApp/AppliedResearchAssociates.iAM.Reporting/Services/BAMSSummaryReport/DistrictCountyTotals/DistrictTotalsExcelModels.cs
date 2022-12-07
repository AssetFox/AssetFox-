using System;
using System.Linq;
using OfficeOpenXml;

using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.DistrictCountyTotals
{
    public static class DistrictTotalsExcelModels
    {
        private static decimal TotalCost(AssetDetail section)
            => section.TreatmentConsiderations.Sum(_ => _.BudgetUsages.Sum(b => b.CoveredCost));

        internal static IExcelModel DistrictTableContent(
            SimulationYearDetail year,
            Func<AssetDetail, bool> inclusionPredicate)
        {
            decimal totalMoney = 0;
            var sections = year.Assets;
            foreach (var section in sections)
            {
                try
                {
                    if (inclusionPredicate(section))
                    {
                        var cost = TotalCost(section);
                        totalMoney += cost;
                    }
                }
                catch
                {
                    // just swallow the error and skip the section
                }
            }
            return StackedExcelModels.Stacked(
                ExcelValueModels.Money(totalMoney),
                ExcelStyleModels.Right,
                ExcelStyleModels.ThinBorder,
                ExcelStyleModels.CurrencyWithoutCentsFormat,
                DistrictTotalsStyleModels.LightGreenFill
                );
        }

        internal static IExcelModel YearlyAverage(SimulationOutput output)
        {
            var sumFunction = ExcelRangeFunctions.StartOffsetRangeSum(-output.Years.Count - 1, 0, -2, 0);
            string averageFunction(ExcelRange x) => $"{sumFunction(x)}/12";
            return StackedExcelModels.Stacked(
                ExcelFormulaModels.FromFunction(averageFunction),
                ExcelStyleModels.Right,
                ExcelStyleModels.ThinBorder,
                ExcelStyleModels.CurrencyFormat,
                DistrictTotalsStyleModels.LightOrangeFill);
        }

        internal static IExcelModel PercentYearlyAverage(int yOffset)
        {
            var localAddress = ExcelRangeFunctions.StartOffset(-1, 0);
            var bottomAddress = ExcelRangeFunctions.StartOffset(-1, yOffset, false, true);
            Func<ExcelRange, string> ratio = (ExcelRange range) =>
            {
                var returnValue = $@"IFERROR({localAddress(range)}/{bottomAddress(range)},0)";
                return returnValue;
            };
            return StackedExcelModels.Stacked(
                ExcelFormulaModels.FromFunction(ratio),
                ExcelStyleModels.HorizontalCenter,
                ExcelStyleModels.ThinBorder,
                ExcelStyleModels.PercentageFormat(2)
                );
        }
    }
}

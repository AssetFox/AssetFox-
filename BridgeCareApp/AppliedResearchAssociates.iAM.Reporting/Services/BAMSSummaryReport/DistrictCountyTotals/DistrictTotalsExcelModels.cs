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
            var totalMoney = DistrictTableContentValue(year, inclusionPredicate);
            return StackedExcelModels.Stacked(
                ExcelValueModels.Money(totalMoney),
                ExcelStyleModels.Right,
                ExcelStyleModels.ThinBorder,
                ExcelStyleModels.CurrencyWithoutCentsFormat,
                DistrictTotalsStyleModels.LightGreenFill
                );
        }

        internal static decimal DistrictTableContentValue(
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
            return totalMoney;
        }
    }
}

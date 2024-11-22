using System;
using System.Linq;
using OfficeOpenXml;

using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.DistrictCountyTotals
{
    public static class DistrictTotalsExcelModels
    {
        private static decimal TotalCost(AssetDetail section, int year)
            => section.TreatmentConsiderations.Sum(_ => _.FundingCalculationOutput?.AllocationMatrix.Where(_ => _.Year == year).Sum(b => b.AllocatedAmount) ?? 0);

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
                        var cost = TotalCost(section, year.Year);
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

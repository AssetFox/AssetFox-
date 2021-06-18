using System;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Services.SummaryReport.Models;

namespace BridgeCareCore.Services.SummaryReport.DistrictTotals
{
    public static class DistrictTotalsExcelModels
    {
        private static decimal TotalCost(SectionDetail section)
            => section.TreatmentConsiderations.Sum(_ => _.BudgetUsages.Sum(b => b.CoveredCost));

        internal static IExcelModel DistrictTableContent(
            SimulationYearDetail year,
            Func<SectionDetail, bool> inclusionPredicate)
        {
            decimal totalMoney = 0;
            var sections = year.Sections;
            foreach (var section in sections)
            {
                if (inclusionPredicate(section))
                {
                    var cost = TotalCost(section);
                    totalMoney += cost;
                }
            }
            return StackedExcelModels.Stacked(
                ExcelValueModels.Money(totalMoney),
                ExcelStyleModels.Right,
                ExcelStyleModels.ThinBorder,
                ExcelStyleModels.CurrencyFormat,
                DistrictTotalsStyleModels.LightGreenFill
                );
        }
    }
}

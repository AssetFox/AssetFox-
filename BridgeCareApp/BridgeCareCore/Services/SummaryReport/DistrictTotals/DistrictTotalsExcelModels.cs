using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            return ExcelValueModels.Money(totalMoney);
        }

        internal static IExcelModel TopTableDistrictContent(SimulationYearDetail year, int districtNumber)
        {
            Func<SectionDetail, bool> predicate = detail => DistrictTotalsSectionDetailPredicates.IsNumberedDistrictTopTable(detail, districtNumber);
            return DistrictTableContent(year, predicate);
        }


        internal static IExcelModel DistrictTableTurnpikeContent(SimulationYearDetail year)
        {
            Func<SectionDetail, bool> predicate = DistrictTotalsSectionDetailPredicates.IsCommittedTurnpike;
            return DistrictTableContent(year, predicate);
        }
    }
}

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

        internal static IEnumerable<IExcelModel> DistrictContent(SimulationOutput output, int districtNumber)
        {
            yield return ExcelIntegerValueModels.WithValue(districtNumber);
            foreach (var year in output.Years)
            {
                yield return DistrictTableContent(output, year, districtNumber);
            }
            var sumRange = ExcelRangeFunctions.StartOffsetRangeSum(-output.Years.Count, 0, -1, 0);
            yield return ExcelFormulaModels.FromFunction(sumRange);
        }

        internal static IExcelModel DistrictTableContent(
            SimulationOutput output,
            SimulationYearDetail year,
            int district)
        {
            decimal totalMoney = 0;
            var sections = year.Sections;
            foreach (var section in sections)
            {
                var actualDistrict = section.ValuePerTextAttribute["DISTRICT"];
                if (int.TryParse(actualDistrict, out var sectionDistrict) && sectionDistrict == district)
                {
                    var ownerCode = section.ValuePerTextAttribute["OWNER_CODE"];
                    if (ownerCode.Trim() != "01")
                    {
                        if (section.TreatmentCause == TreatmentCause.CommittedProject)
                        {
                            var textString = DictionaryDebugConvenience.ToDebugString(section.ValuePerTextAttribute);
                            var numericString = DictionaryDebugConvenience.ToDebugString(section.ValuePerNumericAttribute);
                            var cost = section.TreatmentConsiderations.Sum(_ => _.BudgetUsages.Sum(b => b.CoveredCost));
                            totalMoney += cost;
                        }
                    }
                }
            }
            return new ExcelMoneyValueModel
            {
                Value = totalMoney,
            };
        }
    }
}

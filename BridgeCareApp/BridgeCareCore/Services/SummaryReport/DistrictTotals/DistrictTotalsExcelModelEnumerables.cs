using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Services.SummaryReport.Models;

namespace BridgeCareCore.Services.SummaryReport.DistrictTotals
{
    public static class DistrictTotalsExcelModelEnumerables
    {
        internal static IEnumerable<IExcelModel> TopTableDistrictContent(SimulationOutput output, int districtNumber)
        {
            yield return ExcelValueModels.Integer(districtNumber);
            foreach (var year in output.Years)
            {
               yield return DistrictTotalsExcelModels.TopTableDistrictContent(year, districtNumber);
            }
            var sumRange = ExcelRangeFunctions.StartOffsetRangeSum(-output.Years.Count, 0, -1, 0);
            yield return ExcelFormulaModels.FromFunction(sumRange);
        }

        internal static IEnumerable<IExcelModel> TopTableTurnpikeContent(SimulationOutput output)
        {
            yield return ExcelValueModels.String("Turnpike");
            foreach (var year in output.Years)
            {
                yield return DistrictTotalsExcelModels.DistrictTableTurnpikeContent(year);
            }
            yield return ExcelFormulaModels.StartOffsetRangeSum(-output.Years.Count, 0, -1, 0);
        }
    }
}

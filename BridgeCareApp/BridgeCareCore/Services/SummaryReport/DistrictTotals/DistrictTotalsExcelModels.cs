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
            foreach (var _ in output.Years)
            {
                yield return DistrictTableContent(output);
            }
        }

        internal static IExcelModel DistrictTableContent(SimulationOutput output)
        {
            var function = DistrictTotalsFunctions.DistrictTotalsTableContent();
            return ExcelFormulaModels.FromFunction(function);
        }
    }
}

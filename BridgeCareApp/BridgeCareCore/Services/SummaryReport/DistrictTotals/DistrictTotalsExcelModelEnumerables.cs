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
            yield return StackedExcelModels.Stacked(
                ExcelValueModels.Integer(districtNumber),
                ExcelStyleModels.HorizontalCenter,
                ExcelStyleModels.ThinBorder
                );
            foreach (var year in output.Years)
            {
               yield return DistrictTotalsExcelModels.TopTableDistrictContent(year, districtNumber);
            }
            var sumRange = ExcelRangeFunctions.StartOffsetRangeSum(-output.Years.Count, 0, -1, 0);
            yield return
                StackedExcelModels.Stacked(
                ExcelFormulaModels.FromFunction(sumRange),
                ExcelStyleModels.Right,
                ExcelStyleModels.MediumBorder,
                DistrictTotalsStyleModels.DarkGreenFill
                );
        }

        internal static IEnumerable<IExcelModel> TopTableTurnpikeContent(SimulationOutput output)
        {
            yield return StackedExcelModels.Stacked(
                ExcelValueModels.String("Turnpike"),
                ExcelStyleModels.HorizontalCenter,
                ExcelStyleModels.ThinBorder
                );
            foreach (var year in output.Years)
            {
                yield return DistrictTotalsExcelModels.DistrictTableTurnpikeContent(year);
            }
            yield return StackedExcelModels.Stacked(
                ExcelFormulaModels.StartOffsetRangeSum(-output.Years.Count, 0, -1, 0),
                ExcelStyleModels.MediumBorder,
                DistrictTotalsStyleModels.DarkGreenFill
            );
        }
    }
}

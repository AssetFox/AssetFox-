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
        internal static IEnumerable<IExcelModel> MpmsTableDistrictContent(SimulationOutput output, int districtNumber)
        {
            yield return StackedExcelModels.Stacked(
                ExcelValueModels.Integer(districtNumber),
                ExcelStyleModels.HorizontalCenter,
                ExcelStyleModels.ThinBorder
                );
            foreach (var year in output.Years)
            {
               yield return DistrictTotalsExcelModels.MpmsTableDistrictContent(year, districtNumber);
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

        internal static IEnumerable<IExcelModel> BamsTableDistrictContent(SimulationOutput output, int districtNumber)
        {
            yield return StackedExcelModels.Stacked(
                ExcelValueModels.Integer(districtNumber),
                ExcelStyleModels.HorizontalCenter,
                ExcelStyleModels.ThinBorder
                );
            foreach (var year in output.Years)
            {
                yield return DistrictTotalsExcelModels.BamsTableDistrictContent(year, districtNumber);
            }
            var sumRange = ExcelRangeFunctions.StartOffsetRangeSum(-output.Years.Count, 0, -1, 0);
            yield return
                StackedExcelModels.Stacked(
                ExcelFormulaModels.FromFunction(sumRange),
                DistrictTotalsStyleModels.DarkGreenTotalsCells
                );
        }

        internal static IEnumerable<IExcelModel> TableContent(SimulationOutput output, IExcelModel title, Func<SectionDetail, bool> inclusionPredicate)
        {
            yield return StackedExcelModels.Stacked(
                title,
                ExcelStyleModels.HorizontalCenter,
                ExcelStyleModels.ThinBorder
                );
            foreach (var year in output.Years)
            {
                yield return DistrictTotalsExcelModels.DistrictTableContent(year, inclusionPredicate);
            }
            yield return StackedExcelModels.Stacked(
                ExcelFormulaModels.StartOffsetRangeSum(-output.Years.Count, 0, -1, 0),
                DistrictTotalsStyleModels.DarkGreenTotalsCells
            );
        }
    }
}

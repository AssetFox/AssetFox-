using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using BridgeCareCore.Helpers.Excel;

namespace BridgeCareCore.Services.SummaryReport.DistrictTotals
{
    public static class DistrictTotalsExcelModelEnumerables
    {
        internal static IEnumerable<IExcelModel> TableContent(SimulationOutput output, IExcelModel title, Func<AssetDetail, bool> inclusionPredicate)
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

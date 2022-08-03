﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.DistrictTotals
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

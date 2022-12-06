using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;

using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.DistrictCountyTotals
{
    public static class DistrictTotalsExcelModelEnumerables
    {
        internal static IEnumerable<IExcelModel> TableContent(SimulationOutput output, IExcelModel district, IExcelModel county, Func<AssetDetail, bool> inclusionPredicate)
        {
            yield return StackedExcelModels.Stacked(
                district,
                ExcelStyleModels.HorizontalCenter,
                ExcelStyleModels.ThinBorder
                );
            yield return StackedExcelModels.Stacked(
                county,
                ExcelStyleModels.Left,
                ExcelStyleModels.ThinBorder
                );
            foreach (var year in output.Years)
            {
                yield return DistrictTotalsExcelModels.DistrictTableContent(year, inclusionPredicate);
            }
        }

        internal static IEnumerable<IExcelModel> TableContentTotalsOrTurnpike(SimulationOutput output, Func<AssetDetail, bool> inclusionPredicate)
        {
            foreach (var year in output.Years)
            {
                yield return DistrictTotalsExcelModels.DistrictTableContent(year, inclusionPredicate);
            }
        }
    }
}

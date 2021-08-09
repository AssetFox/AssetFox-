using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Services.SummaryReport.Models;
using BridgeCareCore.Services.SummaryReport.Models.Worksheets;

namespace BridgeCareCore.Services.SummaryReport.DistrictTotals
{
    public static class DistrictTotalsModels
    {
        public static ExcelWorksheetModel DistrictTotals(SimulationOutput output)
            => new ExcelWorksheetModel
            {
                TabName = "District Totals",
                Content = new List<IExcelWorksheetContentModel>
                {
                    DistrictTotalsContent(output),
                    ExcelWorksheetContentModels.AutoFitColumns(12.5),
                    ExcelWorksheetContentModels.SpecificColumnWidthDelta(output.Years.Count + 2, x => x+3),
                }
            };

        public static AnchoredExcelRegionModel DistrictTotalsContent(SimulationOutput output)
        {
            return new AnchoredExcelRegionModel
            {
                Region = RowBasedExcelRegionModels.Concat(
                    DistrictTotalsRegions.MpmsTable(output),
                    RowBasedExcelRegionModels.BlankLine,
                    DistrictTotalsRegions.BamsTable(output),
                    RowBasedExcelRegionModels.BlankLine,
                    DistrictTotalsRegions.TotalsTable(output),
                    RowBasedExcelRegionModels.BlankLine,
                    DistrictTotalsRegions.PercentOverallDollarsByDistrictTable(output)
                    ),
            };
        }
    }
}

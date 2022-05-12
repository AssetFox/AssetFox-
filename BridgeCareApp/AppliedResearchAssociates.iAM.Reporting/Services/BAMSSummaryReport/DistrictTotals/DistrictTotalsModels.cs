using System.Collections.Generic;

using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Reporting.Interfaces.BAMSSummaryReport.Worksheets;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.Regions;
using AppliedResearchAssociates.iAM.Reporting.Models.BAMSSummaryReport.Worksheets;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.DistrictTotals
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

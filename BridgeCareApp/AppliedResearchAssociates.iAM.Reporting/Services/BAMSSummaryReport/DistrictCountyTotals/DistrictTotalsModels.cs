using System.Collections.Generic;

using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.ExcelHelpers;

namespace AppliedResearchAssociates.iAM.Reporting.Services.BAMSSummaryReport.DistrictCountyTotals
{
    public static class DistrictTotalsModels
    {
        public static ExcelWorksheetModel DistrictTotals(SimulationOutput output)
            => new ExcelWorksheetModel
            {
                TabName = "District County Totals",
                Content = new List<IExcelWorksheetContentModel>
                {
                    DistrictTotalsContent(output),
                    ExcelWorksheetContentModels.AutoFitColumns(12.5),
                    ExcelWorksheetContentModels.SpecificColumnWidthDelta(output.Years.Count + 2, x => x+3),
                }
            };

        public static AnchoredExcelRegionModel DistrictTotalsContent(SimulationOutput output)
        {
            int startingRow = 0;
            return new AnchoredExcelRegionModel
            {
                Region = RowBasedExcelRegionModels.Concat(
                    DistrictTotalsRegions.MpmsTable(output, ref startingRow),
                    RowBasedExcelRegionModels.BlankLine,
                    DistrictTotalsRegions.BamsTable(output, ref startingRow),
                    RowBasedExcelRegionModels.BlankLine,
                    DistrictTotalsRegions.OverallDollarsTable(output, ref startingRow),
                    RowBasedExcelRegionModels.BlankLine
                    //DistrictTotalsRegions.PercentOverallDollarsTable(output, ref startingRow)
                    ),
            };
        }
    }
}

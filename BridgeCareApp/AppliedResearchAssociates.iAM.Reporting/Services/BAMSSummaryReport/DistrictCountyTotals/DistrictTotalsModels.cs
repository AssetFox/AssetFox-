using System.Collections.Generic;
using System.Linq;
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

            var districtList = output.InitialAssetSummaries.Select(_ => _.ValuePerTextAttribute["DISTRICT"]).Distinct().Select(_ => int.Parse(_)).OrderBy(_ => _).Where(_ => _ != 37).ToList();

            return new AnchoredExcelRegionModel
            {
                Region = RowBasedExcelRegionModels.Concat(
                    DistrictTotalsRegions.MpmsTable(output, districtList),
                    RowBasedExcelRegionModels.BlankLine,
                    DistrictTotalsRegions.BamsTable(output, districtList),
                    RowBasedExcelRegionModels.BlankLine,
                    DistrictTotalsRegions.OverallDollarsTable(output, districtList),
                    RowBasedExcelRegionModels.BlankLine,
                    DistrictTotalsRegions.PercentageOverallDollarsTable(output, districtList)
                    ),
            };
        }
    }
}

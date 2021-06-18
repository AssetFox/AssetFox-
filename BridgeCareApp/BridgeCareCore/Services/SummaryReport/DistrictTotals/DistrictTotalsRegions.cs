using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Services.SummaryReport.Models;

namespace BridgeCareCore.Services.SummaryReport.DistrictTotals
{
    public static class DistrictTotalsRegions
    {
        public static List<int> NumberedDistricts
            => new List<int> { 1, 2, 3, 4, 5, 6, 8, 9, 10, 11, 12 }; // Seven is indeed skipped.
        public static RowBasedExcelRegionModel TopPortion(SimulationOutput output)
            => new RowBasedExcelRegionModel
            {
                Rows = new List<ExcelRowModel>
                {
                    DistrictTotalsRowModels.IndexingRow(output.Years.Count),
                    DistrictTotalsRowModels.FirstYearRow(output),
                }
            };

        public static RowBasedExcelRegionModel MpmsTable(SimulationOutput output)
        {
            var rows = new List<ExcelRowModel>
            {
                ExcelRowModels.IndentedHeader(1, "Dollars Spent on MPMS Projects by District", output.Years.Count, 1),
                DistrictTotalsRowModels.DistrictAndYearsHeaders(output, DistrictTotalsStringConstants.DistrictTotals)
            };
            foreach (var districtNumber in NumberedDistricts)
            {
                rows.Add(DistrictTotalsRowModels.MpmsTableDistrict(output, districtNumber));
            }
            rows.Add(DistrictTotalsRowModels.MpmsTableTurnpike(output));
            rows.Add(DistrictTotalsRowModels.TableBottomSumRow(output));
            return RowBasedExcelRegionModels.WithRows(rows);
        }

        public static RowBasedExcelRegionModel BamsTable(SimulationOutput output)
        {
            var rows = new List<ExcelRowModel>
            {
                ExcelRowModels.IndentedHeader(1, "Dollars Spent on BAMS Projects by District", output.Years.Count, 1),
                DistrictTotalsRowModels.DistrictAndYearsHeaders(output, DistrictTotalsStringConstants.DistrictTotals),
            };
            foreach (var districtNumber in NumberedDistricts)
            {
                rows.Add(DistrictTotalsRowModels.BamsTableDistrict(output, districtNumber));
            }
            rows.Add(DistrictTotalsRowModels.BamsTableTurnpike(output));
            rows.Add(DistrictTotalsRowModels.TableBottomSumRow(output));
            return RowBasedExcelRegionModels.WithRows(rows);
        }

    }
}

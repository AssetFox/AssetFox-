using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis;
using BridgeCareCore.Services.SummaryReport.Models;

namespace BridgeCareCore.Services.SummaryReport.DistrictTotals
{
    public static class DistrictTotalsModels
    {
        public static RowBasedExcelWorksheetModel DistrictTotals(SimulationOutput output)
        {
            var numberOfYears = output.Years.Count;
            return new RowBasedExcelWorksheetModel
            {
                TabName = "District Totals",
                Rows = new List<ExcelRowModel>
                {
                    DistrictTotalsRowModels.IndexingRow(numberOfYears),
                    DistrictTotalsRowModels.FirstYearRow(output),
                    ExcelRowModels.Empty,
                    ExcelRowModels.IndentedHeader(1, "Dollars Spent on MPMS Projects by District", numberOfYears, 1),
                },
            };
        }
    }
}

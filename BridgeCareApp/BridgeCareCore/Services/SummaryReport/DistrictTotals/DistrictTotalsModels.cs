using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BridgeCareCore.Services.SummaryReport.Models;

namespace BridgeCareCore.Services.SummaryReport.DistrictTotals
{
    public static class DistrictTotalsModels
    {
        public static RowBasedExcelWorksheetModel DistrictTotals
            => new RowBasedExcelWorksheetModel
            {
                TabName = "District Totals",
                Rows = new List<ExcelRowModel>
                {
                    DistrictTotalsRowModels.TopRow(),
                    DistrictTotalsRowModels.IndexingRow(10),
                },
            };
    }
}

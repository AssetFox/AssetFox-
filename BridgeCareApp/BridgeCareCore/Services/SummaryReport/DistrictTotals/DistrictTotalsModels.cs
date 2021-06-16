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
                    ExcelWorksheetContentModels.AutoFitColumns(12.5)
                }
            };

        public static RowBasedExcelWorksheetContentModel DistrictTotalsContent(SimulationOutput output)
        {
            var numberOfYears = output.Years.Count;
            return new RowBasedExcelWorksheetContentModel
            {
                Rows = new List<ExcelRowModel>
                {
                    DistrictTotalsRowModels.IndexingRow(numberOfYears),
                    DistrictTotalsRowModels.FirstYearRow(output),
                    ExcelRowModels.Empty,
                    ExcelRowModels.IndentedHeader(1, "Dollars Spent on MPMS Projects by District", numberOfYears, 1),
                    DistrictTotalsRowModels.DistrictAndYearsHeaders(output, "District Total"),
                },
            };
        }
    }
}

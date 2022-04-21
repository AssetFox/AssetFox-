using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Services.SummaryReport.Models;

namespace BridgeCareCore.Services.Treatment
{
    public static class TreatmentCostsRegion
    {

        internal static RowBasedExcelRegionModel CostsRegion(TreatmentDTO dto)
        {
            var rows = new List<ExcelRowModel>
            {
                CostsTitleRow(),
                CostsHeaderRow(),
            };
            foreach (var cost in dto.Costs)
            {
                var contentRow = CostsContentRow(cost);
                rows.Add(contentRow);
            }
            return RowBasedExcelRegionModels.WithRows(rows);
        }

        private static ExcelRowModel CostsContentRow(TreatmentCostDTO cost)
        {
            var models = TreatmentCostHeaderWithContentModels.TreatmentCostExport();
            var entries = models.Select(m => m.Content(cost)).ToList();
            var r = ExcelRowModels.WithEntries(entries);
            return r;
        }

        private static ExcelRowModel CostsTitleRow()
        {
            var cell = ExcelValueModels.RichString(TreatmentExportStringConstants.Costs, true, 14);
            var r = ExcelRowModels.WithEntries(cell);
            return r;
        }
        private static ExcelRowModel CostsHeaderRow()
        {
            var models = TreatmentCostHeaderWithContentModels.TreatmentCostExport();
            var entries = models.Select(m => m.Header).ToList();
            var r = ExcelRowModels.WithEntries(entries);
            r.EveryCell = ExcelStyleModels.ThinBottomBorder();
            return r;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.ExcelHelpers;

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
            var returnValue = ExcelTableRowModels.ContentRow(models, cost);
            return returnValue;
        }

        private static ExcelRowModel CostsTitleRow()
        {
            var cell = ExcelValueModels.RichString(TreatmentExportStringConstants.Costs, true, 14);
            var returnValue = ExcelRowModels.WithEntries(cell);
            return returnValue;
        }
        private static ExcelRowModel CostsHeaderRow()
        {
            var models = TreatmentCostHeaderWithContentModels.TreatmentCostExport();
            var style = ExcelStyleModels.ThinBottomBorder();
            var returnValue = ExcelTableRowModels.HeaderRow(models, style);
            return returnValue;
        }
    }
}

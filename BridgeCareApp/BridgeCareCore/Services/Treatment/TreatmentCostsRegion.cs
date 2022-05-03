﻿using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Helpers.Excel;

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
            var r = ExcelTableRowModels.ContentRow(models, cost);
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
            var style = ExcelStyleModels.ThinBottomBorder();
            var r = ExcelTableRowModels.HeaderRow(models, style);
            return r;
        }
    }
}

using System.Collections.Generic;
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
            var equation = cost.Equation.Expression;
            var equationCell = ExcelValueModels.String(equation);
            var criteria = cost.CriterionLibrary.MergedCriteriaExpression;
            var criteriaCell = ExcelValueModels.String(criteria);
            var r = ExcelRowModels.WithEntries(equationCell, criteriaCell);
            return r;
        }

        private static ExcelRowModel CostsTitleRow()
        {
            var cell = ExcelValueModels.RichString("Costs", true);
            var r = ExcelRowModels.WithEntries(cell);
            return r;
        }
        private static ExcelRowModel CostsHeaderRow()
        {
            var equationCell = StackedExcelModels.BoldText("Equation");
            var criteriaCell = StackedExcelModels.BoldText("Criteria");
            var r = ExcelRowModels.WithEntries(equationCell, criteriaCell);
            return r;
        }
    }
}

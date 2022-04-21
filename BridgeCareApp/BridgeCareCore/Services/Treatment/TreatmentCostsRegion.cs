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
            var equationId = cost.Equation.Id.ToString();
            var equationIdCell = ExcelValueModels.String(equationId);
            var criteria = cost.CriterionLibrary.MergedCriteriaExpression;
            var criteriaCell = ExcelValueModels.String(criteria);
            var name = cost.CriterionLibrary.Name;
            var nameCell = ExcelValueModels.String(name);
            var description = cost.CriterionLibrary.Description;
            var descriptionCell = ExcelValueModels.String(description);
            var id = cost.CriterionLibrary.Id.ToString();
            var criterionIdCell = ExcelValueModels.String(id);
            var costId = cost.Id;
            var costIdCell = ExcelValueModels.String(costId.ToString());
            var r = ExcelRowModels.WithEntries(equationCell, equationIdCell, criteriaCell, nameCell, descriptionCell, criterionIdCell, costIdCell);
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
            var equationCell = StackedExcelModels.BoldText("Equation");
            var equationIdCell = StackedExcelModels.BoldText(TreatmentExportStringConstants.CostEquationId);
            var criteriaCell = StackedExcelModels.BoldText(TreatmentExportStringConstants.CostCriterion);
            var nameCell = StackedExcelModels.BoldText(TreatmentExportStringConstants.CostCriterionName);
            var descriptionCell = StackedExcelModels.BoldText(TreatmentExportStringConstants.CostCriterionDescription);
            var criterionIdCell = StackedExcelModels.BoldText(TreatmentExportStringConstants.CostCriterionId);
            var costIdCell = StackedExcelModels.BoldText(TreatmentExportStringConstants.CostId);
            var r = ExcelRowModels.WithEntries(equationCell, equationIdCell, criteriaCell, nameCell, descriptionCell, criterionIdCell, costIdCell);
            r.EveryCell = ExcelStyleModels.ThinBottomBorder();
            return r;
        }
    }
}

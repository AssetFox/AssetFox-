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
            var name = cost.CriterionLibrary.Name;
            var nameCell = ExcelValueModels.String(name);
            var description = cost.CriterionLibrary.Description;
            var descriptionCell = ExcelValueModels.String(description);
            var id = cost.CriterionLibrary.Id.ToString();
            var idCell = ExcelValueModels.String(id);
            var r = ExcelRowModels.WithEntries(equationCell, criteriaCell, nameCell, descriptionCell, idCell);
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
            var criteriaCell = StackedExcelModels.BoldText("Criteria");
            var nameCell = StackedExcelModels.BoldText(TreatmentExportStringConstants.CostName);
            var descriptionCell = StackedExcelModels.BoldText(TreatmentExportStringConstants.CostDescription);
            var idCell = StackedExcelModels.BoldText(TreatmentExportStringConstants.Id);
            var r = ExcelRowModels.WithEntries(equationCell, criteriaCell, nameCell, descriptionCell, idCell);
            r.EveryCell = ExcelStyleModels.ThinBottomBorder();
            return r;
        }
    }
}

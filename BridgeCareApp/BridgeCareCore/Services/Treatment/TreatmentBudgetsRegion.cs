using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.ExcelHelpers;

namespace BridgeCareCore.Services.Treatment
{
    public static class TreatmentBudgetsRegion
    {
        internal static RowBasedExcelRegionModel BudgetsRegion(TreatmentDTO dto)
        {
            var rows = new List<ExcelRowModel>
            {
                BudgetsTitleRow(),
                BudgetsHeaderRow(),
            };
            if(dto.Budgets != null)
            {
                foreach (var budget in dto.Budgets)
                {
                    var budgetRow = BudgetsRow(budget);
                    rows.Add(budgetRow);
                }
            }
            
            return RowBasedExcelRegionModels.WithRows(rows);
        }

        private static ExcelRowModel BudgetsRow(TreatmentBudgetDTO budget)
        {
            var name = budget.Name;
            var budgetCell = ExcelValueModels.String(name);
            var returnValue = ExcelRowModels.WithEntries(budgetCell);
            return returnValue;
        }

        private static ExcelRowModel BudgetsTitleRow()
        {
            var cell = ExcelValueModels.RichString(TreatmentExportStringConstants.Budgets, true, 14);
            var returnValue = ExcelRowModels.WithEntries(cell);
            return returnValue;
        }
        private static ExcelRowModel BudgetsHeaderRow()
        {
            var budgetCell = StackedExcelModels.BoldText(TreatmentExportStringConstants.BudgetName);
            var returnValue = ExcelRowModels.WithEntries(budgetCell);
            returnValue.EveryCell = ExcelStyleModels.ThinBottomBorder();
            return returnValue;
        }
    }
}

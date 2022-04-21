using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Services.SummaryReport.Models;

namespace BridgeCareCore.Services.Treatment
{
    public static class TreatmentConsequencesRegion
    {

        internal static RowBasedExcelRegionModel ConsequencesRegion(TreatmentDTO dto)
        {
            var rows = new List<ExcelRowModel>
            {
                ConsequencesTitleRow(),
                ConsequencesHeaderRow(),
            };
            foreach (var consequence in dto.Consequences)
            {
                var consquenceRow = ConsequencesRow(consequence);
                rows.Add(consquenceRow);
            }
            return RowBasedExcelRegionModels.WithRows(rows);
        }

        private static ExcelRowModel ConsequencesRow(TreatmentConsequenceDTO consequence)
        {
            var attribute = consequence.Attribute;
            var attributeCell = ExcelValueModels.String(attribute);
            var changeValue = consequence.ChangeValue;
            var changeValueCell = ExcelValueModels.String(changeValue);
            var equation = consequence.Equation.Expression;
            var equationCell = ExcelValueModels.String(equation);
            var criteria = consequence.CriterionLibrary.MergedCriteriaExpression;
            var criteriaCell = ExcelValueModels.String(criteria);
            var name = consequence.CriterionLibrary.Name;
            var nameCell = ExcelValueModels.String(name);
            var description = consequence.CriterionLibrary.Description;
            var descriptionCell = ExcelValueModels.String(description);
            var id = consequence.CriterionLibrary.Id.ToString();
            var idCell = ExcelValueModels.String(id);
            var r = ExcelRowModels.WithEntries(attributeCell, changeValueCell, equationCell, criteriaCell, nameCell, descriptionCell, idCell);
            return r;
        }

        private static ExcelRowModel ConsequencesTitleRow()
        {
            var cell = ExcelValueModels.RichString(TreatmentExportStringConstants.Consequences, true, 14);
            var r = ExcelRowModels.WithEntries(cell);
            return r;
        }
        private static ExcelRowModel ConsequencesHeaderRow()
        {
            var attributeCell = StackedExcelModels.BoldText(TreatmentExportStringConstants.Attribute);
            var changeValueCell = StackedExcelModels.BoldText(TreatmentExportStringConstants.ChangeVal);
            var equationCell = StackedExcelModels.BoldText(TreatmentExportStringConstants.Equation);
            var criteriaCell = StackedExcelModels.BoldText(TreatmentExportStringConstants.ConsequenceCriterion);
            var nameCell = StackedExcelModels.BoldText(TreatmentExportStringConstants.ConsequenceCriterionName);
            var descriptionCell = StackedExcelModels.BoldText(TreatmentExportStringConstants.ConsequenceCriterionDescription);
            var idCell = StackedExcelModels.BoldText(TreatmentExportStringConstants.ConsequenceCriterionId);
            var r = ExcelRowModels.WithEntries(attributeCell, changeValueCell, equationCell, criteriaCell, nameCell, descriptionCell, idCell);
            r.EveryCell = ExcelStyleModels.ThinBottomBorder();
            return r;
        }
    }
}

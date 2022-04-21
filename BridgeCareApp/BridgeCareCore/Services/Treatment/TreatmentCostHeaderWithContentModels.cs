using System.Collections.Generic;
using BridgeCareCore.Services.SummaryReport.Models;
using TModel = BridgeCareCore.Services.SummaryReport.Models.Tables.ExcelHeaderWithContentModel<AppliedResearchAssociates.iAM.DTOs.TreatmentCostDTO>;

namespace BridgeCareCore.Services.Treatment
{

    public static class TreatmentCostHeaderWithContentModels
    {
        public static TModel Id
            => new()
            {
                Header = StackedExcelModels.BoldText(TreatmentExportStringConstants.CostId),
                Content = cost => ExcelValueModels.String(cost.Id.ToString()),
            };
        public static TModel EquationId
            => new()
            { 
                Header = StackedExcelModels.BoldText(TreatmentExportStringConstants.CostEquationId),
                Content = cost => ExcelValueModels.String(cost.Equation.Id.ToString()),
            };

        public static TModel EquationExpression
            => new()
            {
                Header = StackedExcelModels.BoldText(TreatmentExportStringConstants.Equation),
                Content = cost => ExcelValueModels.String(cost.Equation.Expression),
            };

        public static TModel CriterionExpression
            => new()
            {
                Header = StackedExcelModels.BoldText(TreatmentExportStringConstants.CostCriterion),
                Content = cost => ExcelValueModels.String(cost.CriterionLibrary.MergedCriteriaExpression),
            };
        public static TModel CriterionName
            => new()
            {
                Header = StackedExcelModels.BoldText(TreatmentExportStringConstants.CostCriterionName),
                Content = cost => ExcelValueModels.String(cost.CriterionLibrary.Name),
            };

        public static TModel CriterionDescription
            => new()
            {
                Header = StackedExcelModels.BoldText(TreatmentExportStringConstants.CostCriterionDescription),
                Content = cost => ExcelValueModels.String(cost.CriterionLibrary.Description),
            };

        public static TModel CriterionId
            => new()
            {
                Header = StackedExcelModels.BoldText(TreatmentExportStringConstants.CostCriterionId),
                Content = cost => ExcelValueModels.String(cost.CriterionLibrary.Id.ToString()),
            };

        public static IEnumerable<TModel> TreatmentCostExport()
        {
            yield return EquationExpression;
            yield return EquationId;
            yield return CriterionExpression;
            yield return CriterionName;
            yield return CriterionDescription;
            yield return CriterionId;
            yield return Id;
        }
    }
}

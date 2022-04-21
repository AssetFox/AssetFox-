using System.Collections.Generic;
using BridgeCareCore.Services.SummaryReport.Models;
using TModel = BridgeCareCore.Services.SummaryReport.Models.Tables.ExcelHeaderWithContentModel<AppliedResearchAssociates.iAM.DTOs.TreatmentCostDTO>;

namespace BridgeCareCore.Services.Treatment
{

    public static class TreatmentCostHeaderWithContentModels
    {
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

        public static IEnumerable<TModel> TreatmentCostExport()
        {
            yield return EquationExpression;
            yield return CriterionExpression;
        }
    }
}

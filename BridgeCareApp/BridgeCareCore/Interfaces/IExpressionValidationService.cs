using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models.Validation;

namespace BridgeCareCore.Interfaces
{
    public interface IExpressionValidationService
    {
        ValidationResult ValidateEquation(EquationValidationParameters model);

        CriterionValidationResult ValidateCriterion(string mergedCriteriaExpression,
            UserCriteriaDTO currentUserCriteriaFilter);

        CriterionValidationResult ValidateCriterionWithoutResults(string mergedCriteriaExpression,
            UserCriteriaDTO currentUserCriteriaFilter);
    }
}

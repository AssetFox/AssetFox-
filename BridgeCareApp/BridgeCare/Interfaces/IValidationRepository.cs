using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IValidationRepository
    {
        ValidationResult ValidateEquation(EquationValidationParametersModel data, BridgeCareContext db);

        CriterionValidationResult ValidateCriteria(string data, BridgeCareContext db);
    }
}

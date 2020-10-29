using BridgeCare.Models;

namespace BridgeCare.Interfaces
{
    public interface IValidationRepository
    {
        EquationValidationResult ValidateEquation(ValidateEquationModel data, BridgeCareContext db);

        CriteriaValidationResult ValidateCriteria(string data, BridgeCareContext db);
    }
}

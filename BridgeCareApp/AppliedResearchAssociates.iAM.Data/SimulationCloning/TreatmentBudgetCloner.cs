using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    internal class TreatmentBudgetCloner
    {
        internal static TreatmentBudgetDTO Clone(TreatmentBudgetDTO treatmentBudget)
        {
            var clone = new TreatmentBudgetDTO
            {
                Name = treatmentBudget.Name,
            };
            return clone;
        }

    }
}

using AppliedResearchAssociates.iAM.DTOs;
using System;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    internal class TreatmentCostCloner
    {
        internal static TreatmentCostDTO Clone(TreatmentCostDTO treatmentCost, Guid ownerId)
        {
            var cloneCritionLibrary = CriterionLibraryCloner.Clone(treatmentCost.CriterionLibrary, ownerId);
            var cloneEquation = EquationCloner.Clone(treatmentCost.Equation);
            var clone = new TreatmentCostDTO
            {
                Equation = cloneEquation,
                CriterionLibrary = cloneCritionLibrary,
            };
            return clone;
        }

    }

}

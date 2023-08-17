using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    internal class TreatmentConsequenceCloner
    {
        internal static TreatmentConsequenceDTO Clone(TreatmentConsequenceDTO treatmentConsequence, Guid ownerId)
        {
            var cloneCritionLibrary = CriterionLibraryCloner.Clone(treatmentConsequence.CriterionLibrary, ownerId);
            var cloneEquation = EquationCloner.Clone(treatmentConsequence.Equation);
            var clone = new TreatmentConsequenceDTO
            {
                Equation = cloneEquation,
                CriterionLibrary = cloneCritionLibrary,
            };
            return clone;
        }

    }

}

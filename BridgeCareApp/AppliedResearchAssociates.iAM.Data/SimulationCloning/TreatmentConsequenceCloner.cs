using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    internal class TreatmentConsequenceCloner
    {
        internal static TreatmentConsequenceDTO Clone(TreatmentConsequenceDTO treatmentConsequence, Guid ownerId)
        {
            var cloneCritionLibrary = CriterionLibraryCloner.CloneNullPropagating(treatmentConsequence.CriterionLibrary, ownerId);
            var cloneEquation = EquationCloner.Clone(treatmentConsequence.Equation, ownerId);
            var clone = new TreatmentConsequenceDTO
            {
                Equation = cloneEquation,
                CriterionLibrary = cloneCritionLibrary,
            };
            return clone;
        }

    }

}

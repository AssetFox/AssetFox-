using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    internal class TreatmentConsequenceCloner
    {
        internal static TreatmentConsequenceDTO Clone(TreatmentConsequenceDTO treatmentConsequence, Guid ownerId)
        {
            var cloneCriterionLibrary = CriterionLibraryCloner.CloneNullPropagating(treatmentConsequence.CriterionLibrary, ownerId);
            var cloneEquation = EquationCloner.Clone(treatmentConsequence.Equation, ownerId);
            var clone = new TreatmentConsequenceDTO
            {
                Equation = cloneEquation,
                CriterionLibrary = cloneCriterionLibrary,
            };
            return clone;
        }

    }

}

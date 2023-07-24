﻿using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services.SimulationCloning
{
    internal class TreatmentConsequenceCloner
    {
        internal static TreatmentConsequenceDTO Clone(TreatmentConsequenceDTO treatmentConsequence)
        {
            var cloneCritionLibrary = CriterionLibraryCloner.Clone(treatmentConsequence.CriterionLibrary);
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

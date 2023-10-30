using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services
{ 
    internal class TreatmentConsequenceCloner
    {
        internal static TreatmentConsequenceDTO Clone(TreatmentConsequenceDTO treatmentConsequence, Guid ownerId)
        {
            var cloneCriterionLibrary = CriterionLibraryCloner.CloneNullPropagating(treatmentConsequence.CriterionLibrary, ownerId);
            var cloneEquation = EquationCloner.Clone(treatmentConsequence.Equation, ownerId);
            var clone = new TreatmentConsequenceDTO
            {
                Id = Guid.NewGuid(),
                Equation = cloneEquation,
                Attribute =  treatmentConsequence.Attribute,
                ChangeValue = treatmentConsequence.ChangeValue,
                CriterionLibrary = cloneCriterionLibrary,
            };
            return clone;
        }

        internal static List<TreatmentConsequenceDTO> CloneList(IEnumerable<TreatmentConsequenceDTO> treatmentConsequences, Guid ownerId)
        {
            var clone = new List<TreatmentConsequenceDTO>();
            foreach (var treatmentConsequence in treatmentConsequences)
            {
                var childClone = Clone(treatmentConsequence, ownerId);
                clone.Add(childClone);
            }
            return clone;

        }
    }

}

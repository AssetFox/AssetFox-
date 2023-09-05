using AppliedResearchAssociates.iAM.DTOs;
using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.Data.SimulationCloning
{
    internal class TreatmentCostCloner
    {
        internal static TreatmentCostDTO Clone(TreatmentCostDTO treatmentCost, Guid ownerId)
        {
            var cloneCriterionLibrary = CriterionLibraryCloner.CloneNullPropagating(treatmentCost.CriterionLibrary, ownerId);
            var cloneEquation = EquationCloner.Clone(treatmentCost.Equation, ownerId);
            var clone = new TreatmentCostDTO
            {
                Id = Guid.NewGuid(),
                Equation = cloneEquation,
                CriterionLibrary = cloneCriterionLibrary,
            };
            return clone;
        }

        internal static List<TreatmentCostDTO> CloneList(IEnumerable<TreatmentCostDTO> treatmentCosts, Guid ownerId)
        {
            var clone = new List<TreatmentCostDTO>();
            foreach (var treatmentCost in treatmentCosts)
            {
                var childClone = Clone(treatmentCost, ownerId);
                clone.Add(childClone);
            }
            return clone;

        }
    }

}

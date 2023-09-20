using AppliedResearchAssociates.iAM.DTOs;
using System;
using System.Collections.Generic;

namespace BridgeCareCore.Services.SimulationCloning
{
    internal class TreatmentCloner
    {
        internal static TreatmentDTO Clone(TreatmentDTO treatment, Guid ownerId)
        {            
            var cloneCriterionLibrary = CriterionLibraryCloner.CloneNullPropagating(treatment.CriterionLibrary, ownerId);
            var cloneTreatmentCost = TreatmentCostCloner.CloneList(treatment.Costs, ownerId);
            var cloneTreatmentConsequence = TreatmentConsequenceCloner.CloneList(treatment.Consequences, ownerId);
            var clone = new TreatmentDTO
            {
              Name = treatment.Name,
              LibraryId = treatment.LibraryId,
              Description = treatment.Description,
              ShadowForAnyTreatment = treatment.ShadowForAnyTreatment,
              ShadowForSameTreatment = treatment.ShadowForSameTreatment,
              CriterionLibrary = cloneCriterionLibrary,
              AssetType = treatment.AssetType,
              BudgetIds = treatment.BudgetIds,
              Budgets = treatment.Budgets,
              Category = treatment.Category,
              Consequences = cloneTreatmentConsequence,
              Costs = cloneTreatmentCost,
              Id = Guid.NewGuid(),
              IsModified = treatment.IsModified,
              PerformanceFactors = treatment.PerformanceFactors,              
            };
            return clone;
        }
        internal static List<TreatmentDTO> CloneList(IEnumerable<TreatmentDTO> treatments, Guid ownerId)
        {
            var clone = new List<TreatmentDTO>();
            foreach (var treatment in treatments)
            {
                var childClone = Clone(treatment, ownerId);
                clone.Add(childClone);
            }
            return clone;

        }
    }
}

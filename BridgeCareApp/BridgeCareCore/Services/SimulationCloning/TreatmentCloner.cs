using AppliedResearchAssociates.iAM.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BridgeCareCore.Services
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
              SupersedeRules = new List<TreatmentSupersedeRuleDTO>(),
              IsUnselectable = treatment.IsUnselectable,
            }; 
            return clone;
        }

        internal static List<TreatmentDTO> CloneList(IEnumerable<TreatmentDTO> treatments, Guid ownerId)
        {
            var cloneList = new List<TreatmentDTO>();
            foreach (var treatment in treatments)
            {
                var childClone = Clone(treatment, ownerId);
                cloneList.Add(childClone);
            }

            // Clone supersede rules here(to get correct prevent treatment ids assigned for rules
            foreach (var treatmentClone in cloneList)
            {
                var treatment = treatments.FirstOrDefault(_ => _.Name == treatmentClone.Name);
                if (treatment.SupersedeRules.Any())
                {
                    var cloneSupersedeRules = TreatmentSupersedeRuleCloner.CloneList(treatment.SupersedeRules, cloneList, ownerId);
                    treatmentClone.SupersedeRules = cloneSupersedeRules;
                }
            }

            return cloneList;
        }
    }
}

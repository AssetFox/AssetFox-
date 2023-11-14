using AppliedResearchAssociates.iAM.DTOs;
using System.Collections.Generic;
using System;
using System.Linq;

namespace BridgeCareCore.Services
{
    public class TreatmentSupersedeRuleCloner
    {
        internal static TreatmentSupersedeRuleDTO Clone(TreatmentSupersedeRuleDTO treatmentSupersedeRule, TreatmentDTO preventSupersedeRule, Guid ownerId)
        {
            var cloneCriterionLibrary = CriterionLibraryCloner.CloneNullPropagating(treatmentSupersedeRule.CriterionLibrary, ownerId);            
            var cloneTreatmentSupersedeRule = new TreatmentSupersedeRuleDTO
            {
                Id = Guid.NewGuid(),                
                CriterionLibrary = cloneCriterionLibrary,
                treatment = preventSupersedeRule
            };
            return cloneTreatmentSupersedeRule;
        }

        internal static List<TreatmentSupersedeRuleDTO> CloneList(IEnumerable<TreatmentSupersedeRuleDTO> treatmentSupersedeRules, List<TreatmentDTO> cloneListWithoutSupersedeRules, Guid ownerId)
        {
            var cloneList = new List<TreatmentSupersedeRuleDTO>();
            foreach (var treatmentSupersedeRule in treatmentSupersedeRules)
            {
                var preventSupersedeRule = cloneListWithoutSupersedeRules.FirstOrDefault(_ => _.Name == treatmentSupersedeRule.treatment.Name);
                var cloneTreatmentSupersedeRule = Clone(treatmentSupersedeRule, preventSupersedeRule, ownerId);
                cloneList.Add(cloneTreatmentSupersedeRule);
            }
            return cloneList;
        }
    }
}

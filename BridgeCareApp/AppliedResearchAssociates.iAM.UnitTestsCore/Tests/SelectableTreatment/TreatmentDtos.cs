using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests.SelectableTreatment
{
    public static class TreatmentDtos
    {
        public static TreatmentDTO Dto(Guid? id = null, string name = "Treatment name")
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new TreatmentDTO
            {
                Id = resolveId,
                Name = name,
                Description = "Treatment description",
            };
            return dto;
        }

        public static TreatmentDTO DtoWithEmptyCostsAndConsequencesLists(
            Guid? id = null,
            string name = "Bridge Replacement",
            string treatmentCriterionExpression = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var treatmentCriterion = treatmentCriterionExpression == null ? null :
                CriterionLibraryDtos.Dto(null, treatmentCriterionExpression);
            var dto = new TreatmentDTO
            {
                Id = resolveId,
                AssetType = "",
                Name = name,
                Description = "Treatment description",
                BudgetIds = new List<Guid>(),
                Costs = new List<TreatmentCostDTO>(),
                Consequences = new List<TreatmentConsequenceDTO>(),
                PerformanceFactors = new List<TreatmentPerformanceFactorDTO>(),
                SupersedeRules = new List<TreatmentSupersedeRuleDTO>(),
                ShadowForAnyTreatment = 2,
                ShadowForSameTreatment = 5,
                CriterionLibrary = treatmentCriterion,
            };
            return dto;
        }

        public static TreatmentDTO DtoWithEmptyCostsAndConsequencesListsWithCriterionLibrary(Guid? id = null, string name = "Treatment name")
        {

            var criterionLibrary = CriterionLibraryDtos.Dto();
            var resolveId = id ?? Guid.NewGuid();
            var dto = new TreatmentDTO
            {
                Id = resolveId,
                Name = name,
                AssetType = "",
                Description = "Treatment description",
                Costs = new List<TreatmentCostDTO>(),
                Consequences = new List<TreatmentConsequenceDTO>(),
                PerformanceFactors = new List<TreatmentPerformanceFactorDTO>(),
                SupersedeRules = new List<TreatmentSupersedeRuleDTO>(),
                CriterionLibrary = criterionLibrary
            };
            return dto;
        }

        public static TreatmentDTO DtoWithEmptyListsWithCriterionLibrary(Guid? id = null, string name = "Treatment name")
        {
            var criterionLibrary = CriterionLibraryDtos.Dto();
            var resolveId = id ?? Guid.NewGuid();
            var dto = new TreatmentDTO
            {
                Id = resolveId,
                Name = name,
                AssetType = "",
                Description = "Treatment description",
                Costs = new List<TreatmentCostDTO>(),
                Consequences = new List<TreatmentConsequenceDTO>(),
                PerformanceFactors = new List<TreatmentPerformanceFactorDTO>(),
                BudgetIds = new List<Guid>(),
                Budgets = new List<TreatmentBudgetDTO>(),
                SupersedeRules = new List<TreatmentSupersedeRuleDTO>(),
                CriterionLibrary = criterionLibrary
            };
            return dto;
        }

        public static TreatmentSupersedeRuleDTO SupersedeRule(TreatmentDTO preventTreatment, string mergedCriteriaExpression = null)
        {
            var criterionLibrary = CriterionLibraryDtos.Dto(mergedCriteriaExpression: mergedCriteriaExpression);
            return new TreatmentSupersedeRuleDTO
            {                
                Id = Guid.NewGuid(),
                treatment = preventTreatment,
                CriterionLibrary = criterionLibrary
            };
        }
    }
}

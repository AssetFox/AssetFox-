using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests;

namespace AppliedResearchAssociates.iAM.UnitTestsCore
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
                Name = name,
                Description = "Treatment description",
                BudgetIds = new List<Guid>(),
                Budgets = new List<TreatmentBudgetDTO>(),
                Costs = new List<TreatmentCostDTO>(),
                Consequences = new List<TreatmentConsequenceDTO>(),
                PerformanceFactors = new List<TreatmentPerformanceFactorDTO>(),
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
                Description = "Treatment description",
                Costs = new List<TreatmentCostDTO>(),
                Consequences = new List<TreatmentConsequenceDTO>(),
                PerformanceFactors = new List<TreatmentPerformanceFactorDTO>(),
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

        public static TreatmentDTO DtoWithEmptyCostsAndConsequencesListsWithSupersedeRule(TreatmentDTO supersededTreatment, Guid? id = null, string name = "Treatment name", string mergedCriteriaExpression = null)
        {

            var criterionLibrary = CriterionLibraryDtos.Dto();
            var resolveId = id ?? Guid.NewGuid();
            var dto = new TreatmentDTO
            {
                Id = resolveId,
                BudgetIds = new List<Guid>(),
                Name = name,
                Description = "Treatment description",
                Costs = new List<TreatmentCostDTO>(),
                Consequences = new List<TreatmentConsequenceDTO>(),
                PerformanceFactors = new List<TreatmentPerformanceFactorDTO>(),
                CriterionLibrary = criterionLibrary,
                SupersedeRules = new List<TreatmentSupersedeRuleDTO>()
            {
                    new TreatmentSupersedeRuleDTO
                    {
                        CriterionLibrary = new CriterionLibraryDTO
                        {
                           MergedCriteriaExpression = "[AGE] > 10",
                           IsSingleUse = true,
                           Id = Guid.NewGuid(),
                        },
                        Id = Guid.NewGuid(),
                        treatment = supersededTreatment
                    },
                }
            };
            return dto;

        }
    }
}

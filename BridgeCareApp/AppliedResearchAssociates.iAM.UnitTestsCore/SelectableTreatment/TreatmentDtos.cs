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

        public static TreatmentDTO DtoWithEmptyCostsAndConsequencesLists(Guid? id = null, string name = "Treatment name")
        {

            var resolveId = id ?? Guid.NewGuid();
            var dto = new TreatmentDTO
            {
                Id = resolveId,
                Name = name,
                Description = "Treatment description",
                Costs = new List<TreatmentCostDTO>(),
                Consequences = new List<TreatmentConsequenceDTO>(),
                PerformanceFactors = new List<TreatmentPerformanceFactorDTO>(),
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
    }
}

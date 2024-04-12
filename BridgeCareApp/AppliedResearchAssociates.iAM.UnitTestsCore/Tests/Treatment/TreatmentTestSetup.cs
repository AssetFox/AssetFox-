using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.SelectableTreatment;
using AppliedResearchAssociates.iAM.UnitTestsCore.TestUtils;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class TreatmentTestSetup
    {
        public static TreatmentDTO ModelForSingleTreatmentOfLibraryInDb(IUnitOfWork unitOfWork, Guid treatmentLibraryId, Guid? id = null, string name = "Treatment name")
        {
            var dto = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists(id, name);
            var dtos = new List<TreatmentDTO> { dto };
            unitOfWork.SelectableTreatmentRepo.UpsertOrDeleteTreatments(dtos, treatmentLibraryId);
            return dto;
        }

        public static TreatmentDTO ModelForSingleTreatmentOfLibrary(Guid? id = null, string name = "Treatment name", string criterionExpression = null)
        {
            var dto = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists(id, name, criterionExpression);

            return dto;
        }

        public static TreatmentDTO ModelForSingleTreatmentOfSimulationInDb(IUnitOfWork unitOfWork, Guid simulationId, Guid? id = null, string name = "Treatment name", string criterionExpression = null, List<TreatmentBudgetDTO> budgets = null, List<Guid> budgetIds = null)
        {
            
            budgets ??= new List<TreatmentBudgetDTO>();
            var dto = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists(id, name, criterionExpression);
            dto.Budgets = budgets;
            budgetIds ??= new List<Guid>();
            dto.BudgetIds = budgetIds;
            dto.AssetType = "";
            var treatmentBudget = BudgetDtos.New();
            var treatmentBudgets = new List<BudgetDTO> { treatmentBudget };
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, treatmentBudgets, simulationId);

            var dtos = new List<TreatmentDTO> { dto };
            unitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(dtos, simulationId);
            return dto;
        }

        public static TreatmentDTO ModelForSingleTreatmentOfSimulation(Guid? id = null, string name = "Treatment name", string criterionExpression = null, List<TreatmentBudgetDTO> budgets = null, List<Guid> budgetIds = null)
        {

            budgets ??= new List<TreatmentBudgetDTO>();
            var dto = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists(id, name, criterionExpression);
            dto.Budgets = budgets;
            budgetIds ??= new List<Guid>();
            dto.BudgetIds = budgetIds;           
            
            return dto;
        }

        public static Dictionary<Guid, List<TreatmentSupersedeRuleDTO>> ModelTreatmentSupersedeRules(Guid treatmentId, List<TreatmentSupersedeRuleDTO> supersedeRules)
        {
            var scenarioTreatmentSupersedeRulesPerTreatmentId = new Dictionary<Guid, List<TreatmentSupersedeRuleDTO>>
            {
                { treatmentId, supersedeRules }
            };

            return scenarioTreatmentSupersedeRulesPerTreatmentId;
        }
    }   
}

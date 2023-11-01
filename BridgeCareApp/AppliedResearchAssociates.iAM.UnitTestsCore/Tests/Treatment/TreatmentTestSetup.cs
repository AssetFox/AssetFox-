using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
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

        public static TreatmentDTO ModelForSingleTreatmentOfSimulationInDb(IUnitOfWork unitOfWork, Guid simulationId, Guid? id = null, string name = "Treatment name", string criterionExpression = null, List<TreatmentBudgetDTO> budgets = null, List<Guid> budgetIds = null)
        {
            budgets ??= new List<TreatmentBudgetDTO>();
            var dto = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists(id, name, criterionExpression);
            dto.Budgets = budgets;
            budgetIds ??= new List<Guid>();
            dto.BudgetIds = budgetIds;
            var dtos = new List<TreatmentDTO> { dto };
            unitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(dtos, simulationId);
            return dto;
        }

        public static List<TreatmentDTO> ModelWithSupersededTreatmentOfSimulationInDb(IUnitOfWork unitOfWork, Guid simulationId, Guid? id = null, string name = "Treatment name", string criterionExpression = null, List<TreatmentBudgetDTO> budgets = null, List<Guid> budgetIds = null)
        {
            budgets ??= new List<TreatmentBudgetDTO>();
            var firstTreatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists(null, "Deck Replacement");
            firstTreatment.Budgets = budgets;
            budgetIds ??= new List<Guid>();
            firstTreatment.BudgetIds = budgetIds;
            firstTreatment.SupersedeRules = new List<TreatmentSupersedeRuleDTO>();

            var superBudget = BudgetDtos.New();
            var superBudgets = new List<BudgetDTO> { superBudget };
            ScenarioBudgetTestSetup.UpsertOrDeleteScenarioBudgets(TestHelper.UnitOfWork, superBudgets, simulationId);
            var treatmentId = Guid.NewGuid();
            var treatmentBudget = TreatmentBudgetDtos.Dto(superBudget.Name);
            var treatmentBudgets = new List<TreatmentBudgetDTO> { treatmentBudget };
            var superBudgetIds = new List<Guid> { superBudget.Id };

            var supersedeTreatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesListsWithSupersedeRule(firstTreatment, id, "Bridge Replacement", "Age > 20");
            var dtos = new List<TreatmentDTO> { firstTreatment, supersedeTreatment };
            unitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(dtos, simulationId);
            return dtos;
        }

        public static List<TreatmentDTO> ModelNoSupersededTreatmentOfSimulationInDb(IUnitOfWork unitOfWork, Guid simulationId, Guid? id = null, string name = "Treatment name", string criterionExpression = null, List<TreatmentBudgetDTO> budgets = null, List<Guid> budgetIds = null)
        {
            budgets ??= new List<TreatmentBudgetDTO>();
            var firstTreatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists(null, "Deck Replacement");
            firstTreatment.Budgets = budgets;
            budgetIds ??= new List<Guid>();
            firstTreatment.BudgetIds = budgetIds;
            var secondTreatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists(null, "Super Structure Painting");
            secondTreatment.Budgets = budgets;
            budgetIds ??= new List<Guid>();
            secondTreatment.BudgetIds = budgetIds;
            var dtos = new List<TreatmentDTO> { firstTreatment, secondTreatment };
            unitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(dtos, simulationId);
            return dtos;
        }


    }

   
}

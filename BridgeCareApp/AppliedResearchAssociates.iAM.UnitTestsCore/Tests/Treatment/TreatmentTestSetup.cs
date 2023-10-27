using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;

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
            var firstTreatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists(null, "Superseded treatment");
            firstTreatment.Budgets = budgets;
            budgetIds ??= new List<Guid>();
            firstTreatment.BudgetIds = budgetIds;
            var supersedeTreatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesListsWithSupersedeRule(firstTreatment, id, name);
            var dtos = new List<TreatmentDTO> { firstTreatment, supersedeTreatment };
            unitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(dtos, simulationId);
            return dtos;
        }

        public static List<TreatmentDTO> ModelNoSupersededTreatmentOfSimulationInDb(IUnitOfWork unitOfWork, Guid simulationId, Guid? id = null, string name = "Treatment name", string criterionExpression = null, List<TreatmentBudgetDTO> budgets = null, List<Guid> budgetIds = null)
        {
            budgets ??= new List<TreatmentBudgetDTO>();
            var firstTreatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists(null, "Superseded treatment");
            firstTreatment.Budgets = budgets;
            budgetIds ??= new List<Guid>();
            firstTreatment.BudgetIds = budgetIds;
            var supersedeTreatment = TreatmentDtos.DtoWithEmptyCostsAndConsequencesLists(null, "Superseded treatment");
            supersedeTreatment.Budgets = budgets;
            budgetIds ??= new List<Guid>();
            supersedeTreatment.BudgetIds = budgetIds;
            var dtos = new List<TreatmentDTO> { firstTreatment, supersedeTreatment };
            unitOfWork.SelectableTreatmentRepo.UpsertOrDeleteScenarioSelectableTreatment(dtos, simulationId);
            return dtos;
        }


    }

   
}

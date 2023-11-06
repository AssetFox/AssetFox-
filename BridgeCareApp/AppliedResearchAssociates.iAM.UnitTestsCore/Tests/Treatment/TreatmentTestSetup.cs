﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.UnitTestsCore.Tests.SelectableTreatment;

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
    }

   
}

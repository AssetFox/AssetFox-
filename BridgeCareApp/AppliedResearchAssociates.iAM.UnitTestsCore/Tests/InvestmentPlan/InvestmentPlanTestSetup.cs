using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class InvestmentPlanTestSetup
    {
        public static InvestmentPlanDTO ModelForEntityInDb(
            UnitOfDataPersistenceWork unitOfWork,
            Guid simulationId,
            Guid? id = null,
            int firstYearOfAnalysisPeriod = 2022)
        {
            var dto = InvestmentPlanDtos.Dto(id, firstYearOfAnalysisPeriod);
            unitOfWork.InvestmentPlanRepo.UpsertInvestmentPlan(dto, simulationId);
            return dto;
        }
    }
}

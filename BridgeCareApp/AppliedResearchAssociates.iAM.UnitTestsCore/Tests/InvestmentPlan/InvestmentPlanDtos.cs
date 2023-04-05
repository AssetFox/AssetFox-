using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class InvestmentPlanDtos
    {
        public static InvestmentPlanDTO Dto(Guid? id = null)
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new InvestmentPlanDTO
            {
                FirstYearOfAnalysisPeriod = 2022,
                Id = resolveId,
                InflationRatePercentage = 3,
                NumberOfYearsInAnalysisPeriod = 1,
                MinimumProjectCostLimit = 500000,
            };
            return dto;
        }

    }
}

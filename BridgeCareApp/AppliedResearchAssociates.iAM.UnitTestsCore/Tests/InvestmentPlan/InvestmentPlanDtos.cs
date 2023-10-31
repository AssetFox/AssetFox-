using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.UnitTestsCore.Tests
{
    public static class InvestmentPlanDtos
    {
        public static InvestmentPlanDTO Dto(Guid? id = null, int firstYearOfAnalysisPeriod = 2022)
        {
            var resolveId = id ?? Guid.NewGuid();
            var dto = new InvestmentPlanDTO
            {
                FirstYearOfAnalysisPeriod = firstYearOfAnalysisPeriod,
                Id = resolveId,
                InflationRatePercentage = 3,
                NumberOfYearsInAnalysisPeriod = 1,
                MinimumProjectCostLimit = 500000,
            };
            return dto;
        }

    }
}

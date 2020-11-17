using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class InvestmentPlanMapper
    {
        public static InvestmentPlanEntity ToEntity(this InvestmentPlan domain, Guid simulationId) =>
            new InvestmentPlanEntity
            {
                Id = Guid.NewGuid(),
                SimulationId = simulationId,
                FirstYearOfAnalysisPeriod = domain.FirstYearOfAnalysisPeriod,
                InflationRatePercentage = domain.InflationRatePercentage,
                MinimumProjectCostLimit = domain.MinimumProjectCostLimit,
                NumberOfYearsInAnalysisPeriod = domain.NumberOfYearsInAnalysisPeriod
            };

        public static InvestmentPlan ToDomain(this InvestmentPlanEntity entity) =>
            new InvestmentPlan(entity.Simulation.ToDomain())
            {
                FirstYearOfAnalysisPeriod = entity.FirstYearOfAnalysisPeriod,
                InflationRatePercentage = entity.InflationRatePercentage,
                MinimumProjectCostLimit = entity.MinimumProjectCostLimit,
                NumberOfYearsInAnalysisPeriod = entity.NumberOfYearsInAnalysisPeriod
            };
    }
}

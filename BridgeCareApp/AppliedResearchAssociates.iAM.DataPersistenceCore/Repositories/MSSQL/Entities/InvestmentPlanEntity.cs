using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.Abstract;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class InvestmentPlanEntity : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid SimulationId { get; set; }

        public int FirstYearOfAnalysisPeriod { get; set; }

        public double InflationRatePercentage { get; set; }

        public decimal MinimumProjectCostLimit { get; set; }

        public int NumberOfYearsInAnalysisPeriod { get; set; }

        public bool ShouldAccumulateUnusedBudgetAmounts { get; set; }

        public virtual SimulationEntity Simulation { get; set; }
    }
}

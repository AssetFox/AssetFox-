using System;
using System.Collections.Generic;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class InvestmentPlanEntity
    {
        public InvestmentPlanEntity()
        {
            Budgets = new HashSet<BudgetEntity>();
        }

        public Guid Id { get; set; }

        public Guid SimulationId { get; set; }

        public int FirstYearOfAnalysisPeriod { get; set; }

        public double InflationRatePercentage { get; set; }

        public decimal MinimumProjectCostLimit { get; set; }

        public int NumberOfYearsInAnalysisPeriod { get; set; }

        public virtual SimulationEntity Simulation { get; set; }

        public virtual ICollection<BudgetEntity> Budgets { get; set; }
    }
}

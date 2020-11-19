using System;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities
{
    public class InvestmentPlanSimulationEntity
    {
        public Guid InvestmentPlanId { get; set; }
        public Guid SimulationId { get; set; }

        public virtual InvestmentPlanEntity InvestmentPlan { get; set; }
        public virtual SimulationEntity Simulation { get; set; }
    }
}

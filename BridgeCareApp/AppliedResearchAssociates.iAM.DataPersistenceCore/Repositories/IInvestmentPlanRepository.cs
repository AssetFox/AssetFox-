using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IInvestmentPlanRepository
    {
        void CreateInvestmentPlan(InvestmentPlan investmentPlan, Guid simulationId);
        void GetSimulationInvestmentPlan(Simulation simulation);
    }
}

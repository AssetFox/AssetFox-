using System;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IInvestmentPlanRepository
    {
        void CreateInvestmentPlan(InvestmentPlan investmentPlan, Guid simulationId);
        //
        void GetSimulationInvestmentPlan(Simulation simulation);

        InvestmentPlanDTO GetInvestmentPlan(Guid simulationId);

        void UpsertInvestmentPlan(InvestmentPlanDTO dto, Guid simulationId);
    }
}

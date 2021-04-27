using System;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IInvestmentPlanRepository
    {
        void CreateInvestmentPlan(InvestmentPlan investmentPlan, Guid simulationId);

        void GetSimulationInvestmentPlan(Simulation simulation);

        InvestmentPlanDTO ScenarioInvestmentPlan(Guid simulationId);

        void UpsertPermitted(Guid simulationId, InvestmentPlanDTO dto);

        void UpsertInvestmentPlan(InvestmentPlanDTO dto, Guid simulationId);
    }
}

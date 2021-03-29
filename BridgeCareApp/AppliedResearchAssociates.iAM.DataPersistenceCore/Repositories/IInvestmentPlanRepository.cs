﻿using System;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.Domains;

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

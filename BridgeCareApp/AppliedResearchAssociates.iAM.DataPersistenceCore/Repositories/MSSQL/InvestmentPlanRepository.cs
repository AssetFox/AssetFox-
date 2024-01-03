using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class InvestmentPlanRepository : IInvestmentPlanRepository
    {
        private readonly UnitOfDataPersistenceWork _unitOfWork;

        public InvestmentPlanRepository(UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ??
                                         throw new ArgumentNullException(nameof(unitOfWork));

        private void CreateInvestmentPlanCashFlowRules(SimulationEntity simulationEntity, List<CashFlowRule> cashFlowRules) =>
            _unitOfWork.CashFlowRuleRepo.CreateCashFlowRules(cashFlowRules, simulationEntity.Id);

        public void GetSimulationInvestmentPlan(Simulation simulation)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulation.Id))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            if(!_unitOfWork.Context.InvestmentPlan.Any(_ => _.Simulation.Id == simulation.Id))
            {
                throw new RowNotInTableException("No budget(s) was found for the given scenario.");
            }

            _unitOfWork.Context.InvestmentPlan.AsNoTracking()
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.Budgets)
                .ThenInclude(_ => _.ScenarioBudgetAmounts)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.Budgets)
                .ThenInclude(_ => _.CriterionLibraryScenarioBudgetJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.CashFlowRules)
                .ThenInclude(_ => _.CriterionLibraryScenarioCashFlowRuleJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.CashFlowRules)
                .ThenInclude(_ => _.ScenarioCashFlowDistributionRules)
                .Single(_ => _.Simulation.Id == simulation.Id)
                .FillSimulationInvestmentPlan(simulation);
        }

        public InvestmentPlanDTO GetInvestmentPlan(Guid simulationId)
        {
            if (simulationId == Guid.Empty)
            {
                return new InvestmentPlanDTO();
            }

            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            var investmentPlan = _unitOfWork.Context.InvestmentPlan.AsNoTracking()
                .SingleOrDefault(_ => _.SimulationId == simulationId);
            return investmentPlan != null ? investmentPlan.ToDto() : new InvestmentPlanDTO();
        }

        public void UpsertInvestmentPlan(InvestmentPlanDTO dto, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation was found for the given scenario.");
            }

            var investmentPlanEntity = dto.ToEntity(simulationId);

            _unitOfWork.Context.Upsert(investmentPlanEntity, dto.Id, _unitOfWork.UserEntity?.Id);

            // Update last modified date
            _unitOfWork.SimulationRepo
                .UpdateLastModifiedDate(_unitOfWork.Context.Simulation.FirstOrDefault(_ => _.Id == simulationId));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class InvestmentPlanRepository : IInvestmentPlanRepository
    {
        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public InvestmentPlanRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork) =>
            _unitOfWork = unitOfWork ??
                                         throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateInvestmentPlan(InvestmentPlan investmentPlan, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var simulationEntity = _unitOfWork.Context.Simulation.Single(_ => _.Id == simulationId);

            var investmentPlanEntity = investmentPlan.ToEntity(simulationEntity.Id);

            _unitOfWork.Context.AddEntity(investmentPlanEntity, _unitOfWork.UserEntity?.Id);

            if (investmentPlan.Budgets.Any())
            {
                CreateInvestmentPlanBudgets(simulationEntity, investmentPlan.Budgets.ToList());
            }

            if (investmentPlan.BudgetConditions.Any())
            {
                var budgetEntityIdsPerExpression = investmentPlan.BudgetConditions
                    .Where(_ => !_.Criterion.ExpressionIsBlank)
                    .GroupBy(_ => _.Criterion.Expression, _ => _.Budget.Id)
                    .ToDictionary(_ => _.Key, _ => _.ToList());

                _unitOfWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(budgetEntityIdsPerExpression,
                    DataPersistenceConstants.CriterionLibraryJoinEntities.Budget, simulationEntity.Name);
            }

            if (investmentPlan.CashFlowRules.Any())
            {
                CreateInvestmentPlanCashFlowRules(simulationEntity, investmentPlan.CashFlowRules.ToList());
            }
        }

        private void CreateInvestmentPlanBudgets(SimulationEntity simulationEntity, List<Budget> budgets)
        {
            _unitOfWork.BudgetRepo.CreateBudgetLibrary(
                $"{simulationEntity.Name} Budget Library", simulationEntity.Id);

            _unitOfWork.BudgetRepo.CreateBudgets(budgets, simulationEntity.Id);
        }

        private void CreateInvestmentPlanCashFlowRules(SimulationEntity simulationEntity, List<CashFlowRule> cashFlowRules)
        {
            _unitOfWork.CashFlowRuleRepo.CreateCashFlowRuleLibrary($"{simulationEntity.Name} Simulation Cash Flow Rule Library", simulationEntity.Id);

            _unitOfWork.CashFlowRuleRepo.CreateCashFlowRules(cashFlowRules, simulationEntity.Id);
        }

        public void GetSimulationInvestmentPlan(Simulation simulation)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulation.Id))
            {
                throw new RowNotInTableException($"No simulation found having id {simulation.Id}");
            }

            _unitOfWork.Context.InvestmentPlan
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.BudgetLibrarySimulationJoin)
                .ThenInclude(_ => _.BudgetLibrary)
                .ThenInclude(_ => _.Budgets)
                .ThenInclude(_ => _.BudgetAmounts)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.BudgetLibrarySimulationJoin)
                .ThenInclude(_ => _.BudgetLibrary)
                .ThenInclude(_ => _.Budgets)
                .ThenInclude(_ => _.CriterionLibraryBudgetJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.CashFlowRuleLibrarySimulationJoin)
                .ThenInclude(_ => _.CashFlowRuleLibrary)
                .ThenInclude(_ => _.CashFlowRules)
                .ThenInclude(_ => _.CriterionLibraryCashFlowRuleJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.Simulation)
                .ThenInclude(_ => _.CashFlowRuleLibrarySimulationJoin)
                .ThenInclude(_ => _.CashFlowRuleLibrary)
                .ThenInclude(_ => _.CashFlowRules)
                .ThenInclude(_ => _.CashFlowDistributionRules)
                .Single(_ => _.Simulation.Id == simulation.Id)
                .FillSimulationInvestmentPlan(simulation);
        }

        public InvestmentPlanDTO ScenarioInvestmentPlan(Guid simulationId)
        {
            if (simulationId == Guid.Empty)
            {
                return new InvestmentPlanDTO();
            }

            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var investmentPlan =
                _unitOfWork.Context.InvestmentPlan.SingleOrDefault(_ =>
                    _.SimulationId == simulationId);
            return investmentPlan != null ? investmentPlan.ToDto() : new InvestmentPlanDTO();
        }

        public void UpsertPermitted(Guid simulationId, InvestmentPlanDTO dto)
        {
            if (simulationId != Guid.Empty)
            {
                if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {dto.Id}");
                }

                if (!_unitOfWork.Context.Simulation.Any(_ =>
                    _.Id == dto.Id && _.SimulationUserJoins.Any(__ => __.UserId == _unitOfWork.UserEntity.Id && __.CanModify)))
                {
                    throw new UnauthorizedAccessException("You are not authorized to modify this simulation.");
                }

                UpsertInvestmentPlan(dto, simulationId);
            }
        }

        public void UpsertInvestmentPlan(InvestmentPlanDTO dto, Guid simulationId)
        {
            if (simulationId != Guid.Empty)
            {
                if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
                {
                    throw new RowNotInTableException($"No simulation found having id {simulationId}.");
                }

                var investmentPlanEntity = dto.ToEntity(simulationId);

                _unitOfWork.Context.Upsert(investmentPlanEntity, dto.Id, _unitOfWork.UserEntity?.Id);
            }
        }
    }
}

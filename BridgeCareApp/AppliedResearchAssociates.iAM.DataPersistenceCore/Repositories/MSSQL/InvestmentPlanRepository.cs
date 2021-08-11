using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers;
using AppliedResearchAssociates.iAM.DataPersistenceCore.UnitOfWork;
using AppliedResearchAssociates.iAM.Domains;
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

        public void CreateInvestmentPlan(InvestmentPlan investmentPlan, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException("No simulation found for given scenario.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation.Single(_ => _.Id == simulationId);

            var investmentPlanEntity = investmentPlan.ToEntity(simulationEntity.Id);

            _unitOfWork.Context.AddEntity(investmentPlanEntity, _unitOfWork.UserEntity?.Id);

            if (investmentPlan.Budgets.Any())
            {
                _unitOfWork.BudgetRepo.CreateScenarioBudgets(investmentPlan.Budgets.ToList(), simulationEntity.Id);
            }

            if (investmentPlan.BudgetConditions.Any())
            {
                var criterionJoins = new List<CriterionLibraryScenarioBudgetEntity>();
                var criteria = investmentPlan.BudgetConditions
                    .Where(_ => !_.Criterion.ExpressionIsBlank)
                    .Select(condition =>
                    {
                        var budget = investmentPlan.Budgets.First(_ => _.Id == condition.Budget.Id);
                        var criterion = condition.Criterion.ToEntity($"{budget.Name} Criterion");
                        criterion.IsSingleUse = true;
                        criterionJoins.Add(new CriterionLibraryScenarioBudgetEntity
                        {
                            CriterionLibraryId = criterion.Id, ScenarioBudgetId = budget.Id
                        });
                        return criterion;
                    }).ToList();
                _unitOfWork.Context.AddAll(criteria);
                _unitOfWork.Context.AddAll(criterionJoins);
            }

            if (investmentPlan.CashFlowRules.Any())
            {
                CreateInvestmentPlanCashFlowRules(simulationEntity, investmentPlan.CashFlowRules.ToList());
            }

            // Update last modified date
            _unitOfWork.SimulationRepo.UpdateLastModifiedDate(simulationEntity);
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
                throw new RowNotInTableException("No simulation was found for the given scenario.");
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
                .AsNoTracking()
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

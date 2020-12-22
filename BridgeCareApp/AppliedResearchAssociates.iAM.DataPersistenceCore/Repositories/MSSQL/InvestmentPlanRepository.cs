using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class InvestmentPlanRepository : IInvestmentPlanRepository
    {
        public static readonly bool IsRunningFromNUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("nunit.framework"));

        private readonly UnitOfWork.UnitOfWork _unitOfWork;

        public InvestmentPlanRepository(UnitOfWork.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public void CreateInvestmentPlan(InvestmentPlan investmentPlan, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var simulationEntity = _unitOfWork.Context.Simulation.Single(_ => _.Id == simulationId);

            var investmentPlanEntity = investmentPlan.ToEntity(simulationEntity.Id);

            _unitOfWork.Context.InvestmentPlan.Add(investmentPlanEntity);

            //_unitOfWork.Context.SaveChanges();

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

                _unitOfWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(budgetEntityIdsPerExpression, "BudgetEntity", simulationEntity.Name);
            }

            if (investmentPlan.CashFlowRules.Any())
            {
                CreateInvestmentPlanCashFlowRules(simulationEntity, investmentPlan.CashFlowRules.ToList());
            }
        }

        private void CreateInvestmentPlanBudgets(SimulationEntity simulationEntity, List<Budget> budgets)
        {
            _unitOfWork.BudgetRepo.CreateBudgetLibrary($"{simulationEntity.Name} Simulation Investment Plan Budget Library", simulationEntity.Id);

            _unitOfWork.BudgetRepo.CreateBudgets(budgets, simulationEntity.Id);
        }

        private void CreateInvestmentPlanCashFlowRules(SimulationEntity simulationEntity, List<CashFlowRule> cashFlowRules)
        {
            _unitOfWork.CashFlowRuleRepo.CreateCashFlowRuleLibrary($"{simulationEntity.Name} Simulation Cash Flow Rule Library", simulationEntity.Id);

            _unitOfWork.CashFlowRuleRepo.CreateCashFlowRules(cashFlowRules, simulationEntity.Id);
        }

        public void GetSimulationInvestmentPlan(Simulation simulation)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Name == simulation.Name))
            {
                throw new RowNotInTableException($"No simulation found having name {simulation.Name}");
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
                .Single(_ => _.Simulation.Name == simulation.Name)
                .FillSimulationInvestmentPlan(simulation);
        }
    }
}

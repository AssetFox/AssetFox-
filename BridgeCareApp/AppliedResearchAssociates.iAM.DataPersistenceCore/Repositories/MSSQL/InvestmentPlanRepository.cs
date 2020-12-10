﻿using System;
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
    public class InvestmentPlanRepository : MSSQLRepository, IInvestmentPlanRepository
    {
        public static readonly bool IsRunningFromNUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("nunit.framework"));

        private readonly IBudgetRepository _budgetRepo;
        private readonly ICashFlowRuleRepository _cashFlowRuleRepo;
        private readonly ICriterionLibraryRepository _criterionLibraryRepo;
        

        public InvestmentPlanRepository(IBudgetRepository budgetRepo,
            ICriterionLibraryRepository criterionLibraryRepo,
            ICashFlowRuleRepository cashFlowRuleRepo,
            IAMContext context) : base(context)
        {
            _budgetRepo = budgetRepo ?? throw new ArgumentNullException(nameof(budgetRepo));
            _criterionLibraryRepo = criterionLibraryRepo ?? throw new ArgumentNullException(nameof(criterionLibraryRepo));
            _cashFlowRuleRepo = cashFlowRuleRepo ?? throw new ArgumentNullException(nameof(cashFlowRuleRepo));
        }

        public void CreateInvestmentPlan(InvestmentPlan investmentPlan, Guid simulationId)
        {
            using (var contextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (!Context.Simulation.Any(_ => _.Id == simulationId))
                    {
                        throw new RowNotInTableException($"No simulation found having id {simulationId}");
                    }

                    var simulationEntity = Context.Simulation.Single(_ => _.Id == simulationId);

                    var investmentPlanEntity = investmentPlan.ToEntity(simulationEntity.Id);

                    Context.InvestmentPlan.Add(investmentPlanEntity);

                    Context.SaveChanges();

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

                        _criterionLibraryRepo.JoinEntitiesWithCriteria(budgetEntityIdsPerExpression, "BudgetEntity", simulationEntity.Name);
                    }

                    if (investmentPlan.CashFlowRules.Any())
                    {
                        CreateInvestmentPlanCashFlowRules(simulationEntity, investmentPlan.CashFlowRules.ToList());
                    }

                    contextTransaction.Commit();
                }
                catch (Exception e)
                {
                    contextTransaction.Rollback();
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private void CreateInvestmentPlanBudgets(SimulationEntity simulationEntity, List<Budget> budgets)
        {
            _budgetRepo.CreateBudgetLibrary($"{simulationEntity.Name} Simulation Investment Plan Budget Library", simulationEntity.Id);

            _budgetRepo.CreateBudgets(budgets, simulationEntity.Id);
        }

        private void CreateInvestmentPlanCashFlowRules(SimulationEntity simulationEntity, List<CashFlowRule> cashFlowRules)
        {
            _cashFlowRuleRepo.CreateCashFlowRuleLibrary($"{simulationEntity.Name} Simulation Cash Flow Rule Library", simulationEntity.Id);

            _cashFlowRuleRepo.CreateCashFlowRules(cashFlowRules, simulationEntity.Id);
        }

        public void GetSimulationInvestmentPlan(Simulation simulation)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulation.Name))
            {
                throw new RowNotInTableException($"No simulation found having name {simulation.Name}");
            }

            Context.InvestmentPlan
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

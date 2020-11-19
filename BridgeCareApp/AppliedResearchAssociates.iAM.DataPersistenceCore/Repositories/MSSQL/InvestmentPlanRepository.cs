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

        public void CreateInvestmentPlan(InvestmentPlan investmentPlan, string simulationName)
        {
            using (var contextTransaction = Context.Database.BeginTransaction())
            {
                try
                {
                    if (!Context.Simulation.Any(_ => _.Name == simulationName))
                    {
                        throw new RowNotInTableException($"No simulation found having name {simulationName}");
                    }

                    var simulation = Context.Simulation.Single(_ => _.Name == simulationName);

                    var investmentPlanEntity = investmentPlan.ToEntity();

                    Context.InvestmentPlan.Add(investmentPlanEntity);

                    Context.InvestmentPlanSimulation.Add(new InvestmentPlanSimulationEntity
                    {
                        InvestmentPlanId = investmentPlanEntity.Id, SimulationId = simulation.Id
                    });

                    Context.SaveChanges();

                    if (investmentPlan.Budgets.Any())
                    {
                        _budgetRepo.CreateBudgets(investmentPlan.Budgets.ToList(), investmentPlanEntity.Id);
                    }

                    if (investmentPlan.BudgetConditions.Any())
                    {
                        var budgetEntities = Context.Budget
                            .Where(_ => _.InvestmentPlanId == investmentPlanEntity.Id)
                            .ToList();

                        var budgetEntityIdsPerExpression = investmentPlan.BudgetConditions
                            .Where(_ => !_.Criterion.ExpressionIsBlank)
                            .GroupBy(_ => _.Criterion.Expression, _ => _)
                            .ToDictionary(_ => _.Key, _ =>
                            {
                                var budgetNames = _.Select(__ => __.Budget.Name).Distinct().ToList();
                                return budgetEntities.Where(__ => budgetNames.Contains(__.Name))
                                    .Select(__ => __.Id).ToList();
                            });

                        _criterionLibraryRepo.JoinEntitiesWithCriteria(budgetEntityIdsPerExpression, "BudgetEntity", simulationName);
                    }

                    if (investmentPlan.CashFlowRules.Any())
                    {
                        CreateInvestmentPlanCashFlowRules(simulationName, investmentPlan.CashFlowRules.ToList());
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

        private void CreateInvestmentPlanCashFlowRules(string simulationName, List<CashFlowRule> cashFlowRules)
        {
            _cashFlowRuleRepo.CreateCashFlowRuleLibrary($"{simulationName} Simulation Cash Flow Rule Library", simulationName);

            _cashFlowRuleRepo.CreateCashFlowRules(cashFlowRules, simulationName);
        }

        public InvestmentPlan GetSimulationInvestmentPlan(string simulationName)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulationName))
            {
                throw new RowNotInTableException($"No simulation found having name {simulationName}");
            }

            return Context.InvestmentPlan
                .Include(_ => _.Budgets)
                .ThenInclude(_ => _.BudgetAmounts)
                .Include(_ => _.Budgets)
                .ThenInclude(_ => _.CriterionLibraryBudgetJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.InvestmentPlanSimulationJoins)
                .ThenInclude(_ => _.Simulation)
                .ThenInclude(_ => _.Network)
                .Include(_ => _.InvestmentPlanSimulationJoins)
                .ThenInclude(_ => _.Simulation)
                .ThenInclude(_ => _.CashFlowRuleLibrarySimulationJoin)
                .ThenInclude(_ => _.CashFlowRuleLibrary)
                .ThenInclude(_ => _.CashFlowRules)
                .ThenInclude(_ => _.CriterionLibraryCashFlowRuleJoin)
                .ThenInclude(_ => _.CriterionLibrary)
                .Include(_ => _.InvestmentPlanSimulationJoins)
                .ThenInclude(_ => _.Simulation)
                .ThenInclude(_ => _.CashFlowRuleLibrarySimulationJoin)
                .ThenInclude(_ => _.CashFlowRuleLibrary)
                .ThenInclude(_ => _.CashFlowRules)
                .ThenInclude(_ => _.CashFlowDistributionRules)
                .Single(_ =>
                    _.InvestmentPlanSimulationJoins.SingleOrDefault(__ => __.Simulation.Name == simulationName) != null)
                .ToDomain();
        }
    }
}

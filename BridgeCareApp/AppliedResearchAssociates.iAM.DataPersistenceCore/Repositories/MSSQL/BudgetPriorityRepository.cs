using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class BudgetPriorityRepository : MSSQLRepository, IBudgetPriorityRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly ICriterionLibraryRepository _criterionLibraryRepo;
        private readonly IBudgetPercentagePairRepository _budgetPercentagePairRepo;

        public BudgetPriorityRepository(ICriterionLibraryRepository criterionLibraryRepo,
            IBudgetPercentagePairRepository budgetPercentagePairRepo,
            IAMContext context) : base(context)
        {
            _criterionLibraryRepo =
                criterionLibraryRepo ?? throw new ArgumentNullException(nameof(criterionLibraryRepo));
            _budgetPercentagePairRepo = budgetPercentagePairRepo ??
                                        throw new ArgumentNullException(nameof(budgetPercentagePairRepo));
        }

        public void CreateBudgetPriorityLibrary(string name, string simulationName)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulationName))
            {
                throw new RowNotInTableException($"No simulation found having name {simulationName}");
            }

            var simulationEntity = Context.Simulation.Single(_ => _.Name == simulationName);

            var budgetPriorityLibraryEntity = new BudgetPriorityLibraryEntity { Id = Guid.NewGuid(), Name = name };

            Context.BudgetPriorityLibrary.Add(budgetPriorityLibraryEntity);

            Context.BudgetPriorityLibrarySimulation.Add(new BudgetPriorityLibrarySimulationEntity
            {
                BudgetPriorityLibraryId = budgetPriorityLibraryEntity.Id,
                SimulationId = simulationEntity.Id
            });

            Context.SaveChanges();
        }

        public void CreateBudgetPriorities(List<BudgetPriority> budgetPriorities, string simulationName)
        {
            if (!Context.Simulation.Any(_ => _.Name == simulationName))
            {
                throw new RowNotInTableException($"No simulation found having name {simulationName}");
            }

            var simulationEntity = Context.Simulation
                .Include(_ => _.BudgetPriorityLibrarySimulationJoin)
                .Include(_ => _.InvestmentPlanSimulationJoin)
                .ThenInclude(_ => _.InvestmentPlan)
                .ThenInclude(_ => _.Budgets)
                .Single(_ => _.Name == simulationName);

            var budgetPriorityEntityIdsPerExpression = new Dictionary<string, List<Guid>>();
            var budgetPercentagePairPriorityIdBudgetIdTupleTuple =
                new List<((Guid priorityId, Guid budgetId) priorityIdBudgetIdTuple, BudgetPercentagePair
                    budgetPercentagePair)>();

            var budgetPriorityEntities = budgetPriorities
                .Select(_ =>
                {
                    var budgetPriorityEntity = _.ToEntity(simulationEntity.BudgetPriorityLibrarySimulationJoin.BudgetPriorityLibraryId);

                    if (!_.Criterion.ExpressionIsBlank)
                    {
                        if (budgetPriorityEntityIdsPerExpression.ContainsKey(_.Criterion.Expression))
                        {
                            budgetPriorityEntityIdsPerExpression[_.Criterion.Expression].Add(budgetPriorityEntity.Id);
                        }
                        else
                        {
                            budgetPriorityEntityIdsPerExpression.Add(_.Criterion.Expression, new List<Guid>{ budgetPriorityEntity.Id });
                        }
                    }

                    if (_.BudgetPercentagePairs.Any())
                    {
                        var budgetEntities = simulationEntity.InvestmentPlanSimulationJoin
                            .InvestmentPlan.Budgets.ToList();

                        _.BudgetPercentagePairs.ToList().ForEach(_ =>
                        {
                            var budgetEntity = budgetEntities.Single(__ => __.Name == _.Budget.Name);
                            budgetPercentagePairPriorityIdBudgetIdTupleTuple.Add(((budgetPriorityEntity.Id, budgetEntity.Id), _));
                        });
                    }

                    return budgetPriorityEntity;
                })
                .ToList();

            if (IsRunningFromXUnit)
            {
                Context.BudgetPriority.AddRange(budgetPriorityEntities);
            }
            else
            {
                Context.BulkInsert(budgetPriorityEntities);
            }

            Context.SaveChanges();

            if (budgetPriorityEntityIdsPerExpression.Values.Any())
            {
                _criterionLibraryRepo.JoinEntitiesWithCriteria(budgetPriorityEntityIdsPerExpression, "BudgetPriorityEntity", simulationName);
            }

            if (budgetPercentagePairPriorityIdBudgetIdTupleTuple.Any())
            {
                _budgetPercentagePairRepo.CreateBudgetPercentagePairs(budgetPercentagePairPriorityIdBudgetIdTupleTuple);
            }
        }
    }
}

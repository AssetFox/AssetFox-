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
    public class BudgetPriorityRepository : IBudgetPriorityRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly ICriterionLibraryRepository _criterionLibraryRepo;
        private readonly IBudgetPercentagePairRepository _budgetPercentagePairRepo;
        private readonly IAMContext _context;

        public BudgetPriorityRepository(ICriterionLibraryRepository criterionLibraryRepo,
            IBudgetPercentagePairRepository budgetPercentagePairRepo,
            IAMContext context)
        {
            _criterionLibraryRepo =
                criterionLibraryRepo ?? throw new ArgumentNullException(nameof(criterionLibraryRepo));
            _budgetPercentagePairRepo = budgetPercentagePairRepo ??
                                        throw new ArgumentNullException(nameof(budgetPercentagePairRepo));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void CreateBudgetPriorityLibrary(string name, Guid simulationId)
        {
            if (!_context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var budgetPriorityLibraryEntity = new BudgetPriorityLibraryEntity { Id = Guid.NewGuid(), Name = name };

            _context.BudgetPriorityLibrary.Add(budgetPriorityLibraryEntity);

            _context.BudgetPriorityLibrarySimulation.Add(new BudgetPriorityLibrarySimulationEntity
            {
                BudgetPriorityLibraryId = budgetPriorityLibraryEntity.Id, SimulationId = simulationId
            });
        }

        public void CreateBudgetPriorities(List<BudgetPriority> budgetPriorities, Guid simulationId)
        {
            if (!_context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var simulationEntity = _context.Simulation
                .Include(_ => _.BudgetPriorityLibrarySimulationJoin)
                .Single(_ => _.Id == simulationId);

            if (simulationEntity.BudgetPriorityLibrarySimulationJoin == null)
            {
                throw new RowNotInTableException($"No budget priority library found for simulation having id {simulationId}");
            }

            var budgetPriorityEntities = budgetPriorities
                .Select(_ => _.ToEntity(simulationEntity.BudgetPriorityLibrarySimulationJoin.BudgetPriorityLibraryId))
                .ToList();

            if (IsRunningFromXUnit)
            {
                _context.BudgetPriority.AddRange(budgetPriorityEntities);
            }
            else
            {
                _context.BulkInsert(budgetPriorityEntities);
            }

            if (budgetPriorities.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var priorityIdsPerExpression = budgetPriorities
                    .Where(_ => !_.Criterion.ExpressionIsBlank)
                    .GroupBy(_ => _.Criterion.Expression, _ => _)
                    .ToDictionary(_ => _.Key, _ => _.Select(__ => __.Id).ToList());

                _criterionLibraryRepo.JoinEntitiesWithCriteria(priorityIdsPerExpression, "BudgetPriorityEntity", simulationEntity.Name);
            }

            if (budgetPriorities.Any(_ => _.BudgetPercentagePairs.Any()))
            {
                var percentagePairPerBudgetIdPerPriorityId = budgetPriorities
                    .Where(_ => _.BudgetPercentagePairs.Any())
                    .ToDictionary(_ => _.Id, _ => _.BudgetPercentagePairs.Select(__ => (__.Budget.Id, __)).ToList());

                _budgetPercentagePairRepo.CreateBudgetPercentagePairs(percentagePairPerBudgetIdPerPriorityId);
            }
        }
    }
}

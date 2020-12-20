using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly IBudgetAmountRepository _budgetAmountRepo;
        private readonly IAMContext _context;

        public BudgetRepository(IBudgetAmountRepository budgetAmountRepo, IAMContext context)
        {
            _budgetAmountRepo = budgetAmountRepo ?? throw new ArgumentNullException(nameof(budgetAmountRepo));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void CreateBudgetLibrary(string name, Guid simulationId)
        {
            if (!_context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var budgetLibraryEntity = new BudgetLibraryEntity {Id = Guid.NewGuid(), Name = name};

            _context.BudgetLibrary.Add(budgetLibraryEntity);

            _context.BudgetLibrarySimulation.Add(new BudgetLibrarySimulationEntity
            {
                BudgetLibraryId = budgetLibraryEntity.Id, SimulationId = simulationId
            });
        }

        public void CreateBudgets(List<Budget> budgets, Guid simulationId)
        {
            if (!_context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var simulationEntity = _context.Simulation
                .Include(_ => _.BudgetLibrarySimulationJoin)
                .Single(_ => _.Id == simulationId);

            if (simulationEntity.BudgetLibrarySimulationJoin == null)
            {
                throw new RowNotInTableException($"No budget library found for simulation having id {simulationId}");
            }

            var budgetEntities = budgets
                .Select(_ => _.ToEntity(simulationEntity.BudgetLibrarySimulationJoin.BudgetLibraryId))
                .ToList();

            _context.Budget.AddRange(budgetEntities);

            var budgetAmountsPerBudgetId = budgets
                .Where(_ => _.YearlyAmounts.Any())
                .ToDictionary(_ => _.Id, _ => _.YearlyAmounts.ToList());

            if (budgetAmountsPerBudgetId.Values.Any())
            {
                _budgetAmountRepo.CreateBudgetAmounts(budgetAmountsPerBudgetId, simulationId);
            }
        }
    }
}

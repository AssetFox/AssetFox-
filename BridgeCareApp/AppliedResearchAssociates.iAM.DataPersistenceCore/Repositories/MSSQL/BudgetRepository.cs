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
        private readonly UnitOfWork.UnitOfWork _unitOfWork;

        public BudgetRepository(UnitOfWork.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public void CreateBudgetLibrary(string name, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var budgetLibraryEntity = new BudgetLibraryEntity {Id = Guid.NewGuid(), Name = name};

            _unitOfWork.Context.BudgetLibrary.Add(budgetLibraryEntity);

            _unitOfWork.Context.BudgetLibrarySimulation.Add(new BudgetLibrarySimulationEntity
            {
                BudgetLibraryId = budgetLibraryEntity.Id, SimulationId = simulationId
            });

            _unitOfWork.Context.SaveChanges();
        }

        public void CreateBudgets(List<Budget> budgets, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}.");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
                .Include(_ => _.BudgetLibrarySimulationJoin)
                .Single(_ => _.Id == simulationId);

            if (simulationEntity.BudgetLibrarySimulationJoin == null)
            {
                throw new RowNotInTableException($"No budget library found for simulation having id {simulationId}");
            }

            var budgetEntities = budgets
                .Select(_ => _.ToEntity(simulationEntity.BudgetLibrarySimulationJoin.BudgetLibraryId))
                .ToList();

            _unitOfWork.Context.Budget.AddRange(budgetEntities);
            _unitOfWork.Context.SaveChanges();

            var budgetAmountsPerBudgetId = budgets
                .Where(_ => _.YearlyAmounts.Any())
                .ToDictionary(_ => _.Id, _ => _.YearlyAmounts.ToList());

            if (budgetAmountsPerBudgetId.Values.Any())
            {
                _unitOfWork.BudgetAmountRepo.CreateBudgetAmounts(budgetAmountsPerBudgetId, simulationId);
            }
        }
    }
}

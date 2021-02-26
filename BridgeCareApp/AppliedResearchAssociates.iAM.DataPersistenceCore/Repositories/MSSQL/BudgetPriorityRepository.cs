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

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfDataPersistenceWork;

        public BudgetPriorityRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfDataPersistenceWork)
        {
            _unitOfDataPersistenceWork = unitOfDataPersistenceWork ?? throw new ArgumentNullException(nameof(unitOfDataPersistenceWork));
        }

        public void CreateBudgetPriorityLibrary(string name, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var budgetPriorityLibraryEntity = new BudgetPriorityLibraryEntity { Id = Guid.NewGuid(), Name = name };

            _unitOfDataPersistenceWork.Context.BudgetPriorityLibrary.Add(budgetPriorityLibraryEntity);

            _unitOfDataPersistenceWork.Context.BudgetPriorityLibrarySimulation.Add(new BudgetPriorityLibrarySimulationEntity
            {
                BudgetPriorityLibraryId = budgetPriorityLibraryEntity.Id, SimulationId = simulationId
            });

            _unitOfDataPersistenceWork.Context.SaveChanges();
        }

        public void CreateBudgetPriorities(List<BudgetPriority> budgetPriorities, Guid simulationId)
        {
            if (!_unitOfDataPersistenceWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var simulationEntity = _unitOfDataPersistenceWork.Context.Simulation
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
                _unitOfDataPersistenceWork.Context.BudgetPriority.AddRange(budgetPriorityEntities);
            }
            else
            {
                _unitOfDataPersistenceWork.Context.BulkInsert(budgetPriorityEntities);
            }

            _unitOfDataPersistenceWork.Context.SaveChanges();

            if (budgetPriorities.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var priorityIdsPerExpression = budgetPriorities
                    .Where(_ => !_.Criterion.ExpressionIsBlank)
                    .GroupBy(_ => _.Criterion.Expression, _ => _)
                    .ToDictionary(_ => _.Key, _ => _.Select(__ => __.Id).ToList());

                _unitOfDataPersistenceWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(priorityIdsPerExpression,
                    DataPersistenceConstants.CriterionLibraryJoinEntities.BudgetPriority, simulationEntity.Name);
            }

            if (budgetPriorities.Any(_ => _.BudgetPercentagePairs.Any()))
            {
                var percentagePairPerBudgetIdPerPriorityId = budgetPriorities
                    .Where(_ => _.BudgetPercentagePairs.Any())
                    .ToDictionary(_ => _.Id, _ => _.BudgetPercentagePairs.Select(__ => (__.Budget.Id, __)).ToList());

                _unitOfDataPersistenceWork.BudgetPercentagePairRepo.CreateBudgetPercentagePairs(percentagePairPerBudgetIdPerPriorityId);
            }
        }
    }
}

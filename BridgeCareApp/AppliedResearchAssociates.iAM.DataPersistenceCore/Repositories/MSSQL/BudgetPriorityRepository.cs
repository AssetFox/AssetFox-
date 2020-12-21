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

        private readonly UnitOfWork.UnitOfWork _unitOfWork;

        public BudgetPriorityRepository(UnitOfWork.UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public void CreateBudgetPriorityLibrary(string name, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var budgetPriorityLibraryEntity = new BudgetPriorityLibraryEntity { Id = Guid.NewGuid(), Name = name };

            _unitOfWork.Context.BudgetPriorityLibrary.Add(budgetPriorityLibraryEntity);

            _unitOfWork.Context.BudgetPriorityLibrarySimulation.Add(new BudgetPriorityLibrarySimulationEntity
            {
                BudgetPriorityLibraryId = budgetPriorityLibraryEntity.Id, SimulationId = simulationId
            });

            _unitOfWork.Context.SaveChanges();
        }

        public void CreateBudgetPriorities(List<BudgetPriority> budgetPriorities, Guid simulationId)
        {
            if (!_unitOfWork.Context.Simulation.Any(_ => _.Id == simulationId))
            {
                throw new RowNotInTableException($"No simulation found having id {simulationId}");
            }

            var simulationEntity = _unitOfWork.Context.Simulation
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
                _unitOfWork.Context.BudgetPriority.AddRange(budgetPriorityEntities);
            }
            else
            {
                _unitOfWork.Context.BulkInsert(budgetPriorityEntities);
            }

            _unitOfWork.Context.SaveChanges();

            if (budgetPriorities.Any(_ => !_.Criterion.ExpressionIsBlank))
            {
                var priorityIdsPerExpression = budgetPriorities
                    .Where(_ => !_.Criterion.ExpressionIsBlank)
                    .GroupBy(_ => _.Criterion.Expression, _ => _)
                    .ToDictionary(_ => _.Key, _ => _.Select(__ => __.Id).ToList());

                _unitOfWork.CriterionLibraryRepo.JoinEntitiesWithCriteria(priorityIdsPerExpression, "BudgetPriorityEntity", simulationEntity.Name);
            }

            if (budgetPriorities.Any(_ => _.BudgetPercentagePairs.Any()))
            {
                var percentagePairPerBudgetIdPerPriorityId = budgetPriorities
                    .Where(_ => _.BudgetPercentagePairs.Any())
                    .ToDictionary(_ => _.Id, _ => _.BudgetPercentagePairs.Select(__ => (__.Budget.Id, __)).ToList());

                _unitOfWork.BudgetPercentagePairRepo.CreateBudgetPercentagePairs(percentagePairPerBudgetIdPerPriorityId);
            }
        }
    }
}

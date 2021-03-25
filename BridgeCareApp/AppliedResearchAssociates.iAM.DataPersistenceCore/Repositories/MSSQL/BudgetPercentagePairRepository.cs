using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Extensions;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class BudgetPercentagePairRepository : IBudgetPercentagePairRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfDataPersistenceWork _unitOfWork;

        public BudgetPercentagePairRepository(UnitOfWork.UnitOfDataPersistenceWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateBudgetPercentagePairs(Dictionary<Guid, List<(Guid budgetId, BudgetPercentagePair percentagePair)>> percentagePairPerBudgetIdPerPriorityId)
        {
            var budgetPercentagePairEntities = percentagePairPerBudgetIdPerPriorityId
                .SelectMany(_ => _.Value.Select(__ => __.percentagePair.ToEntity(_.Key, __.budgetId)))
                .ToList();

            _unitOfWork.Context.AddAll(budgetPercentagePairEntities);
        }

        public void UpsertOrDeleteBudgetPercentagePairs(
            Dictionary<Guid, List<BudgetPercentagePairDTO>> percentagePairsPerPriorityId, Guid libraryId)
        {
            var budgetPercentagePairEntities = percentagePairsPerPriorityId.SelectMany(_ => _.Value.Select(__ => __.ToEntity(_.Key))).ToList();

            var entityIds = budgetPercentagePairEntities.Select(_ => _.Id).ToList();

            var existingEntityIds = _unitOfWork.Context.BudgetPercentagePair
                .Where(_ => _.BudgetPriority.BudgetPriorityLibraryId == libraryId && entityIds.Contains(_.Id)).Select(_ => _.Id)
                .ToList();

            var predicatesPerCrudOperation = new Dictionary<string, Expression<Func<BudgetPercentagePairEntity, bool>>>
            {
                {"delete", _ => _.BudgetPriority.BudgetPriorityLibraryId == libraryId && !entityIds.Contains(_.Id)},
                {"update", _ => existingEntityIds.Contains(_.Id)},
                {"add", _ => !existingEntityIds.Contains(_.Id)}
            };

            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.UpsertOrDelete(budgetPercentagePairEntities, predicatesPerCrudOperation, _unitOfWork.UserEntity?.Id);
            }
            else
            {
                _unitOfWork.Context.BulkUpsertOrDelete(budgetPercentagePairEntities, predicatesPerCrudOperation, _unitOfWork.UserEntity?.Id);
            }
        }
    }
}

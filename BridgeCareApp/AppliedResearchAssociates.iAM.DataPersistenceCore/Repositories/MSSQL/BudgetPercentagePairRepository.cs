using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class BudgetPercentagePairRepository : IBudgetPercentagePairRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        private readonly UnitOfWork.UnitOfWork _unitOfWork;

        public BudgetPercentagePairRepository(UnitOfWork.UnitOfWork unitOfWork) => _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        public void CreateBudgetPercentagePairs(Dictionary<Guid, List<(Guid budgetId, BudgetPercentagePair percentagePair)>> percentagePairPerBudgetIdPerPriorityId)
        {
            var budgetPercentagePairEntities = percentagePairPerBudgetIdPerPriorityId
                .SelectMany(_ => _.Value.Select(__ => __.percentagePair.ToEntity(_.Key, __.budgetId)))
                .ToList();

            if (IsRunningFromXUnit)
            {
                _unitOfWork.Context.BudgetPercentagePair.AddRange(budgetPercentagePairEntities);
            }
            else
            {
                _unitOfWork.Context.BulkInsert(budgetPercentagePairEntities);
            }

            _unitOfWork.Context.SaveChanges();
        }
    }
}

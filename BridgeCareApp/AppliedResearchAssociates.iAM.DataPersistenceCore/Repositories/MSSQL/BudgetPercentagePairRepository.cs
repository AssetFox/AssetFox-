using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings;
using AppliedResearchAssociates.iAM.Domains;
using EFCore.BulkExtensions;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL
{
    public class BudgetPercentagePairRepository : MSSQLRepository, IBudgetPercentagePairRepository
    {
        public static readonly bool IsRunningFromXUnit = AppDomain.CurrentDomain.GetAssemblies()
            .Any(a => a.FullName.ToLowerInvariant().StartsWith("xunit"));

        public BudgetPercentagePairRepository(IAMContext context) : base(context) { }

        public void CreateBudgetPercentagePairs(
            List<((Guid priorityId, Guid budgetId) priorityIdBudgetIdTuple, BudgetPercentagePair budgetPercentagePair
                )> budgetPercentagePairPriorityIdBudgetIdTupleTuple)
        {
            var budgetPercentagePairEntities = budgetPercentagePairPriorityIdBudgetIdTupleTuple.Select(_ =>
                _.budgetPercentagePair.ToEntity(_.priorityIdBudgetIdTuple.priorityId,
                    _.priorityIdBudgetIdTuple.budgetId))
                .ToList();

            if (IsRunningFromXUnit)
            {
                Context.BudgetPercentagePair.AddRange(budgetPercentagePairEntities);
            }
            else
            {
                Context.BulkInsert(budgetPercentagePairEntities);
            }

            Context.SaveChanges();
        }
    }
}

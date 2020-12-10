using System;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class BudgetMapper
    {
        public static BudgetEntity ToEntity(this Budget domain, Guid budgetLibraryId) =>
            new BudgetEntity
            {
                Id = domain.Id,
                BudgetLibraryId = budgetLibraryId,
                Name = domain.Name
            };
    }
}

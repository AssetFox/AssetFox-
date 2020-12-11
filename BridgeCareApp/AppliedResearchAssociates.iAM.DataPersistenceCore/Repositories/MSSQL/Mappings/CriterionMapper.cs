using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class CriterionMapper
    {
        public static CriterionLibraryEntity ToEntity(this Criterion domain, string name) =>
            new CriterionLibraryEntity
            {
                Id = domain.Id,
                Name = name,
                MergedCriteriaExpression = domain.Expression
            };
    }
}

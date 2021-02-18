using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
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

        public static CriterionLibraryDTO ToDto(this CriterionLibraryEntity entity) =>
            new CriterionLibraryDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                MergedCriteriaExpression = entity.MergedCriteriaExpression
            };
    }
}

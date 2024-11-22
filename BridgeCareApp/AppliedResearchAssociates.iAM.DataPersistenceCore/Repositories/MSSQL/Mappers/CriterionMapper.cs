using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
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

        public static CriterionLibraryEntity ToEntity(this CriterionLibraryDTO dto, BaseEntityProperties baseEntityProperties=null)
        {
            var entity = new CriterionLibraryEntity
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                MergedCriteriaExpression = dto.MergedCriteriaExpression,
                IsSingleUse = dto.IsSingleUse
            };
            BaseEntityPropertySetter.SetBaseEntityProperties(entity, baseEntityProperties);
            return entity;
        }
        public static CriterionLibraryEntity ToSingleUseEntity(this CriterionLibraryDTO dto, BaseEntityProperties baseEntityProperties)
        {
            var entity = dto.ToEntity(baseEntityProperties);
            entity.IsSingleUse = true;
            return entity;
        }

        public static CriterionLibraryDTO ToDto(this CriterionLibraryEntity entity) =>
            new CriterionLibraryDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Owner = entity.CreatedBy,
                MergedCriteriaExpression = entity.MergedCriteriaExpression,               
                IsSingleUse = entity.IsSingleUse
            };
    }
}

using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class BenefitMapper
    {
        public static BenefitEntity ToEntity(this Benefit domain, Guid analysisMethodId, Guid attributeId) =>
            new BenefitEntity
            {
                Id = domain.Id,
                AnalysisMethodId = analysisMethodId,
                Limit = domain.Limit,
                AttributeId = attributeId
            };

        public static BenefitEntity ToEntity(this BenefitDTO dto, Guid analysisMethodId, Guid attributeId, BaseEntityProperties baseEntityProperties = null)
        {
            var entity = new BenefitEntity

            {
                Id = dto.Id,
                AnalysisMethodId = analysisMethodId,
                Limit = dto.Limit,
                AttributeId = attributeId
            };
            BaseEntityPropertySetter.SetBaseEntityProperties(entity, baseEntityProperties);
            return entity;
        }

        public static BenefitDTO ToDto(this BenefitEntity entity) =>
            new BenefitDTO { Id = entity.Id, Limit = entity.Limit, Attribute = entity.Attribute.Name };
    }
}

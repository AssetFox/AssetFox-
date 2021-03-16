using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class BenefitMapper
    {
        public static BenefitEntity ToEntity(this Benefit domain, Guid analysisMethodId, Guid? attributeId)
        {
            var benefitEntity = new BenefitEntity
            {
                Id = domain.Id,
                AnalysisMethodId = analysisMethodId,
                Limit = domain.Limit
            };

            if (attributeId != null)
            {
                benefitEntity.AttributeId = attributeId;
            }

            return benefitEntity;
        }

        public static BenefitEntity ToEntity(this BenefitDTO dto, Guid analysisMethodId, Guid attributeId) =>
            new BenefitEntity
            {
                Id = dto.Id, AnalysisMethodId = analysisMethodId, Limit = dto.Limit, AttributeId = attributeId
            };

        public static BenefitDTO ToDto(this BenefitEntity entity) =>
            new BenefitDTO {Id = entity.Id, Limit = entity.Limit, Attribute = entity.Attribute.Name};
    }
}

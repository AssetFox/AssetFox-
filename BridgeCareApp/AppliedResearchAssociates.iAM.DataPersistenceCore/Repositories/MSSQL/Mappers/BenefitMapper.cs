using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

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

        public static BenefitEntity ToEntity(this BenefitDTO dto, Guid analysisMethodId, Guid attributeId) =>
            new BenefitEntity
            {
                Id = dto.Id,
                AnalysisMethodId = analysisMethodId,
                Limit = dto.Limit,
                AttributeId = attributeId
            };

        public static BenefitDTO ToDto(this BenefitEntity entity) =>
            new BenefitDTO { Id = entity.Id, Limit = entity.Limit, Attribute = entity.Attribute.Name };
    }
}

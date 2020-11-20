using System;
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
                Id = Guid.NewGuid(),
                AnalysisMethodId = analysisMethodId,
                Limit = domain.Limit
            };

            if (attributeId != null)
            {
                benefitEntity.AttributeId = attributeId;
            }

            return benefitEntity;
        }

        public static Benefit ToDomain(this BenefitEntity entity)
        {
            var benefit = new Benefit
            {
                Limit = entity.Limit
            };

            if (entity.Attribute != null)
            {
                benefit.Attribute = (NumberAttribute)Convert
                    .ChangeType(entity.Attribute.ToDomain().ToSimulationAnalysisAttribute(), typeof(NumberAttribute));
            }

            return benefit;
        }
    }
}

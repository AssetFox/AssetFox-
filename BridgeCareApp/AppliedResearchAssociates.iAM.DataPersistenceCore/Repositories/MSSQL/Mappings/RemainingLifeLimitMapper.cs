using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class RemainingLifeLimitMapper
    {
        public static RemainingLifeLimitEntity ToEntity(this RemainingLifeLimit domain,
            Guid remainingLifeLimitLibraryId, Guid attributeId) =>
            new RemainingLifeLimitEntity
            {
                Id = Guid.NewGuid(),
                RemainingLifeLimitLibraryId = remainingLifeLimitLibraryId,
                AttributeId = attributeId,
                Value = domain.Value
            };

        public static void ToSimulationAnalysisDomain(this RemainingLifeLimitEntity entity,
            AnalysisMethod analysisMethod)
        {
            var limit = analysisMethod.AddRemainingLifeLimit();
            limit.Value = entity.Value;
            limit.Criterion.Expression =
                entity.CriterionLibraryRemainingLifeLimitJoin?.CriterionLibrary.MergedCriteriaExpression ??
                string.Empty;
            if (entity.Attribute != null)
            {
                limit.Attribute =
                    (NumberAttribute)Convert.ChangeType(entity.Attribute.ToDomain().ToSimulationAnalysisAttribute(),
                        typeof(NumberAttribute));
            }
        }
    }
}

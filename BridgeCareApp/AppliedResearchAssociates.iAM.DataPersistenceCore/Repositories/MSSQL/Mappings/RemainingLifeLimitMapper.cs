using System;
using System.Linq;
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
                Id = domain.Id,
                RemainingLifeLimitLibraryId = remainingLifeLimitLibraryId,
                AttributeId = attributeId,
                Value = domain.Value
            };

        public static void CreateRemainingLifeLimit(this RemainingLifeLimitEntity entity, Simulation simulation)
        {
            var limit = simulation.AnalysisMethod.AddRemainingLifeLimit();
            limit.Id = entity.Id;
            limit.Value = entity.Value;
            limit.Criterion.Expression =
                entity.CriterionLibraryRemainingLifeLimitJoin?.CriterionLibrary.MergedCriteriaExpression ??
                string.Empty;

            if (entity.Attribute != null)
            {
                limit.Attribute = simulation.Network.Explorer.NumberAttributes
                    .Single(_ => _.Name == entity.Attribute.Name);
            }
        }
    }
}

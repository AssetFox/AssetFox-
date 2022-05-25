using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class SectionMapper
    {
        public static void CreateMaintainableAsset(this MaintainableAssetEntity entity, Network network)
        {
            var asset = network.AddAsset();
            asset.Id = entity.Id;
            asset.AssetName = entity.AssetName;
            asset.SpatialWeighting.Expression = entity.SpatialWeighting;

            if (entity.AggregatedResults.Any(_ => _.Discriminator == DataPersistenceConstants.AggregatedResultNumericDiscriminator))
            {
                entity.AggregatedResults
                    .Where(_ => _.Discriminator == DataPersistenceConstants.AggregatedResultNumericDiscriminator)
                    .ToList()
                    .SetNumericAttributeValueHistories(asset);
            }

            if (entity.AggregatedResults.Any(_ => _.Discriminator == DataPersistenceConstants.AggregatedResultTextDiscriminator))
            {
                entity.AggregatedResults
                    .Where(_ => _.Discriminator == DataPersistenceConstants.AggregatedResultTextDiscriminator).ToList()
                    .SetTextAttributeValueHistories(asset);
            }
        }
    }
}

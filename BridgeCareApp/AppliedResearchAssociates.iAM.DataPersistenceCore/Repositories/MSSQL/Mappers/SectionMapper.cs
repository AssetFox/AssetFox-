using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public sealed class SectionMapper
    {
        public SectionMapper(Network network)
        {
            Network = network;
            HistoryMapper = new(network);
        }

        public void CreateMaintainableAsset(MaintainableAssetEntity entity)
        {
            var asset = Network.AddAsset();
            asset.Id = entity.Id;
            asset.AssetName = entity.AssetName;
            asset.SpatialWeighting.Expression = entity.SpatialWeighting;

            if (entity.AggregatedResults.Any(_ => _.Discriminator == DataPersistenceConstants.AggregatedResultNumericDiscriminator))
            {
                var numericResults = entity.AggregatedResults
                    .Where(_ => _.Discriminator == DataPersistenceConstants.AggregatedResultNumericDiscriminator)
                    .ToList();

                HistoryMapper.SetNumericAttributeValueHistories(numericResults, asset);
            }

            if (entity.AggregatedResults.Any(_ => _.Discriminator == DataPersistenceConstants.AggregatedResultTextDiscriminator))
            {
                var textResults = entity.AggregatedResults
                    .Where(_ => _.Discriminator == DataPersistenceConstants.AggregatedResultTextDiscriminator)
                    .ToList();

                HistoryMapper.SetTextAttributeValueHistories(textResults, asset);
            }
        }

        private readonly AttributeValueHistoryMapper HistoryMapper;
        private readonly Network Network;
    }
}

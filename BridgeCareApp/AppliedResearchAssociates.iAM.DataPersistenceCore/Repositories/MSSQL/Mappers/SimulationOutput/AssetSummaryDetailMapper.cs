using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class AssetSummaryDetailMapper
    {
        public static AssetSummaryDetailEntityFamily ToEntityLists(List<AssetSummaryDetail> domainList, Guid simulationOutputId, Dictionary<string, Guid> attributeIdLookup)
        {
            var family = new AssetSummaryDetailEntityFamily();
            foreach (var domain in domainList)
            {
                AddToFamily(family, domain, simulationOutputId, attributeIdLookup);
            }
            return family;
        }

        private static void AddToFamily(
            AssetSummaryDetailEntityFamily family,
            AssetSummaryDetail domain,
            Guid simulationOutputId,
            Dictionary<string, Guid> attributeIdLookup)
        {
            var id = Guid.NewGuid();
            var mapNumericValues = AssetSummaryDetailValueMapper.ToNumericEntityList(
                id,
                domain.ValuePerNumericAttribute,
                attributeIdLookup);
            family.AssetSummaryDetailValues.AddRange(mapNumericValues);
            var mapTextValues = AssetSummaryDetailValueMapper.ToTextEntityList(
                id,
                domain.ValuePerTextAttribute,
                attributeIdLookup);
            family.AssetSummaryDetailValues.AddRange(mapTextValues);
            var entity = new AssetSummaryDetailEntity
            {
                Id = id,
                SimulationOutputId = simulationOutputId,
                MaintainableAssetId = domain.AssetId,
            };
            family.AssetSummaryDetails.Add(entity);
        }

        public static AssetSummaryDetail ToDomain(AssetSummaryDetailEntity entity, Dictionary<Guid, string> attributeNameLookup)
        {
            var assetName = entity.MaintainableAsset.AssetName;
            var domain = new AssetSummaryDetail(assetName, entity.MaintainableAssetId);
            AssetSummaryDetailValueMapper.AddToDictionaries(
                entity.AssetSummaryDetailValuesIntId,
                domain.ValuePerNumericAttribute,
                domain.ValuePerTextAttribute,
                attributeNameLookup);
            return domain;
        }

        public static Dictionary<Guid, AssetSummaryDetail> ToDomainDictionaryNullSafe(ICollection<AssetSummaryDetailEntity> entityList, Dictionary<Guid, string> attributeNameLookup)
        {
            var domainDictionary = new Dictionary<Guid, AssetSummaryDetail>();
            if (entityList != null)
            {
                foreach (var entity in entityList)
                {
                    var domain = ToDomain(entity, attributeNameLookup);
                    domainDictionary[entity.Id] = domain;
                }
            }
            return domainDictionary;
        }
    }
}

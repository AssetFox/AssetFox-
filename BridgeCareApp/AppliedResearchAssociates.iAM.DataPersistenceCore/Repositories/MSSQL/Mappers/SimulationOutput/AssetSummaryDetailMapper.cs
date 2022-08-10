using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class AssetSummaryDetailMapper
    {
        public static AssetSummaryDetailEntity ToEntity(
            AssetSummaryDetail domain,
            Guid simulationOutputId,
            Dictionary<string, Guid> attributeIdLookup)
        {
            var id = Guid.NewGuid();
            var mapValues = AssetSummaryDetailValueMapper.ToNumericEntityList(
                domain.ValuePerNumericAttribute,
                attributeIdLookup);
            var mapTextValues = AssetSummaryDetailValueMapper.ToTextEntityList(
                domain.ValuePerTextAttribute,
                attributeIdLookup);
            mapValues.AddRange(mapTextValues);
            var entity = new AssetSummaryDetailEntity
            {
                Id = id,
                SimulationOutputId = simulationOutputId,
                AssetSummaryDetailValues = mapValues,
                MaintainableAssetId = domain.AssetId,
            };
            return entity;
        }

        public static AssetSummaryDetail ToDomain(AssetSummaryDetailEntity entity)
        {
            var assetName = entity.MaintainableAsset.AssetName;
            var domain = new AssetSummaryDetail(assetName, entity.MaintainableAssetId);
            AssetSummaryDetailValueMapper.AddToDictionaries(
                entity.AssetSummaryDetailValues,
                domain.ValuePerNumericAttribute,
                domain.ValuePerTextAttribute);
            return domain;
        }

        public static List<AssetSummaryDetail> ToDomainList(ICollection<AssetSummaryDetailEntity> entityList)
        {
            var domainList = new List<AssetSummaryDetail>();
            foreach (var entity in entityList)
            {
                var domain = ToDomain(entity);
                domainList.Add(domain);
            }
            return domainList;
        }
    }
}

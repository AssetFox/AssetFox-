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
                id,
                domain.ValuePerNumericAttribute,
                attributeIdLookup);
            var mapTextValues = AssetSummaryDetailValueMapper.ToTextEntityList(
                id,
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

        public static List<AssetSummaryDetailEntity> ToEntityList(List<AssetSummaryDetail> domainList, Guid simulationOutputId, Dictionary<string, Guid> attributeIdLookup)
        {
            var entityList = new List<AssetSummaryDetailEntity>();
            foreach (var domain in domainList)
            {
                var entity = ToEntity(domain, simulationOutputId, attributeIdLookup);
                entityList.Add(entity);
            }
            return entityList;
        }

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

        public static List<AssetSummaryDetail> ToDomainListNullSafe(ICollection<AssetSummaryDetailEntity> entityList)
        {
            var domainList = new List<AssetSummaryDetail>();
            if (entityList != null)
            {
                foreach (var entity in entityList)
                {
                    var domain = ToDomain(entity);
                    domainList.Add(domain);
                }
            }
            return domainList;
        }
    }
}

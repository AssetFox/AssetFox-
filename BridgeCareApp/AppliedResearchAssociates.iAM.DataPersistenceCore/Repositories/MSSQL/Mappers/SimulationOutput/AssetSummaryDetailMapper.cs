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
            };
            return entity;
        }
    }
}

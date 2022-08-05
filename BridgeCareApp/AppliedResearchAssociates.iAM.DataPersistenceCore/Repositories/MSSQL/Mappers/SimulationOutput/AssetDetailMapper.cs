using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class AssetDetailMapper
    {
        public static AssetDetailEntity ToEntity(
            AssetDetail domain,
            Guid simulationYearDetailId,
            Dictionary<string, Guid> attributeIdLookup)
        {
            var mapValues = AssetDetailValueMapper.ToNumericEntityList(Guid.Empty, domain.ValuePerNumericAttribute, attributeIdLookup);
            var entity = new AssetDetailEntity
            {
                SimulationYearDetailId = simulationYearDetailId,
                AppliedTreatment = domain.AppliedTreatment,
                TreatmentCause = (int)domain.TreatmentCause,
                TreatmentFundingIgnoresSpendingLimit = domain.TreatmentFundingIgnoresSpendingLimit,
                TreatmentStatus = (int)domain.TreatmentStatus,
                AssetName = domain.AssetName,
            };
            return entity;
        }

        public static List<AssetDetailEntity> ToEntityList(
            List<AssetDetail> domainList,
            Guid simulationYearDetailId,
            Dictionary<string, Guid> attributeIdLookup)
        {
            var entities = new List<AssetDetailEntity>();
            foreach (var domain in domainList)
            {
                var entity = ToEntity(domain, simulationYearDetailId, attributeIdLookup);
                entities.Add(entity);
            }
            return entities;
        }
    }
}

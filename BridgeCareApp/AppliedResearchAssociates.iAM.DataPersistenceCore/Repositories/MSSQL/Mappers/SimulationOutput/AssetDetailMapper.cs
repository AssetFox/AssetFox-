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
            var id = Guid.NewGuid();
            var mapNumericValues = AssetDetailValueMapper.ToNumericEntityList(domain.ValuePerNumericAttribute, attributeIdLookup);
            var mapTextValues = AssetDetailValueMapper.ToTextEntityList(domain.ValuePerTextAttribute, attributeIdLookup);
            var treatmentOptionDetails = TreatmentOptionDetailMapper.ToEntityList(domain.TreatmentOptions, id);
            var treatmentRejectionDetails = TreatmentRejectionDetailMapper.ToEntityList(domain.TreatmentRejections, id);
            var treatmentConsiderationDetails = TreatmentConsiderationDetailMapper.ToEntityList(domain.TreatmentConsiderations, id);
            var treatmentSchedulingCollisionDetails = TreatmentSchedulingCollisionDetailMapper.ToEntityList(domain.TreatmentSchedulingCollisions, id);
            mapNumericValues.AddRange(mapTextValues);
            var entity = new AssetDetailEntity
            {
                Id = id,
                MaintainableAssetId = domain.AssetId,
                SimulationYearDetailId = simulationYearDetailId,
                AppliedTreatment = domain.AppliedTreatment,
                TreatmentCause = (int)domain.TreatmentCause,
                TreatmentConsiderationDetails = treatmentConsiderationDetails,
                TreatmentOptionDetails = treatmentOptionDetails,
                TreatmentRejectionDetails = treatmentRejectionDetails,
                TreatmentSchedulingCollisionDetails = treatmentSchedulingCollisionDetails,
                TreatmentFundingIgnoresSpendingLimit = domain.TreatmentFundingIgnoresSpendingLimit,
                TreatmentStatus = (int)domain.TreatmentStatus,
                AssetDetailValues = mapNumericValues,
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

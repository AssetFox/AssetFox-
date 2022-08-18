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

        public static AssetDetail ToDomain(AssetDetailEntity entity, int year)
        {
            var assetName = entity.MaintainableAsset.AssetName;
            var domain = new AssetDetail(assetName, entity.MaintainableAsset.Id)
            {
                AppliedTreatment = entity.AppliedTreatment,
                TreatmentCause = (TreatmentCause)entity.TreatmentCause,
                TreatmentFundingIgnoresSpendingLimit = entity.TreatmentFundingIgnoresSpendingLimit,
                TreatmentStatus = (TreatmentStatus)entity.TreatmentStatus,                
            };
            AssetDetailValueMapper.AddToDictionaries(entity.AssetDetailValues, domain.ValuePerTextAttribute, domain.ValuePerNumericAttribute);
            var treatmentConsiderationDetails = TreatmentConsiderationDetailMapper.ToDomainList(entity.TreatmentConsiderationDetails);
            domain.TreatmentConsiderations.AddRange(treatmentConsiderationDetails);
            var treatmentOptionDetails = TreatmentOptionDetailMapper.ToDomainList(entity.TreatmentOptionDetails);
            domain.TreatmentOptions.AddRange(treatmentOptionDetails);
            var treatmentRejectionDetails = TreatmentRejectionDetailMapper.ToDomainList(entity.TreatmentRejectionDetails);
            domain.TreatmentRejections.AddRange(treatmentRejectionDetails);
            var treatmentSchedulingCollisionDetails = TreatmentSchedulingCollisionDetailMapper.ToDomainList(entity.TreatmentSchedulingCollisionDetails, year);
            domain.TreatmentSchedulingCollisions.AddRange(treatmentSchedulingCollisionDetails);
            return domain;
        }

        internal static List<AssetDetail> ToDomainList(
            ICollection<AssetDetailEntity> entityCollection,
            int year)
        {
            var domainList = new List<AssetDetail>();
            foreach (var entity in entityCollection)
            {
                var domain = ToDomain(entity, year);
                domainList.Add(domain);
            }
            return domainList;
        }
    }
}

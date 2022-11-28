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
        public static AssetDetailEntity ToEntityWithoutChildEntities(
            AssetDetail domain,
            Guid simulationYearDetailId,
            Dictionary<string, Guid> attributeIdLookup)
        {
            var id = Guid.NewGuid();
            var entity = new AssetDetailEntity
            {
                Id = id,
                MaintainableAssetId = domain.AssetId,
                SimulationYearDetailId = simulationYearDetailId,
                AppliedTreatment = domain.AppliedTreatment,
                TreatmentCause = (int)domain.TreatmentCause,
                TreatmentFundingIgnoresSpendingLimit = domain.TreatmentFundingIgnoresSpendingLimit,
                TreatmentStatus = (int)domain.TreatmentStatus,
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
                var entity = ToEntityWithoutChildEntities(domain, simulationYearDetailId, attributeIdLookup);
                entities.Add(entity);
            }
            return entities;
        }

        public static AssetDetail ToDomain(
            AssetDetailEntity entity,
            int year,
            Dictionary<Guid, string> attributeNameLookup,
            Dictionary<Guid, string> assetNameLookup)
        {
            var assetName = assetNameLookup[entity.MaintainableAssetId];
            var domain = new AssetDetail(assetName, entity.MaintainableAssetId)
            {
                AppliedTreatment = entity.AppliedTreatment,
                TreatmentCause = (TreatmentCause)entity.TreatmentCause,
                TreatmentFundingIgnoresSpendingLimit = entity.TreatmentFundingIgnoresSpendingLimit,
                TreatmentStatus = (TreatmentStatus)entity.TreatmentStatus,                
            };
            AssetDetailValueMapper.AddToDictionaries(entity.AssetDetailValuesIntId, domain.ValuePerTextAttribute, domain.ValuePerNumericAttribute, attributeNameLookup);
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

        internal static void AppendToDomainDictionary(
            Dictionary<Guid, AssetDetail> dictionary,
            ICollection<AssetDetailEntity> entityCollection,
            int year,
            Dictionary<Guid, string> attributeNameLookup,
            Dictionary<Guid, string> assetNameLookup)
        {
            foreach (var entity in entityCollection)
            {
                var domain = ToDomain(entity, year, attributeNameLookup, assetNameLookup);
                dictionary[entity.Id] = domain;
            }
        }

        internal static AssetDetailEntityFamily ToEntityFamily(
            List<AssetDetail> assets,
            Guid yearDetailId,
            Dictionary<string, Guid> attributeIdLookup)
        {
            var family = new AssetDetailEntityFamily();
            foreach (var asset in assets)
            {
                AddToFamily(family, asset, yearDetailId, attributeIdLookup);
            }
            return family;
        }

        private static void AddToFamily(
            AssetDetailEntityFamily family,
            AssetDetail domain,
            Guid yearDetailId,
            Dictionary<string, Guid> attributeIdLookup
            )
        {
            var entity = ToEntityWithoutChildEntities(domain, yearDetailId, attributeIdLookup);
            family.AssetDetails.Add(entity);
            var mapNumericValues = AssetDetailValueMapper.ToNumericEntityList(entity.Id, domain.ValuePerNumericAttribute, attributeIdLookup);
            var mapTextValues = AssetDetailValueMapper.ToTextEntityList(entity.Id, domain.ValuePerTextAttribute, attributeIdLookup);
            var treatmentOptionDetails = TreatmentOptionDetailMapper.ToEntityList(domain.TreatmentOptions, entity.Id);
            var treatmentRejectionDetails = TreatmentRejectionDetailMapper.ToEntityList(domain.TreatmentRejections, entity.Id);
            TreatmentConsiderationDetailMapper.AddToFamily(entity.Id, family, domain.TreatmentConsiderations); 
            var treatmentSchedulingCollisionDetails = TreatmentSchedulingCollisionDetailMapper.ToEntityList(domain.TreatmentSchedulingCollisions, entity.Id);

            family.AssetDetailValues.AddRange(mapNumericValues);
            family.AssetDetailValues.AddRange(mapTextValues);
            family.TreatmentOptionDetails.AddRange(treatmentOptionDetails);
            family.TreatmentRejectionDetails.AddRange(treatmentRejectionDetails);
            family.TreatmentSchedulingCollisionDetails.AddRange(treatmentSchedulingCollisionDetails);
        }
    }
}

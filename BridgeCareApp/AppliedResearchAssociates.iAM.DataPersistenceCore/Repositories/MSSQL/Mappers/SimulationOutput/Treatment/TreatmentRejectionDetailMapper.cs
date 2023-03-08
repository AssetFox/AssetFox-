using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class TreatmentRejectionDetailMapper
    {
        public static TreatmentRejectionDetailEntity ToEntity(
            TreatmentRejectionDetail domain,
            Guid assetDetailId)
        {
            var id = Guid.NewGuid();
            var entity = new TreatmentRejectionDetailEntity
            {
                Id = id,
                AssetDetailId = assetDetailId,
                TreatmentName = domain.TreatmentName,
                TreatmentRejectionReason = (int)domain.TreatmentRejectionReason,
                PotentialConditionChange = domain.PotentialConditionChange,
            };
            return entity;
        }

        public static List<TreatmentRejectionDetailEntity> ToEntityList(
            List<TreatmentRejectionDetail> domainList,
            Guid assetDetailId)
        {
            var entityList = new List<TreatmentRejectionDetailEntity>();
            foreach (var domain in domainList)
            {
                var entity = ToEntity(domain, assetDetailId);
                entityList.Add(entity);
            }
            return entityList;
        }

        public static TreatmentRejectionDetail ToDomain(TreatmentRejectionDetailEntity entity)
        {
            var treatmentRejectionReason = (TreatmentRejectionReason)entity.TreatmentRejectionReason;
            var domain = new TreatmentRejectionDetail(entity.TreatmentName, treatmentRejectionReason, entity.PotentialConditionChange);
            return domain;
        }

        public static List<TreatmentRejectionDetail> ToDomainList(ICollection<TreatmentRejectionDetailEntity> entityCollection)
        {
            var domainList = new List<TreatmentRejectionDetail>();
            foreach (var entity in entityCollection)
            {
                var domain = ToDomain(entity);
                domainList.Add(domain);
            }
            return domainList;
        }
    }
}

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
            };
            return entity;
        }

        internal static List<TreatmentRejectionDetailEntity> ToEntityList(
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
    }
}

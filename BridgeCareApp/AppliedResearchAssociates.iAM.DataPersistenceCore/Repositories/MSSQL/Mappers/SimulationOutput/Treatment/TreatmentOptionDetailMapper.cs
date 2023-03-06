using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class TreatmentOptionDetailMapper
    {
        public static TreatmentOptionDetailEntity ToEntity(
            TreatmentOptionDetail domain,
            Guid assetDetailId)
        {
            var id = Guid.NewGuid();
            var entity = new TreatmentOptionDetailEntity
            {
                Id = id,
                AssetDetailId = assetDetailId,
                Benefit = domain.CumulativeBenefit,
                Cost = domain.Cost,
                RemainingLife = domain.RemainingLife,
                TreatmentName = domain.TreatmentName,
            };
            return entity;
        }

        public static List<TreatmentOptionDetailEntity> ToEntityList(
            List<TreatmentOptionDetail> domainList,
            Guid assetDetailId
            )
        {
            var list = new List<TreatmentOptionDetailEntity>();
            foreach (var domain in domainList)
            {
                var entity = ToEntity(domain, assetDetailId);
                list.Add(entity);
            }
            return list;
        }

        public static TreatmentOptionDetail ToDomain(TreatmentOptionDetailEntity entity)
        {
            var domain = new TreatmentOptionDetail(entity.TreatmentName, entity.Cost, entity.Benefit, entity.RemainingLife);
            return domain;
        }

        internal static List<TreatmentOptionDetail> ToDomainList(ICollection<TreatmentOptionDetailEntity> entityCollection)
        {
            var domainList = new List<TreatmentOptionDetail>();
            foreach (var entity in entityCollection)
            {
                var domain = ToDomain(entity);
                domainList.Add(domain);
            }
            return domainList;
        }
    }
}

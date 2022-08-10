using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class BudgetUsageDetailMapper
    {
        public static BudgetUsageDetailEntity ToEntity(
            BudgetUsageDetail domain,
            Guid treatmentConsiderationDetailId)
        {
            var id = Guid.NewGuid();
            var entity = new BudgetUsageDetailEntity
            {
                Id = id,
                BudgetName = domain.BudgetName,
                CoveredCost = domain.CoveredCost,
                Status = (int)domain.Status,
                TreatmentConsiderationDetailId = treatmentConsiderationDetailId,
            };
            return entity;
        }

        public static List<BudgetUsageDetailEntity> ToEntityList(
            this List<BudgetUsageDetail> domainList,
            Guid treatmentConsiderationDetailId)
        {
            var entityList = new List<BudgetUsageDetailEntity>();
            foreach (var domain in domainList)
            {
                var entity = ToEntity(domain, treatmentConsiderationDetailId);
                entityList.Add(entity);
            }
            return entityList;
        }

        public static BudgetUsageDetail ToDomain(BudgetUsageDetailEntity entity)
        {
            var domain = new BudgetUsageDetail(entity.BudgetName)
            {
                CoveredCost = entity.CoveredCost,
                Status = (BudgetUsageStatus)entity.Status,
            };
            return domain;
        }

        public static List<BudgetUsageDetail> ToDomainList(ICollection<BudgetUsageDetailEntity> entityCollection)
        {
            var domainList = new List<BudgetUsageDetail>();
            foreach (var entity in entityCollection)
            {
                var domain = ToDomain(entity);
                domainList.Add(domain);
            }
            return domainList;
        }
    }
}

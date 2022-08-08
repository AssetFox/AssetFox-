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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class TreatmentConsiderationDetailMapper
    {
        public static TreatmentConsiderationDetailEntity ToEntity(
            this TreatmentConsiderationDetail domain,
            Guid assetDetailId)
        {
            var id = Guid.NewGuid();
            var budgetUsageDetails = BudgetUsageDetailMapper.ToEntityList(domain.BudgetUsages, id);
            var cashFlowConsiderationDetails = CashFlowConsiderationDetailMapper.ToEntityList(domain.CashFlowConsiderations, id);
            var entity = new TreatmentConsiderationDetailEntity
            {
                Id = id,
                AssetDetailId = assetDetailId,
                BudgetPriorityLevel = domain.BudgetPriorityLevel,
                BudgetUsageDetails = budgetUsageDetails,
                CashFlowConsiderationDetails = cashFlowConsiderationDetails,
                TreatmentName = domain.TreatmentName,
            };
            return entity;
        }

        public static List<TreatmentConsiderationDetailEntity> ToEntityList(
            List<TreatmentConsiderationDetail> domainList,
            Guid assetDetailId)
        {
            var entityList = new List<TreatmentConsiderationDetailEntity>();
            foreach (var domain in domainList)
            {
                var entity = ToEntity(domain, assetDetailId);
                entityList.Add(entity);
            }
            return entityList;
        }
    }
}

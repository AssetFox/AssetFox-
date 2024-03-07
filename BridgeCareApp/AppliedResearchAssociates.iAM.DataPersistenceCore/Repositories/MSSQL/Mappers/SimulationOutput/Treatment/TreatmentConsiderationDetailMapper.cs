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
        public static TreatmentConsiderationDetailEntity ToEntityWithoutChildren(
            this TreatmentConsiderationDetail domain,
            Guid assetDetailId)
        {
            var id = Guid.NewGuid();
            var entity = new TreatmentConsiderationDetailEntity
            {
                Id = id,
                AssetDetailId = assetDetailId,
                BudgetPriorityLevel = domain.BudgetPriorityLevel,
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
                var entity = ToEntityWithoutChildren(domain, assetDetailId);
                entityList.Add(entity);
            }
            return entityList;
        }

        private static TreatmentConsiderationDetail ToDomain(TreatmentConsiderationDetailEntity entity)
        {
            var domain = new TreatmentConsiderationDetail(entity.TreatmentName)
            {
                BudgetPriorityLevel = entity.BudgetPriorityLevel,
            };
            //var budgetUsageDetails = BudgetUsageDetailMapper.ToDomainList(entity.BudgetUsageDetails);
            //domain.BudgetUsages.AddRange(budgetUsageDetails);
            var cashFlowConsiderationDetails = CashFlowConsiderationDetailMapper.ToDomainList(entity.CashFlowConsiderationDetails);
            domain.CashFlowConsiderations.AddRange(cashFlowConsiderationDetails);
            return domain;
        }


        internal static List<TreatmentConsiderationDetail> ToDomainList(ICollection<TreatmentConsiderationDetailEntity> entityCollection)
        {
            var domainList = new List<TreatmentConsiderationDetail>();
            foreach (var entity in entityCollection)
            {
                var domain = ToDomain(entity);
                domainList.Add(domain);
            }
            return domainList;
        }

        public static void AddToFamily(
            Guid assetDetailId,
            AssetDetailEntityFamily family,
            List<TreatmentConsiderationDetail> treatmentConsiderations
            )
        {
            foreach (var consideration in treatmentConsiderations)
            {
                var entity = ToEntityWithoutChildren(consideration, assetDetailId);
                family.TreatmentConsiderationDetails.Add(entity);
                //var budgetUsageDetails = BudgetUsageDetailMapper.ToEntityList(consideration.BudgetUsages, entity.Id);
                //family.BudgetUsageDetails.AddRange(budgetUsageDetails);
                var cashFlowConsiderationDetails = CashFlowConsiderationDetailMapper.ToEntityList(consideration.CashFlowConsiderations, entity.Id);
                family.CashFlowConsiderationDetails.AddRange(cashFlowConsiderationDetails);
            }
        }
    }
}

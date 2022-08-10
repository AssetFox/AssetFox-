using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class CashFlowConsiderationDetailMapper
    {
        public static CashFlowConsiderationDetailEntity ToEntity(
            CashFlowConsiderationDetail domain,
            Guid treatmentConsiderationDetailId)
        {
            Guid id = Guid.NewGuid();
            var entity = new CashFlowConsiderationDetailEntity
            {
                Id = id,
                TreatmentConsiderationDetailId = treatmentConsiderationDetailId,
                CashFlowRuleName = domain.CashFlowRuleName,
                ReasonAgainstCashFlow = (int)domain.ReasonAgainstCashFlow,
            };
            return entity;
        }

        public static List<CashFlowConsiderationDetailEntity> ToEntityList(
            List<CashFlowConsiderationDetail> domainList,
            Guid treatmentConsiderationDetailId)
        {
            var entityList = new List<CashFlowConsiderationDetailEntity>();
            foreach (var domain in domainList)
            {
                var entity = ToEntity(domain, treatmentConsiderationDetailId);
                entityList.Add(entity);
            }
            return entityList;
        }

        public static CashFlowConsiderationDetail ToDomain(CashFlowConsiderationDetailEntity entity)
        {
            var domain = new CashFlowConsiderationDetail(entity.CashFlowRuleName)
            {
                ReasonAgainstCashFlow = (ReasonAgainstCashFlow)entity.ReasonAgainstCashFlow,
            };
            return domain;
        }

        public static List<CashFlowConsiderationDetail> ToDomainList(ICollection<CashFlowConsiderationDetailEntity> entityCollection)
        {
            var domainList = new List<CashFlowConsiderationDetail>();
            foreach (var entity in entityCollection)
            {
                var domain = ToDomain(entity);
                domainList.Add(domain);
            }
            return domainList;
        }
    }
}

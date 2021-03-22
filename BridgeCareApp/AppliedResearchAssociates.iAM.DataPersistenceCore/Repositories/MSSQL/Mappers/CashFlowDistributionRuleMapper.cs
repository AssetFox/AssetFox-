using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class CashFlowDistributionRuleMapper
    {
        public static CashFlowDistributionRuleEntity ToEntity(this CashFlowDistributionRule domain, Guid cashFlowRuleId, int durationInYears) =>
            new CashFlowDistributionRuleEntity
            {
                Id = domain.Id,
                CashFlowRuleId = cashFlowRuleId,
                DurationInYears = durationInYears,
                YearlyPercentages = string.Join('/', domain.YearlyPercentages),
                CostCeiling = domain.CostCeiling ?? 0
            };

        public static CashFlowDistributionRuleEntity ToEntity(this CashFlowDistributionRuleDTO dto, Guid cashFlowRuleId) =>
            new CashFlowDistributionRuleEntity
            {
                Id = dto.Id,
                CashFlowRuleId = cashFlowRuleId,
                CostCeiling = dto.CostCeiling,
                DurationInYears = dto.DurationInYears,
                YearlyPercentages = dto.YearlyPercentages
            };

        public static CashFlowDistributionRuleDTO ToDto(this CashFlowDistributionRuleEntity entity) =>
            new CashFlowDistributionRuleDTO
            {
                Id = entity.Id,
                CostCeiling = entity.CostCeiling,
                DurationInYears = entity.DurationInYears,
                YearlyPercentages = entity.YearlyPercentages
            };
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class CashFlowRuleMapper
    {
        public static CashFlowRuleEntity ToEntity(this CashFlowRule domain, Guid cashFlowRuleLibraryId) =>
            new CashFlowRuleEntity
            {
                Id = domain.Id,
                Name = domain.Name,
                CashFlowRuleLibraryId = cashFlowRuleLibraryId
            };

        public static CashFlowRuleEntity ToEntity(this CashFlowRuleDTO dto, Guid libraryId) =>
            new CashFlowRuleEntity { Id = dto.Id, Name = dto.Name, CashFlowRuleLibraryId = libraryId };

        public static CashFlowRuleDTO ToDto(this CashFlowRuleEntity entity) =>
            new CashFlowRuleDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                CashFlowDistributionRules = entity.CashFlowDistributionRules.Any()
                    ? entity.CashFlowDistributionRules.OrderBy(_ => _.DurationInYears)
                        .Select(_ => _.ToDto()).ToList()
                    : new List<CashFlowDistributionRuleDTO>(),
                CriterionLibrary = entity.CriterionLibraryCashFlowRuleJoin != null
                    ? entity.CriterionLibraryCashFlowRuleJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO()
            };

        public static CashFlowRuleLibraryEntity ToEntity(this CashFlowRuleLibraryDTO dto) =>
            new CashFlowRuleLibraryEntity { Id = dto.Id, Name = dto.Name, Description = dto.Description };

        public static CashFlowRuleLibraryDTO ToDto(this CashFlowRuleLibraryEntity entity) =>
            new CashFlowRuleLibraryDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                CashFlowRules = entity.CashFlowRules.Any()
                    ? entity.CashFlowRules.Select(_ => _.ToDto()).ToList()
                    : new List<CashFlowRuleDTO>(),
                AppliedScenarioIds = entity.CashFlowRuleLibrarySimulationJoins.Any()
                    ? entity.CashFlowRuleLibrarySimulationJoins.Select(_ => _.SimulationId).ToList()
                    : new List<Guid>()
            };
    }
}

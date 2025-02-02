﻿using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CashFlow;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DTOs.Static;


namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class CashFlowRuleMapper
    {
        public static ScenarioCashFlowRuleEntity ToScenarioEntity(this CashFlowRule domain, Guid simulationId) =>
            new ScenarioCashFlowRuleEntity
            {
                Id = domain.Id,
                Name = domain.Name,
                SimulationId = simulationId
            };

        public static ScenarioCashFlowRuleEntity ToScenarioEntity(this CashFlowRuleDTO dto, Guid simulationId) =>
            new ScenarioCashFlowRuleEntity
            {
                Id = dto.Id, Name = dto.Name,
                LibraryId = dto.LibraryId,
                IsModified = dto.IsModified,
                SimulationId = simulationId
            };

        public static ScenarioCashFlowRuleEntity ToScenarioEntityWithCriterionLibraryJoin(this CashFlowRuleDTO dto, Guid simulationId, BaseEntityProperties baseEntityProperties)
        {

            var entity = ToScenarioEntity(dto, simulationId);
            var criterionLibraryDto = dto.CriterionLibrary;
            var isvalid = criterionLibraryDto.IsValid();
            if (isvalid)
            {
                var criterionLibrary = criterionLibraryDto.ToSingleUseEntity(baseEntityProperties);

                var join = new CriterionLibraryScenarioCashFlowRuleEntity
                {
                    ScenarioCashFlowRuleId = entity.Id,                    
                    CriterionLibrary = criterionLibrary,
                };
                BaseEntityPropertySetter.SetBaseEntityProperties(entity, baseEntityProperties);
                BaseEntityPropertySetter.SetBaseEntityProperties(join, baseEntityProperties);
                entity.CriterionLibraryScenarioCashFlowRuleJoin = join;
            }
            return entity;
        }



        public static CashFlowRuleDTO ToDto(this ScenarioCashFlowRuleEntity entity) =>
            new CashFlowRuleDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                LibraryId = entity.LibraryId,
                IsModified = entity.IsModified,
                CashFlowDistributionRules = entity.ScenarioCashFlowDistributionRules.Any()
                    ? entity.ScenarioCashFlowDistributionRules.OrderBy(_ => _.DurationInYears)
                        .Select(_ => _.ToDto()).ToList()
                    : new List<CashFlowDistributionRuleDTO>(),
                CriterionLibrary = entity.CriterionLibraryScenarioCashFlowRuleJoin != null
                    ? entity.CriterionLibraryScenarioCashFlowRuleJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO()
            };

        public static CashFlowRuleEntity ToLibraryEntity(this CashFlowRuleDTO dto, Guid libraryId) =>
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
            new CashFlowRuleLibraryEntity { Id = dto.Id, Name = dto.Name, Description = dto.Description, IsShared = dto.IsShared };

        public static CashFlowRuleLibraryDTO ToDto(this CashFlowRuleLibraryEntity entity) =>
            new CashFlowRuleLibraryDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Owner = entity.CreatedBy,
                IsShared = entity.IsShared,
                CashFlowRules = entity.CashFlowRules.Any()
                    ? entity.CashFlowRules.Select(_ => _.ToDto()).ToList()
                    : new List<CashFlowRuleDTO>(),
                AppliedScenarioIds = new List<Guid>()
            };
    }
}

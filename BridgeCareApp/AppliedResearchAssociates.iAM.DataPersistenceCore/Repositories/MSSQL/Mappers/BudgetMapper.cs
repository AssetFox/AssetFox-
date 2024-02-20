using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Budget;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CashFlow;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.RemainingLifeLimit;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Static;
using MathNet.Numerics.Statistics.Mcmc;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class BudgetMapper
    {
        public static ScenarioBudgetEntity ToScenarioEntity(this Budget domain, Guid simulationId) =>
            new ScenarioBudgetEntity { Id = domain.Id, SimulationId = simulationId, Name = domain.Name };

        public static ScenarioBudgetEntity ToScenarioEntity(this BudgetDTO dto, Guid simulationId) =>
            new ScenarioBudgetEntity { Id = dto.Id, SimulationId = simulationId, LibraryId = dto.LibraryId, IsModified = dto.IsModified, Name = dto.Name, BudgetOrder = dto.BudgetOrder };

        public static ScenarioBudgetEntity ToScenarioEntityWithBudgetAmounts(this BudgetDTO dto, Guid simulationId, BaseEntityProperties baseEntityProperties=null)
        {
            var entity = new ScenarioBudgetEntity            
            {
                Id = dto.Id,
                SimulationId = simulationId,
                Name = dto.Name,
                BudgetOrder = dto.BudgetOrder,
                ScenarioBudgetAmounts = dto.BudgetAmounts.Select(_ => _.ToScenarioEntity(dto.Id, baseEntityProperties)).ToList()
            };
            var criterionLibraryDto = dto.CriterionLibrary;
            JoinEntityToCriterionLibrary(entity, criterionLibraryDto, baseEntityProperties);

            BaseEntityPropertySetter.SetBaseEntityProperties(entity, baseEntityProperties);
            return entity;
        }
        

        public static ScenarioBudgetEntity ToScenarioEntityWithCriterionLibraryJoin(this BudgetDTO dto, Guid simulationId, BaseEntityProperties baseEntityProperties)
        {

            var entity = ToScenarioEntity(dto, simulationId);
            var criterionLibraryDto = dto.CriterionLibrary;
            JoinEntityToCriterionLibrary(entity, criterionLibraryDto, baseEntityProperties);
            return entity;
        }

        private static void JoinEntityToCriterionLibrary(ScenarioBudgetEntity entity, CriterionLibraryDTO criterionLibraryDto, BaseEntityProperties baseEntityProperties)
        {
            var isvalid = criterionLibraryDto.IsValid();
            if (isvalid)
            {
                var criterionLibrary = criterionLibraryDto.ToSingleUseEntity(baseEntityProperties);
                BaseEntityPropertySetter.SetBaseEntityProperties(criterionLibrary, baseEntityProperties);
                var join = new CriterionLibraryScenarioBudgetEntity
                {
                    ScenarioBudgetId = entity.Id,
                    CriterionLibrary = criterionLibrary,
                };
                BaseEntityPropertySetter.SetBaseEntityProperties(join, baseEntityProperties);
                entity.CriterionLibraryScenarioBudgetJoin = join;
            }
        }

        public static BudgetEntity ToLibraryEntity(this BudgetDTO dto, Guid libraryId) =>
            new BudgetEntity { Id = dto.Id, BudgetLibraryId = libraryId, Name = dto.Name };

        public static BudgetLibraryEntity ToEntity(this BudgetLibraryDTO dto) =>
            new BudgetLibraryEntity { Id = dto.Id, Name = dto.Name, Description = dto.Description, IsShared = dto.IsShared };

        public static BudgetDTO ToDto(this ScenarioBudgetEntity entity) =>
            new BudgetDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                LibraryId = entity.LibraryId,
                IsModified = entity.IsModified,
                BudgetOrder = entity.BudgetOrder,
                BudgetAmounts = entity.ScenarioBudgetAmounts.Any()
                    ? entity.ScenarioBudgetAmounts.Select(_ => _.ToDto(entity.Name)).ToList()
                    : new List<BudgetAmountDTO>(),
                CriterionLibrary = entity.CriterionLibraryScenarioBudgetJoin != null
                    ? entity.CriterionLibraryScenarioBudgetJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO()
            };

        public static BudgetDTO ToDto(this BudgetEntity entity) =>
            new BudgetDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                BudgetOrder = entity.BudgetOrder,
                BudgetAmounts = entity.BudgetAmounts.Any()
                    ? entity.BudgetAmounts.Select(_ => _.ToDto(entity.Name)).ToList()
                    : new List<BudgetAmountDTO>(),
                CriterionLibrary = entity.CriterionLibraryBudgetJoin != null
                    ? entity.CriterionLibraryBudgetJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO()
            };

     
        public static BudgetLibraryDTO ToDto(this BudgetLibraryEntity entity) =>
            new BudgetLibraryDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Owner = entity.CreatedBy,
                IsShared = entity.IsShared,
                Budgets = entity.Budgets.Any()
                    ? entity.Budgets.Select(_ => _.ToDto()).ToList()
                    : new List<BudgetDTO>(),
            };
    }
}

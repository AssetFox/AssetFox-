using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using MoreLinq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class SelectableTreatmentMapper
    {
        public static SelectableTreatmentEntity ToLibraryEntity(this TreatmentDTO dto, Guid libraryId) =>
            new SelectableTreatmentEntity
            {
                Id = dto.Id,
                TreatmentLibraryId = libraryId,
                Name = dto.Name,
                ShadowForAnyTreatment = dto.ShadowForAnyTreatment,
                ShadowForSameTreatment = dto.ShadowForSameTreatment,
                Description = dto.Description,
                Category = (Enums.TreatmentEnum.TreatmentCategory)dto.Category,
                AssetType = (Enums.TreatmentEnum.AssetCategory)dto.AssetType
            };

        public static ScenarioSelectableTreatmentEntity ToScenarioEntity(this TreatmentDTO dto, Guid simulationId) =>
            new ScenarioSelectableTreatmentEntity
            {
                Id = dto.Id,
                SimulationId = simulationId,
                Name = dto.Name,
                ShadowForAnyTreatment = dto.ShadowForAnyTreatment,
                ShadowForSameTreatment = dto.ShadowForSameTreatment,
                Description = dto.Description,
                Category = (Enums.TreatmentEnum.TreatmentCategory)dto.Category,
                AssetType = (Enums.TreatmentEnum.AssetCategory)dto.AssetType
            };

        public static ScenarioSelectableTreatmentEntity ToScenarioEntity(this Treatment domain, Guid simulationId) =>
            new ScenarioSelectableTreatmentEntity
            {
                Id = domain.Id,
                SimulationId = simulationId,
                Name = domain.Name,
                ShadowForAnyTreatment = domain.ShadowForAnyTreatment,
                ShadowForSameTreatment = domain.ShadowForSameTreatment
            };

        public static TreatmentLibraryEntity ToEntity(this TreatmentLibraryDTO dto) =>
            new TreatmentLibraryEntity { Id = dto.Id, Name = dto.Name, Description = dto.Description, IsShared = dto.IsShared };

        public static void CreateSelectableTreatment(this ScenarioSelectableTreatmentEntity entity, Simulation simulation)
        {
            var selectableTreatment = simulation.AddTreatment();
            selectableTreatment.Id = entity.Id;
            selectableTreatment.Name = entity.Name;
            selectableTreatment.ShadowForAnyTreatment = entity.ShadowForAnyTreatment;
            selectableTreatment.ShadowForSameTreatment = entity.ShadowForSameTreatment;
            selectableTreatment.Description = entity.Description;
            selectableTreatment.Category = (TreatmentCategory)entity.Category;
            selectableTreatment.AssetCategory = (AssetCategory)entity.AssetType;

            if (entity.ScenarioSelectableTreatmentScenarioBudgetJoins.Any())
            {
                var budgetIds = entity.ScenarioSelectableTreatmentScenarioBudgetJoins.Select(_ => _.ScenarioBudget.Id).ToList();
                simulation.InvestmentPlan.Budgets.Where(_ => budgetIds.Contains(_.Id)).ToList()
                    .ForEach(budget => selectableTreatment.Budgets.Add(budget));
            }

            if (entity.ScenarioTreatmentConsequences.Any())
            {
                entity.ScenarioTreatmentConsequences.ForEach(_ => _.CreateConditionalTreatmentConsequence(selectableTreatment, simulation.Network.Explorer.AllAttributes));
            }

            if (entity.ScenarioTreatmentCosts.Any())
            {
                entity.ScenarioTreatmentCosts.ForEach(_ => _.CreateTreatmentCost(selectableTreatment));
            }

            var feasibility = selectableTreatment.AddFeasibilityCriterion();
            feasibility.Expression = entity.CriterionLibraryScenarioSelectableTreatmentJoin?.CriterionLibrary.MergedCriteriaExpression ?? string.Empty;

            if (entity.ScenarioTreatmentSchedulings.Any())
            {
                entity.ScenarioTreatmentSchedulings.ForEach(_ => _.CreateTreatmentScheduling(selectableTreatment));
            }

            if (entity.ScenarioTreatmentSupersessions.Any())
            {
                entity.ScenarioTreatmentSupersessions.ForEach(_ => _.CreateTreatmentSupersession(selectableTreatment));
            }

            if (selectableTreatment.Name == "No Treatment")
            {
                selectableTreatment.DesignateAsPassiveForSimulation();
            }
        }

        public static TreatmentDTO ToDto(this SelectableTreatmentEntity entity) =>
            new TreatmentDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                ShadowForAnyTreatment = entity.ShadowForAnyTreatment,
                ShadowForSameTreatment = entity.ShadowForSameTreatment,
                BudgetIds = new List<Guid>(),
                Costs = entity.TreatmentCosts.Any()
                    ? entity.TreatmentCosts.Select(_ => _.ToDto()).ToList()
                    : new List<TreatmentCostDTO>(),
                Consequences = entity.TreatmentConsequences.Any()
                    ? entity.TreatmentConsequences.Select(_ => _.ToDto()).ToList()
                    : new List<TreatmentConsequenceDTO>(),
                CriterionLibrary = entity.CriterionLibrarySelectableTreatmentJoin != null
                    ? entity.CriterionLibrarySelectableTreatmentJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO(),
                Category = (TreatmentDTOEnum.TreatmentCategory)entity.Category,
                AssetType = (TreatmentDTOEnum.AssetType)entity.AssetType
            };
        public static TreatmentLibraryUserDTO ToDto(this TreatmentLibraryUserEntity entity) =>
            new TreatmentLibraryUserDTO
            {
                CanModify = entity.CanModify,
                IsOwner = entity.IsOwner,
                UserId = entity.UserId,
            };
        public static TreatmentLibraryDTO ToDto(this TreatmentLibraryEntity entity) =>
            new TreatmentLibraryDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Owner = entity.CreatedBy,
                IsShared = entity.IsShared,
                Treatments = entity.Treatments.Any()
                    ? entity.Treatments.Select(_ => _.ToDto()).OrderBy(t => t.Name).ToList()
                    : new List<TreatmentDTO>()
            };

        public static TreatmentDTO ToDto(this ScenarioSelectableTreatmentEntity entity) =>
            new TreatmentDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                BudgetIds = entity.ScenarioSelectableTreatmentScenarioBudgetJoins.Any()
                        ? entity.ScenarioSelectableTreatmentScenarioBudgetJoins.Select(_ => _.ScenarioBudgetId).ToList()
                        : new List<Guid>(),
                Budgets = entity.ScenarioSelectableTreatmentScenarioBudgetJoins.Any()
                        ? entity.ScenarioSelectableTreatmentScenarioBudgetJoins.Select(_ => new TreatmentBudgetDTO
                        { Id = _.ScenarioBudgetId, Name = _.ScenarioBudget.Name }).ToList()
                        : new List<TreatmentBudgetDTO>(),
                Consequences = entity.ScenarioTreatmentConsequences.Any()
                        ? entity.ScenarioTreatmentConsequences.Select(_ => _.ToDto()).ToList()
                        : new List<TreatmentConsequenceDTO>(),
                Costs = entity.ScenarioTreatmentCosts.Any()
                        ? entity.ScenarioTreatmentCosts.Select(_ => _.ToDto()).ToList()
                        : new List<TreatmentCostDTO>(),
                CriterionLibrary = entity.CriterionLibraryScenarioSelectableTreatmentJoin != null
                        ? entity.CriterionLibraryScenarioSelectableTreatmentJoin.CriterionLibrary.ToDto()
                        : new CriterionLibraryDTO(),
                ShadowForAnyTreatment = entity.ShadowForAnyTreatment,
                ShadowForSameTreatment = entity.ShadowForSameTreatment,
                Category = (TreatmentDTOEnum.TreatmentCategory)entity.Category,
                AssetType = (TreatmentDTOEnum.AssetType)entity.AssetType
            };
    }
}

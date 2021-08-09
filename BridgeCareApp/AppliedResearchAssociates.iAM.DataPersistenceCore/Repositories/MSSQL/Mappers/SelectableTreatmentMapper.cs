using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;
using MoreLinq;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class SelectableTreatmentMapper
    {
        public static SelectableTreatmentEntity ToEntity(this SelectableTreatment domain, Guid treatmentLibraryId) =>
            new SelectableTreatmentEntity
            {
                Id = domain.Id,
                TreatmentLibraryId = treatmentLibraryId,
                Name = domain.Name,
                ShadowForAnyTreatment = domain.ShadowForAnyTreatment,
                ShadowForSameTreatment = domain.ShadowForSameTreatment,
                Description = domain.Description
            };

        public static SelectableTreatmentEntity ToLibraryEntity(this TreatmentDTO dto, Guid libraryId) =>
            new SelectableTreatmentEntity
            {
                Id = dto.Id,
                TreatmentLibraryId = libraryId,
                Name = dto.Name,
                ShadowForAnyTreatment = dto.ShadowForAnyTreatment,
                ShadowForSameTreatment = dto.ShadowForSameTreatment,
                Description = dto.Description
            };

        public static ScenarioSelectableTreatmentEntity ToScenarioEntity(this TreatmentDTO dto, Guid simulationId) =>
            new ScenarioSelectableTreatmentEntity
            {
                Id = dto.Id,
                SimulationId = simulationId,
                Name = dto.Name,
                ShadowForAnyTreatment = dto.ShadowForAnyTreatment,
                ShadowForSameTreatment = dto.ShadowForSameTreatment,
                Description = dto.Description
            };

        public static ScenarioSelectableTreatmentEntity ToScenarioEntity(this Treatment domain, Guid simulationId) =>
            new ScenarioSelectableTreatmentEntity
            {
                Id = domain.Id,
                SimulationId = simulationId,
                Name = domain.Name,
                ShadowForAnyTreatment = domain.ShadowForAnyTreatment,
                ShadowForSameTreatment = domain.ShadowForSameTreatment,
                Description = domain.ShortDescription
            };

        public static TreatmentLibraryEntity ToEntity(this TreatmentLibraryDTO dto) =>
            new TreatmentLibraryEntity { Id = dto.Id, Name = dto.Name, Description = dto.Description };

        public static void CreateSelectableTreatment(this ScenarioSelectableTreatmentEntity entity, Simulation simulation)
        {
            var selectableTreatment = simulation.AddTreatment();
            selectableTreatment.Id = entity.Id;
            selectableTreatment.Name = entity.Name;
            selectableTreatment.ShadowForAnyTreatment = entity.ShadowForAnyTreatment;
            selectableTreatment.ShadowForSameTreatment = entity.ShadowForSameTreatment;
            selectableTreatment.Description = entity.Description;

            if (entity.ScenarioTreatmentBudgetJoins.Any())
            {
                var budgetIds = entity.ScenarioTreatmentBudgetJoins.Select(_ => _.Budget.Id).ToList();
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

        public static SimpleBudgetDetailDTO ToDto(this SelectableTreatmentBudgetEntity entity) =>
            new SimpleBudgetDetailDTO { Id = entity.BudgetId, Name = entity.Budget.Name };

        public static TreatmentDTO ToDto(this SelectableTreatmentEntity entity) =>
            new TreatmentDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                ShadowForAnyTreatment = entity.ShadowForAnyTreatment,
                ShadowForSameTreatment = entity.ShadowForSameTreatment,
                BudgetIds = entity.TreatmentBudgetJoins.Any()
                    ? entity.TreatmentBudgetJoins.Select(_ => _.BudgetId).ToList()
                    : new List<Guid>(),
                Costs = entity.TreatmentCosts.Any()
                    ? entity.TreatmentCosts.Select(_ => _.ToDto()).ToList()
                    : new List<TreatmentCostDTO>(),
                Consequences = entity.TreatmentConsequences.Any()
                    ? entity.TreatmentConsequences.Select(_ => _.ToDto()).ToList()
                    : new List<TreatmentConsequenceDTO>(),
                CriterionLibrary = entity.CriterionLibrarySelectableTreatmentJoin != null
                    ? entity.CriterionLibrarySelectableTreatmentJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO()
            };

        public static TreatmentLibraryDTO ToDto(this TreatmentLibraryEntity entity) =>
            new TreatmentLibraryDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
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
                BudgetIds = entity.ScenarioTreatmentBudgetJoins.Any()
                        ? entity.ScenarioTreatmentBudgetJoins.Select(_ => _.BudgetId).ToList()
                        : new List<Guid>(),
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
                ShadowForSameTreatment = entity.ShadowForSameTreatment
            };
    }
}

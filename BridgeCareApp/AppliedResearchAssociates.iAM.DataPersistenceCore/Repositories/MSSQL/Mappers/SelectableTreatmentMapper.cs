﻿using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.Treatment;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.Treatment;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;
using AppliedResearchAssociates.iAM.DTOs.Enums;
using MoreLinq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using Microsoft.Extensions.DependencyModel;
using MathNet.Numerics.Statistics.Mcmc;

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

                IsModified = dto.IsModified,
                LibraryId = dto.LibraryId,

                Category = (Enums.TreatmentEnum.TreatmentCategory)dto.Category,
                AssetType = (Enums.TreatmentEnum.AssetCategory)dto.AssetType,
                ScenarioTreatmentPerformanceFactors = dto.ToScenarioSelectableTreatmentPerformanceFactorEntity(),
            };
            
        public static ScenarioSelectableTreatmentEntity ToScenarioEntity(this Treatment domain, Guid simulationId) =>
            new ScenarioSelectableTreatmentEntity
            {
                Id = domain.Id,
                SimulationId = simulationId,
                Name = domain.Name,
                ShadowForAnyTreatment = domain.ShadowForAnyTreatment,
                ShadowForSameTreatment = domain.ShadowForSameTreatment,
            };

        public static TreatmentLibraryEntity ToEntity(this TreatmentLibraryDTO dto) =>
            new TreatmentLibraryEntity { Id = dto.Id, Name = dto.Name, Description = dto.Description, IsShared = dto.IsShared };

        public static SelectableTreatment CreateSelectableTreatment(this ScenarioSelectableTreatmentEntity entity, Simulation simulation)
        {
            var selectableTreatment = simulation.AddTreatment();
            PopulateSelectableTreatment(entity, selectableTreatment, simulation);

            if (selectableTreatment.Name == "No Treatment")
            {
                selectableTreatment.DesignateAsPassiveForSimulation();
            }
            return selectableTreatment;
        }

        public static SelectableTreatment ToDomain(this ScenarioSelectableTreatmentEntity entity, Simulation simulation)
        {
            var selectableTreatment = new SelectableTreatment(simulation);
            PopulateSelectableTreatment(entity, selectableTreatment, simulation);
            return selectableTreatment;
        }

        private static void PopulateSelectableTreatment(ScenarioSelectableTreatmentEntity entity, SelectableTreatment selectableTreatment, Simulation simulation)
        {
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
            if (entity.ScenarioTreatmentPerformanceFactors.Any())
            {
                entity.ScenarioTreatmentPerformanceFactors.ForEach(_ =>
                {
                    var numberAttributes = simulation.Network.Explorer.NumberAttributes;
                    foreach (var attribute in numberAttributes)
                    {
                        if (attribute.Name == _.Attribute)
                        {
                        selectableTreatment.PerformanceCurveAdjustmentFactors.Add(attribute, _.PerformanceFactor);
                        }
                    }
                });
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
                PerformanceFactors = entity.TreatmentPerformanceFactors.Any()
                    ? entity.TreatmentPerformanceFactors.Select(_ => _.ToDto()).ToList()
                    : new List<TreatmentPerformanceFactorDTO>(),
                Costs = entity.TreatmentCosts.Any()
                    ? entity.TreatmentCosts.Select(_ => _.ToDto()).ToList()
                    : new List<TreatmentCostDTO>(),
                Consequences = entity.TreatmentConsequences.Any()
                    ? entity.TreatmentConsequences.Select(_ => _.ToDto()).ToList()
                    : new List<TreatmentConsequenceDTO>(),
                CriterionLibrary = entity.CriterionLibrarySelectableTreatmentJoin != null
                    ? entity.CriterionLibrarySelectableTreatmentJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO(),
                Category = (TreatmentDTOEnum.TreatmentType)entity.Category,
                AssetType = (TreatmentDTOEnum.AssetType)entity.AssetType
            };

        public static TreatmentDTOWithSimulationId ToDtoWithSimulationId(this ScenarioSelectableTreatmentEntity entity)
        {
            var treatmentDto = entity.ToDto();
            return new TreatmentDTOWithSimulationId
            {
                SimulationId
                = entity.SimulationId,
                Treatment = treatmentDto,
            };
        }
        public static List<ScenarioTreatmentPerformanceFactorEntity> ToScenarioSelectableTreatmentPerformanceFactorEntity(this TreatmentDTO dto)
        {
            List<ScenarioTreatmentPerformanceFactorEntity> treatmentPerformanceFactors = new List<ScenarioTreatmentPerformanceFactorEntity>();
            // need to return a list of scenariotretmentperformancefactorentities
            // how do i get the attribute from the dto?
            dto.PerformanceFactors.ForEach(p =>
            {
                treatmentPerformanceFactors.Add(new ScenarioTreatmentPerformanceFactorEntity()
                {
                    Attribute = p.Attribute,
                    PerformanceFactor = p.PerformanceFactor
                });
            });
            return treatmentPerformanceFactors;
        }
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

        public static TreatmentDTO ToDtoNullSafe(this SelectableTreatmentEntity entity)
        {
            if (entity == null)
            {
                return null;
            }
            return entity.ToDto();
        }

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
                PerformanceFactors = entity.ScenarioTreatmentPerformanceFactors.Any()
                        ? entity.ScenarioTreatmentPerformanceFactors.Select(_ => _.ToDto()).ToList()
                        : new List<TreatmentPerformanceFactorDTO>(),
                CriterionLibrary = entity.CriterionLibraryScenarioSelectableTreatmentJoin != null
                        ? entity.CriterionLibraryScenarioSelectableTreatmentJoin.CriterionLibrary.ToDto()
                        : new CriterionLibraryDTO(),
                ShadowForAnyTreatment = entity.ShadowForAnyTreatment,
                ShadowForSameTreatment = entity.ShadowForSameTreatment,
                Category = (TreatmentDTOEnum.TreatmentType)entity.Category,

                IsModified = entity.IsModified,
                LibraryId = entity.LibraryId,

                AssetType = (TreatmentDTOEnum.AssetType)entity.AssetType
            };
    }
}

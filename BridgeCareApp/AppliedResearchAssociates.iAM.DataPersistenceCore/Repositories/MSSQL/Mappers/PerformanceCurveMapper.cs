﻿using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.PerformanceCurve;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class PerformanceCurveMapper
    {
        public static PerformanceCurveEntity ToLibraryEntity(this PerformanceCurveDTO dto, Guid performanceCurveLibraryId, Guid attributeId) =>
            new PerformanceCurveEntity
            {
                Id = dto.Id,
                PerformanceCurveLibraryId = performanceCurveLibraryId,
                AttributeId = attributeId,
                Name = dto.Name,
                Shift = dto.Shift
            };

        public static ScenarioPerformanceCurveEntity ToScenarioEntity(this PerformanceCurve domain, Guid simulationId, Guid attributeId) =>
            new ScenarioPerformanceCurveEntity
            {
                Id = domain.Id,
                SimulationId = simulationId,
                AttributeId = attributeId,
                Name = domain.Name,
                Shift = domain.Shift
            };


        public static ScenarioPerformanceCurveEntity ToScenarioEntity(this PerformanceCurveDTO dto, Guid simulationId, Guid attributeId) =>
            new ScenarioPerformanceCurveEntity
            {
                Id = dto.Id,
                SimulationId = simulationId,
                AttributeId = attributeId,
                Name = dto.Name,
                Shift = dto.Shift
            };

        public static PerformanceCurveLibraryEntity ToEntity(this PerformanceCurveLibraryDTO dto) =>
            new PerformanceCurveLibraryEntity { Id = dto.Id, Name = dto.Name, Description = dto.Description, IsShared = dto.IsShared };

        public static void CreatePerformanceCurve(this ScenarioPerformanceCurveEntity entity, Simulation simulation)
        {
            var performanceCurve = simulation.AddPerformanceCurve();
            performanceCurve.Id = entity.Id;
            performanceCurve.Attribute = simulation.Network.Explorer.NumberAttributes
                .Single(_ => _.Name == entity.Attribute.Name);
            performanceCurve.Name = entity.Name;
            performanceCurve.Shift = entity.Shift;
            performanceCurve.Equation.Expression = entity.ScenarioPerformanceCurveEquationJoin?.Equation.Expression ?? string.Empty;
            performanceCurve.Criterion.Expression =
                entity.CriterionLibraryScenarioPerformanceCurveJoin?.CriterionLibrary.MergedCriteriaExpression ?? string.Empty;
        }

        public static PerformanceCurveLibraryDTO ToDto(this PerformanceCurveLibraryEntity entity) =>
            new PerformanceCurveLibraryDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Owner = entity.CreatedBy,
                Description = entity.Description,
                IsShared = entity.IsShared,
                PerformanceCurves = entity.PerformanceCurves.Any()
                    ? entity.PerformanceCurves.Select(_ => _.ToDto()).ToList()
                    : new List<PerformanceCurveDTO>(),
            };

        public static PerformanceCurveDTO ToDto(this PerformanceCurveEntity entity) =>
            new PerformanceCurveDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Attribute = entity.Attribute.Name,
                CriterionLibrary = entity.CriterionLibraryPerformanceCurveJoin != null
                    ? entity.CriterionLibraryPerformanceCurveJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO(),
                Equation = entity.PerformanceCurveEquationJoin != null
                    ? entity.PerformanceCurveEquationJoin.Equation.ToDto()
                    : new EquationDTO(),
                Shift = entity.Shift,  // Added by WJ just now. This gets a test past a failure. But is it correct?
            };

        public static PerformanceCurveDTO ToDto(this ScenarioPerformanceCurveEntity entity) =>
            new PerformanceCurveDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Attribute = entity.Attribute.Name,
                CriterionLibrary = entity.CriterionLibraryScenarioPerformanceCurveJoin != null
                    ? entity.CriterionLibraryScenarioPerformanceCurveJoin.CriterionLibrary.ToDto()
                    : new CriterionLibraryDTO(),
                Equation = entity.ScenarioPerformanceCurveEquationJoin != null
                    ? entity.ScenarioPerformanceCurveEquationJoin.Equation.ToDto()
                    : new EquationDTO(),
                Shift = entity.Shift,
            };
    }
}

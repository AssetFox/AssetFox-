using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class PerformanceCurveMapper
    {
        public static PerformanceCurveEntity ToEntity(this PerformanceCurve domain, Guid performanceCurveLibraryId, Guid attributeId) =>
            new PerformanceCurveEntity
            {
                Id = domain.Id,
                PerformanceCurveLibraryId = performanceCurveLibraryId,
                AttributeId = attributeId,
                Name = domain.Name,
                Shift = domain.Shift
            };

        public static PerformanceCurveEntity ToEntity(this PerformanceCurveDTO dto, Guid performanceCurveLibraryId, Guid attributeId) =>
            new PerformanceCurveEntity
            {
                Id = dto.Id,
                PerformanceCurveLibraryId = performanceCurveLibraryId,
                AttributeId = attributeId,
                Name = dto.Name,
                Shift = dto.Shift
            };

        public static PerformanceCurveLibraryEntity ToEntity(this PerformanceCurveLibraryDTO dto) =>
            new PerformanceCurveLibraryEntity { Id = dto.Id, Name = dto.Name, Description = dto.Description };

        public static void CreatePerformanceCurve(this PerformanceCurveEntity entity, Simulation simulation)
        {
            var performanceCurve = simulation.AddPerformanceCurve();
            performanceCurve.Id = entity.Id;
            performanceCurve.Attribute = simulation.Network.Explorer.NumberAttributes
                .Single(_ => _.Name == entity.Attribute.Name);
            performanceCurve.Name = entity.Name;
            performanceCurve.Shift = entity.Shift;
            performanceCurve.Equation.Expression = entity.PerformanceCurveEquationJoin?.Equation.Expression ?? string.Empty;
            performanceCurve.Criterion.Expression =
                entity.CriterionLibraryPerformanceCurveJoin?.CriterionLibrary.MergedCriteriaExpression ?? string.Empty;
        }

        public static PerformanceCurveLibraryDTO ToDto(this PerformanceCurveLibraryEntity entity) =>
            new PerformanceCurveLibraryDTO
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                PerformanceCurves = entity.PerformanceCurves.Any()
                    ? entity.PerformanceCurves.Select(_ => _.ToDto()).ToList()
                    : new List<PerformanceCurveDTO>(),
                AppliedScenarioIds = entity.PerformanceCurveLibrarySimulationJoins.Any()
                    ? entity.PerformanceCurveLibrarySimulationJoins.Select(_ => _.SimulationId).ToList()
                    : new List<Guid>()
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
                    : new EquationDTO()
            };
    }
}

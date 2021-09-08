using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class CalculatedAttributeMapper
    {
        public static CalculatedAttributeLibraryDTO ToDto(this CalculatedAttributeLibraryEntity entity) =>
            new CalculatedAttributeLibraryDTO()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                IsDefault = entity.IsDefault,
                CalculatedAttributes = entity.CalculatedAttributes.Any()
                    ? entity.CalculatedAttributes.Select(_ => _.ToDto()).ToList()
                    : new List<CalculatedAttributeDTO>()
            };

        public static CalculatedAttributeDTO ToDto(this CalculatedAttributeEntity entity) =>
            new CalculatedAttributeDTO()
            {
                Id = entity.Id,
                Attribute = entity.Attribute.Name,
                CalculationTiming = entity.CalculationTiming,
                Equations = entity.Equations.Any()
                    ? entity.Equations.Select(_ => _.ToDto()).ToList()
                    : new List<CalculatedAttributeEquationCriteriaPairDTO>()
            };

        public static CalculatedAttributeEquationCriteriaPairDTO ToDto(this CalculatedAttributeEquationCriteriaPairEntity entity)
        {
            var result = new CalculatedAttributeEquationCriteriaPairDTO()
            {
                Id = entity.Id,
                Equation = entity.EquationCalculatedAttributeJoin.Equation.ToDto()
            };
            if (entity.CriterionLibraryCalculatedAttributeJoin != null)
                result.CriteriaLibrary = entity.CriterionLibraryCalculatedAttributeJoin?.CriterionLibrary.ToDto();
            return result;
        }

        public static CalculatedAttributeDTO ToDto(this ScenarioCalculatedAttributeEntity entity) =>
            new CalculatedAttributeDTO()
            {
                Id = entity.Id,
                Attribute = entity.Attribute.Name,
                CalculationTiming = entity.CalculationTiming,
                Equations = entity.Equations.Any()
                    ? entity.Equations.Select(_ => _.ToDto()).ToList()
                    : new List<CalculatedAttributeEquationCriteriaPairDTO>()
            };

        public static CalculatedAttributeEquationCriteriaPairDTO ToDto(this ScenarioCalculatedAttributeEquationCriteriaPairEntity entity)
        {
            var result = new CalculatedAttributeEquationCriteriaPairDTO()
            {
                Id = entity.Id,
                Equation = entity.EquationCalculatedAttributeJoin.Equation.ToDto()
            };
            if (entity.CriterionLibraryCalculatedAttributeJoin != null)
                result.CriteriaLibrary = entity.CriterionLibraryCalculatedAttributeJoin?.CriterionLibrary.ToDto();
            return result;
        }

        public static CalculatedAttributeLibraryEntity ToLibraryEntity(this CalculatedAttributeLibraryDTO dto) =>
            new CalculatedAttributeLibraryEntity()
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                IsDefault = dto.IsDefault
            };

        public static CalculatedAttributeEntity ToLibraryEntity(this CalculatedAttributeDTO dto, Guid attributeId) =>
            new CalculatedAttributeEntity()
            {
                Id = dto.Id, AttributeId = attributeId, CalculationTiming = dto.CalculationTiming
            };

        public static CalculatedAttributeEquationCriteriaPairEntity ToLibraryEntity(
            this CalculatedAttributeEquationCriteriaPairDTO dto, Guid calculatedAttributeId) =>
            new CalculatedAttributeEquationCriteriaPairEntity
            {
                Id = dto.Id, CalculatedAttributeId = calculatedAttributeId
            };

        public static EquationCalculatedAttributePairEntity ToLibraryEntity(this EquationDTO equation,
            Guid calculatedAttributePairId) =>
            new EquationCalculatedAttributePairEntity
            {
                EquationId = equation.Id, CalculatedAttributePairId = calculatedAttributePairId
            };

        public static CriterionLibraryCalculatedAttributePairEntity ToLibraryEntity(this CriterionLibraryDTO criterion,
            Guid calculatedAttributePairId) =>
            new CriterionLibraryCalculatedAttributePairEntity
            {
                CriterionLibraryId = criterion.Id, CalculatedAttributePairId = calculatedAttributePairId
            };

        public static ScenarioCalculatedAttributeEntity ToScenarioEntity(this CalculatedAttributeDTO dto, SimulationEntity simulation, AttributeEntity attribute)
        {
            var result = new ScenarioCalculatedAttributeEntity()
            {
                Id = dto.Id,
                Attribute = attribute,
                AttributeId = attribute.Id,
                CalculationTiming = dto.CalculationTiming,
                Simulation = simulation,
                SimulationId = simulation.Id
            };
            foreach (var pair in dto.Equations)
            {
                var pairEntity = pair.ToScenarioEntity();
                pairEntity.ScenarioCalculatedAttribute = result;
                pairEntity.ScenarioCalculatedAttributeId = result.Id;
                result.Equations.Add(pairEntity);
            }
            return result;
        }

        public static ScenarioCalculatedAttributeEquationCriteriaPairEntity ToScenarioEntity(this CalculatedAttributeEquationCriteriaPairDTO dto)
        {
            var result = new ScenarioCalculatedAttributeEquationCriteriaPairEntity() { Id = dto.Id };
            var criteria = dto.CriteriaLibrary?.ToEntity();
            if (criteria != null)
            {
                result.CriterionLibraryCalculatedAttributeJoin = new ScenarioCriterionLibraryCalculatedAttributePairEntity()
                {
                    CriterionLibrary = criteria,
                    CriterionLibraryId = criteria.Id,
                    ScenarioCalculatedAttributePair = result,
                    ScenarioCalculatedAttributePairId = result.Id
                };
            }
            var equation = dto.Equation.ToEntity();
            result.EquationCalculatedAttributeJoin = new ScenarioEquationCalculatedAttributePairEntity()
            {
                Equation = equation,
                EquationId = equation.Id,
                ScenarioCalculatedAttributePair = result,
                ScenarioCalculatedAttributePairId = result.Id
            };
            return result;
        }
    }
}

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

        public static CalculatedAttributeEquationCriteriaPairDTO ToDto(this CalculatedAttributeEquationCriteriaPairEntity entity) =>
            new CalculatedAttributeEquationCriteriaPairDTO()
            {
                Id = entity.Id,
                CriteriaLibrary = entity.CriterionLibraryCalculatedAttributeJoin.CriterionLibrary.ToDto(),
                Equation = entity.EquationCalculatedAttributeJoin.Equation.ToDto()
            };

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

        public static CalculatedAttributeEquationCriteriaPairDTO ToDto(this ScenarioCalculatedAttributeEquationCriteriaPairEntity entity) =>
            new CalculatedAttributeEquationCriteriaPairDTO()
            {
                Id = entity.Id,
                CriteriaLibrary = entity.CriterionLibraryCalculatedAttributeJoin.CriterionLibrary.ToDto(),
                Equation = entity.EquationCalculatedAttributeJoin.Equation.ToDto()
            };

        public static CalculatedAttributeLibraryEntity ToLibraryEntity(this CalculatedAttributeLibraryDTO dto, IQueryable<AttributeEntity> attributeList)
        {
            var result = new CalculatedAttributeLibraryEntity()
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                IsDefault = dto.IsDefault
            };
            foreach (var calculatedAttribute in dto.CalculatedAttributes)
            {
                var attribute = attributeList.Any(_ => _.Name == calculatedAttribute.Attribute)
                    ? attributeList.First(_ => _.Name == calculatedAttribute.Attribute)
                    : throw new ArgumentException($"Error in conversion to entity library.  Unable to find attribute {calculatedAttribute.Attribute} in provided attribute library");
                var calculatedAttributeEntity = calculatedAttribute.ToLibraryEntity(attribute);
                calculatedAttributeEntity.CalculatedAttributeLibrary = result;
                calculatedAttributeEntity.CalculatedAttributeLibraryId = result.Id;
                result.CalculatedAttributes.Add(calculatedAttributeEntity);
            }
            return result;
        }

        public static CalculatedAttributeEntity ToLibraryEntity(this CalculatedAttributeDTO dto, AttributeEntity attribute)
        {
            var result = new CalculatedAttributeEntity()
            {
                Id = dto.Id,
                AttributeId = attribute.Id,
                Attribute = attribute,
                CalculationTiming = dto.CalculationTiming
            };
            foreach (var pair in dto.Equations)
            {
                var pairEntity = pair.ToLibraryEntity();
                pairEntity.CalculatedAttribute = result;
                pairEntity.CalculatedAttributeId = result.Id;
                result.Equations.Add(pairEntity);
            }
            return result;
        }

        public static CalculatedAttributeEquationCriteriaPairEntity ToLibraryEntity(this CalculatedAttributeEquationCriteriaPairDTO dto)
        {
            var result = new CalculatedAttributeEquationCriteriaPairEntity() { Id = dto.Id };
            var criteria = dto.CriteriaLibrary.ToEntity();
            result.CriterionLibraryCalculatedAttributeJoin = new CriterionLibraryCalculatedAttributePairEntity()
            {
                CriterionLibrary = criteria,
                CriterionLibraryId = criteria.Id,
                CalculatedAttributePair = result,
                CalculatedAttributePairId = result.Id
            };
            var equation = dto.Equation.ToEntity();
            result.EquationCalculatedAttributeJoin = new EquationCalculatedAttributePairEntity()
            {
                Equation = equation,
                EquationId = equation.Id,
                CalculatedAttributePair = result,
                CalculatedAttributePairId = result.Id
            };

            return result;
        }

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
            var result = new ScenarioCalculatedAttributeEquationCriteriaPairEntity() { Id = dto.Id }
            var criteria = dto.CriteriaLibrary.ToEntity();
            result.CriterionLibraryCalculatedAttributeJoin = new ScenarioCriterionLibraryCalculatedAttributePairEntity()
            {
                CriterionLibrary = criteria,
                CriterionLibraryId = criteria.Id,
                ScenarioCalculatedAttributePair = result,
                ScenarioCalculatedAttributePairId = result.Id
            };
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

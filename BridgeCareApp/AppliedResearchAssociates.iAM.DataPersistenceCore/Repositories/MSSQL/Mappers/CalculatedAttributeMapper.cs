using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.Analysis;
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
                Owner = entity.CreatedBy,
                IsDefault = entity.IsDefault,
                IsShared = entity.IsShared,
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

        public static CalculatedAttributeDTO ToDto(this ScenarioCalculatedAttributeEntity entity) =>
            new CalculatedAttributeDTO()
            {
                Id = entity.Id,
                LibraryId = entity.LibraryId,
                IsModified = entity.IsModified,
                Attribute = entity.Attribute.Name,
                CalculationTiming = entity.CalculationTiming,
                Equations = entity.Equations.Any()
                    ? entity.Equations.Select(_ => _.ToDto()).ToList()
                    : new List<CalculatedAttributeEquationCriteriaPairDTO>()
            };

     
        public static CalculatedAttributeLibraryEntity ToLibraryEntity(this CalculatedAttributeLibraryDTO dto) =>
            new CalculatedAttributeLibraryEntity()
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                IsDefault = dto.IsDefault,
                IsShared = dto.IsShared
            };

        public static CalculatedAttributeEntity ToLibraryEntity(this CalculatedAttributeDTO dto, Guid libraryId, Guid attributeId) =>
            new CalculatedAttributeEntity()
            {
                Id = dto.Id, AttributeId = attributeId, CalculationTiming = dto.CalculationTiming, CalculatedAttributeLibraryId = libraryId
            };

      
        public static EquationCalculatedAttributePairEntity ToLibraryEntity(this EquationDTO equation,
            Guid calculatedAttributePairId) =>
            new EquationCalculatedAttributePairEntity
            {
                EquationId = equation.Id, CalculatedAttributePairId = calculatedAttributePairId
            };

      

        public static ScenarioCalculatedAttributeEntity ToScenarioEntity(this CalculatedAttributeDTO dto, Guid simulationId, Guid attributeId) =>
            new ScenarioCalculatedAttributeEntity()
            {
                Id = dto.Id,
                LibraryId = dto.LibraryId,
                IsModified = dto.IsModified,
                AttributeId = attributeId,
                CalculationTiming = dto.CalculationTiming,
                SimulationId = simulationId,                             
            };

      
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.LibraryEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities.ScenarioEntities.CalculatedAttribute;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class CalculatedAttributeEquationCriteriaPairMapper
    {
        public static ScenarioCalculatedAttributeEquationCriteriaPairEntity ToScenarioEntity(this CalculatedAttributeEquationCriteriaPairDTO dto, Guid calculatedAttributeId, BaseEntityProperties baseEntityProperties=null)
        {
            var entity = new ScenarioCalculatedAttributeEquationCriteriaPairEntity()
            {
                Id = dto.Id,
                ScenarioCalculatedAttributeId = calculatedAttributeId
            };
            BaseEntityPropertySetter.SetBaseEntityProperties(entity, baseEntityProperties);
            return entity;
        }
          public static ScenarioCriterionLibraryCalculatedAttributePairEntity ToScenarioEntity(this CriterionLibraryDTO criterion, Guid calculatedAttributePairId) =>
            new ScenarioCriterionLibraryCalculatedAttributePairEntity()
            {
                CriterionLibraryId = criterion.Id,
                ScenarioCalculatedAttributePairId = calculatedAttributePairId
            };

        public static ScenarioEquationCalculatedAttributePairEntity ToScenarioEntity(this EquationDTO equation, Guid calculatedAttributePairId) =>
            new ScenarioEquationCalculatedAttributePairEntity()
            {
                EquationId = equation.Id,
                ScenarioCalculatedAttributePairId = calculatedAttributePairId
            };
        public static CriterionLibraryCalculatedAttributePairEntity ToLibraryEntity(this CriterionLibraryDTO criterion,
          Guid calculatedAttributePairId) =>
          new CriterionLibraryCalculatedAttributePairEntity
          {
              CriterionLibraryId = criterion.Id,
              CalculatedAttributePairId = calculatedAttributePairId
          };
        public static CalculatedAttributeEquationCriteriaPairEntity ToLibraryEntity(
          this CalculatedAttributeEquationCriteriaPairDTO dto, Guid calculatedAttributeId) =>
          new CalculatedAttributeEquationCriteriaPairEntity
          {
              Id = dto.Id,
              CalculatedAttributeId = calculatedAttributeId
          };
        public static CalculatedAttributeEquationCriteriaPairDTO ToDto(this ScenarioCalculatedAttributeEquationCriteriaPairEntity entity) =>
         new CalculatedAttributeEquationCriteriaPairDTO()
         {
             Id = entity.Id,
             Equation = entity.EquationCalculatedAttributeJoin?.Equation.ToDto(),
             CriteriaLibrary = entity.CriterionLibraryCalculatedAttributeJoin?.CriterionLibrary.ToDto()
         };
        public static CalculatedAttributeEquationCriteriaPairDTO ToDto(this CalculatedAttributeEquationCriteriaPairEntity entity) =>
            new CalculatedAttributeEquationCriteriaPairDTO()
            {
                Id = entity.Id,
                Equation = entity.EquationCalculatedAttributeJoin?.Equation.ToDto(),
                CriteriaLibrary = entity.CriterionLibraryCalculatedAttributeJoin?.CriterionLibrary.ToDto()
            };


    }
}

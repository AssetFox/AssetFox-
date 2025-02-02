﻿using System;
using System.Collections.Generic;
using System.Linq;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class CommittedProjectConsequenceMapper
    {
        public static CommittedProjectConsequenceEntity ToEntity(this TreatmentConsequence domain, Guid committedProjectId, Guid attributeId) =>
            new CommittedProjectConsequenceEntity
            {
                Id = domain.Id,
                CommittedProjectId = committedProjectId,
                AttributeId = attributeId,
                ChangeValue = domain.Change.Expression,
            };

        public static CommittedProjectConsequenceDTO ToDTO(this CommittedProjectConsequenceEntity entity) =>
            new CommittedProjectConsequenceDTO
            {
                Id = entity.Id,
                CommittedProjectId = entity.CommittedProjectId,
                Attribute = entity.Attribute.Name,
                ChangeValue = entity.ChangeValue,
                PerformanceFactor = entity.PerformanceFactor
            };

        public static CommittedProjectConsequenceEntity ToEntity(this CommittedProjectConsequenceDTO dto, IList<AttributeEntity> attributes, BaseEntityProperties baseEntityProperties=null)
        {
            var attributeEntity = attributes.FirstOrDefault(_ => _.Name == dto.Attribute);
            if (attributeEntity == null)
                throw new ArgumentException($"Unable to find {dto.Attribute} in the provided list of attributes");
            var entity = new CommittedProjectConsequenceEntity
            {
                Id = dto.Id,
                CommittedProjectId = dto.CommittedProjectId,
                AttributeId = attributeEntity.Id,
                ChangeValue = dto.ChangeValue,
                PerformanceFactor = dto.PerformanceFactor
               
            };
            BaseEntityPropertySetter.SetBaseEntityProperties(entity, baseEntityProperties);
            return entity;
        }
    }
}

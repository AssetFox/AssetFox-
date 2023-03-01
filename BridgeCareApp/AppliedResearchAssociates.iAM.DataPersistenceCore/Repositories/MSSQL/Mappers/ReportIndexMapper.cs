using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class ReportIndexMapper
    {
        public static ReportIndexDTO ToDTO(this ReportIndexEntity entity) =>
            new ReportIndexDTO
            {
                Id = entity.Id,
                SimulationId = entity.SimulationID,
                CreationDate = entity.CreatedDate,
                ExpirationDate = entity.ExpirationDate,
                Result = entity.Result,
                Type = entity.ReportTypeName
            };

        public static ReportIndexDTO ToDTONullPropagating(this ReportIndexEntity entity)
        {
            if (entity == null)
            {
                return null;
            }
            return ToDTO(entity);
        }

        public static ReportIndexEntity ToEntity(this ReportIndexDTO dto)
        {
            var entity = new ReportIndexEntity
            {
                Id = dto.Id,
                SimulationID = dto.SimulationId,
                CreatedDate = dto.CreationDate,
                ExpirationDate = dto.ExpirationDate,
                Result = dto.Result,
                ReportTypeName = dto.Type,
            };
            return entity;
        }
    }
}

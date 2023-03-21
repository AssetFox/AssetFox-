using System;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class SimulationLogMapper
    {
        public static SimulationLogEntity ToEntity(this SimulationLogDTO dto)
        {
            return new SimulationLogEntity
            {
                Id = Guid.NewGuid(),
                Message = dto.Message,
                SimulationId = dto.SimulationId,
                Status = dto.Status,
                Subject = dto.Subject,
            };
        }

        internal static SimulationLogDTO ToDTO(SimulationLogEntity entity)
        {
            var returnValue = new SimulationLogDTO
            {
                Id = entity.Id,
                Message = entity.Message,
                SimulationId = entity.SimulationId,
                Status = entity.Status,
                Subject = entity.Subject,
                TimeStamp = entity.CreatedDate,
            };
            return returnValue;
        }

    }
}

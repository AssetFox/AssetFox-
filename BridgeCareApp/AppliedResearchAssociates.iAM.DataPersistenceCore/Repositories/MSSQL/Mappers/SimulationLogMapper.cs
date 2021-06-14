using System;
using System.Collections.Generic;
using System.Text;
using AppliedResearchAssociates.CalculateEvaluate;
using AppliedResearchAssociates.iAM.Analysis;
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
            var r = new SimulationLogDTO
            {
                Id = entity.Id,
                Message = entity.Message,
                SimulationId = entity.SimulationId,
                Status = entity.Status,
                Subject = entity.Subject,
                TimeStamp = entity.CreatedDate,
            };
            return r;
        }

        public static SimulationLogDTO ToDTO(SimulationLogMessageBuilder builder)
            => new SimulationLogDTO
            {
                Id = Guid.NewGuid(),
                Message = builder.Message,
                SimulationId = builder.SimulationId,
                Status = (int)builder.Status,
                Subject = (int)builder.Subject,
            };
    }
}

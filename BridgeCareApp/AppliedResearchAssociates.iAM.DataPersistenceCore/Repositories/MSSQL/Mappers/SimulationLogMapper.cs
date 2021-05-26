using System;
using System.Collections.Generic;
using System.Text;
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
                Message = dto.Message,
                SimulationId = dto.SimulationId,
                Status = dto.Status,
                Subject = dto.Subject,
            };
        }
    }
}

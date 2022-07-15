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
    }
}

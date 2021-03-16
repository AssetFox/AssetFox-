using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class SimulationReportDetailMapper
    {
        public static SimulationReportDetailEntity ToEntity(this SimulationReportDetailDTO dto) =>
            new SimulationReportDetailEntity {SimulationId = dto.SimulationId, Status = dto.Status};
    }
}

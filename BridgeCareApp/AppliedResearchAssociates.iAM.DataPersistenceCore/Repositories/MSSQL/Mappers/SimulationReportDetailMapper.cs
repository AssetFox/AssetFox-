using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class SimulationReportDetailMapper
    {
        public static SimulationReportDetailEntity ToEntity(this SimulationReportDetailDTO dto) =>
            new SimulationReportDetailEntity { SimulationId = dto.SimulationId, Status = dto.Status, ReportType = dto.ReportType };
    }
}

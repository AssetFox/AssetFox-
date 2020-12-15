using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappings
{
    public static class SimulationAnalysisDetailMapper
    {
        public static SimulationAnalysisDetailEntity ToEntity(this SimulationAnalysisDetailDTO dto)
        {
            var entity = new SimulationAnalysisDetailEntity
            {
                SimulationId = dto.SimulationId, RunTime = dto.RunTime, Status = dto.Status
            };

            if (dto.LastRun.HasValue)
            {
                entity.LastRun = dto.LastRun.Value;
            }

            return entity;
        }

        public static SimulationAnalysisDetailDTO ToDto(this SimulationAnalysisDetailEntity entity) => new SimulationAnalysisDetailDTO(entity);
    }
}

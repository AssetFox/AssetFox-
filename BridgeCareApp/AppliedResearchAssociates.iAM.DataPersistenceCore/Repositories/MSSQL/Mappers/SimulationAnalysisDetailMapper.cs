using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Mappers
{
    public static class SimulationAnalysisDetailMapper
    {
        public static SimulationAnalysisDetailEntity ToEntity(this SimulationAnalysisDetailDTO dto)
        {
            var entity = new SimulationAnalysisDetailEntity
            {
                SimulationId = dto.SimulationId,
                RunTime = dto.RunTime,
                Status = dto.Status
            };

            if (dto.LastRun.HasValue)
            {
                entity.LastRun = dto.LastRun.Value;
            }

            return entity;
        }

        public static SimulationAnalysisDetailDTO ToDto(this SimulationAnalysisDetailEntity entity) =>
            new SimulationAnalysisDetailDTO
            {
                SimulationId = entity.SimulationId,
                LastRun = entity.LastRun,
                Status = entity.Status,
                RunTime = entity.RunTime
            };
    }
}

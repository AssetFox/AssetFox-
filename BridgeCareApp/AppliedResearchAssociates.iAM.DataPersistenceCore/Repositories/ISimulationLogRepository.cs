using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISimulationLogRepository
    {
        void ClearLog(Guid simulationId);
        void CreateLogs(IList<SimulationLogDTO> dtos);
        Task<List<SimulationLogDTO>> GetLog(Guid simulationId);
    }

    public static class ISimulationLogRepositoryExtensions
    {
        public static void CreateLog(this ISimulationLogRepository repository,
            SimulationLogDTO dto)
        {
            var list = new List<SimulationLogDTO> { dto };
            repository.CreateLogs(list);
        }
    }
}

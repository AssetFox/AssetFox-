using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISimulationLogRepository
    {
        void ClearLog(Guid simulationId);
        void CreateLog(SimulationLogDTO dto);
    }
}

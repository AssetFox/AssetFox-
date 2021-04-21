using System;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISimulationAnalysisDetailRepository
    {
        void UpsertSimulationAnalysisDetail(SimulationAnalysisDetailDTO dto);

        SimulationAnalysisDetailDTO GetSimulationAnalysisDetail(Guid simulationId);
    }
}

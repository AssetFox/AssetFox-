using System;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISimulationAnalysisDetailRepository
    {
        void UpsertSimulationAnalysisDetail(SimulationAnalysisDetailDTO dto);
        SimulationAnalysisDetailDTO GetSimulationAnalysisDetail(Guid simulationId);
    }
}

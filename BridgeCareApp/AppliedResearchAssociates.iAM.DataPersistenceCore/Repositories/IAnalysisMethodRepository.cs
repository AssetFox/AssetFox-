using System;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAnalysisMethodRepository
    {
        void GetSimulationAnalysisMethod(Simulation simulation, string userCriteria);

        AnalysisMethodDTO GetAnalysisMethod(Guid simulationId);

        void UpsertAnalysisMethod(Guid simulationId, AnalysisMethodDTO dto);
    }
}

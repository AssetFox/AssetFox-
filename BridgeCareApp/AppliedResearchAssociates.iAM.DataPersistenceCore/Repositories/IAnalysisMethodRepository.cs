using System;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAnalysisMethodRepository
    {
        void CreateAnalysisMethod(AnalysisMethod analysisMethod, Guid simulationId);

        void GetSimulationAnalysisMethod(Simulation simulation);

        AnalysisMethodDTO GetAnalysisMethod(Guid simulationId);

        void UpsertAnalysisMethod(Guid simulationId, AnalysisMethodDTO dto);
    }
}

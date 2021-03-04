using System;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface IAnalysisMethodRepository
    {
        void CreateAnalysisMethod(AnalysisMethod analysisMethod, Guid simulationId);

        void GetSimulationAnalysisMethod(Simulation simulation);
    }
}

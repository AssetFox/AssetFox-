using System;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Common;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISimulationOutputRepository
    {
        void CreateSimulationOutput(Guid simulationId, SimulationOutput simulationOutput, ILog logger = null);

        SimulationOutput GetSimulationOutput(Guid simulationId, ILog logger = null);
    }
}

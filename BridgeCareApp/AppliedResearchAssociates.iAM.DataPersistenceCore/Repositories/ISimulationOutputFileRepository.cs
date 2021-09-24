using System;
using AppliedResearchAssociates.iAM.Analysis.Engine;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISimulationOutputFileRepository
    {
        SimulationOutput GetSimulationResults(Guid networkId, Guid simulationId);
    }
}

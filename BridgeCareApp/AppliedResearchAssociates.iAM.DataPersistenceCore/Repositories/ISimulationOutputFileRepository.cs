using System;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISimulationOutputFileRepository
    {
        SimulationOutput GetSimulationResults(Guid networkId, Guid simulationId);
    }
}

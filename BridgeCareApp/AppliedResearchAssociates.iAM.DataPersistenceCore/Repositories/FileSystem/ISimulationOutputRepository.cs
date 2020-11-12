using System;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.FileSystem
{
    public interface ISimulationOutputRepository
    {
        SimulationOutput GetSimulationResults(Guid networkId, Guid simulationId);
    }
}
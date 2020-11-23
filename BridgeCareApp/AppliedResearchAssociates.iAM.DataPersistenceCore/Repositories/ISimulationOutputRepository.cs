using System;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISimulationOutputRepository
    {
        SimulationOutput GetSimulationResults(Guid networkId, Guid simulationId);
        void CreateSimulationOutput(string fileName, string simulationName);
    }
}

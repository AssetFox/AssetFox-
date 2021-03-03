using System;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISimulationOutputRepository
    {
        void CreateSimulationOutput(Guid simulationId, SimulationOutput simulationOutput);

        void GetSimulationOutput(Simulation simulation);

        SimulationOutput GetSimulationOutput(Guid simulationId);
    }
}

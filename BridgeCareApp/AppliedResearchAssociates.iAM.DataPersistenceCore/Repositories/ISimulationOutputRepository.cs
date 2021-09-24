using System;
using AppliedResearchAssociates.iAM.Analysis.Engine;
using AppliedResearchAssociates.iAM.Analysis;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISimulationOutputRepository
    {
        void CreateSimulationOutput(Guid simulationId, SimulationOutput simulationOutput);

        void GetSimulationOutput(Simulation simulation);

        SimulationOutput GetSimulationOutput(Guid simulationId);
    }
}

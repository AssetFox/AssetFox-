using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.Domains;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISimulationRepository
    {
        void CreateSimulation(Simulation simulation);

        void GetAllInNetwork(Network network);

        void GetSimulationInNetwork(Guid simulationId, Network network);

        List<SimulationDTO> GetAllInNetwork(Guid networkId);

        void CreateSimulation(Guid networkId, SimulationDTO dto);

        SimulationDTO GetSimulation(Guid simulationId);

        SimulationDTO CloneSimulation(Guid simulationId);

        void UpdatePermittedSimulation(SimulationDTO dto);

        void UpdateSimulation(SimulationDTO dto);

        void DeletePermittedSimulation(Guid simulationId);

        void DeleteSimulation(Guid simulationId);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.Domains;

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

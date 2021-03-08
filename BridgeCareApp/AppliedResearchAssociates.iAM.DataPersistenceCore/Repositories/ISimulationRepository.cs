using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.DTOs;
using AppliedResearchAssociates.iAM.Domains;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISimulationRepository
    {
        void CreateSimulation(Simulation simulation);

        void GetAllInNetwork(Network network);

        List<SimulationDTO> GetAllInNetwork(Guid networkId);

        void GetSimulationInNetwork(Guid simulationId, Network network);

        SimulationDTO GetSimulation(Guid simulationId);

        void DeleteSimulationAndAllRelatedData();
    }
}

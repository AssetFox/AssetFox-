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

        void DeleteSimulationAndAllRelatedData();

        Task<List<SimulationDTO>> GetAllInNetwork(Guid networkId);

        void CreateSimulation(Guid networkId, SimulationDTO dto, UserInfoDTO userInfo);

        Task<SimulationDTO> GetSimulation(Guid simulationId);

        Task<SimulationDTO> CloneSimulation(Guid simulationId, UserInfoDTO userInfo);

        void UpdatePermittedSimulation(UserInfoDTO userInfo, SimulationDTO dto);

        void UpdateSimulation(SimulationDTO dto, UserInfoDTO userInfo);

        void DeletePermittedSimulation(UserInfoDTO userInfo, Guid simulationId);

        void DeleteSimulation(Guid simulationId);
    }
}

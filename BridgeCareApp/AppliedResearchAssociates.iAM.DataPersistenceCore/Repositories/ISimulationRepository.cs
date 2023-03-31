using System;
using System.Collections.Generic;
using AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories.MSSQL.Entities;
using AppliedResearchAssociates.iAM.Analysis;
using AppliedResearchAssociates.iAM.DTOs;

namespace AppliedResearchAssociates.iAM.DataPersistenceCore.Repositories
{
    public interface ISimulationRepository
    {
        void CreateSimulation(Simulation simulation);

        void GetAllInNetwork(Network network);

        void GetSimulationInNetwork(Guid simulationId, Network network);

        List<SimulationDTO> GetAllScenario();

        List<SimulationDTO> GetUserScenarios();

        List<SimulationDTO> GetSharedScenarios(bool hasAdminAccess, bool hasSimulationAccess);

        List<SimulationDTO> GetScenariosWithIds(List<Guid> simulationIds);

        void CreateSimulation(Guid networkId, SimulationDTO dto);

        SimulationDTO GetSimulation(Guid simulationId);

        SimulationCloningResultDTO CloneSimulation(Guid simulationId, Guid networkId, string simulationName);

        /// <summary>Updates the simulation. If the dto has a nonempty
        /// list of users, also updates the users. BUT if the
        /// dto's list of users is empty, the users are unaffected.</summary> 
        void UpdateSimulationAndPossiblyUsers(SimulationDTO dto);

        void DeleteSimulation(Guid simulationId);

        void DeleteSimulationsByNetworkId(Guid networkId);

        void UpdateLastModifiedDate(SimulationEntity entity);

        string GetSimulationName(Guid simulationId);

        SimulationDTO GetCurrentUserOrSharedScenario(Guid simulationId, bool hasAdminAccess, bool hasSimulationAccess);
        
        bool GetNoTreatmentBeforeCommitted(Guid simulationId);

        void SetNoTreatmentBeforeCommitted(Guid simulationId);

        void RemoveNoTreatmentBeforeCommitted(Guid simulationId);
    }
}

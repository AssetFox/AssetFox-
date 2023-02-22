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

        void UpdateSimulation(SimulationDTO dto);

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

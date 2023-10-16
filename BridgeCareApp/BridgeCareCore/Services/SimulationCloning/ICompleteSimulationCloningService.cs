using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services
{
    public interface ICompleteSimulationCloningService
    {
        SimulationCloningResultDTO Clone(CloneSimulationDTO dto);
        CompleteSimulationDTO GetSimulation(string simulationId);
     
        bool CheckCompatibleNetworkAttributes(CloneSimulationDTO dto);
    }
}


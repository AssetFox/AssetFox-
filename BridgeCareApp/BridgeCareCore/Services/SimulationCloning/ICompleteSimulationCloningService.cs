using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services
{
    public interface ICompleteSimulationCloningService
    {
        SimulationCloningResultDTO Clone(CloneSimulationDTO dto);
        CompleteSimulationDTO GetSimulation(string simulationId);
        bool IsCompleteSimulation(CloneSimulationDTO dto);
        bool CheckCompatibleNetworkAttributes(CloneSimulationDTO dto);
    }
}


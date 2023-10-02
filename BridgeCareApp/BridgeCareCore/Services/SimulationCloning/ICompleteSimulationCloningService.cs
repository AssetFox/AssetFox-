using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services.SimulationCloning
{
    public interface ICompleteSimulationCloningService
    {
        SimulationCloningResultDTO Clone(CloneSimulationDTO dto);
        CompleteSimulationDTO GetSimulation(string simulationId);
        bool IsCompleteSimulation(CloneSimulationDTO dto);
        bool CheckCompatibleNetworkAttributes(CloneSimulationDTO dto);
    }
}


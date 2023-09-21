using AppliedResearchAssociates.iAM.DTOs;

namespace BridgeCareCore.Services.SimulationCloning
{
    public interface ICompleteSimulationCloningService
    {
        SimulationCloningResultDTO Clone(CloneSimulationDTO dto);
        CompleteSimulationDTO GetSimulation(string simulationId);
    }
}


using System;
using AppliedResearchAssociates.iAM.DTOs;
using BridgeCareCore.Models;

namespace BridgeCareCore.Interfaces
{
    public interface ISimulationQueueService
    {
        QueuedSimulationDTO GetQueuedSimulation(Guid simulationId);
        PagingPageModel<QueuedSimulationDTO> GetSimulationQueuePage(PagingRequestModel<QueuedSimulationDTO> request);
    }
}
